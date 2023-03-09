using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WindowsGrep.Common;

namespace WindowsGrep.Engine
{
    public static class GrepEngine
    {
        #region Fields..
        private const string FILE_SIZE_PATTERN = @"(?<Size>\d+)(?<SizeType>\S{2})?";
        private const string FILE_NAME_PATTERN = @"^(.+)[\/\\](?<FileName>[^\/\\]+)$";

        private static Regex _fileSizeRegex = new Regex(FILE_SIZE_PATTERN);
        private static Regex _fileNameRegex = new Regex(FILE_NAME_PATTERN);
        private static object _searchLock = new object();
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        #endregion Constructors..

        #region Methods..
        #region BeginProcessCommand
        private static async Task BeginProcessCommandAsync(ConsoleCommand consoleCommand, GrepResultCollection grepResultCollection, CancellationToken cancellationToken)
        {
            bool writeFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Write);
            Stopwatch commandTimer = Stopwatch.StartNew();

            string filepath = GetPath(consoleCommand);
            List<string> files = GetFiles(consoleCommand, grepResultCollection, filepath);
            files = GetFilteredFiles(consoleCommand, files);

            // Clear the result collection between chained commands so that only the results of the final command are returned
            grepResultCollection.Clear();

            ConsoleUtils.WriteConsoleItem(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = $"{Environment.NewLine}[Searching {files.Count} file(s)]{Environment.NewLine}" });

            RegexOptions optionsFlags = GetRegexOptions(consoleCommand);
            await ProcessCommandAsync(grepResultCollection, files, consoleCommand, optionsFlags, cancellationToken);

            if (writeFlag)
            {
                grepResultCollection.Write(consoleCommand.CommandArgs[ConsoleFlag.Write]);
            }

            // Publish command run time
            commandTimer.Stop();

            ConsoleUtils.WriteConsoleItem(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = $"{Environment.NewLine}[{Math.Round((commandTimer.ElapsedMilliseconds / 1000.0), 2)} second(s)]" });
            ConsoleUtils.WriteConsoleItem(new ConsoleItem() { Value = Environment.NewLine + Environment.NewLine });
        }
        #endregion BeginProcessCommand

        #region BuildSearchPattern
        private static string BuildSearchPattern(ConsoleCommand consoleCommand)
        {
            bool fixedStringsFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FixedStrings);
            bool iIgnoreCaseFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.IgnoreCase);

            string searchPattern = string.Empty;
            string searchTerm = consoleCommand.CommandArgs[ConsoleFlag.SearchTerm];
            string ignoreCaseModifier = iIgnoreCaseFlag ? @"(?i)" : string.Empty;

            // Ignore carriage-return and newline characters when using endline regex to match expected behavior from other regex engines
            searchTerm = searchTerm.Replace("$", "[\r\n]*$");

            searchTerm = fixedStringsFlag ? Regex.Escape(searchTerm) : searchTerm;
            searchPattern = @"(?<MatchedString>" + ignoreCaseModifier + searchTerm + @")";

            return searchPattern;
        }
        #endregion BuildSearchPattern

        #region BuildSearchResultsFileContent
        private static void BuildSearchResultsFileContent(ConsoleCommand consoleCommand, GrepResultCollection grepResultCollection, List<Match> matches, string filename, long fileSize, string fileRaw)
        {
            bool ignoreBreaksFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.IgnoreBreaks);
            bool ignoreCaseFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.IgnoreCase);
            bool contextFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Context);
            bool nResultsFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.NResults);
            bool suppressFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Suppress);

            // Build file context search pattern
            string searchTerm = consoleCommand.CommandArgs[ConsoleFlag.SearchTerm];
            int contextLength = contextFlag ? Convert.ToInt32(consoleCommand.CommandArgs[ConsoleFlag.Context]) : 0;

            matches.ToList().ForEach(match =>
            {
                string leadingContext = string.Empty;
                string trailingContext = string.Empty;

                if (contextFlag)
                {
                    // Rebuild matches with contextual text
                    int leadingContextStartIndex = match.Groups["MatchedString"].Index - contextLength < 0 ? 0 : match.Groups["MatchedString"].Index - contextLength;
                    int leadingContextLength = match.Groups["MatchedString"].Index < contextLength ? match.Groups["MatchedString"].Index : contextLength;

                    int trailingContextStartIndex = match.Groups["MatchedString"].Index + match.Groups["MatchedString"].Value.Length;
                    int trailingContextLength = trailingContextStartIndex + contextLength > fileRaw.Length ? fileRaw.Length - trailingContextStartIndex : contextLength;

                    leadingContext = Environment.NewLine + fileRaw.Substring(leadingContextStartIndex, leadingContextLength);
                    trailingContext = fileRaw.Substring(trailingContextStartIndex, trailingContextLength) + Environment.NewLine;
                }

                string matchedString = match.Groups["MatchedString"].Value;

                GrepResult grepResult = new GrepResult(filename, ResultScope.FileContent)
                {
                    Suppressed = suppressFlag,
                    FileSize = fileSize,
                    LeadingContextString = leadingContext,
                    TrailingContextString = trailingContext,
                    MatchedString = matchedString
                };

                // Line number
                int lineNumber = fileRaw.Substring(0, match.Groups["MatchedString"].Index).Split('\n').Length;
                grepResult.LineNumber = lineNumber;

                lock (grepResultCollection)
                {
                    if (!nResultsFlag || grepResultCollection.Count < Convert.ToInt32(consoleCommand.CommandArgs[ConsoleFlag.NResults]))
                    {
                        grepResultCollection.AddItem(grepResult);
                    }
                }
            });
        }
        #endregion BuildSearchResultsFileContent

        #region GetFiles
        private static List<string> GetFiles(ConsoleCommand consoleCommand, IList<GrepResult> grepResultCollection, string filepath)
        {
            bool recursiveFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Recursive);
            bool targetFileFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.TargetFile);

            List<string> files = null;

            if (grepResultCollection.Any())
            {
                if (targetFileFlag)
                {
                    files = grepResultCollection.Where(x => x.SourceFile == filepath).Select(result => result.SourceFile).ToList();
                }
                else
                {
                    files = grepResultCollection.Select(result => result.SourceFile).ToList();
                }
            }
            else
            {
                if (targetFileFlag)
                {
                    files = new List<string>() { filepath };
                }
                else
                {
                    var EnumerationOptions = new EnumerationOptions() { ReturnSpecialDirectories = true, AttributesToSkip = FileAttributes.System };
                    if (recursiveFlag)
                    {
                        EnumerationOptions.RecurseSubdirectories = true;
                    }

                    files = Directory.GetFiles(Path.TrimEndingDirectorySeparator(filepath.TrimEnd()), "*", EnumerationOptions).ToList();
                }
            }

            return files;
        }
        #endregion GetFiles

        #region GetFileContentMatches
        private static async Task GetFileContentMatchesAsync(GrepResultCollection grepResultCollection, IEnumerable<string> files, ConsoleCommand consoleCommand, string searchPattern,
            Regex searchRegex, SearchMetrics searchMetrics, long fileSizeMin, long fileSizeMax, CancellationToken cancellationToken)
        {
            bool fixedStringsFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FixedStrings);
            bool deleteFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Delete);
            bool replaceFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Replace);
            bool writeFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Write);
            bool fileNamesOnlyFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileNamesOnly);
            bool fileSizeMinimumFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileSizeMinimum);
            bool fileSizeMaximumFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileSizeMaximum);
            bool nResultsFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.NResults);
            bool suppressFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Suppress);

            files.AsParallel().ForAll(file =>
            {
                try
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    if (!nResultsFlag || grepResultCollection.Count < Convert.ToInt32(consoleCommand.CommandArgs[ConsoleFlag.NResults]))
                    {
                        List<Match> matches = new List<Match>();

                        // Validate any filesize parameters
                        var fileSize = fileSizeMaximumFlag || fileSizeMinimumFlag ? WindowsUtils.GetFileSizeOnDisk(file) : -1;
                        bool fileSizeValidateSuccess = ValidateFileSize(consoleCommand, fileSize, fileSizeMin, fileSizeMax);

                        if (fileSizeValidateSuccess)
                        {
                            string fileRaw = File.ReadAllText(file);

                            matches = searchRegex.Matches(fileRaw).ToList();
                            if (matches.Any())
                            {
                                // Write operations
                                bool isWriteOperation = replaceFlag || deleteFlag;
                                if (isWriteOperation)
                                    PerformWriteOperations(consoleCommand, file, searchPattern, matches.Count, ref fileRaw, searchMetrics);

                                // Read operations
                                else
                                {
                                    BuildSearchResultsFileContent(consoleCommand, grepResultCollection, matches, file, fileSize, fileRaw);

                                    lock (_searchLock)
                                    {
                                        searchMetrics.TotalFilesMatchedCount++;
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                    lock (_searchLock)
                    {
                        searchMetrics.FileReadFailedCount++;
                    }
                }
            });
        }
        #endregion GetFileContentMatches

        #region GetFileNameMatches
        private static async Task GetFileNameMatchesAsync(GrepResultCollection grepResultCollection, IEnumerable<string> files, ConsoleCommand consoleCommand, 
            string searchPattern, Regex searchRegex, SearchMetrics searchMetrics, long fileSizeMin, long fileSizeMax, CancellationToken cancellationToken)
        {
            var matches = new List<GrepResult>();

            bool fixedStringsFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FixedStrings);
            bool deleteFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Delete);
            bool replaceFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Replace);
            bool writeFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Write);
            bool fileNamesOnlyFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileNamesOnly);         
            bool nResultsFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.NResults);

            int nResults = nResultsFlag ? Convert.ToInt32(consoleCommand.CommandArgs[ConsoleFlag.NResults]) : int.MaxValue;

            files.ToList().ForEach(file =>
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                if (!nResultsFlag || matches.Count < nResults)
                {
                    // Don't want to waste any time on files that have already been added 
                    bool fileAdded = matches.Any(x => x.SourceFile == file);
                    if (!fileAdded)
                    {
                        // Parse filename from path
                        var fileNameMatch = _fileNameRegex.Match(file)?.Groups["FileName"];
                        if (fileNameMatch != null)
                        {
                            // Query against filename
                            var searchMatch = searchRegex.Match(fileNameMatch.Value);
                            if (searchMatch != Match.Empty)
                            {
                                // Write operations
                                bool isWriteOperation = replaceFlag || deleteFlag;
                                if (isWriteOperation)
                                    PerformWriteOperations(consoleCommand, file, searchPattern, matches.Count, ref file, searchMetrics);
                                else
                                    PerformReadOperations(matches, consoleCommand, file, fileNameMatch, searchMatch, fileSizeMin, fileSizeMax);
                            }
                        }
                    }
                }
            });

            searchMetrics.TotalFilesMatchedCount = matches.Count();
            grepResultCollection.AddItemRange(matches);
        }
        #endregion GetFileNameMatches

        #region GetFileSizeMaximum
        private static long GetFileSizeMaximum(ConsoleCommand consoleCommand)
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
        #endregion GetFileSizeMaximum

        #region GetFileSizeMinimum
        private static long GetFileSizeMinimum(ConsoleCommand consoleCommand)
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
        #endregion GetFileSizeMinimum

        #region GetFilteredFiles
        /// <summary>
        /// </summary>
        /// <param name="consoleCommand"></param>
        /// <param name="files"></param>
        /// <returns>Returns files filtered by Inclusion/Exclusion type parameters</returns>
        private static List<string> GetFilteredFiles(ConsoleCommand consoleCommand, List<string> files)
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
        #endregion GetFilteredFiles

        #region GetFileTypeFilters
        private static IEnumerable<string> GetFileTypeFilters(ConsoleCommand consoleCommand)
        {
            bool fileTypeFilterFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileTypeFilter);
            var fileTypeFilters = fileTypeFilterFlag ? consoleCommand.CommandArgs[ConsoleFlag.FileTypeFilter].Split(new char[] { ',', ';' }).Select(x => x.Trim('.')) : null;

            return fileTypeFilters;
        }
        #endregion GetFileTypeFilters

        #region GetFileTypeExcludeFilters
        private static IEnumerable<string> GetFileTypeExcludeFilters(ConsoleCommand consoleCommand)
        {
            bool fileTypeExcludeFilterFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileTypeExcludeFilter);
            var fileTypeExcludeFilters = fileTypeExcludeFilterFlag ? consoleCommand.CommandArgs[ConsoleFlag.FileTypeExcludeFilter].Split(new char[] { ',', ';' }).Select(x => x.Trim('.')) : null;

            return fileTypeExcludeFilters;
        }
        #endregion GetFileTypeFilters

        #region GetPathFilters
        private static IEnumerable<string> GetPathFilters(ConsoleCommand consoleCommand)
        {
            bool pathFilterFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.PathFilter);
            var pathFilters = pathFilterFlag ? consoleCommand.CommandArgs[ConsoleFlag.PathFilter].Split(new char[] { ',', ';' }) : null;

            return pathFilters;
        }
        #endregion GetPathFilters

        #region GetPathExcludeFilters
        private static IEnumerable<string> GetPathExcludeFilters(ConsoleCommand consoleCommand)
        {
            bool pathExcludeFilterFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.PathExcludeFilter);
            var pathExcludeFilters = pathExcludeFilterFlag ? consoleCommand.CommandArgs[ConsoleFlag.PathExcludeFilter].Split(new char[] { ',', ';' }) : null;

            return pathExcludeFilters;
        }
        #endregion GetPathExcludeFilters

        #region GetPath
        private static string GetPath(ConsoleCommand consoleCommand)
        {
            bool directoryFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Directory);
            bool targetFileFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.TargetFile);

            // User specified file should overrule specified directory
            string filepath = directoryFlag ? consoleCommand.CommandArgs[ConsoleFlag.Directory] : Environment.CurrentDirectory;
            filepath = targetFileFlag ? consoleCommand.CommandArgs[ConsoleFlag.TargetFile] : filepath;

            return filepath;
        }
        #endregion GetPath

        #region GetRegexOptions
        private static RegexOptions GetRegexOptions(ConsoleCommand consoleCommand)
        {
            RegexOptions optionsFlags = 0;
            bool ignoreCaseFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.IgnoreCase);
            bool ignoreBreaksFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.IgnoreBreaks);

            if (ignoreCaseFlag)
                optionsFlags |= RegexOptions.IgnoreCase;
            if (ignoreBreaksFlag)
                optionsFlags |= RegexOptions.Singleline;
            else
                optionsFlags |= RegexOptions.Multiline;

            return optionsFlags;
        }
        #endregion GetRegexOptions

        #region ProcessCommand
        private static async Task ProcessCommandAsync(GrepResultCollection grepResultCollection, IEnumerable<string> files, ConsoleCommand consoleCommand, RegexOptions optionsFlags, CancellationToken cancellationToken)
        {
            bool fileNamesOnlyFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileNamesOnly);

            long fileSizeMin = GetFileSizeMinimum(consoleCommand);
            long fileSizeMax = GetFileSizeMaximum(consoleCommand);
            var searchMetrics = new SearchMetrics();

            // Build content search pattern
            string searchPattern = BuildSearchPattern(consoleCommand);
            Regex searchRegex = new Regex(searchPattern, optionsFlags);

            if (fileNamesOnlyFlag)
                await GetFileNameMatchesAsync(grepResultCollection, files, consoleCommand, searchPattern, searchRegex, searchMetrics, fileSizeMin, fileSizeMax, cancellationToken);
            else
            {
                if (consoleCommand.CommandArgs[ConsoleFlag.SearchTerm] == string.Empty)
                    throw new Exception("Error: Search term not supplied");

                await GetFileContentMatchesAsync(grepResultCollection, files, consoleCommand, searchPattern, searchRegex, searchMetrics, fileSizeMin, fileSizeMax, cancellationToken);
            }

            // Notify the user of any files that could not be read from or written to
            PublishFileAccessSummary(searchMetrics);

            // Publish command summary to console
            PublishCommandSummary(consoleCommand, grepResultCollection, searchMetrics);
        }
        #endregion ProcessCommand

        #region PerformReadOperations
        private static void PerformReadOperations(List<GrepResult> matches, ConsoleCommand consoleCommand, string fileName, Group fileNameMatch, Match searchMatch, long fileSizeMin, long fileSizeMax)
        {
            bool suppressFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Suppress);
            bool fileSizeMinimumFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileSizeMinimum);
            bool fileSizeMaximumFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileSizeMaximum);

            int trailingContextStringStartIndex = fileNameMatch.Index + searchMatch.Index + searchMatch.Length;

            // Validate any filesize parameters
            var fileSize = fileSizeMaximumFlag || fileSizeMinimumFlag ? WindowsUtils.GetFileSizeOnDisk(fileName) : -1;
            bool fileSizevalidateSuccess = ValidateFileSize(consoleCommand, fileSize, fileSizeMin, fileSizeMax);

            if (fileSizevalidateSuccess)
            {
                GrepResult grepResult = new GrepResult(fileName, ResultScope.FileName)
                {
                    Suppressed = suppressFlag,
                    FileSize = fileSize,
                    LeadingContextString = fileName.Substring(0, fileNameMatch.Index + searchMatch.Index),
                    MatchedString = searchMatch.Value,
                    TrailingContextString = fileName.Substring(trailingContextStringStartIndex, fileName.Length - trailingContextStringStartIndex)
                };

                matches.Add(grepResult);
            }
        }
        #endregion PerformReadOperations

        #region PerformWriteOperations
        private static List<ConsoleItem> PerformWriteOperations(ConsoleCommand consoleCommand, string fileName, string searchPattern, int fileMatchesCount,
             ref string fileRaw, SearchMetrics searchMetrics)
        {
            var consoleItemCollection = new List<ConsoleItem>();
            bool deleteFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Delete);
            bool replaceFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Replace);

            // FileName
            consoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.DarkYellow, Value = $"{fileName} " });

            try
            {
                if (deleteFlag)
                {
                    // Delete file
                    File.Delete(fileName);

                    consoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = $"Deleted" });

                    lock (_searchLock)
                    {
                        searchMetrics.DeleteSuccessCount++;
                    }
                }
                else if (replaceFlag)
                {
                    // Replace all occurrences within the file
                    fileRaw = Regex.Replace(fileRaw, searchPattern, consoleCommand.CommandArgs[ConsoleFlag.Replace]);
                    File.WriteAllText(fileName, fileRaw);

                    string matchesText = fileMatchesCount == 1 ? "match" : "matches";
                    consoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.DarkMagenta, Value = $"{fileMatchesCount} {matchesText}" });

                    lock (_searchLock)
                    {
                        searchMetrics.ReplacedSuccessCount += fileMatchesCount;
                    }
                }
            }
            catch
            {
                consoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.Gray, BackgroundColor = ConsoleColor.DarkRed, Value = $"Access Denied" });

                lock (_searchLock)
                {
                    searchMetrics.FileWriteFailedCount++;
                }
            }
            finally
            {
                // Empty buffer
                consoleItemCollection.Add(new ConsoleItem() { Value = Environment.NewLine });

                ConsoleUtils.WriteConsoleItemCollection(consoleItemCollection);

                lock (_searchLock)
                {
                    searchMetrics.TotalFilesMatchedCount++;
                }
            }

            return consoleItemCollection;
        }
        #endregion PerformWriteOperations

        #region PublishCommandSummary
        private static void PublishCommandSummary(ConsoleCommand consoleCommand, IList<GrepResult> grepResultCollection, SearchMetrics searchMetrics)
        {
            bool deleteFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Delete);
            bool replaceFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Replace);
            bool fileSizeMinimumFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileSizeMinimum);
            bool fileSizeMaximumFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileSizeMaximum);

            string summary = string.Empty;

            if (deleteFlag)
                summary = $"[{searchMetrics.DeleteSuccessCount} of {searchMetrics.TotalFilesMatchedCount} file(s) deleted]";
            else if (replaceFlag)
                summary = $"[{searchMetrics.ReplacedSuccessCount} occurrence(s) replaced in {searchMetrics.TotalFilesMatchedCount} file(s)]";
            else
                summary = $"[{grepResultCollection.Count} result(s) {searchMetrics.TotalFilesMatchedCount} file(s)]";

            ConsoleUtils.WriteConsoleItem(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = summary });

            if (fileSizeMinimumFlag || fileSizeMaximumFlag)
            {
                var totalFileSize = grepResultCollection.Sum(x => x.FileSize);
                var fileSizeReduced = WindowsUtils.GetReducedSize(totalFileSize, 3, out FileSizeType fileSizeType);

                summary = $" [{fileSizeReduced} {fileSizeType}(s)]";

                ConsoleUtils.WriteConsoleItem(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = summary });
            }
        }
        #endregion PublishCommandSummary

        #region PublishFileAccessSummary
        private static void PublishFileAccessSummary(SearchMetrics searchMetrics)
        {
            if (searchMetrics.FileReadFailedCount > 0 || searchMetrics.FileWriteFailedCount > 0)
            {
                if (searchMetrics.FileReadFailedCount > 0)
                {
                    string unreachableFiles = $"[{searchMetrics.FileReadFailedCount} file(s) unreadable/inaccessible]";
                    ConsoleUtils.WriteConsoleItem(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = unreachableFiles });
                }

                if (searchMetrics.FileWriteFailedCount > 0)
                {
                    string unwriteableFiles = $"[{searchMetrics.FileWriteFailedCount} file(s) could not be modified]";
                    ConsoleUtils.WriteConsoleItem(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = unwriteableFiles });
                }

                ConsoleUtils.WriteConsoleItem(new ConsoleItem() { Value = Environment.NewLine });
            }
        }
        #endregion PublishFileAccessSummary

        #region RunCommand
        public static async Task RunCommandAsync(string commandRaw, GrepResultCollection grepResultCollection, CancellationToken cancellationToken)
        {
            string splitPattern = @"\|(?![^{]*}|[^\(]*\)|[^\[]*\])";
            string[] commandCollection = Regex.Split(commandRaw, splitPattern);

            foreach (string command in commandCollection)
            {
                var commandArgs = ConsoleUtils.DiscoverCommandArgs(command);
                ConsoleCommand consoleCommand = new ConsoleCommand() { CommandArgs = commandArgs };

                await BeginProcessCommandAsync(consoleCommand, grepResultCollection, cancellationToken);
            }
        }
        #endregion RunCommand

        #region ValidateFileSize
        /// <summary>
        /// Check that a given filesize is within any filesize parameters
        /// </summary>
        /// <returns></returns>
        private static bool ValidateFileSize(ConsoleCommand consoleCommand, long fileSize, long fileSizeMinimum, long fileSizeMaximum)
        {
            bool isValid = true;

            bool fileSizeMinimumFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileSizeMinimum);
            bool fileSizeMaximumFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileSizeMaximum);

            isValid &= (!fileSizeMinimumFlag || (fileSizeMinimumFlag && fileSize >= fileSizeMinimum));
            isValid &= (!fileSizeMaximumFlag || (fileSizeMaximumFlag && fileSize <= fileSizeMaximum));

            return isValid;
        }
        #endregion ValidateFileSize
        #endregion Methods..
    }
}
