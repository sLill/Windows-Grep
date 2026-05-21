use std::fmt;
use colored::{Color, ColoredString, Colorize};

#[derive(Debug, Clone)]
pub struct ConsoleItem {
    /// Raw text — written to output files without color codes.
    pub value: String,
    pub fg: Option<Color>,
    pub bg: Option<Color>,
}

impl ConsoleItem {
    pub fn new(value: impl Into<String>) -> Self {
        ConsoleItem { value: value.into(), fg: Some(Color::White), bg: Some(Color::Black) }
    }

    pub fn with_fg(value: impl Into<String>, fg: Color) -> Self {
        ConsoleItem { value: value.into(), fg: Some(fg), bg: Some(Color::Black) }
    }

    pub fn with_bg(value: impl Into<String>, bg: Color) -> Self {
        ConsoleItem { value: value.into(), fg: Some(Color::White), bg: Some(bg) }
    }

    pub fn with_colors(value: impl Into<String>, fg: Color, bg: Color) -> Self {
        ConsoleItem { value: value.into(), fg: Some(fg), bg: Some(bg) }
    }
}

impl fmt::Display for ConsoleItem {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        let cs: ColoredString = match (&self.fg, &self.bg) {
            (Some(fg), Some(bg)) => self.value.as_str().color(*fg).on_color(*bg),
            (Some(fg), None)     => self.value.as_str().color(*fg),
            (None, Some(bg))     => self.value.as_str().on_color(*bg),
            (None, None)         => self.value.as_str().normal(),
        };
        write!(f, "{}", cs)
    }
}
