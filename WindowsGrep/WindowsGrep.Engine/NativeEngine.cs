using System.Collections.Generic;
using System.IO;
using WindowsGrep.Common;
using WindowsGrep.Configuration;
using WindowsGrep.Core;
using WindowsGrep.Engine.Data;

namespace WindowsGrep.Engine
{
    public static class NativeEngine
    {
        #region Methods..
        public static void BeginProcessNativeCommand(NativeCommand nativeCommand, CommandResultCollection commandResultCollection)
        {
            switch (nativeCommand.CommandType)
            {
                case NativeCommandType.List:
                    ListFiles(commandResultCollection);
                    break;

                case NativeCommandType.ChangeDirectory:
                    Directory.SetCurrentDirectory(nativeCommand.CommandParameter);
                    break;

                case NativeCommandType.ClearConsole:
                    ConsoleUtils.ClearConsole();
                    break;
            }
        }

        private static void ListFiles(CommandResultCollection commandResultCollection)
        {
            bool includeSystemProtectedFiles = (bool)ConfigurationManager.Instance.ConfigItemCollection[ConfigItem.IncludeSystemProtectedFiles];
            bool includeHiddenFiles = (bool)ConfigurationManager.Instance.ConfigItemCollection[ConfigItem.IncludeHiddenFiles];

            FileAttributes fileAttributesToSkip = default;
            fileAttributesToSkip |= (includeSystemProtectedFiles ? 0 : FileAttributes.System);
            fileAttributesToSkip |= (includeHiddenFiles ? 0 : FileAttributes.Hidden);

            string targetDirectory = Directory.GetCurrentDirectory();
            List<string> files = WindowsUtils.GetFiles(targetDirectory, false, fileAttributesToSkip);

            files?.ForEach(x => commandResultCollection.AddItem(new NativeCommandResult(x, NativeCommandType.List)));
        }
        #endregion Methods..
    }
}
