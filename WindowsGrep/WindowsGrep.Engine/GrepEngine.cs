using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using WindowsGrep.Common;

namespace WindowsGrep.Engine
{
    public static class GrepEngine
    {
        #region Member Variables..
        private static long _FileSizeMinimum = -1;
        private static long _FileSizeMaximum = -1;

        private static string _FileSizePattern = @"(?<Size>\d+)(?<SizeType>\S{2})?";
        private static Regex _FileSizeRegex = new Regex(_FileSizePattern);

        private static object _SearchLock = new object();
        #endregion Member Variables..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        #endregion Constructors..

        #region Methods..
        #region BeginProcessCommand
        private static void BeginProcessCommand(ConsoleCommand consoleCommand, GrepResultCollection grepResultCollection)
        {
            bool WriteFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Write);

            Stopwatch commandTimer = Stopwatch.StartNew();

            string Filepath = GetPath(consoleCommand);
            List<string> Files = GetFiles(consoleCommand, grepResultCollection, Filepath);
            Files = GetFilteredFiles(consoleCommand, Files);

            // Clear the result collection between chained commands so that only the results of the final command are returned
            grepResultCollection.Clear();

            ConsoleUtils.WriteConsoleItem(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = $"{Environment.NewLine}[Searching {Files.Count} file(s)]{Environment.NewLine}" });

            RegexOptions OptionsFlags = GetRegexOptions(consoleCommand);
            ProcessCommand(grepResultCollection, Files, consoleCommand, OptionsFlags);

            if (WriteFlag)
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
            bool FixedStringsFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FixedStrings);
            bool IgnoreCaseFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.IgnoreCase);

            string SearchPattern = string.Empty;
            string SearchTerm = consoleCommand.CommandArgs[ConsoleFlag.SearchTerm];
            string IgnoreCaseModifier = IgnoreCaseFlag ? @"(?i)" : string.Empty;

            SearchTerm = FixedStringsFlag ? Regex.Escape(SearchTerm) : SearchTerm;
            SearchPattern = @"(?<MatchedString>" + IgnoreCaseModifier + SearchTerm + @")";

            return SearchPattern;
        }
        #endregion BuildSearchPattern

        #region BuildSearchResultsFileContent
        private static void BuildSearchResultsFileContent(ConsoleCommand consoleCommand, GrepResultCollection grepResultCollection, List<Match> matches, string filename, long fileSize, string fileRaw, string searchPattern)
        {
            bool IgnoreBreaksFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.IgnoreBreaks);
            bool IgnoreCaseFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.IgnoreCase);
            bool ContextFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Context);

            // Build file context search pattern
            string SearchTerm = consoleCommand.CommandArgs[ConsoleFlag.SearchTerm];
            int ContextLength = ContextFlag ? Convert.ToInt32(consoleCommand.CommandArgs[ConsoleFlag.Context]) : 0;
            string ContextPattern = @"(?:(?<ContextString>.{0," + ContextLength.ToString() + @"}?))";
            Regex ContextRegex = new Regex(ContextPattern + searchPattern);

            matches.ToList().ForEach(match =>
            {
                // Rebuild matches with contextual text
                if (ContextFlag)
                {
                    int ContextStartIndex = (match.Index - ContextLength) < 0 ? 0 : (match.Index - ContextLength);
                    match = ContextRegex.Match(fileRaw, ContextStartIndex);
                }

                string ContextString = ContextFlag ? match.Groups["ContextString"]?.Value ?? string.Empty : string.Empty;
                string MatchedString = match.Groups["MatchedString"].Value;

                GrepResult GrepResult = new GrepResult(filename, ResultScope.FileContent)
                {
                    FileSize = fileSize,
                    LeadingContextString = ContextString,
                    MatchedString = MatchedString
                };

                // Line number
                if (!IgnoreBreaksFlag)
                {
                    int LineNumber = fileRaw.Substring(0, match.Groups["MatchedString"].Index).Split('\n').Length;
                    GrepResult.LineNumber = LineNumber;
                }

                grepResultCollection.AddItem(GrepResult);
            });
        }
        #endregion BuildSearchResultsFileContent

        #region GetFiles
        private static List<string> GetFiles(ConsoleCommand consoleCommand, IList<GrepResult> grepResultCollection, string filepath)
        {
            bool RecursiveFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Recursive);
            bool TargetFileFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.TargetFile);

            List<string> Files = null;

            if (grepResultCollection.Any())
            {
                if (TargetFileFlag)
                {
                    Files = grepResultCollection.Where(x => x.SourceFile == filepath).Select(result => result.SourceFile).ToList();
                }
                else
                {
                    Files = grepResultCollection.Select(result => result.SourceFile).ToList();
                }
            }
            else
            {
                if (TargetFileFlag)
                {
                    Files = new List<string>() { filepath };
                }
                else
                {
                    var EnumerationOptions = new EnumerationOptions() { ReturnSpecialDirectories = true, AttributesToSkip = FileAttributes.System };
                    if (RecursiveFlag)
                    {
                        EnumerationOptions.RecurseSubdirectories = true;
                    }

                    Files = Directory.GetFiles(Path.TrimEndingDirectorySeparator(filepath.TrimEnd()), "*", EnumerationOptions).ToList();
                }
            }

            return Files;
        }
        #endregion GetFiles

        #region GetFilteredFiles
        /// <summary>
        /// </summary>
        /// <param name="consoleCommand"></param>
        /// <param name="files"></param>
        /// <returns>Returns files filtered by Inclusion/Exclusion type parameters</returns>
        private static List<string> GetFilteredFiles(ConsoleCommand consoleCommand, List<string> files)
        {
            bool FileTypeInclusionFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileTypeInclusions);
            bool FileTypeExclusionFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileTypeExclusions);

            var FileTypeInclusions = FileTypeInclusionFlag ? consoleCommand.CommandArgs[ConsoleFlag.FileTypeInclusions].Split(new char[] { ',', ';' }).Select(x => x.Trim('.')) : null;
            var FileTypeExclusions = FileTypeExclusionFlag ? consoleCommand.CommandArgs[ConsoleFlag.FileTypeExclusions].Split(new char[] { ',', ';' }).Select(x => x.Trim('.')) : null;

            files = FileTypeInclusions == null ? files : files.Where(file => FileTypeInclusions.Contains(Path.GetExtension(file).Trim('.'))).ToList();
            files = FileTypeExclusions == null ? files : files.Where(file => !FileTypeExclusions.Contains(Path.GetExtension(file).Trim('.'))).ToList();

            return files;
        }
        #endregion GetFilteredFiles

        #region GetPath
        private static string GetPath(ConsoleCommand consoleCommand)
        {
            bool DirectoryFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Directory);
            bool TargetFileFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.TargetFile);

            // User specified file should overrule specified directory
            string Filepath = DirectoryFlag ? consoleCommand.CommandArgs[ConsoleFlag.Directory] : Environment.CurrentDirectory;
            Filepath = TargetFileFlag ? consoleCommand.CommandArgs[ConsoleFlag.TargetFile] : Filepath;

            return Filepath;
        }
        #endregion GetPath

        #region GetRegexOptions
        private static RegexOptions GetRegexOptions(ConsoleCommand consoleCommand)
        {
            RegexOptions OptionsFlags = 0;
            bool IgnoreCaseFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.IgnoreCase);

            if (IgnoreCaseFlag)
            {
                OptionsFlags |= RegexOptions.IgnoreCase;
            }

            return OptionsFlags;
        }
        #endregion GetRegexOptions

        #region ProcessCommand
        private static void ProcessCommand(GrepResultCollection grepResultCollection, IEnumerable<string> files, ConsoleCommand consoleCommand, RegexOptions optionsFlags)
        {
            bool FixedStringsFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FixedStrings);
            bool DeleteFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Delete);
            bool ReplaceFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Replace);
            bool WriteFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Write);
            bool FileNamesOnlyFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileNamesOnly);
            bool IgnoreBreaksFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.IgnoreBreaks);
            bool FileSizeFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileSize);
            bool FileSizeMinimumFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileSizeMinimum);
            bool FileSizeMaximumFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileSizeMaximum);

            int FileReadFailedCount = 0;
            int FileWriteFailedCount = 0;

            int DeleteSuccessCount = 0;
            int ReplacedSuccessCount = 0;

            int TotalFilesMatchedCount = 0;

            // Build content search pattern
            string SearchPattern = BuildSearchPattern(consoleCommand);
            Regex SearchRegex = new Regex(SearchPattern, optionsFlags);

            if (FileNamesOnlyFlag)
            {
                Regex FileNameRegex = new Regex(@"^(.+)[\/\\](?<FileName>[^\/\\]+)$");
                var Matches = new List<GrepResult>();

                files.ToList().ForEach(file =>
                {
                    // Don't want to waste any time on files that have already been added 
                    bool FileAdded = Matches.Any(x => x.SourceFile == file);
                    if (!FileAdded)
                    {
                        // Parse filename from path
                        var FileNameMatch = FileNameRegex.Match(file)?.Groups["FileName"];
                        if (FileNameMatch != null)
                        {
                            // Query against filename
                            var SearchMatch = SearchRegex.Match(FileNameMatch.Value);
                            if (SearchMatch != Match.Empty)
                            {
                                // Write operations
                                bool isWriteOperation = ReplaceFlag || DeleteFlag;
                                if (isWriteOperation)
                                {
                                    ProcessWriteOperations(consoleCommand, file, SearchPattern, Matches.Count, ref file, ref TotalFilesMatchedCount, ref DeleteSuccessCount, ref ReplacedSuccessCount, ref FileWriteFailedCount);
                                }
                                else
                                {
                                    int TrailingContextStringStartIndex = FileNameMatch.Index + SearchMatch.Index + SearchMatch.Length;

                                    // Validate any filesize parameters
                                    var FileSize = FileSizeFlag || FileSizeMaximumFlag || FileSizeMinimumFlag ? WindowsUtils.GetFileSizeOnDisk(file) : -1;
                                    bool FileSizevalidateSuccess = ValidateFileSize(consoleCommand, FileSize);

                                    if (FileSizevalidateSuccess)
                                    {
                                        GrepResult GrepResult = new GrepResult(file, ResultScope.FileName)
                                        {
                                            FileSize = FileSize,
                                            LeadingContextString = file.Substring(0, FileNameMatch.Index + SearchMatch.Index),
                                            MatchedString = SearchMatch.Value,
                                            TrailingContextString = file.Substring(TrailingContextStringStartIndex, file.Length - TrailingContextStringStartIndex)
                                        };

                                        Matches.Add(GrepResult);
                                    }
                                }
                            }
                        }
                    }
                });

                TotalFilesMatchedCount = Matches.Count();
                grepResultCollection.AddItemRange(Matches);
            }
            else
            {
                files.AsParallel().ForAll(file =>
                {
                    try
                    {
                        List<Match> Matches = new List<Match>();

                        // Validate any filesize parameters
                        var FileSize = FileSizeFlag || FileSizeMaximumFlag || FileSizeMinimumFlag ? WindowsUtils.GetFileSizeOnDisk(file) : -1;
                        bool FileSizevalidateSuccess = ValidateFileSize(consoleCommand, FileSize);

                        if (FileSizevalidateSuccess)
                        {
                            string FileRaw = File.ReadAllText(file);

                            // Apply linebreak filtering options
                            if (IgnoreBreaksFlag)
                            {
                                string FileLineBreakFilteredNull = FileRaw.Replace("\r", string.Empty).Replace("\n", string.Empty);
                                string FileLineBreakFilteredSpace = Regex.Replace(FileRaw, @"[\r\n]+", " ");

                                Matches.AddRange(SearchRegex.Matches(FileLineBreakFilteredNull));
                                Matches.AddRange(SearchRegex.Matches(FileLineBreakFilteredSpace));
                            }
                            else
                            {
                                Matches = SearchRegex.Matches(FileRaw).ToList();
                            }

                            if (Matches.Any())
                            {
                                // Write operations
                                bool isWriteOperation = ReplaceFlag || DeleteFlag;
                                if (isWriteOperation)
                                {
                                    ProcessWriteOperations(consoleCommand, file, SearchPattern, Matches.Count, ref FileRaw, ref TotalFilesMatchedCount, ref DeleteSuccessCount, ref ReplacedSuccessCount, ref FileWriteFailedCount);
                                }
                                else
                                {
                                    // Read operations
                                    BuildSearchResultsFileContent(consoleCommand, grepResultCollection, Matches, file, FileSize, FileRaw, SearchPattern);

                                    lock (_SearchLock)
                                    {
                                        TotalFilesMatchedCount++;
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        lock (_SearchLock)
                        {
                            FileReadFailedCount++;
                        }
                    }
                });
            }

            // Notify the user of any files that could not be read from or written to
            PublishFileAccessSummary(FileReadFailedCount, FileWriteFailedCount);

            // Publish command summary to console
            PublishCommandSummary(consoleCommand, grepResultCollection, TotalFilesMatchedCount, DeleteSuccessCount, ReplacedSuccessCount);
        }
        #endregion ProcessCommand


        #region ProcessWriteOperations
        private static List<ConsoleItem> ProcessWriteOperations(ConsoleCommand consoleCommand, string fileName, string searchPattern, int fileMatchesCount,
             ref string FileRaw, ref int TotalFilesMatchedCount, ref int DeleteSuccessCount, ref int ReplacedSuccessCount, ref int FileWriteFailedCount)
        {
            var ConsoleItemCollection = new List<ConsoleItem>();
            bool DeleteFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Delete);
            bool ReplaceFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Replace);

            // FileName
            ConsoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.DarkYellow, Value = $"{fileName} " });

            try
            {
                if (DeleteFlag)
                {
                    // Delete file
                    File.Delete(fileName);

                    ConsoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = $"Deleted" });

                    lock (_SearchLock)
                    {
                        DeleteSuccessCount++;
                    }
                }
                else if (ReplaceFlag)
                {
                    // Replace all occurrences in file
                    FileRaw = Regex.Replace(FileRaw, searchPattern, consoleCommand.CommandArgs[ConsoleFlag.Replace]);
                    File.WriteAllText(fileName, FileRaw);

                    string MatchesText = fileMatchesCount == 1 ? "match" : "matches";
                    ConsoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.DarkMagenta, Value = $"{fileMatchesCount} {MatchesText}" });

                    lock (_SearchLock)
                    {
                        ReplacedSuccessCount += fileMatchesCount;
                    }
                }
            }
            catch
            {
                ConsoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.Gray, BackgroundColor = ConsoleColor.DarkRed, Value = $"Access Denied" });

                lock (_SearchLock)
                {
                    FileWriteFailedCount++;
                }
            }
            finally
            {
                // Empty buffer
                ConsoleItemCollection.Add(new ConsoleItem() { Value = Environment.NewLine });

                ConsoleUtils.WriteConsoleItemCollection(ConsoleItemCollection);

                lock (_SearchLock)
                {
                    TotalFilesMatchedCount++;
                }
            }

            return ConsoleItemCollection;
        }
        #endregion ProcessWriteOperations

        #region PublishCommandSummary
        private static void PublishCommandSummary(ConsoleCommand consoleCommand, IList<GrepResult> grepResultCollection, int filesMatchedCount, int deleteSuccessCount, int replaceSuccessCount)
        {
            bool DeleteFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Delete);
            bool ReplaceFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Replace);
            bool FileSizeFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileSize);
            bool FileSizeMinimumFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileSizeMinimum);
            bool FileSizeMaximumFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileSizeMaximum);

            string Summary = string.Empty;
            if (DeleteFlag)
            {
                Summary = $"[{deleteSuccessCount} of {filesMatchedCount} file(s) deleted]";
            }
            else if (ReplaceFlag)
            {
                Summary = $"[{replaceSuccessCount} occurrence(s) replaced in {filesMatchedCount} file(s)]";
            }
            else
            {
                Summary = $"[{grepResultCollection.Count} result(s) {filesMatchedCount} file(s)]";
            }

            ConsoleUtils.WriteConsoleItem(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = Summary });

            if (FileSizeFlag || FileSizeMinimumFlag || FileSizeMaximumFlag)
            {
                var TotalFileSize = grepResultCollection.Sum(x => x.FileSize);
                var FileSizeReduced = WindowsUtils.GetReducedSize(TotalFileSize, 3, out FileSizeType fileSizeType);

                Summary = $"[{FileSizeReduced} {fileSizeType}(s)]";

                ConsoleUtils.WriteConsoleItem(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = Summary });
            }
        }
        #endregion PublishCommandSummary

        #region PublishFileAccessSummary
        private static void PublishFileAccessSummary(int fileReadFailedCount, int fileWriteFailedCount)
        {
            if (fileReadFailedCount > 0 || fileWriteFailedCount > 0)
            {
                if (fileReadFailedCount > 0)
                {
                    string UnreachableFiles = $"[{fileReadFailedCount} file(s) unreadable/inaccessible]";
                    ConsoleUtils.WriteConsoleItem(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = UnreachableFiles });
                }

                if (fileWriteFailedCount > 0)
                {
                    string UnwriteableFiles = $"[{fileWriteFailedCount} file(s) could not be modified]";
                    ConsoleUtils.WriteConsoleItem(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = UnwriteableFiles });
                }

                ConsoleUtils.WriteConsoleItem(new ConsoleItem() { Value = Environment.NewLine });
            }
        }
        #endregion PublishFileAccessSummary

        #region RunCommand
        public static void RunCommand(string commandRaw, GrepResultCollection grepResultCollection)
        {
            string SplitPattern = @"\|(?![^{]*}|[^\(]*\)|[^\[]*\])";
            string[] CommandCollection = Regex.Split(commandRaw, SplitPattern);

            foreach (string command in CommandCollection)
            {
                var CommandArgs = ConsoleUtils.DiscoverCommandArgs(command);
                ConsoleCommand ConsoleCommand = new ConsoleCommand() { CommandArgs = CommandArgs };

                BeginProcessCommand(ConsoleCommand, grepResultCollection);
            }
        }
        #endregion RunCommand

        #region ValidateFileSize
        /// <summary>
        /// Check that a given filesize is within any filesize parameters
        /// </summary>
        /// <returns></returns>
        private static bool ValidateFileSize(ConsoleCommand consoleCommand, long fileSize)
        {
            bool IsValid = true;
            bool FileSizeMinimumFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileSizeMinimum);

            if (FileSizeMinimumFlag)
            {
                if (_FileSizeMinimum >= 0)
                {
                    IsValid &= (fileSize >= _FileSizeMinimum);
                }
                else
                {
                    try
                    {
                        string FileSizeMinimumParameter = consoleCommand.CommandArgs[ConsoleFlag.FileSizeMinimum];

                        var Match = _FileSizeRegex.Match(FileSizeMinimumParameter);
                        long Size = Convert.ToInt64(Match.Groups["Size"].Value);

                        if (Size < 0)
                        {
                            throw new Exception("Error: Size parameter cannot be less than 0");
                        }

                        long FileSizeModifier = FileSizeType.KB.GetCustomAttribute<ValueAttribute>().Value;

                        string SizeType = Match.Groups["SizeType"].Value.ToUpper();
                        if (!SizeType.IsNullOrEmpty())
                        {
                            FileSizeModifier = Enum.Parse<FileSizeType>(SizeType).GetCustomAttribute<ValueAttribute>().Value;
                        }

                        _FileSizeMinimum = Size * FileSizeModifier;
                        IsValid &= (fileSize >= _FileSizeMinimum);
                    }
                    catch
                    {
                        throw new Exception($"Error: could not parse filesize parameter" +
                            $"{Environment.NewLine}Format should follow [SIZE] or [SIZE][TYPE]. Acceptable TYPE parameters are kb, mb, gb, tb" +
                            $"{Environment.NewLine}For more information, visit https://github.com/sLill/Windows-BudgetGrep/wiki/WindowsGrep.CommandFlags");
                    }
                }
            }

            bool FileSizeMaximumFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileSizeMaximum);
            if (FileSizeMaximumFlag)
            {
                if (_FileSizeMaximum >= 0)
                {
                    IsValid &= (fileSize <= _FileSizeMaximum);
                }
                else
                {
                    try
                    {
                        string FileSizeMaximumParameter = consoleCommand.CommandArgs[ConsoleFlag.FileSizeMaximum];

                        var Match = _FileSizeRegex.Match(FileSizeMaximumParameter);
                        long Size = Convert.ToInt64(Match.Groups["Size"].Value);

                        if (Size < 0)
                        {
                            throw new Exception("Error: Size parameter cannot be less than 0");
                        }

                        long FileSizeModifier = FileSizeType.KB.GetCustomAttribute<ValueAttribute>().Value;

                        string SizeType = Match.Groups["SizeType"].Value.ToUpper();
                        if (!SizeType.IsNullOrEmpty())
                        {
                            FileSizeModifier = Enum.Parse<FileSizeType>(SizeType).GetCustomAttribute<ValueAttribute>().Value;
                        }

                        _FileSizeMaximum = Size * FileSizeModifier;
                        IsValid &= (fileSize <= _FileSizeMaximum);
                    }
                    catch
                    {
                        throw new Exception($"Error: could not parse filesize parameter" +
                            $"{Environment.NewLine}Format should follow [SIZE] or [SIZE][TYPE]. Acceptable TYPE parameters are kb, mb, gb, tb" +
                            $"{Environment.NewLine}For more information, visit https://github.com/sLill/Windows-BudgetGrep/wiki/WindowsGrep.CommandFlags");
                    }
                }
            }

            return IsValid;
        }
        #endregion ValidateFileSize
        #endregion Methods..
    }
}
