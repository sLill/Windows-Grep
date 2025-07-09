namespace WindowsGrep.Core;

public class GrepCommand
{
    #region Properties..
    public IDictionary<CommandFlag, string> CommandArgs { get; set; } = new Dictionary<CommandFlag, string>();
    #endregion Properties..

    #region Constructors..
    public GrepCommand() { }
    #endregion Constructors..
}
