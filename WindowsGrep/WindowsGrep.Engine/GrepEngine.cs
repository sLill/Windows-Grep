using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WindowsGrep.Common;

namespace WindowsGrep.Engine
{
    public class GrepEngine
    {
        #region Member Variables..
        private static object _LockObject = new object();
        #endregion Member Variables..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        #endregion Constructors..

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
                Files = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Recursive) ? Directory.GetFiles(InitialPath, "*", SearchOption.AllDirectories).ToList() : Directory.GetFiles(InitialPath, "*", SearchOption.TopDirectoryOnly).ToList();
            }

            // FileType filtering
            string[] FileTypeInclusions = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileTypeInclusions) ? consoleCommand.CommandArgs[ConsoleFlag.FileTypeInclusions].Split(new char[] { ',', ';' }) : null;
            string[] FileTypeExclusions = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FileTypeExclusions) ? consoleCommand.CommandArgs[ConsoleFlag.FileTypeExclusions].Split(new char[] { ',', ';' }) : null;

            Files = FileTypeInclusions == null ? Files : Files.Where(file => FileTypeInclusions.Contains(Path.GetExtension(file))).ToList();
            Files = FileTypeExclusions == null ? Files : Files.Where(file => !FileTypeExclusions.Contains(Path.GetExtension(file))).ToList();

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

            string[] commandCollection = commandRaw.Split('|');
            foreach (string command in commandCollection)
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
            string SearchPattern = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FixedStrings) ? @".{0,50}\b" + consoleCommand.CommandArgs[ConsoleFlag.SearchTerm] + @"\b.{0,50}" : @".{0,50}" + consoleCommand.CommandArgs[ConsoleFlag.SearchTerm] + ".{0,50}";
            int MaxFileNameLength = files.Max(x => x.Length);

            files.AsParallel().ForAll(file =>
            {
                string fileRaw = string.Join(string.Empty, File.ReadLines(file));
                var Matches = Regex.Matches(fileRaw, SearchPattern, optionsFlags);

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
                        string ItemBuffer = new string(' ', (MaxFileNameLength - file.Length) + 4);

                        lock (_LockObject)
                        {
                            // FileName
                            ConsoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.DarkYellow, Value = $"{file}{ItemBuffer}" });

                            int ContextMatchStartIndex = GrepResult.ContextString.IndexOf(GrepResult.MatchedString, StringComparison.OrdinalIgnoreCase);
                            int ContextMatchEndIndex = ContextMatchStartIndex + GrepResult.MatchedString.Length;

                            // Context start
                            ConsoleItemCollection.Add(new ConsoleItem() { Value = GrepResult.ContextString.Substring(0, ContextMatchStartIndex) });

                            // Context matched
                            ConsoleItemCollection.Add(new ConsoleItem() { BackgroundColor = ConsoleColor.DarkCyan, Value = GrepResult.MatchedString });

                            // Context end
                            ConsoleItemCollection.Add(new ConsoleItem() { Value = GrepResult.ContextString.Substring(ContextMatchEndIndex, GrepResult.ContextString.Length - ContextMatchEndIndex) });

                            // Empty buffer
                            ConsoleItemCollection.Add(new ConsoleItem() { Value = Environment.NewLine + Environment.NewLine });

                            ConsoleUtils.WriteConsoleItemCollection(ConsoleItemCollection);
                        }
                    });
                }
            });
        }
        #endregion SearchByFileContent

        #region SearchByFileName
        private static void SearchByFileName(ThreadSafeCollection<GrepResult> grepResultCollection, IEnumerable<string> files, ConsoleCommand consoleCommand, RegexOptions optionsFlags)
        {
            files.AsParallel().ForAll(file =>
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

                    lock (_LockObject)
                    {
                        // Context start
                        ConsoleItemCollection.Add(new ConsoleItem() { Value = GrepResult.ContextString.Substring(0, ContextMatchStartIndex) });

                        // Context matched
                        ConsoleItemCollection.Add(new ConsoleItem() { BackgroundColor = ConsoleColor.DarkCyan, Value = GrepResult.MatchedString });

                        // Context end
                        ConsoleItemCollection.Add(new ConsoleItem() { Value = GrepResult.ContextString.Substring(ContextMatchEndIndex, GrepResult.ContextString.Length - ContextMatchEndIndex) });

                        // Empty buffer
                        ConsoleItemCollection.Add(new ConsoleItem() { Value = Environment.NewLine });

                        ConsoleUtils.WriteConsoleItemCollection(ConsoleItemCollection);
                    }
                }
            });
        }
        #endregion SearchByFileName
        #endregion Methods..
    }
}
