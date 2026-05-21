use crate::models::file_item::FileItem;

#[derive(Debug, Default)]
pub struct SearchMetrics {
    pub total_files_matched_count: i32,
    pub delete_success_count: i32,
    pub replaced_success_count: i32,
    pub failed_read_files: Vec<FileItem>,
    pub failed_write_files: Vec<FileItem>,
}
