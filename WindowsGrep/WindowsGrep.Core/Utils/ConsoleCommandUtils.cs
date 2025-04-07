using System.Text.RegularExpressions;
using WindowsGrep.Common;

namespace WindowsGrep.Core
{
    public static class CommandUtils
    {
        #region Fields..
        private const string FILE_SIZE_PATTERN = @"(?<Size>\d+)(?<SizeType>\S{2})?";

        private static Regex _fileSizeRegex = new Regex(FILE_SIZE_PATTERN);
        #endregion Fields..

        #region Methods..
        public static long GetFileSizeMaximum(GrepCommand grepCommand)
        {
            long fileSizeMaximum = -1;

            bool fileSizeMaximumFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.FileSizeMaximum);
            if (fileSizeMaximumFlag)
            {
                try
                {
                    string fileSizeMaximumParameter = grepCommand.CommandArgs[ConsoleFlag.FileSizeMaximum];

                    var match = _fileSizeRegex.Match(fileSizeMaximumParameter);
                    long size = Convert.ToInt64(match.Groups["Size"].Value);

                    if (size < 0)
                        throw new Exception("Error: Size parameter cannot be less than 0");

                    long fileSizeModifier = FileSizeType.Kb.GetCustomAttribute<ValueAttribute>().Value;

                    string sizeType = match.Groups["SizeType"].Value.ToUpper();
                    if (!sizeType.IsNullOrEmpty())
                        fileSizeModifier = Enum.Parse<FileSizeType>(sizeType, true).GetCustomAttribute<ValueAttribute>().Value;

                    fileSizeMaximum = size * fileSizeModifier;
                }
                catch
                {
                    throw new Exception($"Error: Could not parse filesize parameter" +
                        $"{Environment.NewLine}Format should follow [SIZE] or [SIZE][TYPE]. Acceptable TYPE parameters are kb, mb, gb, tb" +
                        $"{Environment.NewLine}For more information, visit https://github.com/sLill/Windows-BudgetGrep/wiki/WindowsGrep.CommandFlags");
                }
            }

            return fileSizeMaximum;
        }

        public static long GetFileSizeMinimum(GrepCommand grepCommand)
        {
            long fileSizeMinimum = -1;

            bool fileSizeMinimumFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.FileSizeMinimum);
            if (fileSizeMinimumFlag)
            {
                try
                {
                    string fileSizeMinimumParameter = grepCommand.CommandArgs[ConsoleFlag.FileSizeMinimum];

                    var match = _fileSizeRegex.Match(fileSizeMinimumParameter);
                    long size = Convert.ToInt64(match.Groups["Size"].Value);

                    if (size < 0)
                    {
                        throw new Exception("Error: Size parameter cannot be less than 0");
                    }

                    long fileSizeModifier = FileSizeType.Kb.GetCustomAttribute<ValueAttribute>().Value;

                    string sizeType = match.Groups["SizeType"].Value.ToUpper();
                    if (!sizeType.IsNullOrEmpty())
                    {
                        fileSizeModifier = Enum.Parse<FileSizeType>(sizeType).GetCustomAttribute<ValueAttribute>().Value;
                    }

                    fileSizeMinimum = size * fileSizeModifier;
                }
                catch
                {
                    throw new Exception($"Error: could not parse filesize parameter" +
                        $"{Environment.NewLine}Format should follow [SIZE] or [SIZE][TYPE]. Acceptable TYPE parameters are kb, mb, gb, tb" +
                        $"{Environment.NewLine}For more information, visit https://github.com/sLill/Windows-BudgetGrep/wiki/WindowsGrep.CommandFlags");
                }
            }

            return fileSizeMinimum;
        }

        public static HashType GetHashType(GrepCommand grepCommand)
        {
            HashType hashType = default;

            bool fileHashesFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.FileHashes);
            if (fileHashesFlag)
            {
                try
                {
                    string hashTypeParameter = grepCommand.CommandArgs[ConsoleFlag.FileHashes];
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

        public static List<string>? GetFileTypeFilters(GrepCommand grepCommand)
        {
            bool fileTypeFilterFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.FileTypeFilter);
            var fileTypeFilters = fileTypeFilterFlag ? grepCommand.CommandArgs[ConsoleFlag.FileTypeFilter].Split(new char[] { ',', ';' }).Select(x => x.Trim('.')).ToList() : null;

            return fileTypeFilters;
        }

        public static List<string>? GetFileTypeExcludeFilters(GrepCommand grepCommand)
        {
            bool fileTypeExcludeFilterFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.FileTypeExcludeFilter);
            var fileTypeExcludeFilters = fileTypeExcludeFilterFlag ? grepCommand.CommandArgs[ConsoleFlag.FileTypeExcludeFilter].Split(new char[] { ',', ';' }).Select(x => x.Trim('.')).ToList() : null;

            return fileTypeExcludeFilters;
        }

        public static List<string>? GetPathFilters(GrepCommand grepCommand)
        {
            bool pathFilterFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.PathFilter);
            var pathFilters = pathFilterFlag ? grepCommand.CommandArgs[ConsoleFlag.PathFilter].Split(new char[] { ',', ';' }).ToList() : null;

            return pathFilters;
        }

        public static List<string>? GetPathExcludeFilters(GrepCommand grepCommand)
        {
            bool pathExcludeFilterFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.PathExcludeFilter);
            var pathExcludeFilters = pathExcludeFilterFlag ? grepCommand.CommandArgs[ConsoleFlag.PathExcludeFilter].Split(new char[] { ',', ';' }).ToList() : null;

            return pathExcludeFilters;
        }

        public static RegexOptions GetRegexOptions(GrepCommand grepCommand)
        {
            RegexOptions optionsFlags = 0;
            bool ignoreCaseFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.IgnoreCase);
            bool ignoreBreaksFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.IgnoreBreaks);

            if (ignoreCaseFlag)
                optionsFlags |= RegexOptions.IgnoreCase;
            if (ignoreBreaksFlag)
                optionsFlags |= RegexOptions.Singleline;
            else
                optionsFlags |= RegexOptions.Multiline;

            return optionsFlags;
        }

        public static string GetPath(GrepCommand grepCommand)
        {
            bool directoryFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.Directory);
            bool targetFileFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.TargetFile);

            // User specified file should overrule specified directory
            string filepath = directoryFlag ? grepCommand.CommandArgs[ConsoleFlag.Directory] : Environment.CurrentDirectory;
            filepath = targetFileFlag ? grepCommand.CommandArgs[ConsoleFlag.TargetFile] : filepath;

            return filepath;
        }

        public static string BuildSearchPattern(GrepCommand grepCommand)
        {
            bool fixedStringsFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.FixedStrings);
            bool iIgnoreCaseFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.IgnoreCase);

            string searchTerm = grepCommand.CommandArgs[ConsoleFlag.SearchTerm];
            string ignoreCaseModifier = iIgnoreCaseFlag ? @"(?i)" : string.Empty;

            // Ignore carriage-return and newline characters when using endline regex to match expected behavior from other regex engines
            searchTerm = searchTerm.Replace("$", "[\r\n]*$");
            searchTerm = fixedStringsFlag ? Regex.Escape(searchTerm) : searchTerm;

            string searchPattern = @"(?<MatchedString>" + ignoreCaseModifier + searchTerm + @")";
            return searchPattern;
        }
        #endregion Methods..
    }
}
