namespace WindowsGrep.Common;

public static class AnsiColors
{
    #region Properties..
    // Reset
    public const string Reset = "\u001b[0m";

    // Standard 16 Colors (Foreground)
    public const string Black = "\u001b[30m";
    public const string DarkRed = "\u001b[31m";
    public const string DarkGreen = "\u001b[32m";
    public const string DarkYellow = "\u001b[33m";
    public const string DarkBlue = "\u001b[34m";
    public const string DarkMagenta = "\u001b[35m";
    public const string DarkCyan = "\u001b[36m";
    public const string Gray = "\u001b[37m";

    // Bright Colors
    public const string DarkGray = "\u001b[90m";
    public const string Red = "\u001b[91m";
    public const string Green = "\u001b[92m";
    public const string Yellow = "\u001b[93m";
    public const string Blue = "\u001b[94m";
    public const string Magenta = "\u001b[95m";
    public const string Cyan = "\u001b[96m";
    public const string White = "\u001b[97m";

    // Background Colors (Standard)
    public const string BlackBg = "\u001b[40m";
    public const string DarkRedBg = "\u001b[41m";
    public const string DarkGreenBg = "\u001b[42m";
    public const string DarkYellowBg = "\u001b[43m";
    public const string DarkBlueBg = "\u001b[44m";
    public const string DarkMagentaBg = "\u001b[45m";
    public const string DarkCyanBg = "\u001b[46m";
    public const string GrayBg = "\u001b[47m";

    // Background Colors (Bright)
    public const string DarkGrayBg = "\u001b[100m";
    public const string RedBg = "\u001b[101m";
    public const string GreenBg = "\u001b[102m";
    public const string YellowBg = "\u001b[103m";
    public const string BlueBg = "\u001b[104m";
    public const string MagentaBg = "\u001b[105m";
    public const string CyanBg = "\u001b[106m";
    public const string WhiteBg = "\u001b[107m";

    // Common RGB Colors (24-bit)
    public const string Orange = "\u001b[38;2;255;165;0m";
    public const string Pink = "\u001b[38;2;255;192;203m";
    public const string Purple = "\u001b[38;2;128;0;128m";
    public const string Violet = "\u001b[38;2;238;130;238m";
    public const string Indigo = "\u001b[38;2;75;0;130m";
    public const string Brown = "\u001b[38;2;165;42;42m";
    public const string Lime = "\u001b[38;2;0;255;0m";
    public const string Olive = "\u001b[38;2;128;128;0m";
    public const string Navy = "\u001b[38;2;0;0;128m";
    public const string Teal = "\u001b[38;2;0;128;128m";
    public const string Silver = "\u001b[38;2;192;192;192m";
    public const string Maroon = "\u001b[38;2;128;0;0m";
    public const string Aqua = "\u001b[38;2;0;255;255m";
    public const string Fuchsia = "\u001b[38;2;255;0;255m";
    public const string Gold = "\u001b[38;2;255;215;0m";
    public const string Crimson = "\u001b[38;2;220;20;60m";
    public const string Coral = "\u001b[38;2;255;127;80m";
    public const string Salmon = "\u001b[38;2;250;128;114m";
    public const string Khaki = "\u001b[38;2;240;230;140m";
    public const string Lavender = "\u001b[38;2;230;230;250m";
    public const string Plum = "\u001b[38;2;221;160;221m";
    public const string Turquoise = "\u001b[38;2;64;224;208m";
    public const string Tan = "\u001b[38;2;210;180;140m";
    public const string SkyBlue = "\u001b[38;2;135;206;235m";
    public const string LightGreen = "\u001b[38;2;144;238;144m";
    public const string LightBlue = "\u001b[38;2;173;216;230m";
    public const string LightPink = "\u001b[38;2;255;182;193m";
    public const string LightYellow = "\u001b[38;2;255;255;224m";
    public const string LightGray = "\u001b[38;2;211;211;211m";
    public const string DarkOrange = "\u001b[38;2;255;140;0m";
    public const string DeepPink = "\u001b[38;2;255;20;147m";
    public const string ForestGreen = "\u001b[38;2;34;139;34m";
    public const string RoyalBlue = "\u001b[38;2;65;105;225m";

    // 256-Color Palette (Popular Colors)
    public const string Orange256 = "\u001b[38;5;208m";
    public const string Pink256 = "\u001b[38;5;213m";
    public const string Purple256 = "\u001b[38;5;129m";
    public const string Brown256 = "\u001b[38;5;130m";
    public const string Lime256 = "\u001b[38;5;118m";
    public const string Navy256 = "\u001b[38;5;17m";
    public const string Teal256 = "\u001b[38;5;31m";
    public const string Gold256 = "\u001b[38;5;220m";
    public const string Coral256 = "\u001b[38;5;209m";
    public const string Lavender256 = "\u001b[38;5;183m";

    // Text Formatting
    public const string Bold = "\u001b[1m";
    public const string Dim = "\u001b[2m";
    public const string Italic = "\u001b[3m";
    public const string Underline = "\u001b[4m";
    public const string Blink = "\u001b[5m";
    public const string Reverse = "\u001b[7m";
    public const string Strikethrough = "\u001b[9m";

    // Reset Formatting
    public const string ResetBold = "\u001b[22m";
    public const string ResetDim = "\u001b[22m";
    public const string ResetItalic = "\u001b[23m";
    public const string ResetUnderline = "\u001b[24m";
    public const string ResetBlink = "\u001b[25m";
    public const string ResetReverse = "\u001b[27m";
    public const string ResetStrikethrough = "\u001b[29m";
    #endregion Properties..

    #region Methods..
    public static string Rgb(int r, int g, int b) => $"\u001b[38;2;{r};{g};{b}m";
    public static string RgbBg(int r, int g, int b) => $"\u001b[48;2;{r};{g};{b}m";
    public static string Color256(int colorCode) => $"\u001b[38;5;{colorCode}m";
    public static string ColorBg256(int colorCode) => $"\u001b[48;5;{colorCode}m";

    public static string Colorize(string text, string color) => $"{color}{text}{Reset}";
    public static string ColorizeRgb(string text, int r, int g, int b) => $"{Rgb(r, g, b)}{text}{Reset}";
    public static string Colorize256(string text, int colorCode) => $"{Color256(colorCode)}{text}{Reset}";
    #endregion Methods..
}
