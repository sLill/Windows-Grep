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

        #region BeginReplace
        private static void BeginReplace(ConsoleCommand consoleCommand, ThreadSafeCollection<GrepResult> grepResultCollection)
        {
            string Filepath = GetPath(consoleCommand);
            List<string> Files = GetFiles(consoleCommand, grepResultCollection, Filepath);
            Files = GetFilteredFiles(consoleCommand, Files);

            RegexOptions OptionsFlags = GetRegexOptions(consoleCommand);
            string SearchPattern = BuildSearchPattern(consoleCommand);

            int AffectedFilesTotal = 0;
            int ReplacedCountTotal = 0;
            int UnwriteableCount = 0;

            // Preview
            ConsoleUtils.WriteConsoleItem(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = $"{Environment.NewLine}[Searching {Files.Count} file(s)]{Environment.NewLine}" });

            Files.AsParallel().ForAll(file =>
            {
                try
                {
                    string FileRaw = File.ReadAllText(file);
                    MatchCollection Matches = Regex.Matches(FileRaw, SearchPattern, OptionsFlags);

                    if (Matches.Any())
                    {
                        // Replace all occurrences in file
                        FileRaw = Regex.Replace(FileRaw, SearchPattern, consoleCommand.CommandArgs[ConsoleFlag.Replace]);

                        List<ConsoleItem> ConsoleItemCollection = new List<ConsoleItem>();

                        // FileName
                        ConsoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.DarkYellow, Value = $"{file} " });

                        // Overwrite file
                        try
                        {
                            File.WriteAllText(file, FileRaw);

                            ReplacedCountTotal += Matches.Count;
                            AffectedFilesTotal++;

                            // Occurrences
                            string MatchesText = Matches.Count == 1 ? "match" : "matches";
                            ConsoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.DarkMagenta, Value = $"{Matches.Count} {MatchesText}" });

                        }
                        catch (Exception ex)
                        {
                            ConsoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.Gray, BackgroundColor= ConsoleColor.DarkRed, Value = $"Access Denied" });
                            UnwriteableCount++;
                        }
                        finally
                        {
                            ConsoleItemCollection.Add(new ConsoleItem() { Value = Environment.NewLine + Environment.NewLine });
                            MatchFound?.Invoke(ConsoleItemCollection, EventArgs.Empty);
                        }
                    }
                }
                catch (Exception ex) { }
            });

            // Notify the user of any files that could not be written to
            if (UnwriteableCount > 0)
            {
                string UnwriteableFiles = $"[{UnwriteableCount} file(s) unwriteable/inaccessible]";
                ConsoleUtils.WriteConsoleItem(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = UnwriteableFiles });
                ConsoleUtils.WriteConsoleItem(new ConsoleItem() { Value = Environment.NewLine });
            }

            string Summary = $"[{ReplacedCountTotal} occurrence(s) replaced in {AffectedFilesTotal} file(s)]";
            ConsoleUtils.WriteConsoleItem(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = Summary });
            ConsoleUtils.WriteConsoleItem(new ConsoleItem() { Value = Environment.NewLine + Environment.NewLine });
        }
        #endregion BeginReplace

        #region BeginSearch
        private static void BeginSearch(ConsoleCommand consoleCommand, ThreadSafeCollection<GrepResult> grepResultCollection)
        {
            string Filepath = GetPath(consoleCommand);
            List<string> Files = GetFiles(consoleCommand, grepResultCollection, Filepath);
            Files = GetFilteredFiles(consoleCommand, Files);

            ConsoleUtils.WriteConsoleItem(new ConsoleItem() { ForegroundColor = ConsoleColor.Red , Value = $"{Environment.NewLine}[Searching {Files.Count} file(s)]{Environment.NewLine}" });

            RegexOptions OptionsFlags = GetRegexOptions(consoleCommand);

            bool FileNamesOnly = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileNamesOnly);
            if (FileNamesOnly)
            {
                SearchByFileName(grepResultCollection, Files, consoleCommand, OptionsFlags);
            }
            else
            {
                SearchByFileContent(grepResultCollection, Files, consoleCommand, OptionsFlags);
            }

            string SearchSummary = $"[{grepResultCollection.Count} result(s) {grepResultCollection.Select(x => x.SourceFile).Distinct().Count()} file(s)]";           
            ConsoleUtils.WriteConsoleItem(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = SearchSummary });
            ConsoleUtils.WriteConsoleItem(new ConsoleItem() { Value = Environment.NewLine + Environment.NewLine });
        }
        #endregion BeginSearch


        #region BuildSearchPattern
        private static string BuildSearchPattern(ConsoleCommand consoleCommand)
        {
            string SearchTerm = consoleCommand.CommandArgs[ConsoleFlag.SearchTerm];

            int ContextLength = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Context) ? Convert.ToInt32(consoleCommand.CommandArgs[ConsoleFlag.Context]) : 0;
            string ContextPattern = ContextLength <= 0 ? string.Empty : @"(?:(?!" + SearchTerm + @").){0," + ContextLength.ToString() + @"}";

            string SearchPattern = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FixedStrings)
                ? ContextPattern + @"(?<MatchedString>[\b\B]?" + SearchTerm + @"[\b\B]?)" + ContextPattern
                : ContextPattern + @"(?<MatchedString>" + SearchTerm + @")" + ContextPattern;

            return SearchPattern;
        }
        #endregion BuildSearchPattern

        #region GetFiles
        private static List<string> GetFiles(ConsoleCommand consoleCommand, IList<GrepResult> grepResultCollection, string filepath)
        {
            List<string> Files = null;

            if (grepResultCollection.Any())
            {
                Files = grepResultCollection.Select(result => result.SourceFile).ToList();
            }
            else
            {
                var EnumerationOptions = new EnumerationOptions() { ReturnSpecialDirectories = true, AttributesToSkip = FileAttributes.System };
                if (consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Recursive))
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
            string[] FileTypeInclusions = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileTypeInclusions) ? consoleCommand.CommandArgs[ConsoleFlag.FileTypeInclusions].Split(new char[] { ',', ';' }) : null;
            string[] FileTypeExclusions = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileTypeExclusions) ? consoleCommand.CommandArgs[ConsoleFlag.FileTypeExclusions].Split(new char[] { ',', ';' }) : null;

            files = FileTypeInclusions == null ? files : files.Where(file => FileTypeInclusions.Contains(Path.GetExtension(file).Trim('.'))).ToList();
            files = FileTypeExclusions == null ? files : files.Where(file => !FileTypeExclusions.Contains(Path.GetExtension(file).Trim('.'))).ToList();

            return files;
        }
        #endregion GetFilteredFiles

        #region GetPath
        private static string GetPath(ConsoleCommand consoleCommand)
        {
            // User specified file should overrule specified directory
            string Filepath = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Directory) ? consoleCommand.CommandArgs[ConsoleFlag.Directory] : Environment.CurrentDirectory;
            Filepath = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.TargetFile) ? consoleCommand.CommandArgs[ConsoleFlag.TargetFile] : Filepath;

            return Filepath;
        }
        #endregion GetPath

        #region GetRegexOptions
        private static RegexOptions GetRegexOptions(ConsoleCommand consoleCommand)
        {
            RegexOptions OptionsFlags = 0;
            
            if (consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.IgnoreCase))
            {
                OptionsFlags |= RegexOptions.IgnoreCase;
            }

            return OptionsFlags;
        }
        #endregion GetRegexOptions

        #region ProcessCommand
        public static List<ConsoleItem> ProcessCommand(string commandRaw)
        {
            List<ConsoleItem> Result = null;
            var GrepResultCollection = new ThreadSafeCollection<GrepResult>();

            string[] CommandCollection = commandRaw.Split('|');
            foreach (string command in CommandCollection)
            {
                var CommandArgs = ConsoleUtils.DiscoverCommandArgs(command);
                ConsoleCommand ConsoleCommand = new ConsoleCommand() { CommandArgs = CommandArgs };

                if (ConsoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Replace))
                {
                    // Replace command
                    BeginReplace(ConsoleCommand, GrepResultCollection);
                }
                else
                {
                    // Search command
                    BeginSearch(ConsoleCommand, GrepResultCollection);
                }
            }

            return Result;
        }
        #endregion ProcessCommand

        #region SearchByFileContent
        private static void SearchByFileContent(ThreadSafeCollection<GrepResult> grepResultCollection, IEnumerable<string> files, ConsoleCommand consoleCommand, RegexOptions optionsFlags)
        {
            // Build search pattern
            string SearchPattern = BuildSearchPattern(consoleCommand);

            int UnreadableCount = 0;

            // Read in files one at a time to match against
            files.AsParallel().ForAll(file =>
            {
                try
                {
                    MatchCollection Matches = null;

                    string FileRaw = File.ReadAllText(file);

                    // Apply linebreak filtering options
                    if (consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.IgnoreBreaks))
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
                        Matches.ToList().ForEach(match =>
                        {
                            GrepResult GrepResult = new GrepResult(file)
                            {
                                ContextString = match.Captures.FirstOrDefault().Value,
                                MatchedString = consoleCommand.CommandArgs[ConsoleFlag.SearchTerm]
                            };

                            grepResultCollection.AddItem(GrepResult);

                            List<ConsoleItem> ConsoleItemCollection = new List<ConsoleItem>();

                            // FileName
                            ConsoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.DarkYellow, Value = $"{file} " });

                            // Line number
                            if (!consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.IgnoreBreaks))
                            {
                                int LineNumber = FileRaw.Substring(0, match.Groups["MatchedString"].Index).Split('\n').Length;
                                ConsoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.DarkMagenta, Value = $"Line {LineNumber}  " });
                            }

                            int ContextMatchStartIndex = GrepResult.ContextString.IndexOf(GrepResult.MatchedString, consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.IgnoreCase) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
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
                }
                catch (Exception ex) 
                { 
                    lock(_searchLock)
                    {
                        UnreadableCount++;
                    }
                }
            });

            // Notify the user of any files that could not be read from
            if (UnreadableCount > 0)
            {
                string UnreachableFiles = $"[{UnreadableCount} file(s) unreadable/inaccessible]";
                ConsoleUtils.WriteConsoleItem(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = UnreachableFiles });
                ConsoleUtils.WriteConsoleItem(new ConsoleItem() { Value = Environment.NewLine });
            }
        }
        #endregion SearchByFileContent

        #region SearchByFileName
        private static void SearchByFileName(ThreadSafeCollection<GrepResult> grepResultCollection, IEnumerable<string> files, ConsoleCommand consoleCommand, RegexOptions optionsFlags)
        {
            files.AsParallel().ForAll(file =>
            {
                try
                {
                    string SearchPattern = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FixedStrings) ? @"\b" + consoleCommand.CommandArgs[ConsoleFlag.SearchTerm] + @"\b" : consoleCommand.CommandArgs[ConsoleFlag.SearchTerm];
                    var Matches = Regex.Matches(file, SearchPattern, optionsFlags);

                    if (Matches.Any())
                    {
                        GrepResult GrepResult = new GrepResult(file)
                        {
                            ContextString = file,
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
                catch (Exception) { }
            });
        }
        #endregion SearchByFileName
        #endregion Methods..
    }
}
