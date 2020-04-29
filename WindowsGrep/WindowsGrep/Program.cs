using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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

                try
                {
                    string CommandResult = GrepEngine.ProcessCommand(Command);
                    Console.WriteLine(CommandResult);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            while (args.Length == 0);
        }
        #endregion Main

        #region Methods..
        #endregion Methods..
    }
}
