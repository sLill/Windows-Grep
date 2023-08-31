namespace WindowsGrep.Core
{
    public class GrepCommand
    {
        #region Properties..
        public GrepCommandType CommandType { get; private set; }

        public IDictionary<ConsoleFlag, string> CommandArgs { get; set; } = new Dictionary<ConsoleFlag, string>();
        #endregion Properties..

        #region Constructors..
        public GrepCommand(GrepCommandType commandType)
        {
            CommandType = commandType;
        }
        #endregion Constructors..
    }
}
