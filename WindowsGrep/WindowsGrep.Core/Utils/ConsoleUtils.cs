﻿using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using WindowsGrep.Common;

namespace WindowsGrep.Core
{
    public static class ConsoleUtils
    {
        #region Methods..
        public static IDictionary<ConsoleFlag, string> DiscoverCommandArgs(string commandRaw)
        {
            ConcurrentDictionary<ConsoleFlag, string> commandArgs = new ConcurrentDictionary<ConsoleFlag, string>();

            List<ConsoleFlag> consoleFlagValues = EnumUtils.GetValues<ConsoleFlag>().ToList();
            consoleFlagValues.ForEach(flag =>
            {
                bool expectsParameter = flag.GetCustomAttribute<ExpectsParameterAttribute>()?.Value ?? false;
                List<string> flagDescriptionCollection = flag.GetCustomAttribute<DescriptionCollectionAttribute>()?.Value.ToList();

                flagDescriptionCollection?.ForEach(flagDescription =>
                {
                    string flagPattern = GetFlagPattern(flagDescription, expectsParameter);
                    var matches = Regex.Matches(commandRaw, flagPattern);
                    
                    if (expectsParameter && matches.Count > 1)
                        throw new Exception("Error: Arguments of parameter type cannot be specified more than once");
                    else if (matches.Count > 0)
                    {
                        string flagArgument = matches.Select(match => match.Groups["Argument"].Value?.Trim(' ', '\'', '"')).FirstOrDefault();
                        flagArgument = GetSanitizedFlagArgument(flag, flagArgument);

                        commandArgs[flag] = flagArgument;
                        commandRaw = Regex.Replace(commandRaw, flagPattern, " ");
                    }
                });
            });

            // Search term
            commandArgs[ConsoleFlag.SearchTerm] = commandRaw.Trim();

            return commandArgs;
        }

        private static string GetFlagPattern(string flagDescription, bool expectsParameter)
        {
            string flagPattern = $"(\\s|^)(?<FlagDescriptor>{flagDescription})(\\s+|$)?";
            flagPattern = expectsParameter ? flagPattern + "(?<Argument>((['\"][^'\"]+.)|([\\\\/\\s\\S]*[\\\\/]\\s[^-]*)|[^\\s]+))\\s*" : flagPattern;

            return flagPattern;
        }

        private static string GetSanitizedFlagArgument(ConsoleFlag flag, string flagArgument)
        {
            // Filter invalid strings from beginning/end of argument
            List<char> filterCharacterCollection = flag.GetCustomAttribute<FilterCharacterCollectionAttribute>()?.Value.ToList();
            while (true && filterCharacterCollection != null)
            {
                bool flagArgumentModified = false;
                filterCharacterCollection.ForEach(character =>
                {
                    if (flagArgument.StartsWith(character) || flagArgument.EndsWith(character))
                    {
                        flagArgument = flagArgument.Trim(character);
                        flagArgumentModified = true;
                    }
                });

                if (!flagArgumentModified)
                    break;
            }

            return flagArgument;
        }

        public static void PublishReadMe()
        {
            string readMe = Properties.Resources.ReadMe;
            Console.WriteLine(readMe + Environment.NewLine);
        }

        public static void WriteConsoleItem(ConsoleItem consoleItem)
        {
            Console.BackgroundColor = consoleItem.BackgroundColor;
            Console.ForegroundColor = consoleItem.ForegroundColor;

            Console.Write(consoleItem.Value);
            Console.ResetColor();
        }

        public static void WriteConsoleItemCollection(List<ConsoleItem> consoleItemCollection)
        {
            lock (Console.Out)
            {
                consoleItemCollection.ForEach(consoleItem =>
                {
                    WriteConsoleItem(consoleItem);

                    // This seems to help give the native console enough time to finalize changes made to background/foreground color properties
                    Thread.Sleep(5);
                });
            }
        }
        #endregion Methods..
    }
}