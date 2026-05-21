#[derive(Debug, Clone)]
pub struct FileItem {
    pub name: String,
    pub is_directory: bool,
    pub file_size: i64,
}

impl FileItem {
    pub fn new(name: String, is_directory: bool, file_size: i64) -> Self {
        FileItem { name, is_directory, file_size }
    }
}
