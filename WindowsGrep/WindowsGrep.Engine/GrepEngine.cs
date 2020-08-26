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
        #region BeginSearch
        private static void BeginSearch(ConsoleCommand consoleCommand, ThreadSafeCollection<GrepResult> GrepResultCollection)
        {
            // User specified file should overrule specified directory
            string InitialPath = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Directory) ? consoleCommand.CommandArgs[ConsoleFlag.Directory] : Environment.CurrentDirectory;
            InitialPath = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.TargetFile) ? consoleCommand.CommandArgs[ConsoleFlag.TargetFile] : InitialPath;

            List<string> Files = null;
            if (GrepResultCollection.Any())
            {
                Files = GrepResultCollection.Select(result => result.SourceFile).ToList();
            }
            else
            {
                var EnumerationOptions = new EnumerationOptions() { ReturnSpecialDirectories = true, AttributesToSkip = FileAttributes.System };
                if (consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Recursive))
                {
                    EnumerationOptions.RecurseSubdirectories = true;
                }

                Files = Directory.GetFiles(Path.TrimEndingDirectorySeparator(InitialPath.TrimEnd()), "*", EnumerationOptions).ToList();
            }

            // FileType filtering
            string[] FileTypeInclusions = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileTypeInclusions) ? consoleCommand.CommandArgs[ConsoleFlag.FileTypeInclusions].Split(new char[] { ',', ';' }) : null;
            string[] FileTypeExclusions = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileTypeExclusions) ? consoleCommand.CommandArgs[ConsoleFlag.FileTypeExclusions].Split(new char[] { ',', ';' }) : null;

            Files = FileTypeInclusions == null ? Files : Files.Where(file => FileTypeInclusions.Contains(Path.GetExtension(file).Trim('.'))).ToList();
            Files = FileTypeExclusions == null ? Files : Files.Where(file => !FileTypeExclusions.Contains(Path.GetExtension(file).Trim('.'))).ToList();

            ConsoleUtils.WriteConsoleItem(new ConsoleItem() { ForegroundColor = ConsoleColor.Red , Value = $"{Environment.NewLine}[Searching {Files.Count} file(s)]{Environment.NewLine}" });

            RegexOptions OptionsFlags = 0;
            if (consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.IgnoreCase))
            {
                OptionsFlags |= RegexOptions.IgnoreCase;
            }

            bool FileNamesOnly = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileNamesOnly);
            if (FileNamesOnly)
            {
                SearchByFileName(GrepResultCollection, Files, consoleCommand, OptionsFlags);
            }
            else
            {
                SearchByFileContent(GrepResultCollection, Files, consoleCommand, OptionsFlags);
            }

            string SearchSummary = $"[{GrepResultCollection.Count} result(s) {GrepResultCollection.Select(x => x.SourceFile).Distinct().Count()} file(s)]";           
            ConsoleUtils.WriteConsoleItem(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = SearchSummary });
            ConsoleUtils.WriteConsoleItem(new ConsoleItem() { Value = Environment.NewLine + Environment.NewLine });
        }
        #endregion BeginSearch

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

                BeginSearch(ConsoleCommand, GrepResultCollection);
            }

            return Result;
        }
        #endregion ProcessCommand

        #region SearchByFileContent
        private static void SearchByFileContent(ThreadSafeCollection<GrepResult> grepResultCollection, IEnumerable<string> files, ConsoleCommand consoleCommand, RegexOptions optionsFlags)
        {
            // Read in files one at a time to match against
            string SearchTerm = consoleCommand.CommandArgs[ConsoleFlag.SearchTerm];
            string SearchPattern = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FixedStrings) 
                ? @"(?:(?!" + SearchTerm + @").){0,50}?(?<MatchedString>[\b\B]?" + SearchTerm + @"[\b\B]?)(?:(?!" + SearchTerm + @").){0,50}"
                : @"(?:(?!" + SearchTerm + @").){0,50}?(?<MatchedString>" + SearchTerm + @")(?:(?!" + SearchTerm + @").){0,50}";

            files.AsParallel().ForAll(file =>
            {
                try
                {
                    MatchCollection Matches = null;

                    string FileRaw = File.ReadAllText(file);
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
                { }
            });
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
