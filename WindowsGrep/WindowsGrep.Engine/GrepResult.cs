using System;
using System.Collections.Generic;
using System.Text;

namespace WindowsGrep.Engine
{
    public class GrepResult
    {
        #region Properties..
        #region SourceFile
        public string SourceFile { get; set; }
        #endregion SourceFile

        #region Text
        public string Text { get; set; }
        #endregion Text
        #endregion Properties..
    }
}
