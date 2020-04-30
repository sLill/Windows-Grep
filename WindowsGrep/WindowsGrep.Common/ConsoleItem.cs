using System;
using System.Collections.Generic;
using System.Text;

namespace WindowsGrep.Common
{
    public class ConsoleItem
    {
        #region Properties..
        #region BackgroundColor
        public ConsoleColor BackgroundColor { get; set; } = Console.BackgroundColor;
        #endregion BackgroundColor

        #region ForegroundColor
        public ConsoleColor ForegroundColor { get; set; } = Console.ForegroundColor;
        #endregion ForegroundColor

        #region Value
        public string Value { get; set; }
        #endregion Value
        #endregion Properties..

        #region Constructors..
        #endregion Constructors..

        #region Methods..
        #endregion Methods..    
    }
}
