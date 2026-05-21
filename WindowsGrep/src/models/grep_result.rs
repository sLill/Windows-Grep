use colored::Color;

use crate::enums::result_scope::ResultScope;
use crate::models::console_item::ConsoleItem;
use crate::models::file_item::FileItem;
use crate::utils::windows_utils;

#[derive(Debug, Clone)]
pub struct GrepResult {
    pub source_file: FileItem,
    pub scope: ResultScope,
    pub leading_context_string: String,
    pub line_number: i32,
    pub matched_string: String,
    pub trailing_context_string: String,
}

impl GrepResult {
    pub fn new(source_file: FileItem, scope: ResultScope) -> Self {
        GrepResult {
            source_file,
            scope,
            leading_context_string: String::new(),
            line_number: -1,
            matched_string: String::new(),
            trailing_context_string: String::new(),
        }
    }

    pub fn to_console_items(&self) -> Vec<ConsoleItem> {
        let mut items = match self.scope {
            ResultScope::FileContent => self.build_file_content_items(),
            ResultScope::FileName => self.build_file_name_items(),
            ResultScope::FileHash => self.build_file_hash_items(),
        };
        items.push(ConsoleItem::new("\n"));
        items
    }

    fn build_file_content_items(&self) -> Vec<ConsoleItem> {
        let mut items = Vec::new();

        // Filename
        items.push(ConsoleItem::with_fg(
            format!("{} ", self.source_file.name),
            Color::Yellow,
        ));

        // Line number
        if self.line_number > -1 {
            items.push(ConsoleItem::with_fg(
                format!("Line {}  ", self.line_number),
                Color::Magenta,
            ));
        }

        // Leading context
        if !self.leading_context_string.is_empty() {
            items.push(ConsoleItem::new(self.leading_context_string.clone()));
        }

        // Matched string (highlighted)
        items.push(ConsoleItem::with_bg(
            self.matched_string.clone(),
            Color::TrueColor { r: 0, g: 139, b: 139 },
        ));

        // Trailing context
        if !self.trailing_context_string.is_empty() {
            items.push(ConsoleItem::new(self.trailing_context_string.clone()));
        }

        items
    }

    fn build_file_name_items(&self) -> Vec<ConsoleItem> {
        let mut items = Vec::new();

        items.push(ConsoleItem::with_fg(
            self.leading_context_string.clone(),
            Color::Yellow,
        ));
        items.push(ConsoleItem::with_bg(
            self.matched_string.clone(),
            Color::TrueColor { r: 0, g: 139, b: 139 },
        ));
        items.push(ConsoleItem::with_fg(
            self.trailing_context_string.clone(),
            Color::Yellow,
        ));
        items.extend(self.get_file_attribute_items());

        items
    }

    fn build_file_hash_items(&self) -> Vec<ConsoleItem> {
        let mut items = Vec::new();

        items.push(ConsoleItem::with_fg(
            format!("{} ", self.source_file.name),
            Color::Yellow,
        ));
        items.push(ConsoleItem::with_bg(
            self.matched_string.clone(),
            Color::TrueColor { r: 0, g: 139, b: 139 },
        ));
        items.extend(self.get_file_attribute_items());

        items
    }

    fn get_file_attribute_items(&self) -> Vec<ConsoleItem> {
        get_file_attribute_items_for(&self.source_file.name)
    }

    pub fn to_string_sep(&self, separator: char) -> String {
        let line_str = if self.line_number > -1 {
            format!("Line {}", self.line_number)
        } else {
            String::new()
        };

        let size_str = if self.source_file.file_size > -1 {
            let (v, t) = windows_utils::get_reduced_size(self.source_file.file_size, 3);
            format!("{} {}(s){}", v, t, separator)
        } else {
            String::new()
        };

        match self.scope {
            ResultScope::FileContent => format!(
                "{}{}{}{}{}{}{}{}",
                self.source_file.name,
                separator,
                size_str,
                line_str,
                separator,
                self.leading_context_string,
                self.matched_string,
                self.trailing_context_string,
            ),
            ResultScope::FileName => self.source_file.name.clone(),
            ResultScope::FileHash => {
                format!("{}{}{}", self.source_file.name, separator, self.matched_string)
            }
        }
    }
}

fn get_file_attribute_items_for(path: &str) -> Vec<ConsoleItem> {
    let mut items = Vec::new();

    #[cfg(windows)]
    {
        use std::os::windows::fs::MetadataExt;
        const FILE_ATTRIBUTE_HIDDEN: u32 = 0x2;
        const FILE_ATTRIBUTE_SYSTEM: u32 = 0x4;

        if let Ok(meta) = std::fs::metadata(path) {
            let attrs = meta.file_attributes();
            let hidden = (attrs & FILE_ATTRIBUTE_HIDDEN) != 0;
            let system = (attrs & FILE_ATTRIBUTE_SYSTEM) != 0;

            if hidden || system {
                items.push(ConsoleItem::new(" "));
            }
            if system {
                items.push(ConsoleItem::with_fg("[System]", Color::BrightRed));
            }
            if hidden {
                items.push(ConsoleItem::with_fg("[Hidden]", Color::Cyan));
            }
        }
    }

    items
}
