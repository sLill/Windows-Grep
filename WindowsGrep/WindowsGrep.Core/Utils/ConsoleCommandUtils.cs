using System.Text.RegularExpressions;
using WindowsGrep.Common;

namespace WindowsGrep.Core
{
    public static class ConsoleCommandUtils
    {
        #region Fields..
        private const string FILE_SIZE_PATTERN = @"(?<Size>\d+)(?<SizeType>\S{2})?";

        private static Regex _fileSizeRegex = new Regex(FILE_SIZE_PATTERN);
        #endregion Fields..

        #region Methods..
        public static long GetFileSizeMaximum(ConsoleCommand consoleCommand)
        {
            long fileSizeMaximum = -1;
            bool fileSizeMaximumFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileSizeMaximum);

            if (fileSizeMaximumFlag)
            {
                try
                {
                    string fileSizeMaximumParameter = consoleCommand.CommandArgs[ConsoleFlag.FileSizeMaximum];

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

        public static long GetFileSizeMinimum(ConsoleCommand consoleCommand)
        {
            long fileSizeMinimum = -1;
            bool fileSizeMinimumFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileSizeMinimum);

            if (fileSizeMinimumFlag)
            {
                try
                {
                    string fileSizeMinimumParameter = consoleCommand.CommandArgs[ConsoleFlag.FileSizeMinimum];

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

        /// <summary>
        /// </summary>
        /// <param name="consoleCommand"></param>
        /// <param name="files"></param>
        /// <returns>Returns files filtered by Inclusion/Exclusion type parameters</returns>
        public static List<string> GetFilteredFiles(ConsoleCommand consoleCommand, List<string> files)
        {
            var fileTypeFilters = GetFileTypeFilters(consoleCommand);
            var fileTypeExcludeFilters = GetFileTypeExcludeFilters(consoleCommand);
            var pathFilters = GetPathFilters(consoleCommand);
            var pathExcludeFilters = GetPathExcludeFilters(consoleCommand);

            // Filter files by type
            files = fileTypeFilters == null ? files : files.Where(file => fileTypeFilters.Contains(Path.GetExtension(file).Trim('.'))).ToList();
            files = fileTypeExcludeFilters == null ? files : files.Where(file => !fileTypeExcludeFilters.Contains(Path.GetExtension(file).Trim('.'))).ToList();

            // Filter files by relative subpaths
            files = pathFilters == null ? files : files.Where(file => pathFilters.Any(x => Regex.IsMatch(file, x))).ToList();
            files = pathExcludeFilters == null ? files : files.Where(file => !pathExcludeFilters.Any(x => Regex.IsMatch(file, x))).ToList();

            return files;
        }

        public static IEnumerable<string> GetFileTypeFilters(ConsoleCommand consoleCommand)
        {
            bool fileTypeFilterFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileTypeFilter);
            var fileTypeFilters = fileTypeFilterFlag ? consoleCommand.CommandArgs[ConsoleFlag.FileTypeFilter].Split(new char[] { ',', ';' }).Select(x => x.Trim('.')) : null;

            return fileTypeFilters;
        }

        public static IEnumerable<string> GetFileTypeExcludeFilters(ConsoleCommand consoleCommand)
        {
            bool fileTypeExcludeFilterFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileTypeExcludeFilter);
            var fileTypeExcludeFilters = fileTypeExcludeFilterFlag ? consoleCommand.CommandArgs[ConsoleFlag.FileTypeExcludeFilter].Split(new char[] { ',', ';' }).Select(x => x.Trim('.')) : null;

            return fileTypeExcludeFilters;
        }

        public static IEnumerable<string> GetPathFilters(ConsoleCommand consoleCommand)
        {
            bool pathFilterFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.PathFilter);
            var pathFilters = pathFilterFlag ? consoleCommand.CommandArgs[ConsoleFlag.PathFilter].Split(new char[] { ',', ';' }) : null;

            return pathFilters;
        }

        public static IEnumerable<string> GetPathExcludeFilters(ConsoleCommand consoleCommand)
        {
            bool pathExcludeFilterFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.PathExcludeFilter);
            var pathExcludeFilters = pathExcludeFilterFlag ? consoleCommand.CommandArgs[ConsoleFlag.PathExcludeFilter].Split(new char[] { ',', ';' }) : null;

            return pathExcludeFilters;
        }
        #endregion Methods..
    }
}
