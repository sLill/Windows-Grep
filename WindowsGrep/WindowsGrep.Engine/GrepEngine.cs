﻿using System;
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
            Stopwatch CommandTimer = Stopwatch.StartNew();

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
            CommandTimer.Stop();

            ConsoleUtils.WriteConsoleItem(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = $"{Environment.NewLine}[{Math.Round((CommandTimer.ElapsedMilliseconds / 1000.0), 2)} second(s)]" });
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

            // Ignore carriage-return and newline characters when using endline regex to match expected behavior from other regex engines
            SearchTerm = SearchTerm.Replace("$", "[\r\n]*$");

            SearchTerm = FixedStringsFlag ? Regex.Escape(SearchTerm) : SearchTerm;
            SearchPattern = @"(?<MatchedString>" + IgnoreCaseModifier + SearchTerm + @")";

            return SearchPattern;
        }
        #endregion BuildSearchPattern

        #region BuildSearchResultsFileContent
        private static void BuildSearchResultsFileContent(ConsoleCommand consoleCommand, GrepResultCollection grepResultCollection, List<Match> matches, string filename, long fileSize, string fileRaw)
        {
            bool IgnoreBreaksFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.IgnoreBreaks);
            bool IgnoreCaseFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.IgnoreCase);
            bool ContextFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Context);
            bool NResultsFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.NResults);

            // Build file context search pattern
            string SearchTerm = consoleCommand.CommandArgs[ConsoleFlag.SearchTerm];
            int ContextLength = ContextFlag ? Convert.ToInt32(consoleCommand.CommandArgs[ConsoleFlag.Context]) : 0;

            matches.ToList().ForEach(match =>
            {
                string LeadingContext = string.Empty;
                string TrailingContext = string.Empty;

                if (ContextFlag)
                {
                    // Rebuild matches with contextual text
                    int LeadingContextStartIndex = match.Groups["MatchedString"].Index - ContextLength < 0 ? 0 : match.Groups["MatchedString"].Index - ContextLength;
                    int LeadingContextLength = match.Groups["MatchedString"].Index < ContextLength ? match.Groups["MatchedString"].Index : ContextLength;

                    int TrailingContextStartIndex = match.Groups["MatchedString"].Index + match.Groups["MatchedString"].Value.Length;
                    int TrailingContextLength = TrailingContextStartIndex + ContextLength > fileRaw.Length ? fileRaw.Length - TrailingContextStartIndex : ContextLength;

                    LeadingContext = Environment.NewLine + fileRaw.Substring(LeadingContextStartIndex, LeadingContextLength);
                    TrailingContext = fileRaw.Substring(TrailingContextStartIndex, TrailingContextLength) + Environment.NewLine;
                }

                string MatchedString = match.Groups["MatchedString"].Value;

                GrepResult GrepResult = new GrepResult(filename, ResultScope.FileContent)
                {
                    FileSize = fileSize,
                    LeadingContextString = LeadingContext,
                    TrailingContextString = TrailingContext,
                    MatchedString = MatchedString
                };

                // Line number
                int LineNumber = fileRaw.Substring(0, match.Groups["MatchedString"].Index).Split('\n').Length;
                GrepResult.LineNumber = LineNumber;

                lock (grepResultCollection)
                {
                    if (!NResultsFlag || grepResultCollection.Count < Convert.ToInt32(consoleCommand.CommandArgs[ConsoleFlag.NResults]))
                    {
                        grepResultCollection.AddItem(GrepResult);
                    }
                }
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

        #region GetFileSizeMaximum
        private static long GetFileSizeMaximum(ConsoleCommand consoleCommand)
        {
            long FileSizeMaximum = -1;
            bool FileSizeMaximumFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileSizeMaximum);

            if (FileSizeMaximumFlag)
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

                    long FileSizeModifier = FileSizeType.Kb.GetCustomAttribute<ValueAttribute>().Value;

                    string SizeType = Match.Groups["SizeType"].Value.ToUpper();
                    if (!SizeType.IsNullOrEmpty())
                    {
                        FileSizeModifier = Enum.Parse<FileSizeType>(SizeType, true).GetCustomAttribute<ValueAttribute>().Value;
                    }

                    FileSizeMaximum = Size * FileSizeModifier;
                }
                catch
                {
                    throw new Exception($"Error: Could not parse filesize parameter" +
                        $"{Environment.NewLine}Format should follow [SIZE] or [SIZE][TYPE]. Acceptable TYPE parameters are kb, mb, gb, tb" +
                        $"{Environment.NewLine}For more information, visit https://github.com/sLill/Windows-BudgetGrep/wiki/WindowsGrep.CommandFlags");
                }
            }

            return FileSizeMaximum;
        }
        #endregion GetFileSizeMaximum

        #region GetFileSizeMinimum
        private static long GetFileSizeMinimum(ConsoleCommand consoleCommand)
        {
            long FileSizeMinimum = -1;
            bool FileSizeMinimumFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileSizeMinimum);

            if (FileSizeMinimumFlag)
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

                    long FileSizeModifier = FileSizeType.Kb.GetCustomAttribute<ValueAttribute>().Value;

                    string SizeType = Match.Groups["SizeType"].Value.ToUpper();
                    if (!SizeType.IsNullOrEmpty())
                    {
                        FileSizeModifier = Enum.Parse<FileSizeType>(SizeType).GetCustomAttribute<ValueAttribute>().Value;
                    }

                    FileSizeMinimum = Size * FileSizeModifier;
                }
                catch
                {
                    throw new Exception($"Error: could not parse filesize parameter" +
                        $"{Environment.NewLine}Format should follow [SIZE] or [SIZE][TYPE]. Acceptable TYPE parameters are kb, mb, gb, tb" +
                        $"{Environment.NewLine}For more information, visit https://github.com/sLill/Windows-BudgetGrep/wiki/WindowsGrep.CommandFlags");
                }
            }

            return FileSizeMinimum;
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
            bool FileTypeFilterFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileTypeFilter);
            bool FileTypeExcludeFilterFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileTypeExcludeFilter);

            var FileTypeFilters = FileTypeFilterFlag ? consoleCommand.CommandArgs[ConsoleFlag.FileTypeFilter].Split(new char[] { ',', ';' }).Select(x => x.Trim('.')) : null;
            var FileTypeExcludeFilters = FileTypeExcludeFilterFlag ? consoleCommand.CommandArgs[ConsoleFlag.FileTypeExcludeFilter].Split(new char[] { ',', ';' }).Select(x => x.Trim('.')) : null;

            bool PathFilterFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.PathFilter);
            bool PathExcludeFilterFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.PathExcludeFilter);

            var PathFilters = PathFilterFlag ? consoleCommand.CommandArgs[ConsoleFlag.PathFilter].Split(new char[] { ',', ';' }) : null;
            var PathExcludeFilters = PathExcludeFilterFlag ? consoleCommand.CommandArgs[ConsoleFlag.PathExcludeFilter].Split(new char[] { ',', ';' }) : null;

            files = FileTypeFilters == null ? files : files.Where(file => FileTypeFilters.Contains(Path.GetExtension(file).Trim('.'))).ToList();
            files = FileTypeExcludeFilters == null ? files : files.Where(file => !FileTypeExcludeFilters.Contains(Path.GetExtension(file).Trim('.'))).ToList();

            files = PathFilters == null ? files : files.Where(file => PathFilters.Any(x => Regex.IsMatch(file, x))).ToList();
            files = PathExcludeFilters == null ? files : files.Where(file => !PathExcludeFilters.Any(x => Regex.IsMatch(file, x))).ToList();

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
            bool IgnoreBreaksFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.IgnoreBreaks);

            if (IgnoreCaseFlag)
            {
                OptionsFlags |= RegexOptions.IgnoreCase;
            }

            if (IgnoreBreaksFlag)
            {
                OptionsFlags |= RegexOptions.Singleline;
            }
            else
            {
               OptionsFlags |= RegexOptions.Multiline;
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
            bool FileSizeMinimumFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileSizeMinimum);
            bool FileSizeMaximumFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileSizeMaximum);
            bool NResultsFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.NResults);

            int FileReadFailedCount = 0;
            int FileWriteFailedCount = 0;
            int DeleteSuccessCount = 0;
            int ReplacedSuccessCount = 0;
            int TotalFilesMatchedCount = 0;

            long FileSizeMin = GetFileSizeMinimum(consoleCommand);
            long FileSizeMax = GetFileSizeMaximum(consoleCommand);

            // Build content search pattern
            string SearchPattern = BuildSearchPattern(consoleCommand);
            Regex SearchRegex = new Regex(SearchPattern, optionsFlags);

            if (FileNamesOnlyFlag)
            {
                Regex FileNameRegex = new Regex(@"^(.+)[\/\\](?<FileName>[^\/\\]+)$");
                var Matches = new List<GrepResult>();

                files.ToList().ForEach(file =>
                {
                    if (!NResultsFlag || Matches.Count < Convert.ToInt32(consoleCommand.CommandArgs[ConsoleFlag.NResults]))
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
                                        var FileSize = FileSizeMaximumFlag || FileSizeMinimumFlag ? WindowsUtils.GetFileSizeOnDisk(file) : -1;
                                        bool FileSizevalidateSuccess = ValidateFileSize(consoleCommand, FileSize, FileSizeMin, FileSizeMax);

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
                    }
                });

                TotalFilesMatchedCount = Matches.Count();
                grepResultCollection.AddItemRange(Matches);
            }
            else
            {
                if (consoleCommand.CommandArgs[ConsoleFlag.SearchTerm] == string.Empty)
                {
                    throw new Exception("Error: Search term not supplied");
                }

                files.AsParallel().ForAll(file =>
                {
                    try
                    {
                        if (!NResultsFlag || grepResultCollection.Count < Convert.ToInt32(consoleCommand.CommandArgs[ConsoleFlag.NResults]))
                        {
                            List<Match> Matches = new List<Match>();

                            // Validate any filesize parameters
                            var FileSize = FileSizeMaximumFlag || FileSizeMinimumFlag ? WindowsUtils.GetFileSizeOnDisk(file) : -1;
                            bool FileSizevalidateSuccess = ValidateFileSize(consoleCommand, FileSize, FileSizeMin, FileSizeMax);

                            if (FileSizevalidateSuccess)
                            {
                                string FileRaw = File.ReadAllText(file);

                                Matches = SearchRegex.Matches(FileRaw).ToList();
                                if (Matches.Any())
                                {
                                    // Write operations
                                    bool isWriteOperation = ReplaceFlag || DeleteFlag;
                                    if (isWriteOperation)
                                    {
                                        ProcessWriteOperations(consoleCommand, file, SearchPattern, Matches.Count, ref FileRaw, ref TotalFilesMatchedCount, ref DeleteSuccessCount, ref ReplacedSuccessCount, ref FileWriteFailedCount);
                                    }

                                    // Read operations
                                    else
                                    {
                                        BuildSearchResultsFileContent(consoleCommand, grepResultCollection, Matches, file, FileSize, FileRaw);

                                        lock (_SearchLock)
                                        {
                                            TotalFilesMatchedCount++;
                                        }
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
                    // Replace all occurrences within the file
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

            if (FileSizeMinimumFlag || FileSizeMaximumFlag)
            {
                var TotalFileSize = grepResultCollection.Sum(x => x.FileSize);
                var FileSizeReduced = WindowsUtils.GetReducedSize(TotalFileSize, 3, out FileSizeType fileSizeType);

                Summary = $" [{FileSizeReduced} {fileSizeType}(s)]";

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
        private static bool ValidateFileSize(ConsoleCommand consoleCommand, long fileSize, long fileSizeMinimum, long fileSizeMaximum)
        {
            bool IsValid = true;

            bool FileSizeMinimumFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileSizeMinimum);
            bool FileSizeMaximumFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileSizeMaximum);

            IsValid &= (!FileSizeMinimumFlag || (FileSizeMinimumFlag && fileSize >= fileSizeMinimum));
            IsValid &= (!FileSizeMaximumFlag || (FileSizeMaximumFlag && fileSize <= fileSizeMaximum));

            return IsValid;
        }
        #endregion ValidateFileSize
        #endregion Methods..
    }
}
