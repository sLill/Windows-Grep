#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub enum HashType {
    Sha256,
    Md5,
}

impl HashType {
    pub fn from_int(value: i32) -> Option<HashType> {
        match value {
            0 => Some(HashType::Sha256),
            1 => Some(HashType::Md5),
            _ => None,
        }
    }

    pub fn valid_hash_length(&self) -> usize {
        match self {
            HashType::Sha256 => 64,
            HashType::Md5 => 32,
        }
    }
}
