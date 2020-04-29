using System;
using System.Collections.Generic;
using System.Text;

namespace WindowsGrep.Engine
{
    public class GrepResult
    {
        #region Properties..
        #region MatchingText
        public string MatchingText { get; set; }
        #endregion MatchingText

        #region SourceFile
        public string SourceFile { get; set; }
        #endregion SourceFile
        #endregion Properties..

        #region Constructors..
        public GrepResult(string sourceFile)
        {
            SourceFile = sourceFile;
        }
        #endregion Constructors..
    }
}
