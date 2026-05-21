#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub enum NativeCommandType {
    List,
    ChangeDirectory,
    ClearConsole,
    PrintWorkingDirectory,
}

pub struct NativeCommandInfo {
    pub command_type: NativeCommandType,
    pub descriptor: &'static str,
    pub expects_parameter: bool,
}

pub const NATIVE_COMMANDS: &[NativeCommandInfo] = &[
    NativeCommandInfo {
        command_type: NativeCommandType::List,
        descriptor: "ls",
        expects_parameter: false,
    },
    NativeCommandInfo {
        command_type: NativeCommandType::ChangeDirectory,
        descriptor: "cd",
        expects_parameter: true,
    },
    NativeCommandInfo {
        command_type: NativeCommandType::ClearConsole,
        descriptor: "clear",
        expects_parameter: false,
    },
    NativeCommandInfo {
        command_type: NativeCommandType::PrintWorkingDirectory,
        descriptor: "pwd",
        expects_parameter: false,
    },
];
