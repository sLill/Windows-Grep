namespace WindowsGrep
{
    public class Program
    {
        #region Fields..
        private static CancellationTokenSource _cancellationTokenSource;
        #endregion Fields..

        #region Main
        private static async Task Main(string[] args)
        {
            Initialize(args);
            await RunAsync(args);
        }
        #endregion Main

        #region Methods..
        #region Event Handlers..
        private static void Console_OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            _cancellationTokenSource?.Cancel();
        }
        #endregion Event Handlers..

        private static void Initialize(string[] args)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            // Only necessary on older versions of Windows 
            WindowsUtils.TryEnableAnsi();

            // Override the default behavior for the Ctrl+C shortcut if the application was not ran from the command line
            if (Environment.UserInteractive)
                Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_OnCancelKeyPress);
        }

        private static async Task RunAsync(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var windowsGrep = host.Services.GetRequiredService<WindowsGrep>();
            await windowsGrep.RunAsync(args, _cancellationTokenSource);
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host
            .CreateDefaultBuilder(args)
            .ConfigureLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Information)
                       .AddFilter("Microsoft", LogLevel.Warning)
                       .AddFilter("System", LogLevel.Warning)
                       .AddDebug();
#if DEBUG
                builder.AddFilter("WindowsGrep", LogLevel.Debug)
                       .AddConsole();
#endif
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<ConsoleService>()
                        .AddSingleton<FileService>()
                        .AddScoped<WindowsGrep>()
                        .AddScoped<GrepService>()
                        .AddScoped<NativeService>()
                        .AddScoped<PublisherService>();
            });
        }
        #endregion Methods..
    }
}
