use std::sync::Arc;
use std::io::Write as _;

use crate::enums::native_command_type::NativeCommandType;
use crate::models::console_item::ConsoleItem;
use crate::models::native_command::NativeCommand;
use crate::services::console_service::ConsoleService;
use colored::Color;

use crate::utils::windows_utils;

pub struct NativeService;

impl NativeService {
    pub fn run(command: NativeCommand, console: &Arc<ConsoleService>) {
        match command.command_type {
            NativeCommandType::List => list_files(console),
            NativeCommandType::ChangeDirectory => {
                let param = command.command_parameter.trim().to_string();
                let target = if param.is_empty() {
                    std::env::var("HOME")
                        .or_else(|_| std::env::var("USERPROFILE"))
                        .unwrap_or_else(|_| ".".to_string())
                } else {
                    param
                };
                if let Err(e) = std::env::set_current_dir(&target) {
                    console.write(&ConsoleItem::with_fg(
                        format!("{}\n", e),
                        Color::BrightRed,
                    ));
                }
            }
            NativeCommandType::ClearConsole => {
                // ANSI clear screen + move cursor to top
                print!("\x1b[2J\x1b[H");
                let _ = std::io::stdout().flush();
            }
            NativeCommandType::PrintWorkingDirectory => {
                let cwd = std::env::current_dir()
                    .map(|p| p.to_string_lossy().into_owned())
                    .unwrap_or_default();
                console.write(&ConsoleItem::new(format!("{}\n", cwd)));
            }
        }
    }
}

fn list_files(console: &Arc<ConsoleService>) {
    let cwd = match std::env::current_dir() {
        Ok(p) => p.to_string_lossy().into_owned(),
        Err(e) => {
            console.write(&ConsoleItem::with_fg(format!("{}\n", e), Color::BrightRed));
            return;
        }
    };

    use std::sync::atomic::AtomicBool;
    let cancelled = AtomicBool::new(false);

    // List without recursion, excluding hidden and system files
    let files = windows_utils::get_files(&cwd, false, usize::MAX, -1, -1, &cancelled, None, true, true);

    for file in files {
        let mut items = Vec::new();

        let color = if file.is_directory { Color::BrightCyan } else { Color::Yellow };
        items.push(ConsoleItem::with_fg(file.name.clone(), color));

        // File size
        if !file.is_directory && file.file_size > -1 {
            let (v, t) = windows_utils::get_reduced_size(file.file_size, 3);
            items.push(ConsoleItem::with_fg(
                format!(" {} {}(s)", v, t),
                Color::BrightGreen,
            ));
        }

        items.push(ConsoleItem::new("\n"));
        console.write_items(&items);
    }
}
