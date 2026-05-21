#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub enum ResultScope {
    FileContent,
    FileName,
    FileHash,
}
