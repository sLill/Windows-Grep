using System;
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
                        GrepEngine.RunCommand(Command);
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
        #endregion Methods..
    }
}
