namespace WindowsGrep.Core;

public class NativeService
{
    #region Fields..
    private readonly PublisherService _publisherService;
    private readonly ConsoleService _consoleService;
    #endregion Fields..

    #region Constructors..
    public NativeService(PublisherService publisherService, ConsoleService consoleService)
    {
        _publisherService = publisherService;
        _consoleService = consoleService;
    }
    #endregion Constructors..

    #region Methods..
    public void RunCommand(NativeCommand nativeCommand, List<ResultBase> results, CancellationToken cancellationToken)
    {
        _publisherService.Subscribe<ConsoleItem>(_consoleService.Write);

        switch (nativeCommand.CommandType)
        {
            case NativeCommandType.List:
                ListFiles(results, cancellationToken);
                break;

            case NativeCommandType.ChangeDirectory:
                Directory.SetCurrentDirectory(nativeCommand.CommandParameter);
                break;

            case NativeCommandType.ClearConsole:
                ConsoleUtils.ClearConsole();
                break;

            case NativeCommandType.PrintWorkingDirectory:
                _publisherService.Publish(new ConsoleItem() { Value = Directory.GetCurrentDirectory() + '\n' });
                break;
        }
    }

    private void ListFiles(List<ResultBase> results, CancellationToken cancellationToken)
    {
        FileAttributes fileAttributesToSkip = default;
        fileAttributesToSkip |= FileAttributes.System;
        fileAttributesToSkip |= FileAttributes.Hidden;

        string targetDirectory = Directory.GetCurrentDirectory();
        foreach (var file in WindowsUtils.GetFiles(targetDirectory, false, int.MaxValue, -1, -1, cancellationToken, null, fileAttributesToSkip))
        {
            var result = new NativeResult(file, NativeCommandType.List);
            result.ToConsoleItemCollection().ForEach(y => _publisherService.Publish(y));
        }
    }
    #endregion Methods..
}
