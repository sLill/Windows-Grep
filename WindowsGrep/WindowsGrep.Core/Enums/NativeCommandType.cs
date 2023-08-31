using WindowsGrep.Common;

namespace WindowsGrep.Core
{
    public enum NativeCommandType
    {
        [DescriptionCollection("clear")]
        ClearConsole,

        [ExpectsParameter(true)]
        [DescriptionCollection("cd")]
        ChangeDirectory
    }
}
