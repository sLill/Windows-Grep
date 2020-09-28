using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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
        static GrepEngine() { }
        #endregion Constructors..

        #region Methods..
        #region BeginProcessCommand
        private static void BeginProcessCommand(ConsoleCommand consoleCommand, GrepResultCollection grepResultCollection)
        {
            bool WriteFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Write);

            string Filepath = GetPath(consoleCommand);
            List<string> Files = GetFiles(consoleCommand, grepResultCollection, Filepath);
            Files = GetFilteredFiles(consoleCommand, Files);

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
            bool FileNamesOnlyFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileNamesOnly);
            bool ContextFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Context);

            string SearchPattern = string.Empty;
            string SearchTerm = consoleCommand.CommandArgs[ConsoleFlag.SearchTerm];

            if (FileNamesOnlyFlag)
            {
                // Build filename search pattern
                SearchPattern = FixedStringsFlag ? @"\b" + SearchTerm + @"\b" : SearchTerm;
            }
            else
            {
                // Build file content search pattern
                int ContextLength = ContextFlag ? Convert.ToInt32(consoleCommand.CommandArgs[ConsoleFlag.Context]) : 0;
                string ContextPattern = ContextLength <= 0 ? string.Empty : @"(?:(?!" + SearchTerm + @").){0," + ContextLength.ToString() + @"}";

                // Searches utilizing regular expression group index accessors \n need to increment each group index by one to accommodate
                // being embedded within GrepEngine's MatchedString grouping below
                if (!FixedStringsFlag)
                {
                    string GroupIndexorPattern = @"\\(?<Indexor>\d)";
                    Regex.Matches(SearchTerm, GroupIndexorPattern).Cast<Match>().ToList().ForEach(match =>
                    {
                        // Count consecutive escaped characters
                        int consecutiveCharacterCount = 1;
                        for (int i = match.Index - 1; i > 0; i--)
                        {
                            if (SearchTerm[i] == '\\')
                            {
                                consecutiveCharacterCount++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        // An even number of consecutive escaped characters means this is not a real match
                        if (consecutiveCharacterCount % 2 == 1)
                        {
                            // Increment previous group indexor and replace it in the SearchTerm
                            int Indexor = Convert.ToInt32(match.Groups["Indexor"].Value);
                            Indexor++;

                            var StringBuilder = new StringBuilder(SearchTerm);
                            StringBuilder.Remove(match.Index, match.Value.Length);
                            StringBuilder.Insert(match.Index, @"\" + Indexor.ToString());

                            SearchTerm = StringBuilder.ToString();
                        }
                    });
                }

                SearchPattern = FixedStringsFlag
                    ? ContextPattern + @"(?<MatchedString>[\b\B]?" + SearchTerm + @"[\b\B]?)" + ContextPattern
                    : ContextPattern + @"(?<MatchedString>" + SearchTerm + @")" + ContextPattern;
            }

            return SearchPattern;
        }
        #endregion BuildSearchPattern

        #region BuildSearchResultsFileContent
        private static void BuildSearchResultsFileContent(ConsoleCommand consoleCommand, GrepResultCollection grepResultCollection, MatchCollection matches, string filename, string fileRaw)
        {
            bool IgnoreBreaksFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.IgnoreBreaks);
            bool IgnoreCaseFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.IgnoreCase);

            matches.ToList().ForEach(match =>
            {
                string ContextString = match.Captures.FirstOrDefault().Value;
                string MatchedString = match.Groups["MatchedString"].Value;
                int ContextStringIndex = ContextString.IndexOf(MatchedString, IgnoreCaseFlag ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

                GrepResult GrepResult = new GrepResult(filename, ResultScope.FileContent)
                {
                    ContextString = ContextString,
                    ContextStringStartIndex = ContextStringIndex,
                    MatchedString = MatchedString,
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

        #region BuildSearchResultsFileName
        private static void BuildSearchResultsFileName(ConsoleCommand consoleCommand, GrepResultCollection grepResultCollection, List<(string FileName, Match Match)> matchedFiles)
        {
            matchedFiles.ForEach(matchedFile =>
            {
                GrepResult GrepResult = new GrepResult(matchedFile.FileName, ResultScope.FileName)
                {
                    ContextString = matchedFile.FileName,
                    ContextStringStartIndex = matchedFile.Match.Index,
                    MatchedString = matchedFile.Match.Value
                };

                grepResultCollection.AddItem(GrepResult);
            });
        }
        #endregion BuildSearchResultsFileName

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
            bool FilTypeInclusionFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileTypeInclusions);
            bool FilTypeExclusionFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileTypeExclusions);

            string[] FileTypeInclusions = FilTypeInclusionFlag ? consoleCommand.CommandArgs[ConsoleFlag.FileTypeInclusions].Split(new char[] { ',', ';' }) : null;
            string[] FileTypeExclusions = FilTypeExclusionFlag ? consoleCommand.CommandArgs[ConsoleFlag.FileTypeExclusions].Split(new char[] { ',', ';' }) : null;

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
                Regex FileNameRegex = new Regex(@"^(.+)[\/\\]([^\/\\]+)$");

                var Matches = files
                    .Where(x => SearchRegex.IsMatch(Path.GetFileNameWithoutExtension(x)))
                    .Select(x => (x, SearchRegex.Matches(x).Aggregate((x1, x2) => x1.Index > x2.Index ? x1 : x2))).ToList();

                FilesMatchedCount = Matches.Count;
                BuildSearchResultsFileName(consoleCommand, grepResultCollection, Matches);
            }
            else
            {
                files.AsParallel().ForAll(file =>
                {
                    try
                    {
                        MatchCollection Matches = null;

                        string FileRaw = File.ReadAllText(file);

                        // Apply linebreak filtering options
                        if (IgnoreBreaksFlag)
                        {
                            string fileLineBreakFiltered = FileRaw.Replace("\r", string.Empty).Replace("\n", "");
                            Matches = SearchRegex.Matches(fileLineBreakFiltered);
                        }
                        else
                        {
                            Matches = SearchRegex.Matches(FileRaw);
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
                                catch (Exception ex)
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

                                    lock (_searchLock)
                                    {
                                        FilesMatchedCount++;
                                    }
                                }
                            }

                            // Read operations
                            else
                            {
                                BuildSearchResultsFileContent(consoleCommand, grepResultCollection, Matches, file, FileRaw);

                                lock (_searchLock)
                                {
                                    FilesMatchedCount++;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
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
            string[] CommandCollection = commandRaw.Split('|');
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
