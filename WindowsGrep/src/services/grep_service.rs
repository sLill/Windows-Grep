use std::collections::HashMap;
use std::io::Write as _;
use std::path::Path;
use std::sync::{Arc, Mutex};
use std::sync::atomic::{AtomicBool, Ordering};
use std::time::Instant;

use colored::Color;
use rayon::prelude::*;
use regex::Regex;

use crate::enums::command_flag::CommandFlag;
use crate::enums::hash_type::HashType;
use crate::enums::result_scope::ResultScope;
use crate::models::console_item::ConsoleItem;
use crate::models::file_item::FileItem;
use crate::models::grep_result::GrepResult;
use crate::models::search_metrics::SearchMetrics;
use crate::services::console_service::ConsoleService;
use crate::utils::command_flag_utils;
use crate::utils::file_utils;
use crate::utils::windows_utils;

// ---------------------------------------------------------------------------
// Publisher — thread-safe output to console + optional file
// ---------------------------------------------------------------------------

pub(crate) struct Publisher {
    console: Arc<ConsoleService>,
    out_file: Option<Arc<Mutex<std::fs::File>>>,
}

impl Publisher {
    fn new(console: Arc<ConsoleService>, out_file: Option<Arc<Mutex<std::fs::File>>>) -> Self {
        Publisher { console, out_file }
    }

    pub fn publish(&self, item: &ConsoleItem) {
        self.console.write(item);
        if let Some(ref f) = self.out_file {
            if let Ok(mut file) = f.lock() {
                let _ = write!(file, "{}", item.value);
            }
        }
    }

    pub fn publish_many(&self, items: &[ConsoleItem]) {
        for item in items {
            self.publish(item);
        }
    }
}

// ---------------------------------------------------------------------------
// SearchContext — immutable, shared across all parallel workers
// ---------------------------------------------------------------------------

struct SearchContext<'a> {
    command_args: &'a HashMap<CommandFlag, String>,
    pattern: &'a str,
    regex: &'a Regex,
    bytes_regex: &'a regex::bytes::Regex,
    search_term: &'a str,
    hash_type: Option<HashType>,
    delete_flag: bool,
    replace_flag: bool,
    file_names_only: bool,
    file_hashes_only: bool,
    include_filters: Option<Vec<String>>,
    exclude_filters: Option<Vec<String>>,
    path_include: Option<Vec<String>>,
    context_flag: bool,
    context_len: usize,
}

// ---------------------------------------------------------------------------
// GrepService
// ---------------------------------------------------------------------------

pub struct GrepService {
    console: Arc<ConsoleService>,
}

impl GrepService {
    pub fn new(console: Arc<ConsoleService>) -> Self {
        GrepService { console }
    }

    pub fn run_command(
        &self,
        command_args: HashMap<CommandFlag, String>,
        previous_results: Option<Vec<GrepResult>>,
        cancelled: Arc<AtomicBool>,
    ) -> Vec<GrepResult> {
        let start = Instant::now();

        // Initialise output file if -o was provided
        let out_file: Option<Arc<Mutex<std::fs::File>>> =
            command_args.get(&CommandFlag::OutFile).and_then(|path| {
                if let Some(parent) = Path::new(path).parent() {
                    let _ = std::fs::create_dir_all(parent);
                }
                std::fs::File::create(path)
                    .ok()
                    .map(|f| Arc::new(Mutex::new(f)))
            });

        let publisher = Arc::new(Publisher::new(Arc::clone(&self.console), out_file));

        // ---- Collect files ----
        let all_files = collect_files(&command_args, &previous_results, &cancelled);

        // ---- Shared result state ----
        let all_results: Arc<Mutex<Vec<GrepResult>>> = Arc::new(Mutex::new(Vec::new()));
        let metrics: Arc<Mutex<SearchMetrics>> = Arc::new(Mutex::new(SearchMetrics::default()));

        // ---- Build search regex ----
        let pattern = command_flag_utils::build_search_pattern(&command_args);
        let search_regex = match command_flag_utils::build_search_regex(&command_args, &pattern) {
            Ok(r) => r,
            Err(e) => {
                publisher.publish(&ConsoleItem::with_fg(
                    format!("Invalid regex: {}\n", e),
                    Color::BrightRed,
                ));
                return Vec::new();
            }
        };
        let search_regex_bytes = match command_flag_utils::build_search_regex_bytes(&command_args, &pattern) {
            Ok(r) => r,
            Err(e) => {
                publisher.publish(&ConsoleItem::with_fg(
                    format!("Invalid regex: {}\n", e),
                    Color::BrightRed,
                ));
                return Vec::new();
            }
        };

        let search_term = command_args
            .get(&CommandFlag::SearchTerm)
            .cloned()
            .unwrap_or_default();

        let file_names_only = command_args.contains_key(&CommandFlag::FileNamesOnly);
        let file_hashes_only = command_args.contains_key(&CommandFlag::FileHashes);
        let hash_type = command_flag_utils::get_hash_type(&command_args);

        // Validate hash before processing
        if file_hashes_only {
            if let Some(ht) = hash_type {
                if !windows_utils::is_valid_file_hash(&search_term, ht) {
                    publisher.publish(&ConsoleItem::with_fg(
                        format!("Error: Hash does not match {:?} format\n", ht),
                        Color::BrightRed,
                    ));
                    return Vec::new();
                }
            }
        } else if !file_names_only && search_term.is_empty() {
            publisher.publish(&ConsoleItem::with_fg(
                "Error: Missing Search term\n",
                Color::BrightRed,
            ));
            return Vec::new();
        }

        // ---- Build search context (immutable, shared across all workers) ----
        let ctx = SearchContext {
            command_args: &command_args,
            pattern: &pattern,
            regex: &search_regex,
            bytes_regex: &search_regex_bytes,
            search_term: &search_term,
            hash_type,
            delete_flag: command_args.contains_key(&CommandFlag::Delete),
            replace_flag: command_args.contains_key(&CommandFlag::Replace),
            file_names_only,
            file_hashes_only,
            include_filters: command_flag_utils::get_file_type_include_filters(&command_args),
            exclude_filters: command_flag_utils::get_file_type_exclude_filters(&command_args),
            path_include: command_flag_utils::get_path_include_filters(&command_args),
            context_flag: command_args.contains_key(&CommandFlag::Context),
            context_len: command_args
                .get(&CommandFlag::Context)
                .and_then(|s| s.parse().ok())
                .unwrap_or(0),
        };

        // ---- Parallel per-file processing — rayon picks chunks adaptively ----
        all_files.par_iter().for_each(|file| {
            if cancelled.load(Ordering::SeqCst) { return; }
            if file.is_directory { return; }
            if is_file_filtered(&file.name, &ctx.include_filters, &ctx.exclude_filters, &ctx.path_include) {
                return;
            }

            if ctx.file_hashes_only {
                if let Some(ht) = ctx.hash_type {
                    process_hash_match(file, ht, &ctx, &publisher, &all_results, &metrics);
                }
            } else if ctx.file_names_only {
                process_name_match(file, &ctx, &publisher, &all_results, &metrics);
            } else {
                process_content_match(file, &ctx, &publisher, &all_results, &metrics);
            }
        });

        let elapsed = start.elapsed().as_secs_f64();

        // ---- Summaries ----
        {
            let r = all_results.lock().unwrap();
            let m = metrics.lock().unwrap();
            publish_file_access_summary(&command_args, &m, &publisher);
            publish_command_summary(&command_args, &r, &m, &publisher);
        }

        publisher.publish(&ConsoleItem::with_fg(
            format!("\n[{:.2} second(s)]", elapsed),
            Color::BrightRed,
        ));
        publisher.publish(&ConsoleItem::new("\n\n"));

        Arc::try_unwrap(all_results)
            .ok()
            .and_then(|m| m.into_inner().ok())
            .unwrap_or_default()
    }
}

// ---------------------------------------------------------------------------
// File collection
// ---------------------------------------------------------------------------

fn collect_files(
    command_args: &HashMap<CommandFlag, String>,
    previous_results: &Option<Vec<GrepResult>>,
    cancelled: &Arc<AtomicBool>,
) -> Vec<FileItem> {
    if let Some(prev) = previous_results {
        if !prev.is_empty() {
            let mut seen = std::collections::HashSet::new();
            return prev
                .iter()
                .filter_map(|r| {
                    if seen.insert(r.source_file.name.clone()) {
                        Some(r.source_file.clone())
                    } else {
                        None
                    }
                })
                .collect();
        }
    }

    let path = command_args.get(&CommandFlag::Path).map(|s| s.as_str()).unwrap_or(".");
    let recursive = command_args.contains_key(&CommandFlag::Recursive);
    let max_depth = command_args
        .get(&CommandFlag::MaxDepth)
        .and_then(|s| s.parse::<usize>().ok())
        .unwrap_or(usize::MAX);
    let skip_hidden = !command_args.contains_key(&CommandFlag::Hidden);
    let skip_system = !command_args.contains_key(&CommandFlag::System);
    let file_size_min = command_args
        .get(&CommandFlag::FileSizeMinimum)
        .and_then(|s| command_flag_utils::get_file_size(s).ok())
        .unwrap_or(-1);
    let file_size_max = command_args
        .get(&CommandFlag::FileSizeMaximum)
        .and_then(|s| command_flag_utils::get_file_size(s).ok())
        .unwrap_or(-1);
    let exclude_dirs = command_flag_utils::get_path_exclude_filters(command_args);

    windows_utils::get_files(
        path, recursive, max_depth, file_size_min, file_size_max,
        cancelled.as_ref(), exclude_dirs.as_deref(), skip_hidden, skip_system,
    )
}

// ---------------------------------------------------------------------------
// Content search
// ---------------------------------------------------------------------------

fn process_content_match(
    file: &FileItem,
    ctx: &SearchContext,
    publisher: &Publisher,
    results: &Mutex<Vec<GrepResult>>,
    metrics: &Mutex<SearchMetrics>,
) {
    // Stream line-by-line when possible.
    // Fall back to whole-file when we need the entire text in memory:
    //   - delete/replace need to rewrite the file body
    //   - --ignore-breaks lets patterns span newlines
    //   - PDF/DOCX yield extracted text from a binary format
    if can_stream(file, ctx) {
        if stream_content_match(file, ctx, publisher, results, metrics).is_err() {
            metrics.lock().unwrap().failed_read_files.push(file.clone());
        }
        return;
    }

    let text = match read_file_text(&file.name) {
        Ok(t) => t,
        Err(_) => {
            metrics.lock().unwrap().failed_read_files.push(file.clone());
            return;
        }
    };

    // Collect match positions eagerly to avoid lifetime entanglement
    let matches: Vec<(usize, usize, String)> = ctx.regex
        .find_iter(&text)
        .map(|m| (m.start(), m.end(), m.as_str().to_string()))
        .collect();

    if matches.is_empty() { return; }

    if ctx.delete_flag || ctx.replace_flag {
        perform_writes(file, ctx.pattern, matches.len(), &text, ctx.command_args, publisher, metrics);
    } else {
        build_content_results(file, &matches, &text, ctx, publisher, results);
        // Increment once per file (matching original behaviour)
        metrics.lock().unwrap().total_files_matched_count += 1;
    }
}

fn can_stream(file: &FileItem, ctx: &SearchContext) -> bool {
    if ctx.delete_flag || ctx.replace_flag {
        return false;
    }
    if ctx.command_args.contains_key(&CommandFlag::IgnoreBreaks) {
        return false;
    }
    let ext = Path::new(&file.name)
        .extension()
        .map(|e| e.to_string_lossy().to_lowercase())
        .unwrap_or_default();
    ext != "pdf" && ext != "docx"
}

fn stream_content_match(
    file: &FileItem,
    ctx: &SearchContext,
    publisher: &Publisher,
    results: &Mutex<Vec<GrepResult>>,
    metrics: &Mutex<SearchMetrics>,
) -> std::io::Result<()> {
    use std::io::{BufRead, BufReader};

    let fh = std::fs::File::open(&file.name)?;
    let mut reader = BufReader::with_capacity(64 * 1024, fh);

    // Second handle opened lazily — only needed if we hit a match AND -c is set.
    let mut context_handle: Option<std::fs::File> = None;
    let mut file_results: Vec<GrepResult> = Vec::new();
    let mut line_buf: Vec<u8> = Vec::new();
    let mut file_offset: u64 = 0;
    let mut line_number: i32 = 0;

    loop {
        line_buf.clear();
        let n = reader.read_until(b'\n', &mut line_buf)?;
        if n == 0 {
            break;
        }
        line_number += 1;

        for m in ctx.bytes_regex.find_iter(&line_buf) {
            let abs_start = file_offset + m.start() as u64;
            let abs_end = file_offset + m.end() as u64;
            let matched = String::from_utf8_lossy(m.as_bytes()).into_owned();

            let (leading, trailing) = if ctx.context_flag {
                let handle = match context_handle.as_mut() {
                    Some(h) => h,
                    None => {
                        context_handle = Some(std::fs::File::open(&file.name)?);
                        context_handle.as_mut().unwrap()
                    }
                };
                let lead = read_around(handle, abs_start, ctx.context_len, true)?;
                let trail = read_around(handle, abs_end, ctx.context_len, false)?;
                (
                    format!("\n{}", String::from_utf8_lossy(&lead)),
                    format!("{}\n", String::from_utf8_lossy(&trail)),
                )
            } else {
                (String::new(), String::new())
            };

            let mut result = GrepResult::new(file.clone(), ResultScope::FileContent);
            result.leading_context_string = leading;
            result.matched_string = matched;
            result.trailing_context_string = trailing;
            result.line_number = line_number;

            publisher.publish_many(&result.to_console_items());
            file_results.push(result);
        }

        file_offset += n as u64;
    }

    if !file_results.is_empty() {
        results.lock().unwrap().extend(file_results);
        metrics.lock().unwrap().total_files_matched_count += 1;
    }

    Ok(())
}

/// Read up to `len` bytes around `pivot`, rounded outward to UTF-8 char
/// boundaries so `from_utf8_lossy` doesn't produce U+FFFD from a half-read
/// codepoint. `leading=true` returns bytes ending at `pivot`; otherwise bytes
/// starting at `pivot`. Reads clamped at the file's actual size.
fn read_around(
    file: &mut std::fs::File,
    pivot: u64,
    len: usize,
    leading: bool,
) -> std::io::Result<Vec<u8>> {
    use std::io::{Read, Seek, SeekFrom};

    // UTF-8 codepoints are at most 4 bytes — 3 extra bytes is enough slack to
    // walk to a boundary in either direction without re-reading.
    const PAD: usize = 3;

    if leading {
        let read_start = pivot.saturating_sub((len + PAD) as u64);
        let read_len = (pivot - read_start) as usize;
        file.seek(SeekFrom::Start(read_start))?;
        let mut buf = vec![0u8; read_len];
        let n = file.read(&mut buf)?;
        buf.truncate(n);

        let ideal_start = pivot.saturating_sub(len as u64);
        let mut start_in_buf = ((ideal_start - read_start) as usize).min(buf.len());
        // Walk back over UTF-8 continuation bytes (0b10xxxxxx) to a boundary.
        while start_in_buf > 0
            && start_in_buf < buf.len()
            && (buf[start_in_buf] & 0xC0) == 0x80
        {
            start_in_buf -= 1;
        }
        Ok(buf[start_in_buf..].to_vec())
    } else {
        let read_len = len + PAD;
        file.seek(SeekFrom::Start(pivot))?;
        let mut buf = vec![0u8; read_len];
        let n = file.read(&mut buf)?;
        buf.truncate(n);

        let mut end_in_buf = len.min(buf.len());
        while end_in_buf < buf.len() && (buf[end_in_buf] & 0xC0) == 0x80 {
            end_in_buf += 1;
        }
        buf.truncate(end_in_buf);
        Ok(buf)
    }
}

/// Build GrepResult entries for each content match in `text`.
fn build_content_results(
    file: &FileItem,
    matches: &[(usize, usize, String)],
    text: &str,
    ctx: &SearchContext,
    publisher: &Publisher,
    results: &Mutex<Vec<GrepResult>>,
) {
    let mut file_results = Vec::with_capacity(matches.len());

    for &(start, end, ref matched) in matches {
        let (leading, trailing) = if ctx.context_flag {
            // Round to UTF-8 char boundaries to avoid panics on multi-byte text.
            let lead_start = floor_char_boundary(text, start.saturating_sub(ctx.context_len));
            let trail_end = ceil_char_boundary(text, (end + ctx.context_len).min(text.len()));
            (
                format!("\n{}", &text[lead_start..start]),
                format!("{}\n", &text[end..trail_end]),
            )
        } else {
            (String::new(), String::new())
        };

        let line_number = text[..start].bytes().filter(|&b| b == b'\n').count() as i32 + 1;

        let mut result = GrepResult::new(file.clone(), ResultScope::FileContent);
        result.leading_context_string = leading;
        result.trailing_context_string = trailing;
        result.matched_string = matched.clone();
        result.line_number = line_number;

        let items = result.to_console_items();
        publisher.publish_many(&items);

        file_results.push(result);
    }

    results.lock().unwrap().extend(file_results);
}

// ---------------------------------------------------------------------------
// Filename search
// ---------------------------------------------------------------------------

fn process_name_match(
    file: &FileItem,
    ctx: &SearchContext,
    publisher: &Publisher,
    results: &Mutex<Vec<GrepResult>>,
    metrics: &Mutex<SearchMetrics>,
) {
    let file_name = Path::new(&file.name)
        .file_name()
        .map(|n| n.to_string_lossy().into_owned())
        .unwrap_or_default();

    let (match_start, match_end, matched_str) = match ctx.regex.find(&file_name) {
        Some(m) => (m.start(), m.end(), m.as_str().to_string()),
        None => return,
    };

    if ctx.delete_flag || ctx.replace_flag {
        perform_writes(file, ctx.pattern, 1, &file.name, ctx.command_args, publisher, metrics);
        // Match original: also increments TotalFilesMatchedCount after PerformWrites
        metrics.lock().unwrap().total_files_matched_count += 1;
    } else {
        // Leading context = full path up to the filename portion, plus the start of the filename
        let name_offset = file.name.len() - file_name.len();
        let leading = file.name[..name_offset + match_start].to_string();
        let trailing = file.name[name_offset + match_end..].to_string();

        let mut result = GrepResult::new(file.clone(), ResultScope::FileName);
        result.leading_context_string = leading;
        result.matched_string = matched_str;
        result.trailing_context_string = trailing;

        let items = result.to_console_items();
        publisher.publish_many(&items);

        {
            let mut r = results.lock().unwrap();
            r.push(result);
        }
        metrics.lock().unwrap().total_files_matched_count += 1;
    }
}

// ---------------------------------------------------------------------------
// Hash search
// ---------------------------------------------------------------------------

fn process_hash_match(
    file: &FileItem,
    hash_type: HashType,
    ctx: &SearchContext,
    publisher: &Publisher,
    results: &Mutex<Vec<GrepResult>>,
    metrics: &Mutex<SearchMetrics>,
) {
    let hash = match windows_utils::get_file_hash(&file.name, hash_type) {
        Ok(h) => h,
        Err(_) => {
            metrics.lock().unwrap().failed_read_files.push(file.clone());
            return;
        }
    };

    if !hash.eq_ignore_ascii_case(ctx.search_term) { return; }

    if ctx.delete_flag {
        perform_writes(file, ctx.search_term, 1, &file.name, ctx.command_args, publisher, metrics);
        metrics.lock().unwrap().total_files_matched_count += 1;
    } else {
        let file_name = Path::new(&file.name)
            .file_name()
            .map(|n| n.to_string_lossy().into_owned())
            .unwrap_or_default();
        let leading = file.name[..file.name.len() - file_name.len()].to_string();

        let mut result = GrepResult::new(file.clone(), ResultScope::FileHash);
        result.leading_context_string = leading;
        result.matched_string = hash.clone();

        let items = result.to_console_items();
        publisher.publish_many(&items);

        {
            let mut r = results.lock().unwrap();
            r.push(result);
        }
        metrics.lock().unwrap().total_files_matched_count += 1;
    }
}

// ---------------------------------------------------------------------------
// Write operations (delete / replace / rename)
// ---------------------------------------------------------------------------

fn perform_writes(
    file: &FileItem,
    pattern: &str,
    match_count: usize,
    file_raw: &str,
    command_args: &HashMap<CommandFlag, String>,
    publisher: &Publisher,
    metrics: &Mutex<SearchMetrics>,
) {
    let delete_flag = command_args.contains_key(&CommandFlag::Delete);
    let replace_flag = command_args.contains_key(&CommandFlag::Replace);
    let file_names_only = command_args.contains_key(&CommandFlag::FileNamesOnly);
    let replacement = command_args.get(&CommandFlag::Replace).cloned().unwrap_or_default();

    let file_item = ConsoleItem::with_fg(format!("{} ", file.name), Color::Yellow);
    let newline = ConsoleItem::new("\n");

    if delete_flag {
        match std::fs::remove_file(&file.name) {
            Ok(_) => {
                publisher.publish(&file_item);
                publisher.publish(&ConsoleItem::with_fg("Deleted", Color::BrightRed));
                publisher.publish(&newline);
                metrics.lock().unwrap().delete_success_count += 1;
            }
            Err(_) => {
                publisher.publish(&file_item);
                publisher.publish(&ConsoleItem::with_colors("Access Denied", Color::White, Color::Red));
                publisher.publish(&newline);
                metrics.lock().unwrap().failed_write_files.push(file.clone());
            }
        }
    } else if replace_flag {
        if file_names_only {
            // Rename the file
            let dir = Path::new(&file.name)
                .parent()
                .map(|p| p.to_string_lossy().into_owned())
                .unwrap_or_default();
            let fname = Path::new(&file.name)
                .file_name()
                .map(|n| n.to_string_lossy().into_owned())
                .unwrap_or_default();

            match Regex::new(pattern) {
                Ok(re) => {
                    let new_name = re.replace(&fname, replacement.as_str()).into_owned();
                    let new_path = Path::new(&dir).join(&new_name).to_string_lossy().into_owned();
                    match std::fs::rename(&file.name, &new_path) {
                        Ok(_) => {
                            publisher.publish(&file_item);
                            publisher.publish(&ConsoleItem::with_fg("Renamed", Color::BrightRed));
                            publisher.publish(&newline);
                        }
                        Err(_) => {
                            publisher.publish(&file_item);
                            publisher.publish(&ConsoleItem::with_colors("Access Denied", Color::White, Color::Red));
                            publisher.publish(&newline);
                            metrics.lock().unwrap().failed_write_files.push(file.clone());
                        }
                    }
                }
                Err(_) => metrics.lock().unwrap().failed_write_files.push(file.clone()),
            }
        } else {
            // Content replacement — skip PDF/DOCX
            let ext = Path::new(&file.name)
                .extension()
                .map(|e| e.to_string_lossy().to_lowercase())
                .unwrap_or_default();

            if ext == "pdf" || ext == "docx" {
                metrics.lock().unwrap().failed_write_files.push(file.clone());
            } else {
                match Regex::new(pattern) {
                    Ok(re) => {
                        let new_content = re.replace_all(file_raw, replacement.as_str()).into_owned();
                        match std::fs::write(&file.name, new_content.as_bytes()) {
                            Ok(_) => {
                                publisher.publish(&file_item);
                                publisher.publish(&ConsoleItem::with_fg(
                                    format!("{} {}", match_count,
                                        if match_count == 1 { "match" } else { "matches" }),
                                    Color::Magenta,
                                ));
                                publisher.publish(&newline);
                                metrics.lock().unwrap().replaced_success_count += match_count as i32;
                            }
                            Err(_) => {
                                publisher.publish(&file_item);
                                publisher.publish(&ConsoleItem::with_colors("Access Denied", Color::White, Color::Red));
                                publisher.publish(&newline);
                                metrics.lock().unwrap().failed_write_files.push(file.clone());
                            }
                        }
                    }
                    Err(_) => metrics.lock().unwrap().failed_write_files.push(file.clone()),
                }
            }
        }
    }
    metrics.lock().unwrap().total_files_matched_count += 1;
}

// ---------------------------------------------------------------------------
// Summaries
// ---------------------------------------------------------------------------

fn publish_command_summary(
    command_args: &HashMap<CommandFlag, String>,
    results: &[GrepResult],
    metrics: &SearchMetrics,
    publisher: &Publisher,
) {
    let delete_flag = command_args.contains_key(&CommandFlag::Delete);
    let replace_flag = command_args.contains_key(&CommandFlag::Replace);
    let size_min_flag = command_args.contains_key(&CommandFlag::FileSizeMinimum);
    let size_max_flag = command_args.contains_key(&CommandFlag::FileSizeMaximum);

    let summary = if delete_flag {
        format!(
            "[{} of {} file(s) deleted]",
            metrics.delete_success_count, metrics.total_files_matched_count
        )
    } else if replace_flag {
        format!(
            "[{} occurrence(s) replaced in {} file(s)]",
            metrics.replaced_success_count, metrics.total_files_matched_count
        )
    } else {
        format!("[{} result(s) {} file(s)]", results.len(), metrics.total_files_matched_count)
    };

    publisher.publish(&ConsoleItem::with_fg(summary, Color::BrightRed));

    if size_min_flag || size_max_flag {
        let total: i64 = results.iter().map(|r| r.source_file.file_size).sum();
        let (v, t) = windows_utils::get_reduced_size(total, 3);
        publisher.publish(&ConsoleItem::with_fg(
            format!(" [{} {}(s)]", v, t),
            Color::BrightRed,
        ));
    }
}

fn publish_file_access_summary(
    command_args: &HashMap<CommandFlag, String>,
    metrics: &SearchMetrics,
    publisher: &Publisher,
) {
    let verbose = command_args.contains_key(&CommandFlag::Verbose);

    if !metrics.failed_read_files.is_empty() {
        publisher.publish(&ConsoleItem::with_fg(
            format!("[{} file(s) unreadable/inaccessible]", metrics.failed_read_files.len()),
            Color::BrightRed,
        ));
        if verbose {
            for f in &metrics.failed_read_files {
                publisher.publish(&ConsoleItem::with_fg(format!("{}\n", f.name), Color::BrightRed));
            }
        }
    }

    if !metrics.failed_write_files.is_empty() {
        publisher.publish(&ConsoleItem::with_fg(
            format!("[{} file(s) could not be modified]\n", metrics.failed_write_files.len()),
            Color::BrightRed,
        ));
        if verbose {
            for f in &metrics.failed_write_files {
                publisher.publish(&ConsoleItem::with_fg(format!("{}\n", f.name), Color::BrightRed));
            }
        }
    }

    if !metrics.failed_read_files.is_empty() || !metrics.failed_write_files.is_empty() {
        publisher.publish(&ConsoleItem::new("\n"));
    }
}

// ---------------------------------------------------------------------------
// Helpers
// ---------------------------------------------------------------------------

fn is_file_filtered(
    path: &str,
    include_types: &Option<Vec<String>>,
    exclude_types: &Option<Vec<String>>,
    path_include: &Option<Vec<String>>,
) -> bool {
    let ext = Path::new(path)
        .extension()
        .map(|e| e.to_string_lossy().to_lowercase())
        .unwrap_or_default();

    if let Some(ref includes) = include_types {
        if !includes.iter().any(|f| f == &ext) {
            return true;
        }
    }
    if let Some(ref excludes) = exclude_types {
        if excludes.iter().any(|f| f == &ext) {
            return true;
        }
    }
    if let Some(ref path_inc) = path_include {
        let dir = Path::new(path)
            .parent()
            .map(|p| p.to_string_lossy().into_owned())
            .unwrap_or_default();
        if !path_inc.iter().any(|filter| {
            let trimmed = filter.trim_matches(|c| c == '\'' || c == '"');
            dir.contains(trimmed)
        }) {
            return true;
        }
    }

    false
}

/// Walk backwards from `idx` until a UTF-8 char boundary is reached. `idx == 0`
/// and `idx == s.len()` are always boundaries, so this terminates.
fn floor_char_boundary(s: &str, mut idx: usize) -> usize {
    while idx > 0 && !s.is_char_boundary(idx) {
        idx -= 1;
    }
    idx
}

/// Walk forwards from `idx` until a UTF-8 char boundary is reached.
fn ceil_char_boundary(s: &str, mut idx: usize) -> usize {
    let len = s.len();
    while idx < len && !s.is_char_boundary(idx) {
        idx += 1;
    }
    idx
}

fn read_file_text(path: &str) -> Result<String, String> {
    let ext = Path::new(path)
        .extension()
        .map(|e| e.to_string_lossy().to_lowercase())
        .unwrap_or_default();

    if ext == "docx" {
        file_utils::read_docx(path)
    } else if ext == "pdf" {
        file_utils::read_pdf(path)
    } else {
        let bytes = std::fs::read(path).map_err(|e| e.to_string())?;
        Ok(String::from_utf8_lossy(&bytes).into_owned())
    }
}

