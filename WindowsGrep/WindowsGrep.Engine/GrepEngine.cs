using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WindowsGrep.Common;
using WindowsGrep.Core;

namespace WindowsGrep.Engine
{
    public static class GrepEngine
    {
        #region Fields..
        private const string FILE_NAME_PATTERN = @"^(.+)[\/\\](?<FileName>[^\/\\]+)$";

        private static Regex _fileNameRegex = new Regex(FILE_NAME_PATTERN);
        private static object _metricsLock = new object();
        #endregion Fields..

        #region Methods..
        public static async Task BeginProcessGrepCommandAsync(GrepCommand grepCommand, GrepResultCollection grepResultCollection, CancellationToken cancellationToken)
        {
            switch (grepCommand.CommandType)
            {
                case GrepCommandType.Help:
                    ConsoleUtils.PublishReadMe();
                    break;

                case GrepCommandType.Query:
                    await QueryAsync(grepCommand, grepResultCollection, cancellationToken);
                    break;
            }
        }

        private static async Task QueryAsync(GrepCommand grepCommand, GrepResultCollection grepResultCollection, CancellationToken cancellationToken)
        {
            bool writeFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.Write);
            Stopwatch commandTimer = Stopwatch.StartNew();

            string filepath = grepCommandUtils.GetPath(grepCommand);
            List<string> files = GetFiles(grepCommand, grepResultCollection, filepath);
            files = grepCommandUtils.GetFilteredFiles(grepCommand, files);

            // Clear the result collection between chained commands so that only the results of the final command are returned
            grepResultCollection.Clear();

            ConsoleUtils.WriteConsoleItem(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = $"{Environment.NewLine}[Searching {files.Count} file(s)]{Environment.NewLine}" });

            RegexOptions optionsFlags = grepCommandUtils.GetRegexOptions(grepCommand);
            await ProcessCommandAsync(grepResultCollection, files, grepCommand, optionsFlags, cancellationToken);

            if (writeFlag)
                grepResultCollection.Write(grepCommand.CommandArgs[ConsoleFlag.Write]);

            // Publish command run time
            commandTimer.Stop();

            ConsoleUtils.WriteConsoleItem(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = $"{Environment.NewLine}[{Math.Round((commandTimer.ElapsedMilliseconds / 1000.0), 2)} second(s)]" });
            ConsoleUtils.WriteConsoleItem(new ConsoleItem() { Value = Environment.NewLine + Environment.NewLine });
        }

        private static async Task BuildFileContentSearchResultsAsync(GrepCommand grepCommand, GrepResultCollection grepResultCollection, List<Match> matches,
            string filename, long fileSize, string fileRaw, CancellationToken cancellationToken)
        {
            bool ignoreBreaksFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.IgnoreBreaks);
            bool ignoreCaseFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.IgnoreCase);
            bool contextFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.Context);
            bool nResultsFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.NResults);
            bool suppressFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.Suppress);

            // Build file context search pattern
            string searchTerm = grepCommand.CommandArgs[ConsoleFlag.SearchTerm];
            int contextLength = contextFlag ? Convert.ToInt32(grepCommand.CommandArgs[ConsoleFlag.Context]) : 0;

            matches.ToList().ForEach(match =>
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

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
                    if (!nResultsFlag || grepResultCollection.Count < Convert.ToInt32(grepCommand.CommandArgs[ConsoleFlag.NResults]))
                        grepResultCollection.AddItem(grepResult);
                }
            });
        }

        private static List<string> GetFiles(GrepCommand grepCommand, IList<GrepResult> grepResultCollection, string filepath)
        {
            bool recursiveFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.Recursive);
            bool targetFileFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.TargetFile);

            List<string> files = null;

            if (grepResultCollection.Any())
            {
                if (targetFileFlag)
                    files = grepResultCollection.Where(x => x.SourceFile == filepath).Select(result => result.SourceFile).ToList();
                else
                    files = grepResultCollection.Select(result => result.SourceFile).ToList();
            }
            else
            {
                if (targetFileFlag)
                    files = new List<string>() { filepath };
                else
                {
                    var EnumerationOptions = new EnumerationOptions() { ReturnSpecialDirectories = true, AttributesToSkip = FileAttributes.System };
                    if (recursiveFlag)
                        EnumerationOptions.RecurseSubdirectories = true;

                    files = Directory.GetFiles(Path.TrimEndingDirectorySeparator(filepath.TrimEnd()), "*", EnumerationOptions).ToList();
                }
            }

            return files;
        }

        private static async Task GetFileContentMatchesAsync(GrepResultCollection grepResultCollection, IEnumerable<string> files, GrepCommand grepCommand, string searchPattern,
            Regex searchRegex, SearchMetrics searchMetrics, long fileSizeMin, long fileSizeMax, CancellationToken cancellationToken)
        {
            bool fixedStringsFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.FixedStrings);
            bool deleteFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.Delete);
            bool replaceFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.Replace);
            bool writeFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.Write);
            bool fileNamesOnlyFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.FileNamesOnly);
            bool fileSizeMinimumFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.FileSizeMinimum);
            bool fileSizeMaximumFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.FileSizeMaximum);
            bool nResultsFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.NResults);
            bool suppressFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.Suppress);

            files.AsParallel().ForAll(async file =>
            {
                try
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    if (!nResultsFlag || grepResultCollection.Count < Convert.ToInt32(grepCommand.CommandArgs[ConsoleFlag.NResults]))
                    {
                        List<Match> matches = new List<Match>();

                        // Validate any filesize parameters
                        var fileSize = fileSizeMaximumFlag || fileSizeMinimumFlag ? WindowsUtils.GetFileSizeOnDisk(file) : -1;
                        bool fileSizeValidateSuccess = ValidateFileSize(grepCommand, fileSize, fileSizeMin, fileSizeMax);

                        if (fileSizeValidateSuccess)
                        {
                            string fileRaw = File.ReadAllText(file);

                            matches = searchRegex.Matches(fileRaw).ToList();
                            if (matches.Any())
                            {
                                // Write operations
                                bool isWriteOperation = replaceFlag || deleteFlag;
                                if (isWriteOperation)
                                    PerformWriteOperations(grepCommand, file, searchPattern, matches.Count, ref fileRaw, searchMetrics);

                                // Read operations
                                else
                                {
                                    await BuildFileContentSearchResultsAsync(grepCommand, grepResultCollection, matches, file, fileSize, fileRaw, cancellationToken);

                                    lock (_metricsLock)
                                        searchMetrics.TotalFilesMatchedCount++;
                                }
                            }
                        }
                    }
                }
                catch
                {
                    lock (_metricsLock)
                        searchMetrics.FileReadFailedCount++;
                }
            });
        }

        private static void GetFileHashMatchesAsync(GrepResultCollection grepResultCollection, IEnumerable<string> files, GrepCommand grepCommand,
            string searchTerm, SearchMetrics searchMetrics, HashType hashType, long fileSizeMin, long fileSizeMax, CancellationToken cancellationToken)
        {
            var matches = new List<GrepResult>();

            bool deleteFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.Delete);
            bool writeFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.Write);
            bool nResultsFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.NResults);

            int nResults = nResultsFlag ? Convert.ToInt32(grepCommand.CommandArgs[ConsoleFlag.NResults]) : int.MaxValue;
            Regex _anyRegex = new Regex(@".*");

            files.AsParallel().ForAll(file =>
            {
                try
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    if (!nResultsFlag || matches.Count < nResults)
                    {
                        // Don't want to waste any time on files that have already been added 
                        bool fileAdded = matches.Any(x => x.SourceFile == file);
                        if (!fileAdded)
                        {
                            // Compare file hashes
                            string fileHash = WindowsUtils.GetFileHash(file, hashType);
                            if (string.Equals(fileHash, searchTerm, StringComparison.OrdinalIgnoreCase))
                            {
                                // Parse filename from path
                                Match fileNameMatch = _fileNameRegex.Match(file);
                                if (fileNameMatch != null)
                                {
                                    Group fileNameGroup = fileNameMatch.Groups["FileName"];
                                    Match searchMatch = _anyRegex.Match(fileHash);

                                    // Write operations
                                    bool isWriteOperation = deleteFlag;
                                    if (isWriteOperation)
                                        PerformWriteOperations(grepCommand, file, searchTerm, matches.Count, ref file, searchMetrics);
                                    else
                                        PerformReadOperations(matches, grepCommand, file, fileNameMatch, searchMatch, fileSizeMin, fileSizeMax);
                                }
                            }
                        }
                    }
                }
                catch
                {
                    lock (_metricsLock)
                        searchMetrics.FileReadFailedCount++;
                }
            });

            searchMetrics.TotalFilesMatchedCount = matches.Count();
            grepResultCollection.AddItemRange(matches);
        }

        private static void GetFileNameMatchesAsync(GrepResultCollection grepResultCollection, IEnumerable<string> files, GrepCommand grepCommand,
            string searchPattern, Regex searchRegex, SearchMetrics searchMetrics, long fileSizeMin, long fileSizeMax, CancellationToken cancellationToken)
        {
            var matches = new List<GrepResult>();

            bool fixedStringsFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.FixedStrings);
            bool deleteFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.Delete);
            bool replaceFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.Replace);
            bool writeFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.Write);
            bool nResultsFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.NResults);

            int nResults = nResultsFlag ? Convert.ToInt32(grepCommand.CommandArgs[ConsoleFlag.NResults]) : int.MaxValue;

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
                                    PerformWriteOperations(grepCommand, file, searchPattern, matches.Count, ref file, searchMetrics);
                                else
                                    PerformReadOperations(matches, grepCommand, file, fileNameMatch, searchMatch, fileSizeMin, fileSizeMax);
                            }
                        }
                    }
                }
            });

            searchMetrics.TotalFilesMatchedCount = matches.Count();
            grepResultCollection.AddItemRange(matches);
        }

        private static async Task ProcessCommandAsync(GrepResultCollection grepResultCollection, IEnumerable<string> files, GrepCommand grepCommand, RegexOptions optionsFlags, CancellationToken cancellationToken)
        {
            bool fileNamesOnlyFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.FileNamesOnly);
            bool fileHashesOnlyFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.FileHashes);

            long fileSizeMin = grepCommandUtils.GetFileSizeMinimum(grepCommand);
            long fileSizeMax = grepCommandUtils.GetFileSizeMaximum(grepCommand);
            HashType hashType = grepCommandUtils.GetHashType(grepCommand);

            var searchMetrics = new SearchMetrics();

            // Build content search pattern
            string searchTerm = grepCommand.CommandArgs[ConsoleFlag.SearchTerm];
            string searchPattern = grepCommandUtils.BuildSearchPattern(grepCommand);
            Regex searchRegex = new Regex(searchPattern, optionsFlags);

            if (fileHashesOnlyFlag)
            {
                bool isValidFileHash = WindowsUtils.IsValidFileHash(searchTerm, hashType);
                if (!isValidFileHash)
                    throw new Exception($"Error: Hash does not match {hashType} format");

                GetFileHashMatchesAsync(grepResultCollection, files, grepCommand, searchTerm, searchMetrics, hashType, fileSizeMin, fileSizeMax, cancellationToken);
            }
            else if (fileNamesOnlyFlag)
                GetFileNameMatchesAsync(grepResultCollection, files, grepCommand, searchPattern, searchRegex, searchMetrics, fileSizeMin, fileSizeMax, cancellationToken);
            else
            {
                if (grepCommand.CommandArgs[ConsoleFlag.SearchTerm] == string.Empty)
                    throw new Exception("Error: Search term not supplied");

                await GetFileContentMatchesAsync(grepResultCollection, files, grepCommand, searchPattern, searchRegex, searchMetrics, fileSizeMin, fileSizeMax, cancellationToken);
            }

            // Notify the user of any files that could not be read from or written to
            PublishFileAccessSummary(searchMetrics);

            // Publish command summary to console
            PublishCommandSummary(grepCommand, grepResultCollection, searchMetrics);
        }

        private static void PerformReadOperations(List<GrepResult> matches, GrepCommand grepCommand, string fileName, Group fileNameMatch, Match searchMatch, long fileSizeMin, long fileSizeMax)
        {
            bool suppressFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.Suppress);
            bool fileHashesFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.FileHashes);
            bool fileSizeMinimumFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.FileSizeMinimum);
            bool fileSizeMaximumFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.FileSizeMaximum);

            // Validate any filesize parameters
            var fileSize = fileSizeMaximumFlag || fileSizeMinimumFlag ? WindowsUtils.GetFileSizeOnDisk(fileName) : -1;
            bool fileSizevalidateSuccess = ValidateFileSize(grepCommand, fileSize, fileSizeMin, fileSizeMax);

            if (fileSizevalidateSuccess)
            {
                var resultScope = fileHashesFlag ? ResultScope.FileHash : ResultScope.FileName;

                string leadingContextString = string.Empty;
                int leadingContextStringStartIndex = 0;
                int leadingContextStringEndIndex = 0;

                string trailingContextString = string.Empty;
                int trailingContextStringStartIndex = 0;
                int trailingContextStringEndIndex = 0;

                switch (resultScope)
                {
                    case ResultScope.FileName:
                        leadingContextStringEndIndex = fileNameMatch.Index + searchMatch.Index;
                        trailingContextStringStartIndex = fileNameMatch.Index + searchMatch.Index + searchMatch.Length;
                        trailingContextStringEndIndex = fileName.Length - trailingContextStringStartIndex;

                        leadingContextString = fileName.Substring(leadingContextStringStartIndex, leadingContextStringEndIndex);
                        trailingContextString = fileName.Substring(trailingContextStringStartIndex, trailingContextStringEndIndex);
                        break;

                    case ResultScope.FileHash:
                        break;
                }

                GrepResult grepResult = new GrepResult(fileName, resultScope)
                {
                    Suppressed = suppressFlag,
                    FileSize = fileSize,
                    LeadingContextString = leadingContextString,
                    MatchedString = searchMatch.Value,
                    TrailingContextString = trailingContextString
                };

                matches.Add(grepResult);
            }
        }

        private static List<ConsoleItem> PerformWriteOperations(GrepCommand grepCommand, string filePath, string searchPattern, int fileMatchesCount,
             ref string fileRaw, SearchMetrics searchMetrics)
        {
            var consoleItemCollection = new List<ConsoleItem>();

            bool suppressFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.Suppress);
            bool deleteFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.Delete);
            bool replaceFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.Replace);
            bool fileNamesOnlyFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.FileNamesOnly);

            // FileName
            consoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.DarkYellow, Value = $"{filePath} " });

            try
            {
                if (deleteFlag)
                {
                    // Delete file
                    File.Delete(filePath);

                    if (!suppressFlag)
                        consoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = $"Deleted" });

                    lock (_metricsLock)
                        searchMetrics.DeleteSuccessCount++;
                }
                else if (replaceFlag)
                {
                    if (fileNamesOnlyFlag)
                    {
                        // Rename file
                        string directory = Path.GetDirectoryName(filePath);
                        string fileName = Path.GetFileNameWithoutExtension(filePath);

                        File.Move(filePath, Path.Combine(directory, Regex.Replace(fileName, searchPattern, grepCommand.CommandArgs[ConsoleFlag.Replace])));

                        if (!suppressFlag)
                            consoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = $"Renamed" });
                    }
                    else
                    {
                        // Replace all occurrences within the file
                        fileRaw = Regex.Replace(fileRaw, searchPattern, grepCommand.CommandArgs[ConsoleFlag.Replace]);
                        File.WriteAllText(filePath, fileRaw);

                        string matchesText = fileMatchesCount == 1 ? "match" : "matches";
                        consoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.DarkMagenta, Value = $"{fileMatchesCount} {matchesText}" });
                    }

                    lock (_metricsLock)
                        searchMetrics.ReplacedSuccessCount += fileMatchesCount;
                }
            }
            catch
            {
                consoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.Gray, BackgroundColor = ConsoleColor.DarkRed, Value = $"Access Denied" });

                lock (_metricsLock)
                    searchMetrics.FileWriteFailedCount++;
            }
            finally
            {
                // Empty buffer
                consoleItemCollection.Add(new ConsoleItem() { Value = Environment.NewLine });

                ConsoleUtils.WriteConsoleItemCollection(consoleItemCollection);

                lock (_metricsLock)
                    searchMetrics.TotalFilesMatchedCount++;
            }

            return consoleItemCollection;
        }

        private static void PublishCommandSummary(GrepCommand grepCommand, IList<GrepResult> grepResultCollection, SearchMetrics searchMetrics)
        {
            bool deleteFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.Delete);
            bool replaceFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.Replace);
            bool fileSizeMinimumFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.FileSizeMinimum);
            bool fileSizeMaximumFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.FileSizeMaximum);

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

        /// <summary>
        /// Check that a given filesize is within any filesize parameters
        /// </summary>
        /// <returns></returns>
        private static bool ValidateFileSize(GrepCommand grepCommand, long fileSize, long fileSizeMinimum, long fileSizeMaximum)
        {
            bool isValid = true;

            bool fileSizeMinimumFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.FileSizeMinimum);
            bool fileSizeMaximumFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.FileSizeMaximum);

            isValid &= (!fileSizeMinimumFlag || (fileSizeMinimumFlag && fileSize >= fileSizeMinimum));
            isValid &= (!fileSizeMaximumFlag || (fileSizeMaximumFlag && fileSize <= fileSizeMaximum));

            return isValid;
        }
        #endregion Methods..
    }
}
