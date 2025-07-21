namespace WindowsGrep.Core;

public class ConsoleService
{
    #region Constructors..
    public ConsoleService() { }
    #endregion Constructors..

    #region Methods..		
    public void Write(ConsoleItem consoleItem)
    {
        lock (Console.Out)
            Console.Write(consoleItem.ToString());
    }
    #endregion Methods..
}
