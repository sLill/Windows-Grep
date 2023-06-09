using System.Collections.Generic;

namespace WindowsGrep.Core
{
    public class ConsoleCommand
    {
        #region Properties..
        public CommandType CommandType { get; private set; }

        public IDictionary<ConsoleFlag, string> CommandArgs { get; set; } = new Dictionary<ConsoleFlag, string>();
        #endregion Properties..

        #region Constructors..
        public ConsoleCommand(CommandType commandType)
        {
            CommandType = commandType;
        }
        #endregion Constructors..
    }
}
