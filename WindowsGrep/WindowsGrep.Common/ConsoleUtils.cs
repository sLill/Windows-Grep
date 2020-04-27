using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WindowsGrep.Common
{
    public static class ConsoleUtils
    {
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
                
                // Search filter 
                if (flag == ConsoleFlag.Default)
                {
                    string SearchFilterPattern = "\"(?<SearchFilter>[^\"]*)\"";
                    CommandArgs[ConsoleFlag.Default] = Regex.Match(commandRaw, SearchFilterPattern).Groups["SearchFilter"].Value;
                }

                DescriptionCollection?.ForEach(description =>
                {
                    string FlagPattern = $"(^|\\s|-)(?<FlagDescriptor>{description})";
                    FlagPattern = ExpectsParameter ? FlagPattern + "\\s*(?<Argument>\\S*)" : FlagPattern;
                   
                    var Matches = Regex.Matches(commandRaw.ToLower(), FlagPattern.ToLower());
                    if (ExpectsParameter && Matches.Count > 1)
                    {
                        throw new Exception("Arguments of parameter type cannot be specified more than once");
                    }
                    else if (Matches.Count > 0)
                    {
                        CommandArgs[flag] = Matches.Select(match => match.Groups["Argument"].Value).FirstOrDefault();
                    }
                });
            });

            if (CommandArgs[ConsoleFlag.Default] == string.Empty)
            {
                throw new Exception("Search term not supplied");
            }

            return CommandArgs;
        }
        #endregion DiscoverCommandArgs
        #endregion Methods..
    }
}
