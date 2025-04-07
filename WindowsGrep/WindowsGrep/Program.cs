using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WindowsGrep.Configuration;
using WindowsGrep.Core;
using WindowsGrep.Engine;

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

            do
            {
                string command = string.Empty;
                if (args.Length == 0)
                {
                    ConsoleUtils.PublishPrompt();
                    command = Console.ReadLine();
                }
                else
                    command = string.Join(" ", args);

                if (command.Length > 0)
                {
                    try
                    {
                        var commandResultCollection = new CommandResultCollection();
                        commandResultCollection.ItemsAdded += OnResultsAdded;

                        _cancellationTokenSource = new CancellationTokenSource();
                        await Task.Run(() => WindowsGrep.RunCommand(command, commandResultCollection, _cancellationTokenSource.Token));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message + Environment.NewLine);
                    }
                }
            }
            while (args.Length == 0);
        }
        #endregion Main

        #region Methods..
        #region Event Handlers..
        private static void Console_OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            _cancellationTokenSource?.Cancel();
        }

        private static async void OnResultsAdded(object sender, EventArgs e)
        {
            var commandResultCollection = sender as List<CommandResultBase>;
            commandResultCollection.ForEach(result =>
            {
                if (_cancellationTokenSource.IsCancellationRequested)
                    return;

                if (!result.Suppressed)
                    ConsoleUtils.WriteConsoleItemCollection(result.ToConsoleItemCollection(), _cancellationTokenSource.Token);
            });
        }
        #endregion Event Handlers..

        private static void Initialize(string[] args)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            // Load config
            ConfigurationManager.Instance.Initialize();

            // Publish ReadMe
            if (args.Length == 0)
                ConsoleUtils.PublishReadMe();

            // Override the default behavior for the Ctrl+C shortcut if the application was not ran from the command line
            if (Environment.UserInteractive)
                Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_OnCancelKeyPress);
        }     
        #endregion Methods..
    }
}
