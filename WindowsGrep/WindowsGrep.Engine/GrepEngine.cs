using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using WindowsGrep.Common;

namespace WindowsGrep.Engine
{
    public static class GrepEngine
    {
        #region Member Variables..
        private static object _searchLock = new object();
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
        private static void BuildSearchResultsFileContent(ConsoleCommand consoleCommand, GrepResultCollection grepResultCollection, List<Match> matches, string filename, string fileRaw, string searchPattern)
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

            int FileReadFailedCount = 0;
            int FileWriteFailedCount = 0;

            int DeleteSuccessCount = 0;
            int ReplacedSuccessCount = 0;

            int FilesMatchedCount = 0;

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
                                int TrailingContextStringStartIndex = FileNameMatch.Index + SearchMatch.Index + SearchMatch.Length;

                                GrepResult GrepResult = new GrepResult(file, ResultScope.FileName)
                                {
                                    LeadingContextString = file.Substring(0, FileNameMatch.Index + SearchMatch.Index),
                                    MatchedString = SearchMatch.Value,
                                    TrailingContextString = file.Substring(TrailingContextStringStartIndex, file.Length - TrailingContextStringStartIndex)
                                };

                                Matches.Add(GrepResult);
                            }
                        }
                    }
                });

                FilesMatchedCount = Matches.Count();
                grepResultCollection.AddItemRange(Matches);
            }
            else
            {
                files.AsParallel().ForAll(file =>
                {
                    try
                    {
                        List<Match> Matches = new List<Match>();

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
                                List<ConsoleItem> ConsoleItemCollection = new List<ConsoleItem>();

                                // FileName
                                ConsoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.DarkYellow, Value = $"{file} " });

                                try
                                {
                                    if (DeleteFlag)
                                    {
                                        // Delete file
                                        File.Delete(file);

                                        ConsoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = $"Deleted" });

                                        lock (_searchLock)
                                        {
                                            DeleteSuccessCount++;
                                        }
                                    }
                                    else if (ReplaceFlag)
                                    {
                                        // Replace all occurrences in file
                                        FileRaw = Regex.Replace(FileRaw, SearchPattern, consoleCommand.CommandArgs[ConsoleFlag.Replace]);
                                        File.WriteAllText(file, FileRaw);

                                        string MatchesText = Matches.Count == 1 ? "match" : "matches";
                                        ConsoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.DarkMagenta, Value = $"{Matches.Count} {MatchesText}" });

                                        lock (_searchLock)
                                        {
                                            ReplacedSuccessCount += Matches.Count;
                                        }
                                    }
                                }
                                catch
                                {
                                    ConsoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.Gray, BackgroundColor = ConsoleColor.DarkRed, Value = $"Access Denied" });

                                    lock (_searchLock)
                                    {
                                        FileWriteFailedCount++;
                                    }
                                }
                                finally
                                {
                                    // Empty buffer
                                    ConsoleItemCollection.Add(new ConsoleItem() { Value = Environment.NewLine });

                                    ConsoleUtils.WriteConsoleItemCollection(ConsoleItemCollection);

                                    lock (_searchLock)
                                    {
                                        FilesMatchedCount++;
                                    }
                                }
                            }

                            // Read operations
                            else
                            {
                                BuildSearchResultsFileContent(consoleCommand, grepResultCollection, Matches, file, FileRaw, SearchPattern);

                                lock (_searchLock)
                                {
                                    FilesMatchedCount++;
                                }
                            }
                        }
                    }
                    catch
                    {
                        lock (_searchLock)
                        {
                            FileReadFailedCount++;
                        }
                    }
                });
            }

            // Notify the user of any files that could not be read from or written to
            PublishFileAccessSummary(FileReadFailedCount, FileWriteFailedCount);

            // Publish command summary to console
            PublishCommandSummary(consoleCommand, grepResultCollection, FilesMatchedCount, DeleteSuccessCount, ReplacedSuccessCount);
        }
        #endregion ProcessCommand

        #region PublishCommandSummary
        private static void PublishCommandSummary(ConsoleCommand consoleCommand, IList<GrepResult> grepResultCollection, int filesMatchedCount, int deleteSuccessCount, int replaceSuccessCount)
        {
            bool DeleteFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Delete);
            bool ReplaceFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Replace);

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
        #endregion Methods..
    }
}
