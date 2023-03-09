using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace WindowsGrep.Common
{
    public static class ConsoleUtils
    {
        #region Methods..
        #region DiscoverCommandArgs
        public static IDictionary<ConsoleFlag, string> DiscoverCommandArgs(string commandRaw)
        {
            ConcurrentDictionary<ConsoleFlag, string> commandArgs = new ConcurrentDictionary<ConsoleFlag, string>();

            List<ConsoleFlag> consoleFlagValues = EnumUtils.GetValues<ConsoleFlag>().ToList();
            consoleFlagValues.ForEach(flag =>
            {
                bool expectsParameter = flag.GetCustomAttribute<ExpectsParameterAttribute>()?.Value ?? false;
                List<string> descriptionCollection = flag.GetCustomAttribute<DescriptionCollectionAttribute>()?.Value.ToList();

                descriptionCollection?.ForEach(description =>
                {
                    string flagPattern = $"(\\s|^)(?<FlagDescriptor>{description})(\\s+|$)";
                    flagPattern = expectsParameter ? flagPattern + "(?<Argument>((['\"][^'\"]+.)|([\\\\/\\s\\S]*[\\\\/]\\s[^-]*)|[^\\s]+))\\s*" : flagPattern;

                    var matches = Regex.Matches(commandRaw, flagPattern);
                    if (expectsParameter && matches.Count > 1)
                    {
                        throw new Exception("Error: Arguments of parameter type cannot be specified more than once");
                    }
                    else if (matches.Count > 0)
                    {
                        string argument = matches.Select(match => match.Groups["Argument"].Value?.Trim(' ', '\'', '"')).FirstOrDefault();

                        // Filter invalid strings from beginning/end of argument
                        List<char> filterCharacterCollection = flag.GetCustomAttribute<FilterCharacterCollectionAttribute>()?.Value.ToList();
                        while (true && filterCharacterCollection != null)
                        {
                            bool argumentModified = false;
                            filterCharacterCollection.ForEach(character =>
                            {
                                if (argument.StartsWith(character) || argument.EndsWith(character))
                                {
                                    argument = argument.Trim(character);
                                    argumentModified = true;
                                }
                            });

                            if (!argumentModified)
                            {
                                break;
                            }
                        }

                        commandArgs[flag] = argument;
                        commandRaw = Regex.Replace(commandRaw, flagPattern, " ");
                    }
                });
            });

            // Search term
            commandArgs[ConsoleFlag.SearchTerm] = commandRaw.Trim();

            return commandArgs;
        }
        #endregion DiscoverCommandArgs

        #region WriteConsoleItem
        public static void WriteConsoleItem(ConsoleItem consoleItem)
        {
            Console.BackgroundColor = consoleItem.BackgroundColor;
            Console.ForegroundColor = consoleItem.ForegroundColor;

            Console.Write(consoleItem.Value);

            Console.ResetColor();
        }
        #endregion WriteConsoleItem

        #region WriteConsoleItemCollection
        public static void WriteConsoleItemCollection(List<ConsoleItem> consoleItemCollection)
        {
            lock (Console.Out)
            {
                consoleItemCollection.ForEach(consoleItem =>
                {
                    WriteConsoleItem(consoleItem);

                    // This seems to help give the console enough time to finalize changes made to background/foreground color properties
                    System.Threading.Thread.Sleep(5);
                });
            }
        }
        #endregion WriteConsoleItemCollection
        #endregion Methods..
    }
}
