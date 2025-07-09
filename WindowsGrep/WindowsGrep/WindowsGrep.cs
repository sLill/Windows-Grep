using System.IO;

namespace WindowsGrep
{
    public class WindowsGrep
    {
        #region Fields..
        private static Regex _descriptorRegex = new Regex(@"^[\s]*?(?<Descriptor>[-]+\S+)", RegexOptions.Compiled);
        private static Regex _longDescriptorRegex = new Regex(@"^[^=$]*", RegexOptions.Compiled);
        private static Regex _parameterRegex = new Regex(@"(?<!\\)(['""])(?<Parameter>.*?)(?<!\\)\1|^[^\s\'""]+", RegexOptions.Compiled);

        private readonly IServiceProvider _serviceProvider;
        #endregion Fields..

        #region Constructors..
        public WindowsGrep(IServiceProvider serviceProvider)
        {
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

        public static IDictionary<ConsoleFlag, string> ParseGrepCommandArgs(string commandRaw)
        {
            commandRaw = commandRaw.Trim();

            Dictionary<ConsoleFlag, string> commandArgs = new Dictionary<ConsoleFlag, string>();
            List<ConsoleFlag> consoleFlagCollection = EnumUtils.GetValues<ConsoleFlag>().ToList();

            // Option args
            while (true)
            {
                Match descriptorMatch = _descriptorRegex.Match(commandRaw);
                if (descriptorMatch.Success)
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
                                            throw new Exception($"Option '{descriptor}' expects a parameter, but none was provided.");

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

                    commandRaw = _descriptorRegex.Replace(commandRaw, string.Empty);
                }
                else
                    break;
            }

            // Search term
            commandRaw = commandRaw.Trim();
            Match searchTermMatch = _parameterRegex.Match(commandRaw);
            if (searchTermMatch.Success)
            {
                commandArgs[ConsoleFlag.SearchTerm] = searchTermMatch.Groups["Parameter"].Value;
                commandRaw = _parameterRegex.Replace(commandRaw, string.Empty, 1);

                // Directory
                string targetPath = Environment.CurrentDirectory;
                Match directoryMatch = _parameterRegex.Match(commandRaw);
                if (directoryMatch.Success)
                {
                    if (Path.IsPathFullyQualified(commandRaw))
                        targetPath = directoryMatch.Groups["Paramter"].Value;
                    else
                        targetPath = Path.GetFullPath(directoryMatch.Groups["Parameter"].Value);
                }

                if (Path.Exists(targetPath))
                    commandArgs[ConsoleFlag.Directory] = targetPath;
                else
                    throw new Exception($"File or directory '{targetPath}' does not exist");
            }
            else
                throw new Exception("Missing Search Term.\n\nUsage:   WindowsGrep [options] search_term directory");

            return commandArgs;
        }

        public static string GetFlagPattern(string flagDescriptor, bool expectsParameter)
        {
            string pattern = $"(\\s|^)(?<Descriptor>{flagDescriptor})(\\s+|$)?";
            pattern = expectsParameter ? pattern + "(?<Parameter>((['\"][^'\"]+.)|([\\\\/\\s\\S]*[\\\\/]\\s[^-]*)|[^\\s]+))\\s*" : pattern;

            return pattern;
        }

        public static string GetSanitizedFlagArgument(ConsoleFlag flag, string flagArgument)
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

        public static string GetCommandPattern(string commandDescriptor, bool expectsParameter)
        {
            string pattern = $"(\\s|^){commandDescriptor}";
            pattern = expectsParameter ? $"{pattern}\\s.*?(?<Parameter>.*?)$" : $"{pattern}[^\\S]*?$";
            return pattern;
        }
        #endregion Methods..
    }
}
