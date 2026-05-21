use crate::enums::native_command_type::NativeCommandType;

#[derive(Debug, Clone)]
pub struct NativeCommand {
    pub command_type: NativeCommandType,
    pub command_parameter: String,
}
