namespace WindowsGrep.Core;

public class NativeService
{
    #region Methods..
    public void BeginProcessNativeCommand(NativeCommand nativeCommand, CommandResultCollection commandResultCollection, CancellationToken cancellationToken)
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

    private void ListFiles(CommandResultCollection commandResultCollection, CancellationToken cancellationToken)
    {
        FileAttributes fileAttributesToSkip = default;
        fileAttributesToSkip |= FileAttributes.System;
        fileAttributesToSkip |= FileAttributes.Hidden;

        string targetDirectory = Directory.GetCurrentDirectory();
        foreach (var file in WindowsUtils.GetFiles(targetDirectory, false, int.MaxValue, -1, -1, cancellationToken, null, fileAttributesToSkip))
            commandResultCollection.AddItem(new NativeCommandResult(file, NativeCommandType.List));
    }
    #endregion Methods..
}
