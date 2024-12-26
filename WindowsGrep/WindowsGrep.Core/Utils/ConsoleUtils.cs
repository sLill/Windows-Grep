namespace WindowsGrep.Core;

public static class ConsoleUtils
{
    #region Methods..
    public static void ClearConsole()
        => Console.Clear();

    private static string GetCommandPattern(string commandDescriptor, bool expectsParameter)
    {
        string pattern = $"(\\s|^){commandDescriptor}";
        pattern = expectsParameter ? $"{pattern}\\s.*?(?<Parameter>.*?)$" : $"{pattern}[^\\S]*?$";

        return pattern;
    }

    private static string GetFlagPattern(string flagDescriptor, bool expectsParameter)
    {
        string pattern = $"(\\s|^)(?<Descriptor>{flagDescriptor})(\\s+|$)?";
        pattern = expectsParameter ? pattern + "(?<Parameter>((['\"][^'\"]+.)|([\\\\/\\s\\S]*[\\\\/]\\s[^-]*)|[^\\s]+))\\s*" : pattern;

        return pattern;
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

    public static IDictionary<ConsoleFlag, string> ParseGrepCommandArgs(string commandRaw)
    {
        ConcurrentDictionary<ConsoleFlag, string> commandArgs = new ConcurrentDictionary<ConsoleFlag, string>();

        List<ConsoleFlag> consoleFlagCollection = EnumUtils.GetValues<ConsoleFlag>().ToList();
        consoleFlagCollection.ForEach(flag =>
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
                    string flagArgument = matches.Select(match => match.Groups["Parameter"].Value?.Trim(' ', '\'', '"')).FirstOrDefault();
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

    public static (NativeCommandType? CommandType, string CommandParameter) ParseNativeCommandArgs(string commandRaw)
    {
        List<NativeCommandType> nativeCommandCollection = EnumUtils.GetValues<NativeCommandType>().ToList();
        foreach (var commandType in nativeCommandCollection)
        {
            List<string> commandDescriptionCollection = commandType.GetCustomAttribute<DescriptionCollectionAttribute>()?.Value.ToList();

            foreach (var commandDescription in commandDescriptionCollection)
            {
                bool expectsParameter = commandType.GetCustomAttribute<ExpectsParameterAttribute>()?.Value ?? false;
                string commandPattern = GetCommandPattern(commandDescription, expectsParameter);

                var matches = Regex.Matches(commandRaw, commandPattern);
                if (matches.Any())
                {
                    string commandParameter = matches.Select(match => match.Groups["Parameter"].Value?.Trim(' ', '\'', '"')).FirstOrDefault();
                    return (commandType, commandParameter);
                }
            }
        }

        return default;
    }

    public static void PublishReadMe()
    {
        string readMe = Properties.Resources.ReadMe;
        Console.WriteLine(readMe + Environment.NewLine);
    }

    public static void PublishPrompt()
    {
        string prompt = string.Empty;

        bool displayWorkingDirectoryInPrompt = (bool)ConfigurationManager.Instance.ConfigItemCollection[ConfigItem.DisplayWorkingDirectoryInPrompt];
        if (displayWorkingDirectoryInPrompt)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            currentDirectory = WindowsUtils.GetCompressedPath(currentDirectory);
            prompt = $"{currentDirectory}> ";
        }
        else
            prompt = "$ ";

        Console.Write(prompt);
    }

    public static void WriteConsoleItem(ConsoleItem consoleItem)
    {
        Console.BackgroundColor = consoleItem.BackgroundColor;
        Console.ForegroundColor = consoleItem.ForegroundColor;

        Console.Write(consoleItem.Value);
        Console.ResetColor();
    }

    public static async Task WriteConsoleItemCollectionAsync(List<ConsoleItem> consoleItemCollection, CancellationToken cancellationToken)
    {
        lock (Console.Out)
        {
            consoleItemCollection.ForEach(consoleItem =>
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                WriteConsoleItem(consoleItem);
            });
        }
    }
    #endregion Methods..
}
