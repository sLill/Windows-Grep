use std::collections::HashMap;
use regex::Regex;

use crate::enums::command_flag::CommandFlag;
use crate::enums::file_size_type::FileSizeType;
use crate::enums::hash_type::HashType;

pub fn get_file_size(size_str: &str) -> Result<i64, String> {
    let re = Regex::new(r"(?i)^(?P<Size>\d+)(?P<SizeType>[a-zA-Z]{2})?$").unwrap();
    let m = re.captures(size_str.trim()).ok_or_else(|| {
        "Error: Could not parse filesize parameter\nFormat should follow [SIZE] or [SIZE][TYPE]. Acceptable TYPE parameters are kb, mb, gb, tb".to_string()
    })?;

    let size: i64 = m["Size"].parse().map_err(|_| "Error: Size must be a non-negative integer".to_string())?;
    if size < 0 {
        return Err("Error: Size parameter cannot be less than 0".to_string());
    }

    let multiplier = match m.name("SizeType") {
        Some(t) if !t.as_str().is_empty() => {
            FileSizeType::from_str(t.as_str())
                .ok_or_else(|| format!("Error: Unknown size type '{}'", t.as_str()))?
                .multiplier()
        }
        _ => FileSizeType::Bytes.multiplier(),
    };

    Ok(size * multiplier)
}

pub fn get_hash_type(command_args: &HashMap<CommandFlag, String>) -> Option<HashType> {
    let val = command_args.get(&CommandFlag::FileHashes)?;
    let n: i32 = val.parse().ok()?;
    HashType::from_int(n)
}

pub fn get_file_type_include_filters(command_args: &HashMap<CommandFlag, String>) -> Option<Vec<String>> {
    let val = command_args.get(&CommandFlag::FileTypeIncludeFilter)?;
    Some(
        trim_once_chars(val, &['"', '\''])
            .split(|c| c == ',' || c == ';')
            .map(|s| trim_once_chars(s, &['.']).to_string())
            .collect(),
    )
}

pub fn get_file_type_exclude_filters(command_args: &HashMap<CommandFlag, String>) -> Option<Vec<String>> {
    let val = command_args.get(&CommandFlag::FileTypeExcludeFilter)?;
    Some(
        trim_once_chars(val, &['"', '\''])
            .split(|c| c == ',' || c == ';')
            .map(|s| trim_once_chars(s, &['.']).to_string())
            .collect(),
    )
}

pub fn get_path_include_filters(command_args: &HashMap<CommandFlag, String>) -> Option<Vec<String>> {
    let val = command_args.get(&CommandFlag::PathIncludeFilter)?;
    Some(val.split(|c| c == ',' || c == ';').map(|s| s.to_string()).collect())
}

pub fn get_path_exclude_filters(command_args: &HashMap<CommandFlag, String>) -> Option<Vec<String>> {
    let val = command_args.get(&CommandFlag::PathExcludeFilter)?;
    Some(val.split(|c| c == ',' || c == ';').map(|s| s.to_string()).collect())
}

pub fn build_search_pattern(command_args: &HashMap<CommandFlag, String>) -> String {
    let fixed_strings = command_args.contains_key(&CommandFlag::FixedString);
    let ignore_case = command_args.contains_key(&CommandFlag::IgnoreCase);

    let search_term = command_args
        .get(&CommandFlag::SearchTerm)
        .cloned()
        .unwrap_or_default();

    let search_term = if fixed_strings {
        // In fixed-string mode, every char is literal — including '$'.
        regex::escape(&search_term)
    } else {
        // Regex mode: rewrite '$' to also consume trailing \r\n so end-of-line
        // anchors work the same across Unix/Windows line endings.
        search_term.replace('$', r"[\r\n]*$")
    };

    let case_mod = if ignore_case { "(?i)" } else { "" };
    format!("(?P<MatchedString>{}{})", case_mod, search_term)
}

/// Build a compiled Regex with the right options (dot-all vs multiline).
pub fn build_search_regex(
    command_args: &HashMap<CommandFlag, String>,
    pattern: &str,
) -> Result<regex::Regex, String> {
    let ignore_case = command_args.contains_key(&CommandFlag::IgnoreCase);
    let ignore_breaks = command_args.contains_key(&CommandFlag::IgnoreBreaks);

    regex::RegexBuilder::new(pattern)
        .case_insensitive(ignore_case)
        .dot_matches_new_line(ignore_breaks)
        .multi_line(!ignore_breaks)
        .build()
        .map_err(|e| e.to_string())
}

/// Byte-level variant of `build_search_regex`, used by the streaming content
/// search path so match positions correspond to real file byte offsets (no
/// UTF-8 lossy remapping). Same pattern syntax.
pub fn build_search_regex_bytes(
    command_args: &HashMap<CommandFlag, String>,
    pattern: &str,
) -> Result<regex::bytes::Regex, String> {
    let ignore_case = command_args.contains_key(&CommandFlag::IgnoreCase);
    let ignore_breaks = command_args.contains_key(&CommandFlag::IgnoreBreaks);

    regex::bytes::RegexBuilder::new(pattern)
        .case_insensitive(ignore_case)
        .dot_matches_new_line(ignore_breaks)
        .multi_line(!ignore_breaks)
        .build()
        .map_err(|e| e.to_string())
}

// Trim at most one occurrence of any of `chars` from start and end.
pub fn trim_once_chars<'a>(s: &'a str, chars: &[char]) -> &'a str {
    if s.is_empty() {
        return s;
    }
    let bytes = s.as_bytes();
    let start = if chars.iter().any(|&c| c as u8 == bytes[0]) { 1 } else { 0 };
    let end = if bytes.len() > start && chars.iter().any(|&c| c as u8 == *bytes.last().unwrap()) {
        bytes.len() - 1
    } else {
        bytes.len()
    };
    &s[start..end]
}
