using System;
using System.Collections.Generic;
using System.Text;

namespace WindowsGrep.Common
{
    public static class StringExtensions
    {
        #region Member Variables..
        #endregion Member Variables..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        #endregion Constructors..

        #region Methods..
        #region EqualsIgnoreCase
        public static bool EqualsIgnoreCase(this string str1, string str2)
            => str1.ToLower() == str2.ToLower();
        #endregion EqualsIgnoreCase
        #endregion Methods..
    }
}
