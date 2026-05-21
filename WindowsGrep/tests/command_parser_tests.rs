use windows_grep::enums::command_flag::CommandFlag;
use windows_grep::utils::windows_grep_utils::parse_grep_command_args;

fn td() -> String {
    std::path::Path::new(env!("CARGO_MANIFEST_DIR"))
        .parent().unwrap()
        .join("WindowsGrep")
        .join("WindowsGrep.Test")
        .join("Properties")
        .join("Resources")
        .join("TestData")
        .to_string_lossy()
        .into_owned()
}

// --- CommandParser_SearchTerm_Tests ---

#[test]
fn search_term_valid() {
    let cases = [
        "-ri search_term .",
        "-ri search_term",
        "search_term .",
        "search_term",
        "-ri \"search_term\" .",
        "-ri \"search_term\"",
        "\"search_term\" .",
        "\"search_term\"",
        "-ri 'search_term' .",
        "-ri 'search_term'",
        "'search_term' .",
        "'search_term'",
    ];
    for cmd in &cases {
        let args = parse_grep_command_args(cmd)
            .unwrap_or_else(|e| panic!("'{}' failed: {}", cmd, e));
        assert!(args.contains_key(&CommandFlag::SearchTerm), "no SearchTerm for '{}'", cmd);
    }
}

// --- CommandParser_Path_Tests ---

#[test]
fn path_valid() {
    let td = td();
    let cases = [
        "search_term".to_string(),
        format!("-ri search_term {}", td),
        format!("search_term {}", td),
        "search_term .".to_string(),
        format!("-ri search_term \"{}\"", td),
        format!("search_term \"{}\"", td),
        format!("-ri search_term '{}'", td),
        format!("search_term '{}'", td),
    ];
    for cmd in &cases {
        let args = parse_grep_command_args(cmd)
            .unwrap_or_else(|e| panic!("'{}' failed: {}", cmd, e));
        assert!(args.contains_key(&CommandFlag::Path), "no Path for '{}'", cmd);
    }
}

#[test]
fn path_invalid() {
    assert!(
        parse_grep_command_args("search_term U:/Path/Does/Not/Exist").is_err(),
        "expected error for non-existent path"
    );
}

// --- CommandParser_Flag_Tests ---

#[test]
fn short_descriptors_basic_valid() {
    let cases: &[(&str, &[CommandFlag])] = &[
        ("-r -i search_term .", &[CommandFlag::Recursive, CommandFlag::IgnoreCase]),
        ("-r -k -i search_term", &[CommandFlag::Recursive, CommandFlag::IgnoreCase, CommandFlag::FileNamesOnly]),
        ("-rk -i search_term .", &[CommandFlag::Recursive, CommandFlag::IgnoreCase, CommandFlag::FileNamesOnly]),
        ("-rk -i search_term", &[CommandFlag::Recursive, CommandFlag::IgnoreCase, CommandFlag::FileNamesOnly]),
        ("-r --ignore-breaks -k -i search_term", &[CommandFlag::Recursive, CommandFlag::IgnoreCase, CommandFlag::FileNamesOnly, CommandFlag::IgnoreBreaks]),
        ("-r --ignore-breaks -k -i search_term .", &[CommandFlag::Recursive, CommandFlag::IgnoreCase, CommandFlag::FileNamesOnly, CommandFlag::IgnoreBreaks]),
    ];
    for (cmd, expected_flags) in cases {
        let args = parse_grep_command_args(cmd)
            .unwrap_or_else(|e| panic!("'{}' failed: {}", cmd, e));
        for flag in *expected_flags {
            assert!(args.contains_key(flag), "missing {:?} for '{}'", flag, cmd);
        }
    }
}

#[test]
fn short_descriptors_parameters_valid() {
    let cases: &[(&str, &[CommandFlag])] = &[
        ("-r -t '.cpp;.txt' search_term .", &[CommandFlag::Recursive, CommandFlag::FileTypeIncludeFilter]),
        ("-r -t .cpp;.txt -i search_term .", &[CommandFlag::Recursive, CommandFlag::IgnoreCase, CommandFlag::FileTypeIncludeFilter]),
        ("-r -t .cpp,.txt search_term .", &[CommandFlag::Recursive, CommandFlag::FileTypeIncludeFilter]),
        ("-r -i -c 20 search_term .", &[CommandFlag::Recursive, CommandFlag::IgnoreCase, CommandFlag::Context]),
        ("-r -c 20 -i search_term", &[CommandFlag::Recursive, CommandFlag::IgnoreCase, CommandFlag::Context]),
        ("-c 20 -rk -i search_term .", &[CommandFlag::Context, CommandFlag::Recursive, CommandFlag::IgnoreCase, CommandFlag::FileNamesOnly]),
        ("-c 20 --ignore-breaks -k -i search_term", &[CommandFlag::Context, CommandFlag::IgnoreBreaks, CommandFlag::IgnoreCase, CommandFlag::FileNamesOnly]),
    ];
    let param_flags = [CommandFlag::FileTypeIncludeFilter, CommandFlag::Context];
    for (cmd, expected_flags) in cases {
        let args = parse_grep_command_args(cmd)
            .unwrap_or_else(|e| panic!("'{}' failed: {}", cmd, e));
        for flag in *expected_flags {
            assert!(args.contains_key(flag), "missing {:?} for '{}'", flag, cmd);
            if param_flags.contains(flag) {
                assert!(!args[flag].is_empty(), "{:?} param empty for '{}'", flag, cmd);
            }
        }
    }
}

#[test]
fn short_descriptors_invalid() {
    let cases = [
        "-r search_term -k",
        "-r search_term -k .",
        "-z search_term",
    ];
    for cmd in &cases {
        assert!(parse_grep_command_args(cmd).is_err(), "expected error for '{}'", cmd);
    }
}
