namespace WindowsGrep
{
    public class WindowsGrep
    {
        #region Fields..
        private readonly IServiceProvider _serviceProvider;
        #endregion Fields..

        #region Properties..
        public List<ResultBase> Results { get; private set; } = new List<ResultBase>();
        #endregion Properties..

        #region Constructors..
        public WindowsGrep(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        #endregion Constructors..

        #region Methods..
        public async Task RunAsync(string[] args, CancellationTokenSource cancellationTokenSource)
        {
            do
            {
                using (var serviceScope = _serviceProvider.CreateScope())
                {
                    var consoleService = serviceScope.ServiceProvider.GetRequiredService<ConsoleService>();
                    var grepService = serviceScope.ServiceProvider.GetRequiredService<GrepService>();
                    var nativeService = serviceScope.ServiceProvider.GetRequiredService<NativeService>();
                    var publisherService = serviceScope.ServiceProvider.GetRequiredService<PublisherService>();

                    try
                    {
                        Results = new List<ResultBase>();

                        string commandRaw = string.Empty;
                        if (args.Length == 0)
                        {
                            ConsoleUtils.PublishPrompt();
                            commandRaw = Console.ReadLine();
                        }
                        else
                            commandRaw = string.Join(' ', args.Select(x => x.Contains(' ') ? $"\"{x}\"" : x));

                        var commands = commandRaw?.Split('|').Select(x => x.Trim()) ?? Array.Empty<string>();
                        foreach (string command in commands)
                        {
                            publisherService.RemoveAllSubscribers();

                            // Native commands
                            var nativeCommandArgs = WindowsGrepUtils.ParseNativeCommandArgs(command);
                            if (nativeCommandArgs != default)
                            {
                                var nativeCommand = new NativeCommand() { CommandType = nativeCommandArgs.CommandType.Value, CommandParameter = nativeCommandArgs.CommandParameter };
                                await Task.Run(() => nativeService.RunCommand(nativeCommand, Results, cancellationTokenSource.Token));
                            }

                            // Grep commands
                            else
                            {
                                var grepCommandArgs = WindowsGrepUtils.ParseGrepCommandArgs(command);
                                if (grepCommandArgs.ContainsKey(CommandFlag.Help))
                                    ConsoleUtils.PublishHelp(false);
                                else if (grepCommandArgs.ContainsKey(CommandFlag.Help_Full))
                                    ConsoleUtils.PublishHelp(true);
                                else
                                {
                                    var grepCommand = new GrepCommand() { CommandArgs = grepCommandArgs };
                                    await Task.Run(() => grepService.RunCommand(grepCommand, Results, cancellationTokenSource.Token));
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        consoleService.Write(new() { Value = $"\n{ex.Message}\n", ForegroundColor = ConsoleColor.Red });
                        consoleService.Write(new() { Value = $"Usage:   grep [options] search_term [path]\n\n", ForegroundColor = ConsoleColor.White });
                    }
                }
            }
            while (args.Length == 0 && !cancellationTokenSource.IsCancellationRequested);
        }
        #endregion Methods..
    }
}
