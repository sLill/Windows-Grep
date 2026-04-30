use std::io::Write;
use std::sync::Mutex;
use crate::models::console_item::ConsoleItem;

pub struct ConsoleService {
    lock: Mutex<()>,
}

impl ConsoleService {
    pub fn new() -> Self {
        ConsoleService { lock: Mutex::new(()) }
    }

    pub fn write(&self, item: &ConsoleItem) {
        let _guard = self.lock.lock().unwrap_or_else(|p| p.into_inner());
        print!("{}", item);
        let _ = std::io::stdout().flush();
    }

    pub fn write_items(&self, items: &[ConsoleItem]) {
        let _guard = self.lock.lock().unwrap_or_else(|p| p.into_inner());
        for item in items {
            print!("{}", item);
        }
        let _ = std::io::stdout().flush();
    }

    pub fn write_raw(&self, text: &str) {
        let _guard = self.lock.lock().unwrap_or_else(|p| p.into_inner());
        print!("{}", text);
        let _ = std::io::stdout().flush();
    }
}
