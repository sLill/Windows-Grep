using System.Collections.Generic;

namespace WindowsGrep.Common
{
    public class ConsoleCommand
    {
        #region Properties..
        public IDictionary<ConsoleFlag, string> CommandArgs { get; set; } = new Dictionary<ConsoleFlag, string>();
        #endregion Properties..
    }
}
