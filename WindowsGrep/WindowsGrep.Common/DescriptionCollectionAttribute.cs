using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace WindowsGrep.Common
{
    public class DescriptionCollectionAttribute : Attribute
    {
        #region Properties..
        public string[] Value { get; private set; }
        #endregion Properties..

        #region Constructors..
        public DescriptionCollectionAttribute(params string[] descriptions) 
        {
            this.Value = descriptions;
        }
        #endregion Constructors..
    }
}
