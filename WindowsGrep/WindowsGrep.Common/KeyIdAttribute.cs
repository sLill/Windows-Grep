using System;
using System.Collections.Generic;
using System.Text;

namespace WindowsGrep.Common
{
    public class KeyIdAttribute : Attribute
    {
        #region Properties..
        public int Value { get; private set; }
        #endregion Properties..

        #region Constructors..
        public KeyIdAttribute(int value)
        {
            this.Value = value;
        }
        #endregion Constructors..
    }
}
