using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
        static GrepEngine()
        {
            MatchFound += ConsoleUtils.WriteConsoleItemCollection;
        }
        #endregion Constructors..

        #region Event Handlers..
        public static event EventHandler MatchFound;
        #endregion Event Handlers..

        #region Methods..
        #region BeginProcessCommand
        private static void BeginProcessCommand(ConsoleCommand consoleCommand, ThreadSafeCollection<GrepResult> grepResultCollection)
        {
            string Filepath = GetPath(consoleCommand);
            List<string> Files = GetFiles(consoleCommand, grepResultCollection, Filepath);
            Files = GetFilteredFiles(consoleCommand, Files);

            ConsoleUtils.WriteConsoleItem(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = $"{Environment.NewLine}[Searching {Files.Count} file(s)]{Environment.NewLine}" });

            RegexOptions OptionsFlags = GetRegexOptions(consoleCommand);
            ProcessCommand(grepResultCollection, Files, consoleCommand, OptionsFlags);
            
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
                // Build for filename search
                SearchPattern = FixedStringsFlag ? @"\b" + SearchTerm + @"\b" : SearchTerm;
            }
            else
            { 
                // Build for content search
                int ContextLength = ContextFlag ? Convert.ToInt32(consoleCommand.CommandArgs[ConsoleFlag.Context]) : 0;
                string ContextPattern = ContextLength <= 0 ? string.Empty : @"(?:(?!" + SearchTerm + @").){0," + ContextLength.ToString() + @"}";

                SearchPattern = FixedStringsFlag
                    ? ContextPattern + @"(?<MatchedString>[\b\B]?" + SearchTerm + @"[\b\B]?)" + ContextPattern
                    : ContextPattern + @"(?<MatchedString>" + SearchTerm + @")" + ContextPattern;
            }

            return SearchPattern;
        }
        #endregion BuildSearchPattern

        #region BuildSearchResultsFileContent
        private static void BuildSearchResultsFileContent(ConsoleCommand consoleCommand, ThreadSafeCollection<GrepResult> grepResultCollection, MatchCollection matches, string filename, string fileRaw)
        {
            bool IgnoreBreaksFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.IgnoreBreaks);
            bool IgnoreCaseFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.IgnoreCase);

            matches.ToList().ForEach(match =>
            {
                GrepResult GrepResult = new GrepResult(filename)
                {
                    ContextString = match.Captures.FirstOrDefault().Value,
                    MatchedString = consoleCommand.CommandArgs[ConsoleFlag.SearchTerm]
                };

                grepResultCollection.AddItem(GrepResult);

                List<ConsoleItem> ConsoleItemCollection = new List<ConsoleItem>();

                // FileName
                ConsoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.DarkYellow, Value = $"{filename} " });

                // Line number
                if (!IgnoreBreaksFlag)
                {
                    int LineNumber = fileRaw.Substring(0, match.Groups["MatchedString"].Index).Split('\n').Length;
                    ConsoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.DarkMagenta, Value = $"Line {LineNumber}  " });
                }

                int ContextMatchStartIndex = GrepResult.ContextString.IndexOf(GrepResult.MatchedString, IgnoreCaseFlag ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
                int ContextMatchEndIndex = ContextMatchStartIndex + GrepResult.MatchedString.Length;

                // Context start
                ConsoleItemCollection.Add(new ConsoleItem() { Value = GrepResult.ContextString.Substring(0, ContextMatchStartIndex) });

                // Context matched
                ConsoleItemCollection.Add(new ConsoleItem() { BackgroundColor = ConsoleColor.DarkCyan, Value = GrepResult.ContextString.Substring(ContextMatchStartIndex, GrepResult.MatchedString.Length) });

                // Context end
                ConsoleItemCollection.Add(new ConsoleItem() { Value = GrepResult.ContextString.Substring(ContextMatchEndIndex, GrepResult.ContextString.Length - ContextMatchEndIndex) });

                // Empty buffer
                ConsoleItemCollection.Add(new ConsoleItem() { Value = Environment.NewLine + Environment.NewLine });

                MatchFound?.Invoke(ConsoleItemCollection, EventArgs.Empty);
            });
        }
        #endregion BuildSearchResultsFileContent

        #region BuildSearchResultsFilename
        private static void BuildSearchResultsFilename(ConsoleCommand consoleCommand, ThreadSafeCollection<GrepResult> grepResultCollection, MatchCollection matches, string filename)
        {
            if (matches.Any())
            {
                GrepResult GrepResult = new GrepResult(filename)
                {
                    ContextString = filename,
                    MatchedString = consoleCommand.CommandArgs[ConsoleFlag.SearchTerm]
                };

                grepResultCollection.AddItem(GrepResult);

                List<ConsoleItem> ConsoleItemCollection = new List<ConsoleItem>();

                int ContextMatchStartIndex = GrepResult.ContextString.IndexOf(GrepResult.MatchedString, StringComparison.OrdinalIgnoreCase);
                int ContextMatchEndIndex = ContextMatchStartIndex + GrepResult.MatchedString.Length;

                // Context start
                ConsoleItemCollection.Add(new ConsoleItem() { Value = GrepResult.ContextString.Substring(0, ContextMatchStartIndex) });

                // Context matched
                ConsoleItemCollection.Add(new ConsoleItem() { BackgroundColor = ConsoleColor.DarkCyan, Value = GrepResult.MatchedString });

                // Context end
                ConsoleItemCollection.Add(new ConsoleItem() { Value = GrepResult.ContextString.Substring(ContextMatchEndIndex, GrepResult.ContextString.Length - ContextMatchEndIndex) });

                // Empty buffer
                ConsoleItemCollection.Add(new ConsoleItem() { Value = Environment.NewLine });

                MatchFound?.Invoke(ConsoleItemCollection, EventArgs.Empty);
            }
        }
        #endregion BuildSearchResultsFilename

        #region GetFiles
        private static List<string> GetFiles(ConsoleCommand consoleCommand, IList<GrepResult> grepResultCollection, string filepath)
        {
            bool RecursiveFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Recursive);

            List<string> Files = null;

            if (grepResultCollection.Any())
            {
                Files = grepResultCollection.Select(result => result.SourceFile).ToList();
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
        private static void ProcessCommand(ThreadSafeCollection<GrepResult> grepResultCollection, IEnumerable<string> files, ConsoleCommand consoleCommand, RegexOptions optionsFlags)
        {
            bool DeleteFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Delete);
            bool ReplaceFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Replace);
            bool FileNamesOnlyFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileNamesOnly);
            bool IgnoreBreaksFlag = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.IgnoreBreaks);

            // Build search pattern
            string SearchPattern = BuildSearchPattern(consoleCommand);

            int FileReadFailedCount = 0;
            int FileWriteFailedCount = 0;

            int DeleteSuccessCount = 0;
            int ReplacedSuccessCount = 0;

            int FilesMatchedCount = 0;

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
                        Matches = Regex.Matches(fileLineBreakFiltered, SearchPattern, optionsFlags);
                    }
                    else
                    {
                        Matches = Regex.Matches(FileRaw, SearchPattern, optionsFlags);
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
                                    DeleteSuccessCount++;
                                }
                                else if (ReplaceFlag)
                                {
                                    // Replace all occurrences in file
                                    FileRaw = Regex.Replace(FileRaw, SearchPattern, consoleCommand.CommandArgs[ConsoleFlag.Replace]);
                                    File.WriteAllText(file, FileRaw);

                                    string MatchesText = Matches.Count == 1 ? "match" : "matches";
                                    ConsoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.DarkMagenta, Value = $"{Matches.Count} {MatchesText}" });
                                    ReplacedSuccessCount += Matches.Count;
                                }
                            }
                            catch (Exception ex)
                            {
                                ConsoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.Gray, BackgroundColor = ConsoleColor.DarkRed, Value = $"Access Denied" });
                                FileWriteFailedCount++;
                            }
                            finally
                            {
                                // Empty buffer
                                ConsoleItemCollection.Add(new ConsoleItem() { Value = Environment.NewLine });

                                FilesMatchedCount++;

                                MatchFound?.Invoke(ConsoleItemCollection, EventArgs.Empty);
                            }
                        }

                        // Read operations
                        else
                        {
                           
                            if (FileNamesOnlyFlag)
                            {
                                BuildSearchResultsFilename(consoleCommand, grepResultCollection, Matches, file);
                            }
                            else
                            {
                                BuildSearchResultsFileContent(consoleCommand, grepResultCollection, Matches, file, FileRaw);
                            }

                            FilesMatchedCount++;
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
        public static List<ConsoleItem> RunCommand(string commandRaw)
        {
            List<ConsoleItem> Result = null;
            var GrepResultCollection = new ThreadSafeCollection<GrepResult>();

            string[] CommandCollection = commandRaw.Split('|');
            foreach (string command in CommandCollection)
            {
                var CommandArgs = ConsoleUtils.DiscoverCommandArgs(command);
                ConsoleCommand ConsoleCommand = new ConsoleCommand() { CommandArgs = CommandArgs };

                BeginProcessCommand(ConsoleCommand, GrepResultCollection);
            }

            return Result;
        }
        #endregion RunCommand
        #endregion Methods..
    }
}
