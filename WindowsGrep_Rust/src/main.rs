mod enums;
mod models;
mod services;
mod utils;

use std::io::{self, BufRead, Write as _};
use std::sync::Arc;
use std::sync::atomic::{AtomicBool, Ordering};

use models::grep_result::GrepResult;
use services::console_service::ConsoleService;
use services::grep_service::GrepService;
use services::native_service::NativeService;
use utils::console_utils;
use utils::windows_grep_utils;
use utils::windows_utils;
use colored::Color;
use models::console_item::ConsoleItem;
use enums::command_flag::CommandFlag;

fn main() {
    windows_utils::try_enable_ansi();

    let args: Vec<String> = std::env::args().skip(1).collect();
    let cancelled = Arc::new(AtomicBool::new(false));
    let console = Arc::new(ConsoleService::new());

    // Handle Ctrl+C: set the cancellation flag rather than killing the process
    {
        let c = Arc::clone(&cancelled);
        let _ = ctrlc::set_handler(move || {
            c.store(true, Ordering::SeqCst);
        });
    }

    if args.is_empty() {
        // REPL mode
        console_utils::publish_splash(&console);
        repl_loop(console, cancelled);
    } else {
        // Single command from CLI args
        let command = args
            .iter()
            .map(|a| if a.contains(' ') { format!("\"{}\"", a) } else { a.clone() })
            .collect::<Vec<_>>()
            .join(" ");
        run_command(&command, &console, Arc::clone(&cancelled));
    }
}

fn repl_loop(console: Arc<ConsoleService>, cancelled: Arc<AtomicBool>) {
    let stdin = io::stdin();
    loop {
        console_utils::publish_prompt(&console);
        let _ = io::stdout().flush();

        let mut line = String::new();
        match stdin.lock().read_line(&mut line) {
            Ok(0) => break, // EOF
            Ok(_) => {}
            Err(_) => break,
        }

        let line = line.trim().to_string();
        if line.is_empty() {
            continue;
        }
        if line.eq_ignore_ascii_case("exit") || line.eq_ignore_ascii_case("quit") {
            break;
        }

        cancelled.store(false, Ordering::SeqCst);
        run_command(&line, &console, Arc::clone(&cancelled));
    }
}

fn run_command(command: &str, console: &Arc<ConsoleService>, cancelled: Arc<AtomicBool>) {
    let commands = windows_grep_utils::split_commands(command);
    let mut previous_results: Option<Vec<GrepResult>> = None;

    for cmd in &commands {
        let cmd = cmd.trim();
        if cmd.is_empty() {
            continue;
        }
        if cancelled.load(Ordering::SeqCst) {
            break;
        }

        if let Some(native_cmd) = windows_grep_utils::parse_native_command_args(cmd) {
            NativeService::run(native_cmd, console);
            previous_results = None;
        } else {
            match windows_grep_utils::parse_grep_command_args(cmd) {
                Ok(args) => {
                    if args.contains_key(&CommandFlag::HelpFull) {
                        console_utils::publish_help(console, true);
                        previous_results = None;
                        continue;
                    }
                    if args.contains_key(&CommandFlag::Help) {
                        console_utils::publish_help(console, false);
                        previous_results = None;
                        continue;
                    }

                    let service = GrepService::new(Arc::clone(console));
                    let results = service.run_command(args, previous_results, Arc::clone(&cancelled));
                    previous_results = Some(results);
                }
                Err(e) => {
                    console.write(&ConsoleItem::with_fg(format!("\n{}\n", e), Color::BrightRed));
                    console.write(&ConsoleItem::with_fg(
                        "Usage:   grep [options] search_term [path]\n\n",
                        Color::TrueColor { r: 255, g: 165, b: 0 },
                    ));
                    previous_results = None;
                }
            }
        }
    }
}
