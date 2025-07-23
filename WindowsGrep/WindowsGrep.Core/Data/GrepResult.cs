namespace WindowsGrep.Core;

public class GrepResult : ResultBase
{
    #region Fields..
    private ResultScope _scope;
    #endregion Fields..

    #region Properties..
    public string LeadingContextString { get; set; }

    public int LineNumber { get; set; } = -1;

    public string MatchedString { get; set; }

    public string TrailingContextString { get; set; }
    #endregion Properties..

    #region Constructors..
    public GrepResult(FileItem sourceFile, ResultScope resultScope)
        : base(sourceFile)
    {
        _scope = resultScope;
    }
    #endregion Constructors..

    #region Methods..
    public List<ConsoleItem> ToConsoleItemCollection()
    {
        List<ConsoleItem> consoleItemCollection = new List<ConsoleItem>();

        switch (_scope)
        {
            case ResultScope.FileContent:
                BuildFileContentConsoleItemCollection(consoleItemCollection);
                break;

            case ResultScope.FileName:
                BuildFileNameConsoleItemCollection(consoleItemCollection);
                break;

            case ResultScope.FileHash:
                BuildFileHashConsoleItemCollection(consoleItemCollection);
                break;
        }

        // Empty buffer
        consoleItemCollection.Add(new ConsoleItem { Value = Environment.NewLine });
        return consoleItemCollection;
    }

    private void BuildFileContentConsoleItemCollection(List<ConsoleItem> consoleItemCollection)
    {
        // FileName
        consoleItemCollection.Add(new ConsoleItem { ForegroundColor = AnsiColors.DarkYellow, Value = $"{SourceFile.Name} " });

        // FileSize
        //if (SourceFile.FileSize > -1)
        //{
        //    var fileSizeReduced = WindowsUtils.GetReducedSize(SourceFile.FileSize, 3, out FileSizeType fileSizeType);
        //    consoleItemCollection.Add(new ConsoleItem { ForegroundColor = ConsoleColor.Green, Value = $"{fileSizeReduced} {fileSizeType}(s) " });
        //}

        // Line number
        if (LineNumber > -1)
            consoleItemCollection.Add(new ConsoleItem { ForegroundColor = AnsiColors.DarkMagenta, Value = $"Line {LineNumber}  " });

        // Context start
        consoleItemCollection.Add(new ConsoleItem { Value = LeadingContextString });

        // Context matched
        consoleItemCollection.Add(new ConsoleItem { BackgroundColor = AnsiColors.DarkerCyanBg, Value = MatchedString });

        // Context end
        consoleItemCollection.Add(new ConsoleItem { Value = TrailingContextString });
    }

    private void BuildFileNameConsoleItemCollection(List<ConsoleItem> consoleItemCollection)
    {
        // Context start
        consoleItemCollection.Add(new ConsoleItem { ForegroundColor = AnsiColors.DarkYellow, Value = LeadingContextString });

        // Context matched
        consoleItemCollection.Add(new ConsoleItem { BackgroundColor = AnsiColors.DarkerCyanBg, Value = MatchedString });

        // Context end
        consoleItemCollection.Add(new ConsoleItem { ForegroundColor = AnsiColors.DarkYellow, Value = TrailingContextString });

        // File attributes
        consoleItemCollection.AddRange(GetFileAttributeConsoleItems());

        // FileSize
        //consoleItemCollection.AddRange(GetFileSizeConsoleItems());
    }

    private void BuildFileHashConsoleItemCollection(List<ConsoleItem> consoleItemCollection)
    {
        // FileName
        consoleItemCollection.Add(new ConsoleItem { ForegroundColor = AnsiColors.DarkYellow, Value = $"{SourceFile.Name} " });

        // Context matched
        consoleItemCollection.Add(new ConsoleItem { BackgroundColor = AnsiColors.DarkerCyanBg, Value = MatchedString });

        // File attributes
        consoleItemCollection.AddRange(GetFileAttributeConsoleItems());

        // FileSize
        //consoleItemCollection.AddRange(GetFileSizeConsoleItems());
    }

    public override string ToString(char separator)
    {
        string result = string.Empty;

        string lineNumberString = LineNumber > -1 ? $"Line {LineNumber}" : string.Empty;

        string fileSizeString = string.Empty;
        if (SourceFile.FileSize > -1)
        {
            var fileSizeReduced = WindowsUtils.GetReducedSize(SourceFile.FileSize, 3, out FileSizeType fileSizeType);
            fileSizeString = $"{fileSizeReduced} {fileSizeType}(s){separator}";
        }

        result = (_scope) switch
        {
            ResultScope.FileContent => $"{SourceFile.Name}{separator}{fileSizeString}{lineNumberString}{separator}{LeadingContextString}{MatchedString}{TrailingContextString}",
            ResultScope.FileName => SourceFile.Name,
            ResultScope.FileHash => $"{SourceFile.Name}{separator}{MatchedString}"
        };

        return result;
    }
    #endregion Methods..
}
