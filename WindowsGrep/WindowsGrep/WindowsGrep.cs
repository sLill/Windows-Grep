namespace WindowsGrep
{
    public class WindowsGrep
    {
        #region Fields..
        private readonly ILogger<WindowsGrep> _logger;
        private readonly IServiceProvider _serviceProvider;
        #endregion Fields..

        #region Constructors..
        public WindowsGrep(ILogger<WindowsGrep> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        #endregion Constructors..

        #region Methods..
        public async Task RunAsync(string[] args, CancellationTokenSource cancellationTokenSource)
        {
            if (args.Length == 0)
                ConsoleUtils.PublishSplash();

            do
            {
                using (var serviceScope = _serviceProvider.CreateScope())
                {
                    var publisherService = serviceScope.ServiceProvider.GetRequiredService<PublisherService>();
                    var consoleService = serviceScope.ServiceProvider.GetRequiredService<ConsoleService>();
                    var grepService = serviceScope.ServiceProvider.GetRequiredService<GrepService>();
                    var nativeService = serviceScope.ServiceProvider.GetRequiredService<NativeService>();

                    try
                    {
                        var results = new List<ResultBase>();

                        string commandRaw = string.Empty;
                        if (args.Length == 0)
                        {
                            ConsoleUtils.PublishPrompt();
                            commandRaw = Console.ReadLine();
                        }
                        else
                            commandRaw = string.Join(" ", args);

                        var commands = commandRaw.Split('|').Select(x => x.Trim());
                        foreach (string command in commands)
                        {
                            publisherService.RemoveAllSubscribers();

                            // Native commands
                            var nativeCommandArgs = ParseNativeCommandArgs(command);
                            if (nativeCommandArgs != default)
                            {
                                var nativeCommand = new NativeCommand() { CommandType = nativeCommandArgs.CommandType.Value, CommandParameter = nativeCommandArgs.CommandParameter };
                                await Task.Run(() => nativeService.RunCommand(nativeCommand, results, cancellationTokenSource.Token));
                            }

                            // Grep commands
                            else
                            {
                                var grepCommandArgs = ParseGrepCommandArgs(command);
                                if (grepCommandArgs.ContainsKey(ConsoleFlag.Help))
                                    ConsoleUtils.PublishHelp(false);
                                else if (grepCommandArgs.ContainsKey(ConsoleFlag.Help_Full))
                                    ConsoleUtils.PublishHelp(true);
                                else
                                {
                                    var grepCommand = new GrepCommand() { CommandArgs = grepCommandArgs };
                                    await Task.Run(() => grepService.RunCommand(grepCommand, results, cancellationTokenSource.Token));
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        consoleService.Write(new() { Value = $"\nError: {ex.Message}\n\n", ForegroundColor = ConsoleColor.Red });
                    }
                }
            }
            while (args.Length == 0 && !cancellationTokenSource.IsCancellationRequested);

        }

        private static (NativeCommandType? CommandType, string CommandParameter) ParseNativeCommandArgs(string commandRaw)
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

        private static IDictionary<ConsoleFlag, string> ParseGrepCommandArgs(string commandRaw)
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

        private static string GetCommandPattern(string commandDescriptor, bool expectsParameter)
        {
            string pattern = $"(\\s|^){commandDescriptor}";
            pattern = expectsParameter ? $"{pattern}\\s.*?(?<Parameter>.*?)$" : $"{pattern}[^\\S]*?$";
            return pattern;
        }
        #endregion Methods..
    }
}
