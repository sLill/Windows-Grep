use std::path::Path;
use std::sync::Arc;
use std::sync::Once;
use std::sync::atomic::AtomicBool;

use windows_grep::services::console_service::ConsoleService;
use windows_grep::services::grep_service::GrepService;
use windows_grep::utils::file_utils::read_pdf;
use windows_grep::utils::windows_grep_utils::parse_grep_command_args;
use windows_grep::utils::windows_utils::{get_file_size_on_disk, get_files};

static ATTR_SETUP: Once = Once::new();

fn test_data_dir() -> std::path::PathBuf {
    let dir = std::path::Path::new(env!("CARGO_MANIFEST_DIR"))
        .join("tests")
        .join("test_data");

    // Git does not preserve NTFS Hidden/System attributes. Apply them once per
    // test process so the hidden/system filter tests behave correctly.
    ATTR_SETUP.call_once(|| {
        let hidden = dir.join("TestData_Hidden");
        let system = dir.join("TestData_System");
        if hidden.exists() {
            let _ = std::process::Command::new("attrib").arg("+H").arg(&hidden).status();
        }
        if system.exists() {
            let _ = std::process::Command::new("attrib").arg("+S").arg(&system).status();
        }
    });

    dir
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

// --- Directory walk ---

#[test]
fn walk_matches_sequential_order() {
    // The walk lists directories in parallel but must return what a sequential
    // depth-first walk would: each directory's entries, then each subdirectory's
    // subtree in listing order.
    fn sequential_walk(dir: &Path, out: &mut Vec<(String, bool)>) {
        let mut subdirs = Vec::new();
        for entry in std::fs::read_dir(dir).unwrap() {
            let path = entry.unwrap().path();
            let is_dir = path.is_dir();
            out.push((path.to_string_lossy().into_owned(), is_dir));
            if is_dir {
                subdirs.push(path);
            }
        }
        for sub in subdirs {
            sequential_walk(&sub, out);
        }
    }

    let tmp = std::env::temp_dir().join("wgrep_walk_order");
    std::fs::remove_dir_all(&tmp).ok();
    for a in 0..4 {
        for b in 0..4 {
            let dir = tmp.join(format!("dir_{}", a)).join(format!("sub_{}", b));
            std::fs::create_dir_all(&dir).unwrap();
            std::fs::write(dir.join("leaf.txt"), "x").unwrap();
        }
        std::fs::write(tmp.join(format!("file_{}.txt", a)), "x").unwrap();
    }

    let mut expected = Vec::new();
    sequential_walk(&tmp, &mut expected);

    let cancelled = AtomicBool::new(false);
    let root = tmp.to_string_lossy();
    let files = get_files(&root, true, usize::MAX, -1, -1, &cancelled, None, false, false, &|_| true);
    let actual: Vec<(String, bool)> = files.iter().map(|f| (f.name.clone(), f.is_directory)).collect();
    assert_eq!(actual, expected);
    assert!(files.iter().all(|f| f.file_size == -1), "no size filter, so nothing should be measured");

    std::fs::remove_dir_all(&tmp).ok();
}

#[test]
fn recursive_follows_junctions() {
    // Directory-ness comes from the listing, which describes a junction rather than
    // its target; the walk must still descend into it.
    let tmp = std::env::temp_dir().join("wgrep_junction");
    std::fs::remove_dir_all(&tmp).ok();
    let target = tmp.join("target");
    std::fs::create_dir_all(&target).unwrap();
    std::fs::write(target.join("inside.txt"), "junction needle").unwrap();
    let status = std::process::Command::new("cmd")
        .args(["/C", "mklink", "/J"])
        .arg(tmp.join("link"))
        .arg(&target)
        .status()
        .unwrap();
    assert!(status.success(), "mklink /J failed");

    let cmd = format!("-r 'junction needle' '{}'", tmp.display());
    let names: Vec<String> = run_grep(&cmd).into_iter().map(|r| r.source_file.name).collect();
    assert!(names.iter().any(|n| n.ends_with("target\\inside.txt")), "missing direct match: {:?}", names);
    assert!(names.iter().any(|n| n.ends_with("link\\inside.txt")), "junction not descended: {:?}", names);

    std::fs::remove_dir_all(&tmp).ok();
}

#[test]
fn piped_results_are_filtered_and_measured() {
    // A piped command searches the previous results' files instead of walking:
    // its -t filter must still apply, and its size summary needs sizes that the
    // first command never measured.
    let td = test_data_dir();
    let first = run_grep(&format!("-r sample '{}'", td.display()));
    assert!(first.iter().any(|r| !r.source_file.name.ends_with(".txt")), "expected non-.txt matches to filter out");

    let console = Arc::new(ConsoleService::new());
    let cancelled = Arc::new(AtomicBool::new(false));
    let args = parse_grep_command_args(&format!("-t txt --filesize-min=1 sample '{}'", td.display())).unwrap();
    let results = GrepService::new(console).run_command(args, Some(first), cancelled);

    assert!(!results.is_empty());
    for r in &results {
        assert!(r.source_file.name.ends_with(".txt"), "-t txt let through '{}'", r.source_file.name);
        assert_eq!(Some(r.source_file.file_size), get_file_size_on_disk(&r.source_file.name).ok(),
            "size not filled in for '{}'", r.source_file.name);
    }
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
fn context_streams_across_line_boundary() {
    // Streaming search is line-by-line, but -c context must still pull bytes
    // from the previous/next line. Match 'X' starts at the first byte of line
    // 2; -c 3 leading context needs 'ef\n' from line 1.
    let tmp = std::env::temp_dir().join("wgrep_cross_line_ctx");
    std::fs::create_dir_all(&tmp).unwrap();
    let dest = tmp.join("multi.txt");
    std::fs::write(&dest, "abcdef\nXyzabc\n").unwrap();

    let cmd = format!("-c 3 'X' '{}'", dest.display());
    let results = run_grep(&cmd);

    assert_eq!(results.len(), 1, "expected one match");
    let r = &results[0];
    assert!(r.leading_context_string.contains("ef"),
        "leading context should reach into the prior line, got {:?}", r.leading_context_string);
    assert!(r.trailing_context_string.contains("yza"),
        "trailing context wrong, got {:?}", r.trailing_context_string);
    assert_eq!(r.line_number, 2, "match is on line 2");

    std::fs::remove_dir_all(&tmp).ok();
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

// --- PDF extraction ---

/// Writes a PDF from its object bodies, numbered from 1; object 1 is the catalog.
fn write_pdf(path: &Path, objects: &[&str]) {
    let mut pdf = b"%PDF-1.4\n".to_vec();
    let mut offsets = Vec::new();
    for (i, body) in objects.iter().enumerate() {
        offsets.push(pdf.len());
        pdf.extend_from_slice(format!("{} 0 obj\n{}\nendobj\n", i + 1, body).as_bytes());
    }
    let xref_offset = pdf.len();
    pdf.extend_from_slice(format!("xref\n0 {}\n0000000000 65535 f \n", objects.len() + 1).as_bytes());
    for offset in offsets {
        pdf.extend_from_slice(format!("{:010} 00000 n \n", offset).as_bytes());
    }
    pdf.extend_from_slice(format!(
        "trailer\n<< /Size {} /Root 1 0 R >>\nstartxref\n{}\n%%EOF\n",
        objects.len() + 1, xref_offset
    ).as_bytes());
    std::fs::write(path, pdf).unwrap();
}

/// Writes a one-page PDF that lopdf parses but pdf-extract panics on: the page
/// selects an /Indexed colour space, a family pdf-extract doesn't model.
fn write_pdf_that_panics_pdf_extract(path: &Path) {
    write_pdf(path, &[
        "<< /Type /Catalog /Pages 2 0 R >>",
        "<< /Type /Pages /Kids [3 0 R] /Count 1 >>",
        "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Contents 4 0 R \
         /Resources << /ColorSpace << /CS0 [/Indexed /DeviceRGB 1 <000000FFFFFF>] >> >> >>",
        "<< /Length 7 >>\nstream\n/CS0 cs\nendstream",
    ]);
}

/// Writes a one-page PDF showing "needle" in a font whose /Differences names code
/// 101 /E while its ToUnicode CMap maps it to "e"; pdf-extract 0.7 printed that
/// conflict to stdout ("Unicode mismatch ...").
fn write_pdf_with_unicode_mismatch(path: &Path) {
    let content = "BT /F1 12 Tf 72 720 Td (needle) Tj ET";
    let cmap = "begincmap\n1 begincodespacerange <00> <FF> endcodespacerange\n\
                1 beginbfrange <20> <7E> <0020> endbfrange\nendcmap";
    write_pdf(path, &[
        "<< /Type /Catalog /Pages 2 0 R >>",
        "<< /Type /Pages /Kids [3 0 R] /Count 1 >>",
        "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Contents 4 0 R \
         /Resources << /Font << /F1 5 0 R >> >> >>",
        &format!("<< /Length {} >>\nstream\n{}\nendstream", content.len(), content),
        "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica \
         /Encoding << /Differences [101 /E] >> /ToUnicode 6 0 R >>",
        &format!("<< /Length {} >>\nstream\n{}\nendstream", cmap.len(), cmap),
    ]);
}

#[test]
fn pdf_extraction_panic_becomes_error() {
    let tmp = std::env::temp_dir().join("wgrep_pdf_panic_read");
    std::fs::create_dir_all(&tmp).unwrap();
    let pdf = tmp.join("indexed.pdf");
    write_pdf_that_panics_pdf_extract(&pdf);

    match read_pdf(&pdf.to_string_lossy()) {
        Err(e) => assert!(e.contains("panicked"), "expected a contained panic, got error {:?}", e),
        Ok(text) => panic!("fixture no longer makes pdf-extract panic (extracted {:?})", text),
    }

    std::fs::remove_dir_all(&tmp).ok();
}

#[test]
fn pdf_extraction_panic_counts_file_as_unreadable() {
    // The panic used to unwind out of the parallel search, ending the process
    // (exit code 101) before any summary was printed.
    let tmp = std::env::temp_dir().join("wgrep_pdf_panic_search");
    std::fs::remove_dir_all(&tmp).ok();
    let docs = tmp.join("docs");
    std::fs::create_dir_all(&docs).unwrap();
    write_pdf_that_panics_pdf_extract(&docs.join("indexed.pdf"));
    std::fs::write(docs.join("notes.txt"), "needle").unwrap();
    let out = tmp.join("output.txt");

    let results = run_grep(&format!("-o {} needle '{}'", out.display(), docs.display()));

    assert_eq!(results.len(), 1, "the search should still cover the text file");
    assert!(results[0].source_file.name.ends_with("notes.txt"));
    let output = std::fs::read_to_string(&out).unwrap();
    assert!(output.contains("[1 file(s) unreadable/inaccessible]"), "PDF not counted as unreadable: {:?}", output);
    assert!(output.contains("[1 result(s) 1 file(s)]"), "summary missing: {:?}", output);

    std::fs::remove_dir_all(&tmp).ok();
}

#[test]
fn pdf_extraction_prints_nothing_of_its_own() {
    // pdf-extract 0.7 println!'d font diagnostics straight to stdout, outside the
    // console lock, so they landed among and inside result lines. stdout must carry
    // exactly what grep published, which -o also copies to a file.
    let tmp = std::env::temp_dir().join("wgrep_pdf_quiet");
    std::fs::remove_dir_all(&tmp).ok();
    std::fs::create_dir_all(&tmp).unwrap();
    let pdf = tmp.join("mismatch.pdf");
    write_pdf_with_unicode_mismatch(&pdf);
    let out = tmp.join("output.txt");

    // The real binary, since in-process prints would vanish into the test harness's capture
    let grep = std::process::Command::new(env!("CARGO_BIN_EXE_grep"))
        .arg("-o").arg(&out).arg("needle").arg(&pdf)
        .output()
        .unwrap();

    assert!(grep.status.success(), "grep exited with {:?}", grep.status);
    assert!(grep.stderr.is_empty(), "stderr: {}", String::from_utf8_lossy(&grep.stderr));
    let published = std::fs::read_to_string(&out).unwrap();
    assert!(published.contains("[1 result(s) 1 file(s)]"), "PDF text not searched: {:?}", published);
    let ansi = regex::Regex::new("\x1b\\[[0-9;]*m").unwrap();
    let stdout = String::from_utf8(grep.stdout).unwrap();
    assert_eq!(ansi.replace_all(&stdout, ""), published, "stdout carries more than grep published");

    std::fs::remove_dir_all(&tmp).ok();
}
