use std::path::Path;
use std::sync::Arc;
use std::sync::atomic::AtomicBool;

use windows_grep::services::console_service::ConsoleService;
use windows_grep::services::grep_service::GrepService;
use windows_grep::utils::windows_grep_utils::parse_grep_command_args;
use windows_grep::utils::windows_utils::get_file_size_on_disk;

fn test_data_dir() -> std::path::PathBuf {
    std::path::Path::new(env!("CARGO_MANIFEST_DIR"))
        .parent().unwrap()
        .join("WindowsGrep")
        .join("WindowsGrep.Test")
        .join("Properties")
        .join("Resources")
        .join("TestData")
}

fn run_grep(command: &str) -> Vec<windows_grep::models::grep_result::GrepResult> {
    let console = Arc::new(ConsoleService::new());
    let cancelled = Arc::new(AtomicBool::new(false));
    let args = parse_grep_command_args(command)
        .unwrap_or_else(|e| panic!("parse failed for '{}': {}", command, e));
    GrepService::new(console).run_command(args, None, cancelled)
}

// --- NoFlags ---

#[test]
fn no_flag_phrase() {
    let td = test_data_dir();
    let cmd = format!("'This is sample text' '{}'", td.display());
    assert!(!run_grep(&cmd).is_empty());
}

#[test]
fn no_flag_word() {
    let td = test_data_dir();
    let cmd = format!("sample '{}'", td.display());
    assert!(!run_grep(&cmd).is_empty());
}

// --- IgnoreCase ---

#[test]
fn ignore_case_enabled() {
    let td = test_data_dir();
    let cmd = format!("-i 'THIS IS SAMPLE TEXT' '{}'", td.display());
    assert!(!run_grep(&cmd).is_empty());
}

#[test]
fn ignore_case_disabled() {
    let td = test_data_dir();
    let cmd = format!("'THIS IS SAMPLE TEXT' '{}'", td.display());
    assert!(run_grep(&cmd).is_empty());
}

// --- Recursive ---

#[test]
fn recursive_enabled() {
    let td = test_data_dir();
    let cmd = format!("-r 'sample' '{}'", td.display());
    let results = run_grep(&cmd);
    let dirs: std::collections::HashSet<String> = results
        .iter()
        .filter_map(|r| Path::new(&r.source_file.name).parent().map(|p| p.to_string_lossy().into_owned()))
        .collect();
    assert!(dirs.len() > 1, "expected results from multiple directories");
}

#[test]
fn recursive_disabled() {
    let td = test_data_dir();
    let cmd = format!("'sample' '{}'", td.display());
    let results = run_grep(&cmd);
    let dirs: std::collections::HashSet<String> = results
        .iter()
        .filter_map(|r| Path::new(&r.source_file.name).parent().map(|p| p.to_string_lossy().into_owned()))
        .collect();
    assert_eq!(dirs.len(), 1, "expected results from exactly one directory");
}

// --- FileNamesOnly (-k) ---

#[test]
fn filename_enabled() {
    let td = test_data_dir();
    let cmd = format!("-k 'TestData_One' '{}'", td.display());
    assert_eq!(run_grep(&cmd).len(), 1);
}

#[test]
fn filename_disabled() {
    let td = test_data_dir();
    let cmd = format!("'TestData_One' '{}'", td.display());
    assert!(run_grep(&cmd).is_empty());
}

// --- Context (-c) ---

#[test]
fn context_with_flag() {
    let td = test_data_dir();
    let cmd = format!("-c 1 'hitespac' '{}'", td.display());
    let results = run_grep(&cmd);
    assert!(!results.is_empty());
    for r in &results {
        assert_eq!(r.leading_context_string.trim().len(), 1, "expected leading context length 1");
        assert_eq!(r.trailing_context_string.trim().len(), 1, "expected trailing context length 1");
    }
}

#[test]
fn context_without_flag() {
    let td = test_data_dir();
    let cmd = format!("'hitespac' '{}'", td.display());
    let results = run_grep(&cmd);
    assert!(!results.is_empty());
    for r in &results {
        assert_eq!(r.leading_context_string.trim().len(), 0, "expected empty leading context");
        assert_eq!(r.trailing_context_string.trim().len(), 0, "expected empty trailing context");
    }
}

#[test]
fn context_spans_utf8_boundary() {
    // Match is ASCII 'X' surrounded by 3-byte UTF-8 chars. A context length of 1
    // byte lands mid-codepoint on both sides — would panic before the fix.
    let tmp = std::env::temp_dir().join("wgrep_utf8_context");
    std::fs::create_dir_all(&tmp).unwrap();
    let dest = tmp.join("utf8.txt");
    std::fs::write(&dest, "日本語Xテスト").unwrap();

    let cmd = format!("-c 1 'X' '{}'", dest.display());
    let results = run_grep(&cmd);

    assert_eq!(results.len(), 1, "expected exactly one match");
    let r = &results[0];
    // Boundary-rounding expands the slice to include the whole adjacent char.
    assert!(r.leading_context_string.trim().ends_with('語'),
        "leading context should include the preceding char, got {:?}", r.leading_context_string);
    assert!(r.trailing_context_string.trim().starts_with('テ'),
        "trailing context should include the following char, got {:?}", r.trailing_context_string);

    std::fs::remove_dir_all(&tmp).ok();
}

// --- FileTypeInclude (-t) ---

#[test]
fn filetype_include() {
    let td = test_data_dir();
    let cases: &[(&str, &[&str])] = &[
        ("-r -i -k -t txt \".*\"", &[".txt"]),
        ("-t .cpp 'Hello, World!'", &[".cpp"]),
        ("-t .cpp,.go 'Hello, World!'", &[".cpp", ".go"]),
        ("-t cpp 'Hello, World!'", &[".cpp"]),
        ("-t '.cpp,.go' 'Hello, World!'", &[".cpp", ".go"]),
        ("-t \".cpp,.go\" 'Hello, World!'", &[".cpp", ".go"]),
    ];
    for (flags, valid_exts) in cases {
        let cmd = format!("{} '{}'", flags, td.display());
        let results = run_grep(&cmd);
        for r in &results {
            let ext = Path::new(&r.source_file.name)
                .extension()
                .map(|e| format!(".{}", e.to_string_lossy().to_lowercase()))
                .unwrap_or_default();
            assert!(
                valid_exts.contains(&ext.as_str()),
                "unexpected extension '{}' for cmd '{}'", ext, cmd
            );
        }
    }
}

// --- FileTypeExclude (-T) ---

#[test]
fn filetype_exclude() {
    let td = test_data_dir();
    let cases: &[(&str, &[&str])] = &[
        ("-T .cpp 'Hello, World!'", &[".cpp"]),
        ("-T .cpp,.go 'Hello, World!'", &[".cpp", ".go"]),
        ("-T '.cpp,.go' 'Hello, World!'", &[".cpp", ".go"]),
        ("-T \".cpp,.go\" 'Hello, World!'", &[".cpp", ".go"]),
        ("-T cpp 'Hello, World!'", &[".cpp"]),
    ];
    for (flags, excluded_exts) in cases {
        let cmd = format!("{} '{}'", flags, td.display());
        let results = run_grep(&cmd);
        for r in &results {
            let ext = Path::new(&r.source_file.name)
                .extension()
                .map(|e| format!(".{}", e.to_string_lossy().to_lowercase()))
                .unwrap_or_default();
            assert!(
                !excluded_exts.contains(&ext.as_str()),
                "excluded extension '{}' appeared for cmd '{}'", ext, cmd
            );
        }
    }
}

// --- PathInclude (-p) ---

#[test]
fn path_include() {
    let td = test_data_dir();
    let cases: &[(&str, &[&str])] = &[
        ("-r -p TestData_0 'This is sample text'", &["TestData_0"]),
        ("-r -p TestData_ 'This is sample text'", &["TestData_0"]),
        ("-rk -p 'Test Data (x86)' .*", &["Test Data (x86)"]),
        ("-rk -p '(x86),TestData_' .*", &["Test Data (x86)", "TestData_0"]),
        ("-rk -p 'Test Data (x86)',TestData_ .*", &["Test Data (x86)", "TestData_0"]),
    ];
    for (flags, include_paths) in cases {
        let cmd = format!("{} '{}'", flags, td.display());
        let results = run_grep(&cmd);
        for r in &results {
            let dir = Path::new(&r.source_file.name)
                .parent()
                .map(|p| p.to_string_lossy().into_owned())
                .unwrap_or_default();
            assert!(
                include_paths.iter().any(|p| dir.contains(p)),
                "result '{}' not in any include path {:?} for cmd '{}'", r.source_file.name, include_paths, cmd
            );
        }
    }
}

// --- PathExclude (-P) ---

#[test]
fn path_exclude() {
    let td = test_data_dir();
    let cases: &[(&str, &[&str])] = &[
        ("-r -P TestData_0 'This is sample text'", &["TestData_0"]),
        ("-r -P TestData_ 'This is sample text'", &["TestData_0"]),
        ("-rk -P 'Test Data (x86)' .*", &["Test Data (x86)"]),
        ("-rk -P '(x86),TestData_' .*", &["Test Data (x86)", "TestData_0"]),
        ("-rk -P 'Test Data (x86)',TestData_ .*", &["Test Data (x86)", "TestData_0"]),
    ];
    for (flags, exclude_paths) in cases {
        let cmd = format!("{} '{}'", flags, td.display());
        let results = run_grep(&cmd);
        for r in &results {
            let dir = Path::new(&r.source_file.name)
                .parent()
                .map(|p| p.to_string_lossy().into_owned())
                .unwrap_or_default();
            assert!(
                !exclude_paths.iter().any(|p| dir.contains(p)),
                "result '{}' in excluded path {:?} for cmd '{}'", r.source_file.name, exclude_paths, cmd
            );
        }
    }
}

// --- FixedString / Plaintext (-F) ---

#[test]
fn plaintext_enabled() {
    // '*Markdown' is a regex special char; -F treats it literally
    let td = test_data_dir();
    let cmd = format!("-F '*Markdown' '{}'", td.display());
    assert!(!run_grep(&cmd).is_empty());
}

#[test]
fn plaintext_disabled() {
    // Without -F, 'M\Srkdown' is a regex that matches 'Markdown'
    let td = test_data_dir();
    let cmd = format!("'M\\Srkdown' '{}'", td.display());
    assert!(!run_grep(&cmd).is_empty());
}

#[test]
fn plaintext_treats_dollar_as_literal() {
    // -F must treat '$' as a literal char. Before the fix, the search term was
    // rewritten to 'price[\r\n]*$5' *before* being escaped, so it could never
    // match the literal 'price$5'.
    let tmp = std::env::temp_dir().join("wgrep_fixed_dollar");
    std::fs::create_dir_all(&tmp).unwrap();
    let dest = tmp.join("price.txt");
    std::fs::write(&dest, "The product costs price$5 today.").unwrap();

    let cmd = format!("-F 'price$5' '{}'", dest.display());
    let results = run_grep(&cmd);
    assert_eq!(results.len(), 1, "expected one literal '$' match");
    assert_eq!(results[0].matched_string, "price$5");

    std::fs::remove_dir_all(&tmp).ok();
}

// --- Replace (--replace) ---

#[test]
fn replace_content() {
    let tmp = std::env::temp_dir().join("wgrep_replace_content");
    std::fs::create_dir_all(&tmp).unwrap();
    let src = test_data_dir().join("TestData_One.txt");
    let dest = tmp.join("ReplaceTest_Blue.txt");
    std::fs::copy(&src, &dest).unwrap();

    let cmd = format!("--replace='text sample is This' 'This is sample text' '{}'", dest.display());
    run_grep(&cmd);

    let content = std::fs::read_to_string(&dest).unwrap();
    assert!(content.contains("text sample is This"), "replacement not found in file");

    std::fs::remove_dir_all(&tmp).ok();
}

#[test]
fn replace_filename() {
    let tmp = std::env::temp_dir().join("wgrep_replace_filename");
    std::fs::create_dir_all(&tmp).unwrap();
    let src = test_data_dir().join("TestData_One.txt");
    std::fs::copy(&src, tmp.join("ReplaceTest_Blue.txt")).unwrap();

    let cmd = format!("-k --replace=Red 'Blue' '{}'", tmp.display());
    run_grep(&cmd);

    assert!(tmp.join("ReplaceTest_Red.txt").exists(), "renamed file not found");

    std::fs::remove_dir_all(&tmp).ok();
}

#[test]
fn replace_filename_regex() {
    let tmp = std::env::temp_dir().join("wgrep_replace_filename_re");
    std::fs::create_dir_all(&tmp).unwrap();
    let src = test_data_dir().join("TestData_One.txt");
    std::fs::copy(&src, tmp.join("ReplaceTest_Blue.txt")).unwrap();

    let cmd = format!("-k --replace=Red 'Bl[^\\.]*' '{}'", tmp.display());
    run_grep(&cmd);

    assert!(tmp.join("ReplaceTest_Red.txt").exists(), "renamed file not found");

    std::fs::remove_dir_all(&tmp).ok();
}

#[test]
fn replace_filename_subdirectory() {
    let tmp = std::env::temp_dir().join("wgrep_replace_subdir");
    let sub = tmp.join("ReplaceTest_Green");
    std::fs::create_dir_all(&sub).unwrap();
    let src = test_data_dir().join("TestData_One.txt");
    std::fs::copy(&src, sub.join("ReplaceTest_Green.txt")).unwrap();

    let cmd = format!("-r -k --replace=Red 'Green' '{}'", tmp.display());
    run_grep(&cmd);

    assert!(
        tmp.join("ReplaceTest_Green").join("ReplaceTest_Red.txt").exists(),
        "renamed file in subdirectory not found"
    );

    std::fs::remove_dir_all(&tmp).ok();
}

// --- Delete (--delete) ---

#[test]
fn delete_content_match() {
    let tmp = std::env::temp_dir().join("wgrep_delete_content");
    std::fs::create_dir_all(&tmp).unwrap();
    let dest = tmp.join("DeleteTest_Blue.txt");
    std::fs::write(&dest, "The quick brown fox jumps over the lazy dog").unwrap();

    let cmd = format!("--delete 'quick brown fox' '{}'", dest.display());
    let results = run_grep(&cmd);

    for r in &results {
        assert!(!Path::new(&r.source_file.name).exists(),
            "file should have been deleted: {}", r.source_file.name);
    }

    std::fs::remove_dir_all(&tmp).ok();
}

#[test]
fn delete_filename_match() {
    let tmp = std::env::temp_dir().join("wgrep_delete_filename");
    std::fs::create_dir_all(&tmp).unwrap();
    std::fs::write(tmp.join("DeleteTest_Blue.txt"), "The quick brown fox jumps over the lazy dog").unwrap();

    let cmd = format!("-k --delete 'DeleteTest_' '{}'", tmp.display());
    let results = run_grep(&cmd);

    for r in &results {
        assert!(!Path::new(&r.source_file.name).exists(),
            "file should have been deleted: {}", r.source_file.name);
    }

    std::fs::remove_dir_all(&tmp).ok();
}

#[test]
fn delete_filename_recursive() {
    let tmp = std::env::temp_dir().join("wgrep_delete_recursive");
    let sub = tmp.join("DeleteTest_Green");
    std::fs::create_dir_all(&sub).unwrap();
    std::fs::write(tmp.join("DeleteTest_Blue.txt"), "fox").unwrap();
    std::fs::write(sub.join("DeleteTest_Green.txt"), "fox").unwrap();

    let cmd = format!("-r -k --delete 'DeleteTest_' '{}'", tmp.display());
    let results = run_grep(&cmd);

    for r in &results {
        assert!(!Path::new(&r.source_file.name).exists(),
            "file should have been deleted: {}", r.source_file.name);
    }

    std::fs::remove_dir_all(&tmp).ok();
}

// --- IncludeHidden (--hidden) ---

#[test]
fn include_hidden_content_enabled() {
    let td = test_data_dir();
    let cmd = format!("--hidden 'This is a hidden file' '{}'", td.display());
    assert!(!run_grep(&cmd).is_empty());
}

#[test]
fn include_hidden_content_disabled() {
    let td = test_data_dir();
    let cmd = format!("'This is a hidden file' '{}'", td.display());
    assert!(run_grep(&cmd).is_empty());
}

#[test]
fn include_hidden_filename_enabled() {
    let td = test_data_dir();
    let cmd = format!("-k --hidden '_Hidden' '{}'", td.display());
    assert!(!run_grep(&cmd).is_empty());
}

#[test]
fn include_hidden_filename_disabled() {
    let td = test_data_dir();
    let cmd = format!("-k '_Hidden' '{}'", td.display());
    assert!(run_grep(&cmd).is_empty());
}

// --- IncludeSystem (--system) ---

#[test]
fn include_system_content_enabled() {
    let td = test_data_dir();
    let cmd = format!("--system 'This is a system file' '{}'", td.display());
    assert!(!run_grep(&cmd).is_empty());
}

#[test]
fn include_system_content_disabled() {
    let td = test_data_dir();
    let cmd = format!("'This is a system file' '{}'", td.display());
    assert!(run_grep(&cmd).is_empty());
}

#[test]
fn include_system_filename_enabled() {
    let td = test_data_dir();
    let cmd = format!("-k --system '_System' '{}'", td.display());
    assert!(!run_grep(&cmd).is_empty());
}

#[test]
fn include_system_filename_disabled() {
    let td = test_data_dir();
    let cmd = format!("-k '_System' '{}'", td.display());
    assert!(run_grep(&cmd).is_empty());
}

// --- MaxDepth (--max-depth) ---

#[test]
fn max_depth_limits_results() {
    let td = test_data_dir();
    let td_str = td.to_string_lossy();
    for max_depth in [0usize, 1] {
        let cmd = format!("-r --max-depth={} 'This is sample text' '{}'", max_depth, td_str);
        let results = run_grep(&cmd);
        assert!(!results.is_empty(), "expected results for max-depth={}", max_depth);
        for r in &results {
            let rel = Path::new(&r.source_file.name)
                .strip_prefix(&td)
                .map(|p| p.components().count().saturating_sub(1))
                .unwrap_or(0);
            assert!(rel <= max_depth,
                "depth {} exceeds max-depth {} for '{}'", rel, max_depth, r.source_file.name);
        }
    }
}

#[test]
fn max_depth_disabled_finds_deep() {
    let td = test_data_dir();
    let cmd = format!("-r 'This is sample text' '{}'", td.display());
    let results = run_grep(&cmd);
    let has_deep = results.iter().any(|r| {
        Path::new(&r.source_file.name)
            .strip_prefix(&td)
            .map(|p| p.components().count() > 1)
            .unwrap_or(false)
    });
    assert!(has_deep, "expected at least one result from a subdirectory");
}

// --- FileSizeMax (--filesize-max) ---

#[test]
fn filesize_max() {
    let td = test_data_dir();
    let cases: &[(&str, i64)] = &[
        ("--filesize-max=5kb", 5000),
        ("--filesize-max=5KB", 5000),
        ("--filesize-max=5Kb", 5000),
        ("--filesize-max=5000", 5000),
    ];
    for (flag, max_size) in cases {
        let cmd = format!("{} 'sample' '{}'", flag, td.display());
        let results = run_grep(&cmd);
        for r in &results {
            let size = get_file_size_on_disk(&r.source_file.name).unwrap_or(0);
            assert!(size <= *max_size,
                "file '{}' size {} exceeds max {} for flag '{}'",
                r.source_file.name, size, max_size, flag);
        }
    }
}

// --- FileSizeMin (--filesize-min) ---

#[test]
fn filesize_min() {
    let td = test_data_dir();
    let cases: &[(&str, i64)] = &[
        ("--filesize-min=5kb", 5000),
        ("--filesize-min=5KB", 5000),
        ("--filesize-min=5Kb", 5000),
        ("--filesize-min=5000", 5000),
    ];
    for (flag, min_size) in cases {
        let cmd = format!("{} 'sample' '{}'", flag, td.display());
        let results = run_grep(&cmd);
        for r in &results {
            let size = get_file_size_on_disk(&r.source_file.name).unwrap_or(0);
            assert!(size >= *min_size,
                "file '{}' size {} is below min {} for flag '{}'",
                r.source_file.name, size, min_size, flag);
        }
    }
}

// --- OutFile (-o) ---

#[test]
fn outfile_enabled() {
    let td = test_data_dir();
    let tmp = std::env::temp_dir().join("wgrep_outfile");
    std::fs::create_dir_all(&tmp).unwrap();

    let cases = [
        tmp.join("output.txt"),
        tmp.join("subdir").join("output.txt"),
    ];

    for out_path in &cases {
        // Pass the output path unquoted so the parser stores it without quote characters.
        // Paths with spaces would need quoting, but the temp dir path won't have them.
        let cmd = format!("-o {} 'This is sample text' '{}'", out_path.display(), td.display());
        run_grep(&cmd);

        assert!(out_path.exists(), "output file not created: {}", out_path.display());
        assert!(out_path.metadata().unwrap().len() > 0, "output file is empty");
    }

    std::fs::remove_dir_all(&tmp).ok();
}
