namespace WindowsGrep.Core;

public class ConsoleService
{
    #region Fields..
    private readonly ILogger<ConsoleService> _logger;
    #endregion Fields..

    #region Properties..
    #endregion Properties..

    #region Constructors..
    public ConsoleService(ILogger<ConsoleService> logger)
    {
        _logger = logger;
    }
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
