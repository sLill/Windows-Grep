using System;
using System.Collections.Generic;
using System.Text;

namespace WindowsGrep.Common
{
    public class KeyModifierAttribute : Attribute
    {
        #region Properties..
        public KeyModifier Value { get; private set; }
        #endregion Properties..

        #region Constructors..
        public KeyModifierAttribute(KeyModifier value)
        {
            this.Value = value;
        }
        #endregion Constructors..
    }
}
