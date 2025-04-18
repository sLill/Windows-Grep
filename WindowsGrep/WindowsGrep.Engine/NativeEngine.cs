﻿namespace WindowsGrep.Engine;

public static class NativeEngine
{
    #region Methods..
    public static void BeginProcessNativeCommand(NativeCommand nativeCommand, CommandResultCollection commandResultCollection, CancellationToken cancellationToken)
    {
        switch (nativeCommand.CommandType)
        {
            case NativeCommandType.List:
                ListFiles(commandResultCollection, cancellationToken);
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

    private static void ListFiles(CommandResultCollection commandResultCollection, CancellationToken cancellationToken)
    {
        bool includeSystemProtectedFiles = (bool)ConfigurationManager.Instance.ConfigItemCollection[ConfigItem.IncludeSystemProtectedFiles];
        bool includeHiddenFiles = (bool)ConfigurationManager.Instance.ConfigItemCollection[ConfigItem.IncludeHiddenFiles];

        FileAttributes fileAttributesToSkip = default;
        fileAttributesToSkip |= (includeSystemProtectedFiles ? 0 : FileAttributes.System);
        fileAttributesToSkip |= (includeHiddenFiles ? 0 : FileAttributes.Hidden);

        string targetDirectory = Directory.GetCurrentDirectory();
        foreach (var file in WindowsUtils.GetFiles(targetDirectory, false, int.MaxValue, cancellationToken, null, fileAttributesToSkip))
            commandResultCollection.AddItem(new NativeCommandResult(file, NativeCommandType.List));
    }
    #endregion Methods..
}
