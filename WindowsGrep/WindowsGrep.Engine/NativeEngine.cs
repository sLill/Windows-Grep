using System.IO;
using WindowsGrep.Core;

namespace WindowsGrep.Engine
{
    public static class NativeEngine
    {
        #region Methods..
        public static void BeginProcessNativeCommand(NativeCommand nativeCommand)
        {
            switch (nativeCommand.CommandType)
            {
                case NativeCommandType.ClearConsole:
                    ConsoleUtils.ClearConsole();
                    break;

                case NativeCommandType.ChangeDirectory:
                    Directory.SetCurrentDirectory(nativeCommand.CommandParameter);
                    break;
            }
        }
        #endregion Methods..
    }
}
