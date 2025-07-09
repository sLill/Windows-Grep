namespace WindowsGrep.Core;

public static class WindowsGrepUtils
{
    #region Fields..
    private static Regex _descriptorRegex = new Regex(@"^[\s]*?(?<Descriptor>[-]+\S+)", RegexOptions.Compiled);
    private static Regex _longDescriptorRegex = new Regex(@"^[^=$]*", RegexOptions.Compiled);
    private static Regex _parameterRegex = new Regex(@"(?<!\\)(['""])(?<Parameter_Quoted>.*?)(?<!\\)\1|(?<Parameter_Unquoted>[^\s\'""]+)", RegexOptions.Compiled);
    #endregion Fields..

    #region Methods..
    public static bool ArePathsEqual(string path1, string path2)
    {
        // Normalize paths to ensure consistent comparison
        var normalizedPath1 = Path.GetFullPath(path1).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var normalizedPath2 = Path.GetFullPath(path2).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        return string.Equals(normalizedPath1, normalizedPath2, StringComparison.OrdinalIgnoreCase);
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
                if (matches.Count > 0)
                {
                    string commandParameter = matches.Select(match => match.Groups["Parameter"].Value?.Trim(' ', '\'', '"')).FirstOrDefault();
                    return (commandType, commandParameter);
                }
            }
        }

        return default;
    }

    public static IDictionary<CommandFlag, string> ParseGrepCommandArgs(string commandString)
    {
        Dictionary<CommandFlag, string> commandArgs = new Dictionary<CommandFlag, string>();

        // Remove arbitrary "grep" from commands ran from the grep console
        commandString = commandString.Trim();
        if (commandString.StartsWith("grep"))
            commandString = commandString.Substring(4).Trim();

        ParseCommandFlags(commandArgs, commandString);

        // Return early for Help commands
        if (commandArgs.ContainsKey(CommandFlag.Help) || commandArgs.ContainsKey(CommandFlag.Help_Full))
            return commandArgs;

        ParseSearchTermAndPath(commandArgs, commandString);

        // Throw an exception if the command has not been fully consumed at this point
        if (commandString.Trim().Length > 0)
            throw new Exception($"Unrecognized command: {commandString}");

        return commandArgs;
    }

    private static void ParseCommandFlags(Dictionary<CommandFlag, string> commandArgs, string commandString)
    {
        List<CommandFlag> consoleFlagCollection = EnumUtils.GetValues<CommandFlag>().ToList();

        while (true)
        {
            Match descriptorMatch = _descriptorRegex.Match(commandString);
            if (descriptorMatch.Groups["Descriptor"].Success)
            {
                string descriptor = descriptorMatch.Groups["Descriptor"].Value;

                // Long descriptors
                if (descriptor.StartsWith("--"))
                {
                    descriptor = descriptor.TrimStart('-');
                    string descriptorFlag = _longDescriptorRegex.Match(descriptor).Value;

                    consoleFlagCollection.ForEach(x =>
                    {
                        var descriptionAttribute = x.GetCustomAttribute<DescriptionCollectionAttribute>();
                        if (descriptionAttribute != default)
                        {
                            if (descriptionAttribute.Value.Select(y => y.Trim(['-', '='])).Contains(descriptorFlag))
                            {
                                bool expectsParameter = x.GetCustomAttribute<ExpectsParameterAttribute>()?.Value ?? false;
                                if (expectsParameter)
                                {
                                    if (descriptor.Split('=').Length < 2)
                                        throw new Exception($"Option '{descriptor}' expects a parameter, but none was provided");

                                    string descriptorParameter = descriptor.Split('=')[1];
                                    commandArgs[x] = descriptorParameter;
                                }
                                else
                                    commandArgs[x] = string.Empty;
                            }
                        }
                    });
                }

                // Short descriptors
                else
                {
                    descriptor = descriptor.TrimStart('-');

                    // Handle combined/merged descriptors (ex. -ri)
                    for (int i = 0; i < descriptor.Length; i++)
                    {
                        consoleFlagCollection.ForEach(x =>
                        {
                            var descriptionAttribute = x.GetCustomAttribute<DescriptionCollectionAttribute>();
                            if (descriptionAttribute != default)
                            {
                                if (descriptionAttribute.Value.Select(y => y.Trim('-')).Contains(descriptor[i].ToString()))
                                    commandArgs[x] = string.Empty;
                            }
                        });
                    }
                }

                commandString = _descriptorRegex.Replace(commandString, string.Empty);
            }
            else
                break;
        }
    }

    private static void ParseSearchTermAndPath(Dictionary<CommandFlag, string> commandArgs, string commandString)
    {
        commandString = commandString.Trim();
        Match searchTermMatch = _parameterRegex.Match(commandString);
        string searchParameter = searchTermMatch.Groups["Parameter_Quoted"].Success ? searchTermMatch.Groups["Parameter_Quoted"].Value : string.Empty;
        searchParameter = string.IsNullOrEmpty(searchParameter) ? searchTermMatch.Groups["Parameter_Unquoted"].Value : searchParameter;

        if (!string.IsNullOrEmpty(searchParameter))
        {
            commandArgs[CommandFlag.SearchTerm] = searchParameter;
            commandString = _parameterRegex.Replace(commandString, string.Empty, 1);
            commandString = commandString.Trim();

            // Path
            string targetPath = Environment.CurrentDirectory;
            Match pathMatch = _parameterRegex.Match(commandString);
            string pathParameter = pathMatch.Groups["Parameter_Quoted"].Success ? pathMatch.Groups["Parameter_Quoted"].Value : string.Empty;
            pathParameter = string.IsNullOrEmpty(pathParameter) ? pathMatch.Groups["Parameter_Unquoted"].Value : pathParameter;

            if (!string.IsNullOrEmpty(pathParameter))
            {
                if (Path.Exists(pathParameter))
                {
                    targetPath = Path.IsPathFullyQualified(pathParameter) ? pathParameter : Path.GetFullPath(pathParameter);
                    commandString = _parameterRegex.Replace(commandString, string.Empty, 1);
                }
                else
                    throw new Exception($"Specified path does not exist '{pathParameter}'");
            }

            if (Path.Exists(targetPath))
                commandArgs[CommandFlag.Path] = targetPath;
            else
                throw new Exception($"File or directory '{targetPath}' does not exist");
        }
        else
            throw new Exception("Missing Search Term");
    }

    public static string GetCommandPattern(string commandDescriptor, bool expectsParameter)
    {
        string pattern = $"(\\s|^){commandDescriptor}";
        pattern = expectsParameter ? $"{pattern}\\s.*?(?<Parameter>.*?)$" : $"{pattern}[^\\S]*?$";
        return pattern;
    }
    #endregion Methods..
}
