use std::fmt;

#[derive(Debug, Clone, Copy, PartialEq, Eq, PartialOrd, Ord)]
pub enum FileSizeType {
    Bytes,
    Kb,
    Mb,
    Gb,
    Tb,
}

impl FileSizeType {
    pub fn multiplier(&self) -> i64 {
        match self {
            FileSizeType::Bytes => 1,
            FileSizeType::Kb => 1_000,
            FileSizeType::Mb => 1_000_000,
            FileSizeType::Gb => 1_000_000_000,
            FileSizeType::Tb => 1_000_000_000_000,
        }
    }

    pub fn from_str(s: &str) -> Option<FileSizeType> {
        match s.to_uppercase().as_str() {
            "BYTES" | "B" => Some(FileSizeType::Bytes),
            "KB" => Some(FileSizeType::Kb),
            "MB" => Some(FileSizeType::Mb),
            "GB" => Some(FileSizeType::Gb),
            "TB" => Some(FileSizeType::Tb),
            _ => None,
        }
    }

    pub fn all() -> &'static [FileSizeType] {
        &[
            FileSizeType::Bytes,
            FileSizeType::Kb,
            FileSizeType::Mb,
            FileSizeType::Gb,
            FileSizeType::Tb,
        ]
    }
}

impl fmt::Display for FileSizeType {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        let s = match self {
            FileSizeType::Bytes => "BYTES",
            FileSizeType::Kb => "KB",
            FileSizeType::Mb => "MB",
            FileSizeType::Gb => "GB",
            FileSizeType::Tb => "TB",
        };
        write!(f, "{}", s)
    }
}
