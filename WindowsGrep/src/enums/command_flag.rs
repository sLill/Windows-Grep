#[derive(Debug, Clone, Copy, PartialEq, Eq, Hash)]
pub enum CommandFlag {
    SearchTerm,
    Path,
    Context,
    FixedString,
    IgnoreBreaks,
    IgnoreCase,
    Recursive,
    MaxDepth,
    FileTypeIncludeFilter,
    FileTypeExcludeFilter,
    PathIncludeFilter,
    PathExcludeFilter,
    FileNamesOnly,
    FileSizeMinimum,
    FileSizeMaximum,
    OutFile,
    Replace,
    Delete,
    FileHashes,
    Help,
    HelpFull,
    Hidden,
    System,
    Verbose,
}

pub struct FlagInfo {
    pub flag: CommandFlag,
    /// All descriptors for this flag (e.g. ["-r", "--recursive"]).
    /// Long flags that take inline params end with "=" (e.g. "--max-depth=").
    pub descriptors: &'static [&'static str],
    pub expects_parameter: bool,
    /// Characters to strip from the extracted parameter value.
    pub filter_chars: &'static [char],
}

pub const ALL_FLAGS: &[FlagInfo] = &[
    FlagInfo {
        flag: CommandFlag::Context,
        descriptors: &["-c"],
        expects_parameter: true,
        filter_chars: &[],
    },
    FlagInfo {
        flag: CommandFlag::FixedString,
        descriptors: &["-F"],
        expects_parameter: false,
        filter_chars: &[],
    },
    FlagInfo {
        flag: CommandFlag::IgnoreBreaks,
        descriptors: &["--ignore-breaks"],
        expects_parameter: false,
        filter_chars: &[],
    },
    FlagInfo {
        flag: CommandFlag::IgnoreCase,
        descriptors: &["-i"],
        expects_parameter: false,
        filter_chars: &[],
    },
    FlagInfo {
        flag: CommandFlag::Recursive,
        descriptors: &["-r"],
        expects_parameter: false,
        filter_chars: &[],
    },
    FlagInfo {
        flag: CommandFlag::MaxDepth,
        descriptors: &["--max-depth="],
        expects_parameter: true,
        filter_chars: &['\'', '"'],
    },
    FlagInfo {
        flag: CommandFlag::FileTypeIncludeFilter,
        descriptors: &["-t"],
        expects_parameter: true,
        filter_chars: &['\'', '"', '.', '\\'],
    },
    FlagInfo {
        flag: CommandFlag::FileTypeExcludeFilter,
        descriptors: &["-T"],
        expects_parameter: true,
        filter_chars: &['\'', '"', '.', '\\'],
    },
    FlagInfo {
        flag: CommandFlag::PathIncludeFilter,
        descriptors: &["-p"],
        expects_parameter: true,
        filter_chars: &['\'', '"'],
    },
    FlagInfo {
        flag: CommandFlag::PathExcludeFilter,
        descriptors: &["-P"],
        expects_parameter: true,
        filter_chars: &['\'', '"'],
    },
    FlagInfo {
        flag: CommandFlag::FileNamesOnly,
        descriptors: &["-k"],
        expects_parameter: false,
        filter_chars: &[],
    },
    FlagInfo {
        flag: CommandFlag::FileSizeMinimum,
        descriptors: &["--filesize-min="],
        expects_parameter: true,
        filter_chars: &['\'', '"'],
    },
    FlagInfo {
        flag: CommandFlag::FileSizeMaximum,
        descriptors: &["--filesize-max="],
        expects_parameter: true,
        filter_chars: &['\'', '"'],
    },
    FlagInfo {
        flag: CommandFlag::OutFile,
        descriptors: &["-o"],
        expects_parameter: true,
        filter_chars: &['\'', '"', '\\'],
    },
    FlagInfo {
        flag: CommandFlag::Replace,
        descriptors: &["--replace="],
        expects_parameter: true,
        filter_chars: &['\'', '"'],
    },
    FlagInfo {
        flag: CommandFlag::Delete,
        descriptors: &["--delete"],
        expects_parameter: false,
        filter_chars: &[],
    },
    FlagInfo {
        flag: CommandFlag::FileHashes,
        descriptors: &["--hash="],
        expects_parameter: true,
        filter_chars: &[],
    },
    FlagInfo {
        flag: CommandFlag::Help,
        descriptors: &["-h"],
        expects_parameter: false,
        filter_chars: &[],
    },
    FlagInfo {
        flag: CommandFlag::HelpFull,
        descriptors: &["--help"],
        expects_parameter: false,
        filter_chars: &[],
    },
    FlagInfo {
        flag: CommandFlag::Hidden,
        descriptors: &["--hidden"],
        expects_parameter: false,
        filter_chars: &[],
    },
    FlagInfo {
        flag: CommandFlag::System,
        descriptors: &["--system"],
        expects_parameter: false,
        filter_chars: &[],
    },
    FlagInfo {
        flag: CommandFlag::Verbose,
        descriptors: &["-v"],
        expects_parameter: false,
        filter_chars: &[],
    },
];
