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
        {
            Console.BackgroundColor = consoleItem.BackgroundColor;
            Console.ForegroundColor = consoleItem.ForegroundColor;

            Console.Write(consoleItem.Value);
            Console.ResetColor();
        }
    }
    #endregion Methods..
}
