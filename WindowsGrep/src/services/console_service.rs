use std::fmt::Display;
use std::io::{self, Stdout, Write};
use std::sync::atomic::{AtomicBool, Ordering};
use std::sync::{Arc, OnceLock};
use crate::models::console_item::ConsoleItem;

pub struct ConsoleService {
    /// None for a console that discards its output.
    stdout: Option<Stdout>,
    /// The first failed write to stdout; nothing is written after it.
    write_error: OnceLock<io::Error>,
    /// Set along with `write_error`: the flag Ctrl+C sets to cancel a search.
    cancelled: Arc<AtomicBool>,
}

impl ConsoleService {
    pub fn new(cancelled: Arc<AtomicBool>) -> Self {
        ConsoleService { stdout: Some(io::stdout()), write_error: OnceLock::new(), cancelled }
    }

    /// Discards all output, for callers that only use the returned results.
    pub fn silent() -> Self {
        ConsoleService { stdout: None, write_error: OnceLock::new(), cancelled: Arc::default() }
    }

    pub fn write(&self, item: &ConsoleItem) {
        self.print_all(std::slice::from_ref(item));
    }

    pub fn write_items(&self, items: &[ConsoleItem]) {
        self.print_all(items);
    }

    pub fn write_raw(&self, text: &str) {
        self.print_all(&[text]);
    }

    /// The error that stopped output, if a write to stdout failed.
    pub fn write_error(&self) -> Option<&io::Error> {
        self.write_error.get()
    }

    fn print_all(&self, items: &[impl Display]) {
        let Some(stdout) = &self.stdout else { return };
        // Hold stdout for the whole slice, so no other output, including code printing
        // straight to stdout as pdf-extract 0.7 did from worker threads, lands mid-slice.
        let mut out = stdout.lock();
        if self.write_error.get().is_some() {
            return;
        }
        let written = items
            .iter()
            .try_for_each(|item| write!(out, "{}", item))
            .and_then(|()| out.flush());
        // print! would panic here instead, in every worker still publishing results
        if let Err(e) = written {
            let _ = self.write_error.set(e);
            self.cancelled.store(true, Ordering::SeqCst);
        }
    }
}
