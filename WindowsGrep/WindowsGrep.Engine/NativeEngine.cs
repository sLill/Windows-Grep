using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WindowsGrep.Common;
using WindowsGrep.Configuration;
using WindowsGrep.Core;
using WindowsGrep.Engine.Data;

namespace WindowsGrep.Engine
{
    public static class NativeEngine
    {
        #region Methods..
        public static async Task BeginProcessNativeCommandAsync(NativeCommand nativeCommand, CommandResultCollection commandResultCollection, CancellationToken cancellationToken)
        {
            switch (nativeCommand.CommandType)
            {
                case NativeCommandType.List:
                    await ListFilesAsync(commandResultCollection, cancellationToken);
                    break;

                case NativeCommandType.ChangeDirectory:
                    Directory.SetCurrentDirectory(nativeCommand.CommandParameter);
                    break;

                case NativeCommandType.ClearConsole:
                    ConsoleUtils.ClearConsole();
                    break;

                case NativeCommandType.PrintWorkingDirectory:
                    ConsoleUtils.WriteConsoleItem(new ConsoleItem() { Value = Directory.GetCurrentDirectory() + '\n' });
                    break;
            }
        }

        private static async Task ListFilesAsync(CommandResultCollection commandResultCollection, CancellationToken cancellationToken)
        {
            bool includeSystemProtectedFiles = (bool)ConfigurationManager.Instance.ConfigItemCollection[ConfigItem.IncludeSystemProtectedFiles];
            bool includeHiddenFiles = (bool)ConfigurationManager.Instance.ConfigItemCollection[ConfigItem.IncludeHiddenFiles];

            FileAttributes fileAttributesToSkip = default;
            fileAttributesToSkip |= (includeSystemProtectedFiles ? 0 : FileAttributes.System);
            fileAttributesToSkip |= (includeHiddenFiles ? 0 : FileAttributes.Hidden);

            string targetDirectory = Directory.GetCurrentDirectory();
            List<string> files = await WindowsUtils.GetFilesAsync(targetDirectory, false, cancellationToken, fileAttributesToSkip);

            files?.ForEach(x => commandResultCollection.AddItem(new NativeCommandResult(x, NativeCommandType.List)));
        }
        #endregion Methods..
    }
}
