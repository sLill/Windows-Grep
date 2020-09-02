using System;

namespace WindowsGrep.Common
{
    public class KeyAttribute : Attribute
    {
        #region Properties..
        public int Value { get; private set; }
        #endregion Properties..

        #region Constructors..
        public KeyAttribute(int value)
        {
            this.Value = value;
        }
        #endregion Constructors..
    }
}
