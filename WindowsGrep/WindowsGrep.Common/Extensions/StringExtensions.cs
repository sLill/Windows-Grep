namespace WindowsGrep.Common;

public static class StringExtensions
{
    #region Methods..
    public static bool EqualsIgnoreCase(this string str1, string str2)
        => str1.ToLower() == str2.ToLower();

    public static bool IsNullOrEmpty(this string value)
        => value == null || value == string.Empty;

    /// <summary>Trim at most one character from the start and end of the string</summary>
    public static string TrimOnce(this string value, char character)
        => TrimOnce(value, [character]);

    /// <summary>Trim at most one character from the start and end of the string</summary>
    public static string TrimOnce(this string value, params char[] characters)
    {
        int startIndex = 0;
        int endIndex = value.Length - 1;

        if (characters.Contains(value[value.Length - 1]))
            endIndex -= 1;

        if (characters.Contains(value[0]))
            startIndex = 1;

        return value.Substring(startIndex, endIndex - startIndex + 1);
    }
    #endregion Methods..
}
