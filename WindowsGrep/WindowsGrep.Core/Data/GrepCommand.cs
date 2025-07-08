namespace WindowsGrep.Core;

public class GrepCommand
{
    #region Properties..
    public IDictionary<ConsoleFlag, string> CommandArgs { get; set; } = new Dictionary<ConsoleFlag, string>();
    #endregion Properties..

    #region Constructors..
    public GrepCommand() { }
    #endregion Constructors..
}
