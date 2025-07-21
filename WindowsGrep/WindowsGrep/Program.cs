
namespace WindowsGrep
{
    public class Program
    {
        #region Fields..
        private static IHost _host;
        #endregion Fields..

        #region Main
        private static async Task Main(string[] args)
        {
            Initialize(args);

            if (args.Length == 0)
                ConsoleUtils.PublishSplash();

            await RunAsync(args);
        }
        #endregion Main

        #region Methods..
        #region Event Handlers..
        private static void Console_OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;

            var windowsGrep = _host.Services.GetRequiredService<WindowsGrep>();
            windowsGrep.Cancel();
        }
        #endregion Event Handlers..

        private static void Initialize(string[] args)
        {
            // Only necessary on older versions of Windows 
            WindowsUtils.TryEnableAnsi();

            // Override the default behavior for the Ctrl+C shortcut if the application was not ran from the command line
            if (Environment.UserInteractive)
                Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_OnCancelKeyPress);
        }

        private static async Task RunAsync(string[] args)
        {
            _host = CreateHostBuilder(args).Build();
            var windowsGrep = _host.Services.GetRequiredService<WindowsGrep>();
            await windowsGrep.RunAsync(args);
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host
            .CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<ConsoleService>()
                        .AddSingleton<FileService>()
                        .AddSingleton<PublisherService>()
                        .AddScoped<WindowsGrep>()
                        .AddScoped<GrepService>()
                        .AddScoped<NativeService>();
            });
        }
        #endregion Methods..
    }
}
