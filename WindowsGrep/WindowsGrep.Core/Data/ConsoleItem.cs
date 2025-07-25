﻿namespace WindowsGrep.Core;

public class ConsoleItem
{
    #region Properties..
    public string BackgroundColor { get; set; } = AnsiColors.BlackBg;
    public string ForegroundColor { get; set; } = AnsiColors.Gray;
    public string Value { get; set; } = string.Empty;
    #endregion Properties..

    #region Methods..
    public override string ToString()
        => $"{BackgroundColor}{ForegroundColor}{Value}{AnsiColors.Reset}";
    #endregion Methods..
}
