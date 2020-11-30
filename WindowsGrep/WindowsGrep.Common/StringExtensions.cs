namespace WindowsGrep.Common
{
    public static class StringExtensions
    {
        #region Methods..
        #region EqualsIgnoreCase
        public static bool EqualsIgnoreCase(this string str1, string str2)
            => str1.ToLower() == str2.ToLower();
        #endregion EqualsIgnoreCase


        #region IsNullOrEmpty
        public static bool IsNullOrEmpty(this string value)
            => value == null || value == string.Empty;
        #endregion IsNullOrEmpty
        #endregion Methods..
    }
}
