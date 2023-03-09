using System;
using System.Collections.Generic;
using WindowsGrep.Common;
using WindowsGrep.Engine;

namespace WindowsGrep
{
    public class Program
    {
        #region Main
        static void Main(string[] args)
        {
            // ReadMe
            if (args.Length == 0)
            {
                string readMe = Properties.Resources.ReadMe;
                Console.WriteLine(readMe + Environment.NewLine);
            }

            do
            {
                string command = args.Length == 0 ? Console.ReadLine() : string.Join(" ", args);
                if (command.Length > 0)
                {
                    try
                    {
                        var grepResultCollection = new GrepResultCollection();
                        grepResultCollection.ItemsAdded += OnResultsAdded;

                        GrepEngine.RunCommand(command, grepResultCollection);
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
        #region OnResultsAdded
        private static void OnResultsAdded(object sender, EventArgs e)
        {
            var grepResults = sender as List<GrepResult>;
            grepResults.ForEach(result =>
            {
                if (!result.Suppressed)
                {
                    ConsoleUtils.WriteConsoleItemCollection(result.ToConsoleItemCollection());
                }
            });
        }
        #endregion OnResultsAdded
        #endregion Event Handlers..
        #endregion Methods..
    }
}
