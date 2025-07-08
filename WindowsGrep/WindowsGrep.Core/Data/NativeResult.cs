namespace WindowsGrep.Core;

public class NativeResult : ResultBase
{
    #region Fields..
    private NativeCommandType _commandType;
    #endregion Fields..

    #region Constructors..
    public NativeResult(FileItem sourceFile, NativeCommandType nativeCommandType)
        : base(sourceFile)
    {
        _commandType = nativeCommandType;
    }
    #endregion Constructors..

    #region Methods..
    public List<ConsoleItem> ToConsoleItemCollection()
    {
        var consoleItems = new List<ConsoleItem>();

        // Filename
        consoleItems.Add(new ConsoleItem()
        {
            ForegroundColor = SourceFile.IsDirectory ? ConsoleColor.Cyan : ConsoleColor.DarkYellow,
            Value = SourceFile.Name
        });

        // File attributes
        consoleItems.AddRange(GetFileAttributeConsoleItems());

        // FileSize
        consoleItems.AddRange(GetFileSizeConsoleItems());
        consoleItems.Add(new ConsoleItem() { Value = Environment.NewLine });

        return consoleItems;
    }
    #endregion Methods..
}
