using System;

namespace WindowsGrep.Common
{
    /// <summary>
    /// When set to true, the associated property expects an additional user argument to follow
    /// </summary>
    public class ExpectsParameterAttribute : Attribute
    {
        #region Properties..
        public bool Value { get; private set; }
        #endregion Properties..

        #region Constructors..
        public ExpectsParameterAttribute(bool value)
        {
            this.Value = value;
        }
        #endregion Constructors..
    }
}
