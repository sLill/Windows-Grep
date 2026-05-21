use std::collections::HashMap;
use std::path::Path;
use once_cell::sync::Lazy;
use regex::Regex;

use crate::enums::command_flag::{ALL_FLAGS, CommandFlag};
use crate::enums::native_command_type::NATIVE_COMMANDS;
use crate::models::native_command::NativeCommand;

// Matches a flag descriptor at the start of the command string.
// Handles quoted content within the descriptor.
static DESCRIPTOR_REGEX: Lazy<Regex> = Lazy::new(|| {
    Regex::new(r#"^(?P<Descriptor>-+(?:[^\s'"]+|'[^']*'|"[^"]*")+)"#).unwrap()
});

// Extracts the part of a long flag name before "=" or "$".
static LONG_DESCRIPTOR_REGEX: Lazy<Regex> = Lazy::new(|| Regex::new(r"^[^=$]*").unwrap());

// Matches a parameter token (possibly quoted) at the start of the string.
static SHORT_PARAM_REGEX: Lazy<Regex> = Lazy::new(|| {
    Regex::new(r#"^(?P<Parameter>(?:[^'"\s]|"[^"]*"|'[^']*')+)"#).unwrap()
});


/// Split a raw command string on `|` while respecting single- and double-quoted sections.
pub fn split_commands(command: &str) -> Vec<String> {
    let mut parts = Vec::new();
    let mut current = String::new();
    let mut in_single = false;
    let mut in_double = false;

    for ch in command.chars() {
        match ch {
            '\'' if !in_double => in_single = !in_single,
            '"' if !in_single => in_double = !in_double,
            '|' if !in_single && !in_double => {
                parts.push(current.clone());
                current.clear();
                continue;
            }
            _ => {}
        }
        current.push(ch);
    }
    if !current.is_empty() {
        parts.push(current);
    }
    parts
}

/// Checks whether the command string is a native shell command (ls, cd, pwd, clear).
/// Returns the parsed NativeCommand if matched, None otherwise.
pub fn parse_native_command_args(command_raw: &str) -> Option<NativeCommand> {
    for info in NATIVE_COMMANDS {
        let pattern = build_native_pattern(info.descriptor, info.expects_parameter);
        let re = Regex::new(&pattern).ok()?;
        if let Some(caps) = re.captures(command_raw) {
            let param = caps
                .name("Parameter")
                .map(|m| trim_once(m.as_str(), &[' ', '\'', '"']))
                .unwrap_or_default()
                .to_string();
            return Some(NativeCommand {
                command_type: info.command_type,
                command_parameter: param,
            });
        }
    }
    None
}

fn build_native_pattern(descriptor: &str, expects_parameter: bool) -> String {
    let base = format!(r"(\s|^){}", regex::escape(descriptor));
    if expects_parameter {
        format!(r"{}\s.*?(?P<Parameter>.*)$", base)
    } else {
        format!(r"{}[^\S]*?$", base)
    }
}

/// Parses a grep command string into a map of CommandFlag → parameter value.
pub fn parse_grep_command_args(command_str: &str) -> Result<HashMap<CommandFlag, String>, String> {
    let mut args: HashMap<CommandFlag, String> = HashMap::new();

    let mut command = command_str.trim().to_string();

    // Strip optional leading "grep" keyword
    if command.starts_with("grep") {
        command = command[4..].trim().to_string();
    }

    parse_command_flags(&mut args, &mut command)?;

    // Early-out for help flags
    if args.contains_key(&CommandFlag::Help) || args.contains_key(&CommandFlag::HelpFull) {
        return Ok(args);
    }

    parse_search_term_and_path(&mut args, &mut command)?;

    let remaining = command.trim();
    if !remaining.is_empty() {
        return Err(format!("Unrecognized command: {}", remaining));
    }

    Ok(args)
}

fn parse_command_flags(
    args: &mut HashMap<CommandFlag, String>,
    command: &mut String,
) -> Result<(), String> {
    loop {
        let caps = match DESCRIPTOR_REGEX.captures(command) {
            Some(c) => c,
            None => break,
        };

        let descriptor = caps["Descriptor"].to_string();
        if descriptor.starts_with("--") {
            parse_long_flag(args, &descriptor, command)?;
        } else {
            parse_short_flag(args, &descriptor, command)?;
        }
    }
    Ok(())
}

fn parse_short_flag(
    args: &mut HashMap<CommandFlag, String>,
    descriptor: &str,
    command: &mut String,
) -> Result<(), String> {
    let chars: Vec<char> = descriptor.trim_start_matches('-').chars().collect();

    for (i, &ch) in chars.iter().enumerate() {
        let is_last = i == chars.len() - 1;

        // Remove the full descriptor from the command once we reach the last character
        if is_last {
            *command = DESCRIPTOR_REGEX
                .replace(command, "")
                .trim()
                .to_string();
        }

        let flag_info = ALL_FLAGS
            .iter()
            .find(|f| {
                f.descriptors.iter().any(|d| {
                    let stripped = d.trim_start_matches('-');
                    stripped.len() == 1 && stripped.chars().next() == Some(ch)
                })
            })
            .ok_or_else(|| format!("Unrecognized command flag: -{}", ch))?;

        if flag_info.expects_parameter {
            let caps = SHORT_PARAM_REGEX
                .captures(command)
                .ok_or_else(|| format!("Option '-{}' expects a parameter, but none was provided", ch))?;
            let param = caps["Parameter"].to_string();
            args.insert(flag_info.flag, param.clone());
            *command = SHORT_PARAM_REGEX.replace(command, "").trim().to_string();
        } else {
            args.insert(flag_info.flag, String::new());
        }
    }

    Ok(())
}

fn parse_long_flag(
    args: &mut HashMap<CommandFlag, String>,
    descriptor: &str,
    command: &mut String,
) -> Result<(), String> {
    // Strip leading "--"
    let stripped = descriptor.trim_start_matches('-');

    // Extract the name portion before any "="
    let flag_name = LONG_DESCRIPTOR_REGEX
        .find(stripped)
        .map(|m| m.as_str())
        .unwrap_or(stripped);

    let flag_info = ALL_FLAGS
        .iter()
        .find(|f| {
            f.descriptors.iter().any(|d| {
                d.trim_matches(|c| c == '-' || c == '=') == flag_name
            })
        })
        .ok_or_else(|| format!("Unrecognized command flag: --{}", flag_name))?;

    // Remove the full descriptor from the command string
    *command = DESCRIPTOR_REGEX.replace(command, "").trim().to_string();

    if flag_info.expects_parameter {
        // Parameter follows the "=" in the descriptor
        let parts: Vec<&str> = stripped.splitn(2, '=').collect();
        if parts.len() < 2 {
            return Err(format!(
                "Option '--{}' expects a parameter, but none was provided",
                flag_name
            ));
        }
        let param = trim_once(parts[1], &['\'', '"']).to_string();
        args.insert(flag_info.flag, param);
    } else {
        args.insert(flag_info.flag, String::new());
    }

    Ok(())
}

fn parse_search_term_and_path(
    args: &mut HashMap<CommandFlag, String>,
    command: &mut String,
) -> Result<(), String> {
    *command = command.trim().to_string();

    // First positional: search term
    let (search_param, rest) = next_positional_token(command)
        .ok_or_else(|| "Missing Search Term".to_string())?;
    if search_param.is_empty() {
        return Err("Missing Search Term".to_string());
    }
    args.insert(CommandFlag::SearchTerm, search_param);
    *command = rest.trim().to_string();

    // Second positional: path (optional, defaults to current directory)
    let target_path = if let Some((path_param, rest2)) = next_positional_token(command) {
        if path_param.is_empty() {
            current_dir_string()
        } else if Path::new(&path_param).exists() {
            let abs = if Path::new(&path_param).is_absolute() {
                path_param.clone()
            } else {
                Path::new(&path_param)
                    .canonicalize()
                    .map(|p| p.to_string_lossy().into_owned())
                    .unwrap_or(path_param.clone())
            };
            *command = rest2.trim().to_string();
            abs
        } else {
            return Err(format!("Specified path does not exist '{}'", path_param));
        }
    } else {
        current_dir_string()
    };

    if !Path::new(&target_path).exists() {
        return Err(format!("File or directory '{}' does not exist", target_path));
    }

    args.insert(CommandFlag::Path, target_path);
    Ok(())
}

/// Extracts the next token (quoted or unquoted) from `s`, returning the token
/// and the remaining input after the token.
fn next_positional_token(s: &str) -> Option<(String, &str)> {
    let s = s.trim_start();
    if s.is_empty() {
        return None;
    }
    let bytes = s.as_bytes();
    let quote = bytes[0];
    if quote == b'\'' || quote == b'"' {
        let closing = quote as char;
        if let Some(end) = s[1..].find(closing) {
            let token = s[1..1 + end].to_string();
            let rest = &s[1 + end + 1..];
            Some((token, rest))
        } else {
            Some((s[1..].to_string(), ""))
        }
    } else {
        let end = s
            .find(|c: char| c.is_whitespace() || c == '\'' || c == '"')
            .unwrap_or(s.len());
        let token = s[..end].to_string();
        Some((token, &s[end..]))
    }
}

fn current_dir_string() -> String {
    std::env::current_dir()
        .map(|p| p.to_string_lossy().into_owned())
        .unwrap_or_else(|_| ".".to_string())
}

/// Trim at most one occurrence of any character in `chars` from start and end.
fn trim_once<'a>(s: &'a str, chars: &[char]) -> &'a str {
    if s.is_empty() {
        return s;
    }
    let bytes = s.as_bytes();
    let start = if chars.iter().any(|&c| bytes[0] == c as u8) { 1 } else { 0 };
    let end = if bytes.len() > start
        && chars.iter().any(|&c| *bytes.last().unwrap() == c as u8)
    {
        bytes.len() - 1
    } else {
        bytes.len()
    };
    &s[start..end]
}
