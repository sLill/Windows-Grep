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
                string ReadMe = Properties.Resources.ReadMe;
                Console.WriteLine(ReadMe + Environment.NewLine);
            }

            do
            {
                string Command = args.Length == 0 ? Console.ReadLine() : string.Join(" ", args);
                if (Command.Length > 0)
                {
                    try
                    {
                        GrepResultCollection grepResultCollection = new GrepResultCollection();
                        grepResultCollection.ItemsAdded += OnResultsAdded;

                        GrepEngine.RunCommand(Command, grepResultCollection);
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
            var GrepResults = sender as List<GrepResult>;
            GrepResults.ForEach(result =>
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
