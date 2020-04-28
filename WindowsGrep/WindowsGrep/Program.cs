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
                    string CommandResult = GrepEngine.ProcessCommand(Command);

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
        #endregion Methods..
    }
}
