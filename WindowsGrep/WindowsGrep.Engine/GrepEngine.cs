using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WindowsGrep.Common;

namespace WindowsGrep.Engine
{
    public class GrepEngine
    {
        #region Member Variables..
        #endregion Member Variables..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        #endregion Constructors..

        #region Methods..

        #region BeginSearch
        private static void BeginSearch(ConsoleCommand consoleCommand, ConcurrentDictionary<string, GrepResult> GrepResultCollection)
        {
            string InitialPath = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.File) ? consoleCommand.CommandArgs[ConsoleFlag.File] : Environment.CurrentDirectory;

            List<string> Files = null;
            if (GrepResultCollection.Any())
            {
                Files = GrepResultCollection.Keys.ToList();
            }
            else
            {
                Files = consoleCommand.CommandArgs.ContainsKey(ConsoleFlag.Recursive) ? Directory.GetFiles(InitialPath, "*", SearchOption.AllDirectories).ToList() : Directory.GetFiles(InitialPath, "*", SearchOption.TopDirectoryOnly).ToList();
            }

            Files.AsParallel().ForAll(file =>
            {
                string fileRaw = File.ReadAllText(file);

                //using (FileStream fileStream = File.OpenRead(file))
                //{
                //    int BufferSize = 4096;
                //    using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
                //    {
                //        string Line;
                //        while ((Line = streamReader.ReadLine()) != null)
                //        {

                //        }
                //    }
                //}
            });
        }
        #endregion BeginSearch

        #region ProcessCommand
        public static string ProcessCommand(string commandRaw)
        {
            string Result = string.Empty;

            ConcurrentDictionary<string, GrepResult> GrepResultCollection = new ConcurrentDictionary<string, GrepResult>();

            string[] commandCollection = commandRaw.Split('|');
            foreach (string command in commandCollection)
            {
                var CommandArgs = ConsoleUtils.DiscoverCommandArgs(command);
                ConsoleCommand ConsoleCommand = new ConsoleCommand() { CommandArgs = CommandArgs };

                BeginSearch(ConsoleCommand, GrepResultCollection);
            }

            return Result;
        }
        #endregion ProcessCommand
        #endregion Methods..
    }
}
