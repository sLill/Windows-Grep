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
            while (true)
            {
                try
                {
                    string Command = Console.ReadLine();
                    string CommandResult = ProcessCommand(Command);

                    Console.WriteLine(CommandResult);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());                
                }
            }
        }
        #endregion Main

        #region Methods..
        #region ProcessCommand
        private static string ProcessCommand(string commandRaw)
        {
            string Result = string.Empty;

            string[] commandCollection = commandRaw.Split('|');
            foreach (string command in commandCollection)
            {
                var CommandArgs = ConsoleUtils.DiscoverCommandArgs(command);
                ConsoleCommand ConsoleCommand = new ConsoleCommand() { CommandArgs = CommandArgs };
                Result = GrepEngine.ProcessGrepCommand(ConsoleCommand);
            }

            return Result;
        }
        #endregion ProcessCommand
        #endregion Methods..
    }
}
