namespace WindowsGrep.Core;

public class ConsoleItem
{
    #region Properties..
    public ConsoleColor BackgroundColor { get; set; } = Console.BackgroundColor;

    public ConsoleColor ForegroundColor { get; set; } = Console.ForegroundColor;

    public string Value { get; set; }
    #endregion Properties..
}
