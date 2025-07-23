namespace WindowsGrep.Core;

public static class CommandFlagUtils
{
    #region Fields..
    private const string FILE_SIZE_PATTERN = @"(?<Size>\d+)(?<SizeType>\S{2})?";

    private static Regex _fileSizeRegex = new Regex(FILE_SIZE_PATTERN);
    #endregion Fields..

    #region Methods..
    public static long GetFileSize(string fileSizeString)
    {
        long fileSize = -1;

        try
        {
            var match = _fileSizeRegex.Match(fileSizeString);
            long size = Convert.ToInt64(match.Groups["Size"].Value);

            if (size < 0)
                throw new Exception("Error: Size parameter cannot be less than 0");

            long fileSizeModifier = FileSizeType.BYTES.GetCustomAttribute<ValueAttribute>().Value;

            string sizeType = match.Groups["SizeType"].Value.ToUpper();
            if (!sizeType.IsNullOrEmpty())
                fileSizeModifier = Enum.Parse<FileSizeType>(sizeType, true).GetCustomAttribute<ValueAttribute>().Value;

            fileSize = size * fileSizeModifier;
        }
        catch
        {
            throw new Exception($"Error: Could not parse filesize parameter" +
                $"{Environment.NewLine}Format should follow [SIZE] or [SIZE][TYPE]. Acceptable TYPE parameters are kb, mb, gb, tb");
        }

        return fileSize;
    }

    public static HashType GetHashType(GrepCommand grepCommand)
    {
        HashType hashType = default;

        bool fileHashesFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.FileHashes);
        if (fileHashesFlag)
        {
            try
            {
                string hashTypeParameter = grepCommand.CommandArgs[CommandFlag.FileHashes];
                hashType = (HashType)Convert.ToInt32(hashTypeParameter);
            }
            catch
            {
                throw new Exception($"Error: could not parse hash type" +
                    $"{Environment.NewLine}Expected format, -H [INT]" +
                    $"{Environment.NewLine}For more information, visit https://github.com/sLill/Windows-BudgetGrep/wiki/WindowsGrep.CommandFlags");
            }
        }

        return hashType;
    }

    public static List<string>? GetFileTypeIncludeFilters(GrepCommand grepCommand)
    {
        bool filetypeFilterFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.FileTypeIncludeFilter);
        var filetypeFilters = filetypeFilterFlag ? grepCommand.CommandArgs[CommandFlag.FileTypeIncludeFilter].TrimOnce(new[] { '"', '\'' }).Split(new char[] { ',', ';' }).Select(x => x.TrimOnce('.')).ToList() : null;

        return filetypeFilters;
    }

    public static List<string>? GetFileTypeExcludeFilters(GrepCommand grepCommand)
    {
        bool filetypeExcludeFilterFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.FileTypeExcludeFilter);
        var filetypeExcludeFilters = filetypeExcludeFilterFlag ? grepCommand.CommandArgs[CommandFlag.FileTypeExcludeFilter].TrimOnce(new[] { '"', '\'' }).Split(new char[] { ',', ';' }).Select(x => x.TrimOnce('.')).ToList() : null;

        return filetypeExcludeFilters;
    }

    public static List<string>? GetPathIncludeFilters(GrepCommand grepCommand)
    {
        bool pathFilterFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.PathIncludeFilter);
        var pathFilters = pathFilterFlag ? grepCommand.CommandArgs[CommandFlag.PathIncludeFilter].Split(new char[] { ',', ';' }).ToList() : null;

        return pathFilters;
    }

    public static List<string>? GetPathExcludeFilters(GrepCommand grepCommand)
    {
        bool pathExcludeFilterFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.PathExcludeFilter);
        var pathExcludeFilters = pathExcludeFilterFlag ? grepCommand.CommandArgs[CommandFlag.PathExcludeFilter].Split(new char[] { ',', ';' }).ToList() : null;

        return pathExcludeFilters;
    }

    public static RegexOptions GetRegexOptions(GrepCommand grepCommand)
    {
        RegexOptions optionsFlags = 0;
        bool ignoreCaseFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.IgnoreCase);
        bool ignoreBreaksFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.IgnoreBreaks);

        if (ignoreCaseFlag)
            optionsFlags |= RegexOptions.IgnoreCase;
        if (ignoreBreaksFlag)
            optionsFlags |= RegexOptions.Singleline;
        else
            optionsFlags |= RegexOptions.Multiline;

        return optionsFlags;
    }

    public static string BuildSearchPattern(GrepCommand grepCommand)
    {
        bool fixedStringsFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.FixedString);
        bool ignoreCaseFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.IgnoreCase);

        string searchTerm = grepCommand.CommandArgs[CommandFlag.SearchTerm];
        string ignoreCaseModifier = ignoreCaseFlag ? @"(?i)" : string.Empty;

        // Ignore carriage-return and newline characters when using endline regex to match expected behavior from other regex engines
        searchTerm = searchTerm.Replace("$", "[\r\n]*$");
        searchTerm = fixedStringsFlag ? Regex.Escape(searchTerm) : searchTerm;

        string searchPattern = @"(?<MatchedString>" + ignoreCaseModifier + searchTerm + @")";
        return searchPattern;
    }
    #endregion Methods..
}
