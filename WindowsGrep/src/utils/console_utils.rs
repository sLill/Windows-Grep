use std::sync::Arc;
use crate::services::console_service::ConsoleService;

// Resource files embed proper ANSI escape bytes directly.
const SPLASH: &str = include_str!("../../resources/splash.txt");
const HELP: &str = include_str!("../../resources/help.txt");
const HELP_EXTENDED: &str = include_str!("../../resources/help_extended.txt");

pub fn publish_splash(console: &Arc<ConsoleService>) {
    console.write_raw(SPLASH);
    console.write_raw("\n");
}

pub fn publish_help(console: &Arc<ConsoleService>, extended: bool) {
    let content = if extended { HELP_EXTENDED } else { HELP };
    console.write_raw(content);
    console.write_raw("\n");
}

pub fn publish_prompt(console: &Arc<ConsoleService>) {
    console.write_raw("$ ");
}

pub fn clear_console() {
    print!("\x1b[2J\x1b[H");
}
