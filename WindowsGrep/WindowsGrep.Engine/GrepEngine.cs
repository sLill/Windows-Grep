using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        #endregion Member Variables..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        #endregion Constructors..

        #region Methods..

        #region BeginSearch
        private static void BeginSearch(ConsoleCommand consoleCommand, ConcurrentDictionary<string, List<GrepResult>> GrepResultCollection)
        {
            // User specified file should overrule specified directory
            string InitialPath = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Directory) ? consoleCommand.CommandArgs[ConsoleFlag.Directory] : Environment.CurrentDirectory;
            InitialPath = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.File) ? consoleCommand.CommandArgs[ConsoleFlag.File] : InitialPath;

            List<string> Files = null;
            if (GrepResultCollection.Any())
            {
                Files = GrepResultCollection.Keys.ToList();
            }
            else
            {
                Files = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Recursive) ? Directory.GetFiles(InitialPath, "*", SearchOption.AllDirectories).ToList() : Directory.GetFiles(InitialPath, "*", SearchOption.TopDirectoryOnly).ToList();
            }

            // Read in files one at a time to match against
            string SearchTermPattern = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.FixedStrings) ? @"\b" + consoleCommand.CommandArgs[ConsoleFlag.SearchTerm] + @"\b" : consoleCommand.CommandArgs[ConsoleFlag.SearchTerm];
            Files.AsParallel().ForAll(file =>
            {
                string fileRaw = string.Join(string.Empty, File.ReadLines(file));

                RegexOptions OptionsFlags = 0;
                if (consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.IgnoreCase))
                {
                    OptionsFlags |= RegexOptions.IgnoreCase;
                }

                var Matches = Regex.Matches(fileRaw, SearchTermPattern, OptionsFlags);
                if (Matches.Any())
                {
                    if (!GrepResultCollection.ContainsKey(file))
                    {
                        GrepResultCollection[file] = new List<GrepResult>();
                    }

                    Matches.ToList().ForEach(match =>
                    {
                        GrepResultCollection[file].Add(new GrepResult(file));
                    });
                }
            });
        }
        #endregion BeginSearch

        #region FormatCommandResult
        private static string FormatCommandResult(IDictionary<string, List<GrepResult>> grepResultCollection)
        {
            StringBuilder Result = new StringBuilder(Environment.NewLine);

            Result.Append($"[{grepResultCollection.Sum(x => x.Value.Count)} result(s) {grepResultCollection.Keys.Count} file(s)]{Environment.NewLine}");
            Result.Append(string.Join(Environment.NewLine, grepResultCollection.Select(result => result.Key)));

            Result.Append(Environment.NewLine);
            return Result.ToString();
        }
        #endregion FormatCommandResult

        #region ProcessCommand
        public static string ProcessCommand(string commandRaw)
        {
            string Result = string.Empty;

            ConcurrentDictionary<string, List<GrepResult>> GrepResultCollection = new ConcurrentDictionary<string, List<GrepResult>>();

            string[] commandCollection = commandRaw.Split('|');
            foreach (string command in commandCollection)
            {
                var CommandArgs = ConsoleUtils.DiscoverCommandArgs(command);
                ConsoleCommand ConsoleCommand = new ConsoleCommand() { CommandArgs = CommandArgs };

                BeginSearch(ConsoleCommand, GrepResultCollection);
            }

            Result = FormatCommandResult(GrepResultCollection);
            return Result;
        }
        #endregion ProcessCommand
        #endregion Methods..
    }
}
