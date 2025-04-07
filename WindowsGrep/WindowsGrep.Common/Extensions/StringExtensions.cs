namespace WindowsGrep.Common;

public static class StringExtensions
{
    #region Methods..
    public static bool EqualsIgnoreCase(this string str1, string str2)
        => str1.ToLower() == str2.ToLower();

    public static bool IsNullOrEmpty(this string value)
        => value == null || value == string.Empty;
    #endregion Methods..
}
