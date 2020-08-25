using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace WindowsGrep.Common
{
    public static class ConsoleUtils
    {
        #region Member Variables..
        private static object _consoleLock = new object();
        #endregion Member Variables..

        #region Methods..
        #region DiscoverCommandArgs
        public static IDictionary<ConsoleFlag, string> DiscoverCommandArgs(string commandRaw)
        {
            ConcurrentDictionary<ConsoleFlag, string> CommandArgs = new ConcurrentDictionary<ConsoleFlag, string>();

            List<ConsoleFlag> ConsoleFlagValues = EnumUtils.GetValues<ConsoleFlag>().ToList();
            ConsoleFlagValues.ForEach(flag =>
            {
                bool ExpectsParameter = flag.GetCustomAttribute<ExpectsParameterAttribute>()?.Value ?? false;
                List<string> DescriptionCollection = flag.GetCustomAttribute<DescriptionCollectionAttribute>()?.Value.ToList();

                DescriptionCollection?.ForEach(description =>
                {
                    string FlagPattern = $"(^|\\s|-)(?<FlagDescriptor>{description})\\s?";

                    //FlagPattern = ExpectsParameter ? FlagPattern + "\\s?(?<Argument>([\\\\/]*[\\s\\S]*[\\\\/]+(\\s?[^-])*)|[^\\s]+)\\s*" : FlagPattern;
                    FlagPattern = ExpectsParameter ? FlagPattern + "\\s?(?<Argument>([\\\\/\\s\\S]*[\\\\/]\\s[^-]*)|[^\\s]+)\\s*" : FlagPattern;

                    var Matches = Regex.Matches(commandRaw, FlagPattern);
                    if (ExpectsParameter && Matches.Count > 1)
                    {
                        throw new Exception("Error: Arguments of parameter type cannot be specified more than once");
                    }
                    else if (Matches.Count > 0)
                    {
                        string Argument = Matches.Select(match => match.Groups["Argument"].Value?.Trim()).FirstOrDefault();

                        // Filter invalid strings from beginning/end of argument
                        List<char> FilterCharacterCollection = flag.GetCustomAttribute<FilterCharacterCollectionAttribute>()?.Value.ToList();
                        while (true && FilterCharacterCollection != null)
                        {
                            bool argumentModified = false;
                            FilterCharacterCollection.ForEach(character =>
                            {
                                if (Argument.StartsWith(character) || Argument.EndsWith(character))
                                {
                                    Argument = Argument.Trim(character);
                                    argumentModified = true;
                                }
                            });

                            if (!argumentModified)
                            {
                                break;
                            }
                        }

                        CommandArgs[flag] = Argument;
                        commandRaw = Regex.Replace(commandRaw, FlagPattern, " ");
                    }
                });
            });

            // Search term
            CommandArgs[ConsoleFlag.SearchTerm] = commandRaw.Trim();
            if (CommandArgs[ConsoleFlag.SearchTerm] == string.Empty)
            {
                throw new Exception("Error: Search term not supplied");
            }

            return CommandArgs;
        }
        #endregion DiscoverCommandArgs

        #region WriteConsoleItem
        public static void WriteConsoleItem(ConsoleItem consoleItem)
        {
            ConsoleColor ConsoleBackgroundOriginalColor = Console.BackgroundColor;
            ConsoleColor ConsoleForegroundOriginalColor = Console.ForegroundColor;

            Console.BackgroundColor = consoleItem.BackgroundColor;
            Console.ForegroundColor = consoleItem.ForegroundColor;
            Console.Write(consoleItem.Value);

            Console.BackgroundColor = ConsoleBackgroundOriginalColor;
            Console.ForegroundColor = ConsoleForegroundOriginalColor;
        }
        #endregion WriteConsoleItem

        #region WriteConsoleItemCollection
        public static void WriteConsoleItemCollection(object sender, EventArgs e)
        {
            var ConsoleItemCollection = sender as List<ConsoleItem>;
            lock (_consoleLock)
            {
                ConsoleItemCollection.ForEach(consoleItem =>
                {
                    WriteConsoleItem(consoleItem);
                });
            }
        }
        #endregion WriteConsoleItemCollection
        #endregion Methods..
    }
}
