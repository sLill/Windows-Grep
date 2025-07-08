namespace WindowsGrep.Core;

public abstract class ResultBase
{
    #region Properties..
    public FileItem SourceFile { get; set; }
    #endregion Properties..

    #region Constructors..
    public ResultBase(FileItem sourceFile)
    {
        SourceFile = sourceFile;
    }
    #endregion Constructors..

    #region Methods..
    protected List<ConsoleItem> GetFileAttributeConsoleItems()
    {
        List<ConsoleItem> consoleItemCollection = new List<ConsoleItem>();

        var fileAttributes = File.GetAttributes(SourceFile.Name);
        if ((fileAttributes & FileAttributes.System) == FileAttributes.System)
            consoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = $"[System]" });
        if ((fileAttributes & FileAttributes.Hidden) == FileAttributes.Hidden)
            consoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.DarkCyan, Value = $"[Hidden]" });

        if (consoleItemCollection.Count > 0)
            consoleItemCollection.Insert(0, new ConsoleItem() { Value = " " });

        return consoleItemCollection;
    }

    protected List<ConsoleItem> GetFileSizeConsoleItems()
    {
        List<ConsoleItem> consoleItemCollection = new List<ConsoleItem>();

        if (SourceFile.FileSize > -1)
        {
            var fileSizeReduced = WindowsUtils.GetReducedSize(SourceFile.FileSize, 3, out FileSizeType fileSizeType);
            consoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.Green, Value = $" {fileSizeReduced} {fileSizeType}(s) " });
        }

        return consoleItemCollection;
    }

    public virtual string ToString(char separator)
    {
        return string.Empty;
    }
    #endregion Methods..
}
