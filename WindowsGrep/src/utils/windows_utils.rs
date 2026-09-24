use std::path::Path;
use std::sync::atomic::{AtomicBool, Ordering};
use std::sync::{Arc, OnceLock};

use once_cell::sync::Lazy;
use sha2::{Digest as Sha256Digest, Sha256};
use md5::Md5;

use crate::enums::file_size_type::FileSizeType;
use crate::enums::hash_type::HashType;
use crate::models::file_item::FileItem;

// ---------------------------------------------------------------------------
// Disk-cluster cache
// ---------------------------------------------------------------------------

#[cfg(windows)]
static CLUSTER_CACHE: Lazy<std::sync::Mutex<std::collections::HashMap<String, u64>>> =
    Lazy::new(|| std::sync::Mutex::new(std::collections::HashMap::new()));

// ---------------------------------------------------------------------------
// Public API
// ---------------------------------------------------------------------------

/// Lists the directories under `root_path` and the files that pass
/// `include_file` and the size limits (or just `root_path` when it is a file).
/// With `recursive`, subdirectories are listed in parallel, but the result keeps
/// the order of a sequential depth-first walk: each directory's entries, then
/// each subdirectory's subtree in listing order.
///
/// Measuring a file's on-disk size opens it, so the walk only measures files
/// when a size limit is set; otherwise their `file_size` is -1.
pub fn get_files(
    root_path: &str,
    recursive: bool,
    max_depth: usize,
    file_size_min: i64,
    file_size_max: i64,
    cancelled: &AtomicBool,
    exclude_dirs: Option<&[String]>,
    skip_hidden: bool,
    skip_system: bool,
    include_file: &(dyn Fn(&str) -> bool + Sync),
) -> Vec<FileItem> {
    if cancelled.load(Ordering::SeqCst) {
        return Vec::new();
    }

    let path = Path::new(root_path);

    // If the path points directly at a file, yield it and return
    if path.extension().is_some() && path.is_file() {
        if !include_file(root_path) {
            return Vec::new();
        }
        return match get_file_size_on_disk(root_path) {
            Ok(size) if validate_file_size(size, file_size_min, file_size_max) => {
                vec![FileItem::new(root_path.to_string(), false, size)]
            }
            _ => Vec::new(),
        };
    }

    let options = WalkOptions {
        recursive, max_depth, file_size_min, file_size_max, cancelled,
        exclude_dirs, skip_hidden, skip_system, include_file,
    };
    let root = Arc::new(DirScan::new(root_path.to_string(), 0));
    rayon::scope(|scope| scan_dir(scope, Arc::clone(&root), &options));
    flatten(root)
}

struct WalkOptions<'a> {
    recursive: bool,
    max_depth: usize,
    file_size_min: i64,
    file_size_max: i64,
    cancelled: &'a AtomicBool,
    exclude_dirs: Option<&'a [String]>,
    skip_hidden: bool,
    skip_system: bool,
    include_file: &'a (dyn Fn(&str) -> bool + Sync),
}

/// A directory in the walk. The job that lists it fills `listing`; `flatten`
/// reads the tree back once every job has finished.
struct DirScan {
    path: String,
    depth: usize,
    listing: OnceLock<DirListing>,
}

struct DirListing {
    items: Vec<FileItem>,
    subdirs: Vec<Arc<DirScan>>,
}

impl DirScan {
    fn new(path: String, depth: usize) -> Self {
        DirScan { path, depth, listing: OnceLock::new() }
    }
}

// Each directory is its own job that spawns its subdirectories and returns, so
// stack depth stays flat however deep the tree goes.
fn scan_dir<'scope>(
    scope: &rayon::Scope<'scope>,
    dir: Arc<DirScan>,
    options: &'scope WalkOptions<'scope>,
) {
    if options.cancelled.load(Ordering::SeqCst) {
        return;
    }

    let listing = list_dir(&dir, options);
    for subdir in &listing.subdirs {
        let subdir = Arc::clone(subdir);
        scope.spawn(move |scope| scan_dir(scope, subdir, options));
    }
    let _ = dir.listing.set(listing);
}

fn list_dir(dir: &DirScan, options: &WalkOptions) -> DirListing {
    let mut listing = DirListing { items: Vec::new(), subdirs: Vec::new() };

    let read_dir = match std::fs::read_dir(&dir.path) {
        Ok(rd) => rd,
        Err(_) => return listing,
    };

    let descend = options.recursive && dir.depth < options.max_depth;
    let measure_size = options.file_size_min != -1 || options.file_size_max != -1;

    for entry in read_dir {
        let entry = match entry {
            Ok(e) => e,
            Err(_) => continue,
        };

        let entry_path = entry.path();
        let entry_path_str = entry_path.to_string_lossy().into_owned();

        // On Windows this is read from the directory listing: no extra syscall
        let metadata = entry.metadata().ok();

        // Skip hidden / system files on Windows
        if options.skip_hidden || options.skip_system {
            if let Some(ref metadata) = metadata {
                if should_skip_entry(metadata, options.skip_hidden, options.skip_system) {
                    continue;
                }
            }
        }

        if is_dir_entry(metadata.as_ref(), &entry_path) {
            if descend && !is_excluded_dir(&entry_path, options.exclude_dirs) {
                listing.subdirs.push(Arc::new(DirScan::new(entry_path_str.clone(), dir.depth + 1)));
            }
            listing.items.push(FileItem::new(entry_path_str, true, -1));
        } else if (options.include_file)(&entry_path_str) {
            let size = if measure_size {
                get_file_size_on_disk(&entry_path_str).unwrap_or(-1)
            } else {
                -1
            };
            if validate_file_size(size, options.file_size_min, options.file_size_max) {
                listing.items.push(FileItem::new(entry_path_str, false, size));
            }
        }
    }

    listing
}

fn flatten(root: Arc<DirScan>) -> Vec<FileItem> {
    let mut result = Vec::new();
    let mut pending = vec![root];
    while let Some(dir) = pending.pop() {
        // A directory left unlisted by cancellation has no listing
        if let Some(listing) = Arc::into_inner(dir).and_then(|d| d.listing.into_inner()) {
            result.extend(listing.items);
            pending.extend(listing.subdirs.into_iter().rev());
        }
    }
    result
}

fn is_excluded_dir(dir: &Path, exclude_dirs: Option<&[String]>) -> bool {
    if let Some(excludes) = exclude_dirs {
        let dir_name = dir
            .file_name()
            .map(|n| n.to_string_lossy().into_owned())
            .unwrap_or_default();
        return excludes.iter().any(|ex| {
            let trimmed = ex.trim_matches(|c| c == '\'' || c == '"');
            dir_name.contains(trimmed)
        });
    }
    false
}

fn validate_file_size(size: i64, min: i64, max: i64) -> bool {
    (min == -1 || size >= min) && (max == -1 || size <= max)
}

/// Same answer as `entry_path.is_dir()` without opening the entry. The listing
/// describes a junction or symlink itself rather than its target, so reparse
/// points still get the link-following stat and are descended into as before.
#[cfg(windows)]
fn is_dir_entry(metadata: Option<&std::fs::Metadata>, entry_path: &Path) -> bool {
    use std::os::windows::fs::MetadataExt;
    const FILE_ATTRIBUTE_REPARSE_POINT: u32 = 0x400;
    match metadata {
        Some(m) if m.file_attributes() & FILE_ATTRIBUTE_REPARSE_POINT == 0 => m.is_dir(),
        _ => entry_path.is_dir(),
    }
}

#[cfg(not(windows))]
fn is_dir_entry(_: Option<&std::fs::Metadata>, entry_path: &Path) -> bool {
    entry_path.is_dir()
}

#[cfg(windows)]
fn should_skip_entry(metadata: &std::fs::Metadata, skip_hidden: bool, skip_system: bool) -> bool {
    use std::os::windows::fs::MetadataExt;
    const FILE_ATTRIBUTE_HIDDEN: u32 = 0x2;
    const FILE_ATTRIBUTE_SYSTEM: u32 = 0x4;
    let attrs = metadata.file_attributes();
    (skip_hidden && (attrs & FILE_ATTRIBUTE_HIDDEN) != 0)
        || (skip_system && (attrs & FILE_ATTRIBUTE_SYSTEM) != 0)
}

#[cfg(not(windows))]
fn should_skip_entry(_: &std::fs::Metadata, _: bool, _: bool) -> bool {
    false
}

// ---------------------------------------------------------------------------
// File size
// ---------------------------------------------------------------------------

#[cfg(windows)]
pub fn get_file_size_on_disk(file_path: &str) -> Result<i64, String> {
    use std::ffi::OsStr;
    use std::os::windows::ffi::OsStrExt;
    use winapi::um::fileapi::GetCompressedFileSizeW;

    let wide: Vec<u16> = OsStr::new(file_path)
        .encode_wide()
        .chain(std::iter::once(0))
        .collect();

    let mut high: u32 = 0;
    let low = unsafe { GetCompressedFileSizeW(wide.as_ptr(), &mut high) };

    if low == 0xFFFF_FFFF {
        return std::fs::metadata(file_path)
            .map(|m| m.len() as i64)
            .map_err(|e| e.to_string());
    }

    let raw_size: i64 = ((high as i64) << 32) | (low as i64);
    let cluster = cluster_size_for(file_path);
    if cluster > 0 {
        Ok(((raw_size + cluster - 1) / cluster) * cluster)
    } else {
        Ok(raw_size)
    }
}

#[cfg(windows)]
fn cluster_size_for(file_path: &str) -> i64 {
    use std::ffi::OsStr;
    use std::os::windows::ffi::OsStrExt;
    use winapi::um::fileapi::GetDiskFreeSpaceW;

    let root = Path::new(file_path)
        .ancestors()
        .last()
        .map(|p| p.to_string_lossy().into_owned())
        .unwrap_or_else(|| "C:\\".to_string());

    let mut cache = CLUSTER_CACHE.lock().unwrap_or_else(|p| p.into_inner());

    if let Some(&sz) = cache.get(&root) {
        return sz as i64;
    }

    let wide_root: Vec<u16> = OsStr::new(&root)
        .encode_wide()
        .chain(std::iter::once(0))
        .collect();

    let mut sectors: u32 = 0;
    let mut bytes_per: u32 = 0;
    let mut free: u32 = 0;
    let mut total: u32 = 0;

    let ok = unsafe {
        GetDiskFreeSpaceW(wide_root.as_ptr(), &mut sectors, &mut bytes_per, &mut free, &mut total)
    };

    let size = if ok != 0 { (sectors as u64) * (bytes_per as u64) } else { 0 };
    cache.insert(root, size);
    size as i64
}

#[cfg(not(windows))]
pub fn get_file_size_on_disk(file_path: &str) -> Result<i64, String> {
    std::fs::metadata(file_path)
        .map(|m| m.len() as i64)
        .map_err(|e| e.to_string())
}

// ---------------------------------------------------------------------------
// Hashing
// ---------------------------------------------------------------------------

pub fn get_file_hash(file_path: &str, hash_type: HashType) -> Result<String, String> {
    use std::io::Read;

    let file = std::fs::File::open(file_path).map_err(|e| e.to_string())?;
    let mut reader = std::io::BufReader::with_capacity(64 * 1024, file);
    // Heap, not stack: LTO inlines this into rayon's recursive work-splitter, so a
    // stack array here is paid at every split/steal level and overflows worker stacks.
    let mut buf = vec![0u8; 64 * 1024];

    let hash = match hash_type {
        HashType::Sha256 => {
            let mut h = Sha256::new();
            loop {
                let n = reader.read(&mut buf).map_err(|e| e.to_string())?;
                if n == 0 { break; }
                h.update(&buf[..n]);
            }
            hex::encode(h.finalize())
        }
        HashType::Md5 => {
            let mut h = Md5::new();
            loop {
                let n = reader.read(&mut buf).map_err(|e| e.to_string())?;
                if n == 0 { break; }
                h.update(&buf[..n]);
            }
            hex::encode(h.finalize())
        }
    };

    Ok(hash.to_lowercase())
}

pub fn is_valid_file_hash(value: &str, hash_type: HashType) -> bool {
    value.len() == hash_type.valid_hash_length()
        && value.chars().all(|c| c.is_ascii_hexdigit())
}

// ---------------------------------------------------------------------------
// Size display
// ---------------------------------------------------------------------------

/// Returns `(value, unit)` for `size` bytes, rounded to `precision` decimal places.
pub fn get_reduced_size(size: i64, precision: u32) -> (f64, FileSizeType) {
    let best = FileSizeType::all()
        .iter()
        .filter(|&&t| size > t.multiplier())
        .copied()
        .max()
        .unwrap_or(FileSizeType::Kb);

    let v = size as f64 / best.multiplier() as f64;
    let factor = 10f64.powi(precision as i32);
    ((v * factor).round() / factor, best)
}

// ---------------------------------------------------------------------------
// ANSI terminal support
// ---------------------------------------------------------------------------

#[cfg(windows)]
pub fn try_enable_ansi() {
    use winapi::um::consoleapi::{GetConsoleMode, SetConsoleMode};
    use winapi::um::processenv::GetStdHandle;

    // STD_OUTPUT_HANDLE = (DWORD)(-11)
    const STD_OUTPUT_HANDLE: u32 = 0xFFFF_FFF5u32;
    const ENABLE_VIRTUAL_TERMINAL_PROCESSING: u32 = 0x0004;

    unsafe {
        let handle = GetStdHandle(STD_OUTPUT_HANDLE);
        if handle.is_null() {
            return;
        }
        let mut mode: u32 = 0;
        if GetConsoleMode(handle, &mut mode) != 0 {
            let _ = SetConsoleMode(handle, mode | ENABLE_VIRTUAL_TERMINAL_PROCESSING);
        }
    }
}

