using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WindowsGrep.Common;
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
                string command = args.Length == 0 ? Console.ReadLine() : string.Join(" ", args);
                if (command.Length > 0)
                {
                    try
                    {
                        var grepResultCollection = new GrepResultCollection();
                        grepResultCollection.ItemsAdded += OnResultsAdded;

                        _cancellationTokenSource = new CancellationTokenSource();
                        await Task.Run(() => GrepEngine.RunCommandAsync(command, grepResultCollection, _cancellationTokenSource.Token));
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

        #region OnResultsAdded
        private static void OnResultsAdded(object sender, EventArgs e)
        {
            var grepResults = sender as List<GrepResult>;
            grepResults.ForEach(result =>
            {
                if (!result.Suppressed)
                    ConsoleUtils.WriteConsoleItemCollection(result.ToConsoleItemCollection());
            });
        }
        #endregion OnResultsAdded
        #endregion Event Handlers..

        private static void Initialize(string[] args)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            // Display ReadMe
            if (args.Length == 0)
            {
                string readMe = Properties.Resources.ReadMe;
                Console.WriteLine(readMe + Environment.NewLine);
            }

            // Override the default behavior for the Ctrl+C shortcut if the application was not ran from the command line
            if (Environment.UserInteractive)
                Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_OnCancelKeyPress);
        }
        #endregion Methods..
    }
}
