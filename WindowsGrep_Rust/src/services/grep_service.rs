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
use crate::enums::result_scope::ResultScope;
use crate::models::console_item::ConsoleItem;
use crate::models::file_item::FileItem;
use crate::models::grep_result::GrepResult;
use crate::models::search_metrics::SearchMetrics;
use crate::services::console_service::ConsoleService;
use crate::utils::command_flag_utils;
use crate::utils::file_utils;
use crate::utils::windows_utils;

const FILES_PER_TASK: usize = 10;

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
            Ok(r) => Arc::new(r),
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

        // ---- Parallel processing in batches of FILES_PER_TASK ----
        let chunks: Vec<&[FileItem]> = all_files.chunks(FILES_PER_TASK).collect();

        chunks.par_iter().for_each(|chunk| {
            if cancelled.load(Ordering::SeqCst) {
                return;
            }
            if file_hashes_only {
                if let Some(ht) = hash_type {
                    process_hash_matches(
                        chunk, &search_term, ht, &command_args,
                        &publisher, &all_results, &metrics, &cancelled,
                    );
                }
            } else if file_names_only {
                process_name_matches(
                    chunk, &pattern, &search_regex, &command_args,
                    &publisher, &all_results, &metrics, &cancelled,
                );
            } else {
                process_content_matches(
                    chunk, &pattern, &search_regex, &command_args,
                    &publisher, &all_results, &metrics, &cancelled,
                );
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

fn process_content_matches(
    files: &[FileItem],
    pattern: &str,
    regex: &Regex,
    command_args: &HashMap<CommandFlag, String>,
    publisher: &Publisher,
    results: &Mutex<Vec<GrepResult>>,
    metrics: &Mutex<SearchMetrics>,
    cancelled: &AtomicBool,
) {
    let delete_flag = command_args.contains_key(&CommandFlag::Delete);
    let replace_flag = command_args.contains_key(&CommandFlag::Replace);
    let include_filters = command_flag_utils::get_file_type_include_filters(command_args);
    let exclude_filters = command_flag_utils::get_file_type_exclude_filters(command_args);
    let path_include = command_flag_utils::get_path_include_filters(command_args);

    for file in files {
        if cancelled.load(Ordering::SeqCst) { return; }
        if file.is_directory { continue; }
        if is_file_filtered(&file.name, &include_filters, &exclude_filters, &path_include) { continue; }

        let text = match read_file_text(&file.name) {
            Ok(t) => t,
            Err(_) => {
                metrics.lock().unwrap().failed_read_files.push(file.clone());
                continue;
            }
        };

        // Collect match positions eagerly to avoid lifetime entanglement
        let matches: Vec<(usize, usize, String)> = regex
            .find_iter(&text)
            .map(|m| (m.start(), m.end(), m.as_str().to_string()))
            .collect();

        if matches.is_empty() { continue; }

        if delete_flag || replace_flag {
            perform_writes(file, pattern, matches.len(), &text, command_args, publisher, metrics);
        } else {
            build_content_results(file, &matches, &text, command_args, publisher, results);
            // Increment once per file (matching original behaviour)
            metrics.lock().unwrap().total_files_matched_count += 1;
        }
    }
}

/// Build GrepResult entries for each content match in `text`.
fn build_content_results(
    file: &FileItem,
    matches: &[(usize, usize, String)],
    text: &str,
    command_args: &HashMap<CommandFlag, String>,
    publisher: &Publisher,
    results: &Mutex<Vec<GrepResult>>,
) {
    let context_flag = command_args.contains_key(&CommandFlag::Context);
    let context_len: usize = command_args
        .get(&CommandFlag::Context)
        .and_then(|s| s.parse().ok())
        .unwrap_or(0);

    let mut file_results = Vec::with_capacity(matches.len());

    for &(start, end, ref matched) in matches {
        let (leading, trailing) = if context_flag {
            // Round to UTF-8 char boundaries to avoid panics on multi-byte text.
            let lead_start = floor_char_boundary(text, start.saturating_sub(context_len));
            let trail_end = ceil_char_boundary(text, (end + context_len).min(text.len()));
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

fn process_name_matches(
    files: &[FileItem],
    pattern: &str,
    regex: &Regex,
    command_args: &HashMap<CommandFlag, String>,
    publisher: &Publisher,
    results: &Mutex<Vec<GrepResult>>,
    metrics: &Mutex<SearchMetrics>,
    cancelled: &AtomicBool,
) {
    let delete_flag = command_args.contains_key(&CommandFlag::Delete);
    let replace_flag = command_args.contains_key(&CommandFlag::Replace);
    let include_filters = command_flag_utils::get_file_type_include_filters(command_args);
    let exclude_filters = command_flag_utils::get_file_type_exclude_filters(command_args);
    let path_include = command_flag_utils::get_path_include_filters(command_args);

    for file in files {
        if cancelled.load(Ordering::SeqCst) { return; }
        if file.is_directory { continue; }
        if is_file_filtered(&file.name, &include_filters, &exclude_filters, &path_include) { continue; }

        let file_name = Path::new(&file.name)
            .file_name()
            .map(|n| n.to_string_lossy().into_owned())
            .unwrap_or_default();

        let (match_start, match_end, matched_str) = match regex.find(&file_name) {
            Some(m) => (m.start(), m.end(), m.as_str().to_string()),
            None => continue,
        };

        if delete_flag || replace_flag {
            perform_writes(file, pattern, 1, &file.name, command_args, publisher, metrics);
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
}

// ---------------------------------------------------------------------------
// Hash search
// ---------------------------------------------------------------------------

fn process_hash_matches(
    files: &[FileItem],
    search_term: &str,
    hash_type: crate::enums::hash_type::HashType,
    command_args: &HashMap<CommandFlag, String>,
    publisher: &Publisher,
    results: &Mutex<Vec<GrepResult>>,
    metrics: &Mutex<SearchMetrics>,
    cancelled: &AtomicBool,
) {
    let delete_flag = command_args.contains_key(&CommandFlag::Delete);
    let include_filters = command_flag_utils::get_file_type_include_filters(command_args);
    let exclude_filters = command_flag_utils::get_file_type_exclude_filters(command_args);
    let path_include = command_flag_utils::get_path_include_filters(command_args);

    for file in files {
        if cancelled.load(Ordering::SeqCst) { return; }
        if file.is_directory { continue; }
        if is_file_filtered(&file.name, &include_filters, &exclude_filters, &path_include) { continue; }

        let hash = match windows_utils::get_file_hash(&file.name, hash_type) {
            Ok(h) => h,
            Err(_) => {
                metrics.lock().unwrap().failed_read_files.push(file.clone());
                continue;
            }
        };

        if !hash.eq_ignore_ascii_case(search_term) { continue; }

        if delete_flag {
            perform_writes(file, search_term, 1, &file.name, command_args, publisher, metrics);
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

