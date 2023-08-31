using System.Text.RegularExpressions;
using WindowsGrep.Common;

namespace WindowsGrep.Core
{
    public static class grepCommandUtils
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

        /// <summary>
        /// </summary>
        /// <param name="grepCommand"></param>
        /// <param name="files"></param>
        /// <returns>Returns files filtered by Inclusion/Exclusion type parameters</returns>
        public static List<string> GetFilteredFiles(GrepCommand grepCommand, List<string> files)
        {
            var fileTypeFilters = GetFileTypeFilters(grepCommand);
            var fileTypeExcludeFilters = GetFileTypeExcludeFilters(grepCommand);
            var pathFilters = GetPathFilters(grepCommand);
            var pathExcludeFilters = GetPathExcludeFilters(grepCommand);

            // Filter files by type
            files = fileTypeFilters == null ? files : files.Where(file => fileTypeFilters.Contains(Path.GetExtension(file).Trim('.'))).ToList();
            files = fileTypeExcludeFilters == null ? files : files.Where(file => !fileTypeExcludeFilters.Contains(Path.GetExtension(file).Trim('.'))).ToList();

            // Filter files by relative subpaths
            files = pathFilters == null ? files : files.Where(file => pathFilters.Any(x => Regex.IsMatch(file, x))).ToList();
            files = pathExcludeFilters == null ? files : files.Where(file => !pathExcludeFilters.Any(x => Regex.IsMatch(file, x))).ToList();

            return files;
        }

        public static IEnumerable<string> GetFileTypeFilters(GrepCommand grepCommand)
        {
            bool fileTypeFilterFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.FileTypeFilter);
            var fileTypeFilters = fileTypeFilterFlag ? grepCommand.CommandArgs[ConsoleFlag.FileTypeFilter].Split(new char[] { ',', ';' }).Select(x => x.Trim('.')) : null;

            return fileTypeFilters;
        }

        public static IEnumerable<string> GetFileTypeExcludeFilters(GrepCommand grepCommand)
        {
            bool fileTypeExcludeFilterFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.FileTypeExcludeFilter);
            var fileTypeExcludeFilters = fileTypeExcludeFilterFlag ? grepCommand.CommandArgs[ConsoleFlag.FileTypeExcludeFilter].Split(new char[] { ',', ';' }).Select(x => x.Trim('.')) : null;

            return fileTypeExcludeFilters;
        }

        public static IEnumerable<string> GetPathFilters(GrepCommand grepCommand)
        {
            bool pathFilterFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.PathFilter);
            var pathFilters = pathFilterFlag ? grepCommand.CommandArgs[ConsoleFlag.PathFilter].Split(new char[] { ',', ';' }) : null;

            return pathFilters;
        }

        public static IEnumerable<string> GetPathExcludeFilters(GrepCommand grepCommand)
        {
            bool pathExcludeFilterFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.PathExcludeFilter);
            var pathExcludeFilters = pathExcludeFilterFlag ? grepCommand.CommandArgs[ConsoleFlag.PathExcludeFilter].Split(new char[] { ',', ';' }) : null;

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
