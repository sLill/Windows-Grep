using System;
using System.Collections.Generic;
using WindowsGrep.Common;

namespace WindowsGrep.Engine
{
    public class GrepResult
    {
        #region Properties..
        #region CaseSensitive
        public bool CaseSensitive { get; set; }
        #endregion CaseSensitive

        #region ContextString
        public string ContextString { get; set; }
        #endregion ContextString

        #region FileName
        public string FileName { get; set; }
        #endregion FileName

        #region LineNumber
        public int LineNumber { get; set; } = -1;
        #endregion LineNumber

        #region MatchedString
        public string MatchedString { get; set; }
        #endregion MatchedString

        #region SourceFile
        public string SourceFile { get; set; }
        #endregion SourceFile
        #endregion Properties..

        #region Constructors..
        #region GrepResult
        public GrepResult(string sourceFile)
        {
            SourceFile = sourceFile;
        }
        #endregion GrepResult
        #endregion Constructors..

        #region Methods..

        #region ToConsoleItemCollection
        public List<ConsoleItem> ToConsoleItemCollection()
        {
            List<ConsoleItem> ConsoleItemCollection = new List<ConsoleItem>();

            // FileName
            ConsoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.DarkYellow, Value = $"{FileName} " });

            // Line number
            if (LineNumber > -1)
            {
                ConsoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.DarkMagenta, Value = $"Line {LineNumber}  " });
            }

            int ContextMatchStartIndex = ContextString.IndexOf(MatchedString, CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
            int ContextMatchEndIndex = ContextMatchStartIndex + MatchedString.Length;

            // Context start
            ConsoleItemCollection.Add(new ConsoleItem() { Value = ContextString.Substring(0, ContextMatchStartIndex) });

            // Context matched
            ConsoleItemCollection.Add(new ConsoleItem() { BackgroundColor = ConsoleColor.DarkCyan, Value = ContextString.Substring(ContextMatchStartIndex, MatchedString.Length) });

            // Context end
            ConsoleItemCollection.Add(new ConsoleItem() { Value = ContextString.Substring(ContextMatchEndIndex, ContextString.Length - ContextMatchEndIndex) });

            // Empty buffer
            ConsoleItemCollection.Add(new ConsoleItem() { Value = Environment.NewLine + Environment.NewLine });

            return ConsoleItemCollection;
        }
        #endregion ToConsoleItemCollection
        #endregion Methods..
    }
}
