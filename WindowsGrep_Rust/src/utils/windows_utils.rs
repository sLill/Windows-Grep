use std::path::Path;
use std::sync::atomic::{AtomicBool, Ordering};

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
) -> Vec<FileItem> {
    let mut result = Vec::new();
    get_files_inner(
        root_path, root_path, recursive, max_depth,
        file_size_min, file_size_max, cancelled,
        exclude_dirs, skip_hidden, skip_system,
        &mut result,
    );
    result
}

fn get_files_inner(
    root_path: &str,
    sub_path: &str,
    recursive: bool,
    max_depth: usize,
    file_size_min: i64,
    file_size_max: i64,
    cancelled: &AtomicBool,
    exclude_dirs: Option<&[String]>,
    skip_hidden: bool,
    skip_system: bool,
    result: &mut Vec<FileItem>,
) {
    if cancelled.load(Ordering::SeqCst) {
        return;
    }

    let path = Path::new(sub_path);

    // If the path points directly at a file, yield it and return
    if path.extension().is_some() && path.is_file() {
        match get_file_size_on_disk(sub_path) {
            Ok(size) if validate_file_size(size, file_size_min, file_size_max) => {
                result.push(FileItem::new(sub_path.to_string(), false, size));
            }
            _ => {}
        }
        return;
    }

    let read_dir = match std::fs::read_dir(sub_path) {
        Ok(rd) => rd,
        Err(_) => return,
    };

    let mut subdirs: Vec<String> = Vec::new();

    for entry in read_dir {
        let entry = match entry {
            Ok(e) => e,
            Err(_) => continue,
        };

        let entry_path = entry.path();
        let entry_path_str = entry_path.to_string_lossy().into_owned();

        // Skip hidden / system files on Windows
        if skip_hidden || skip_system {
            if let Ok(metadata) = entry.metadata() {
                if should_skip_entry(&metadata, skip_hidden, skip_system) {
                    continue;
                }
            }
        }

        if entry_path.is_dir() {
            result.push(FileItem::new(entry_path_str.clone(), true, -1));
            subdirs.push(entry_path_str);
        } else {
            let size = get_file_size_on_disk(&entry_path_str).unwrap_or(-1);
            if validate_file_size(size, file_size_min, file_size_max) {
                result.push(FileItem::new(entry_path_str, false, size));
            }
        }
    }

    if !recursive {
        return;
    }

    let depth = compute_depth(root_path, sub_path);
    if depth >= max_depth {
        return;
    }

    for subdir in subdirs {
        if cancelled.load(Ordering::SeqCst) {
            return;
        }

        if let Some(excludes) = exclude_dirs {
            let dir_name = Path::new(&subdir)
                .file_name()
                .map(|n| n.to_string_lossy().into_owned())
                .unwrap_or_default();
            if excludes.iter().any(|ex| {
                let trimmed = ex.trim_matches(|c| c == '\'' || c == '"');
                dir_name.contains(trimmed)
            }) {
                continue;
            }
        }

        get_files_inner(
            root_path, &subdir, recursive, max_depth,
            file_size_min, file_size_max, cancelled,
            exclude_dirs, skip_hidden, skip_system,
            result,
        );
    }
}

fn compute_depth(root: &str, sub: &str) -> usize {
    if root == sub {
        return 0;
    }
    Path::new(sub)
        .strip_prefix(root)
        .map(|rel| rel.components().count())
        .unwrap_or(0)
}

fn validate_file_size(size: i64, min: i64, max: i64) -> bool {
    (min == -1 || size >= min) && (max == -1 || size <= max)
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
    let mut buf = [0u8; 64 * 1024];

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

