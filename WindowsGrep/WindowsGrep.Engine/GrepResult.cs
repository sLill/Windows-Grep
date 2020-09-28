using System;
using System.Collections.Generic;
using System.Text;
using WindowsGrep.Common;

namespace WindowsGrep.Engine
{
    public class GrepResult
    {
        #region Properties..
        #region ContextString
        public string ContextString { get; set; }
        #endregion ContextString

        #region ContextStringStartIndex
        public int ContextStringStartIndex { get; set; }
        #endregion ContextStContextStringStartIndexring

        #region LineNumber
        public int LineNumber { get; set; } = -1;
        #endregion LineNumber

        #region MatchedString
        public string MatchedString { get; set; }
        #endregion MatchedString

        #region Scope
        public ResultScope Scope { get; set; }
        #endregion Scope

        #region SourceFile
        public string SourceFile { get; set; }
        #endregion SourceFile
        #endregion Properties..

        #region Constructors..
        #region GrepResult
        public GrepResult(string sourceFile, ResultScope resultScope)
        {
            Scope = resultScope;
            SourceFile = sourceFile;
        }
        #endregion GrepResult
        #endregion Constructors..

        #region Methods..
        #region ToConsoleItemCollection
        public List<ConsoleItem> ToConsoleItemCollection()
        {
            List<ConsoleItem> ConsoleItemCollection = new List<ConsoleItem>();

            if (Scope == ResultScope.FileContent)
            {
                // FileName
                ConsoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.DarkYellow, Value = $"{SourceFile} " });

                // Line number
                if (LineNumber > -1)
                {
                    ConsoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.DarkMagenta, Value = $"Line {LineNumber}  " });
                }

                // Context start
                ConsoleItemCollection.Add(new ConsoleItem() { Value = ContextString.Substring(0, ContextStringStartIndex) });

                // Context matched
                ConsoleItemCollection.Add(new ConsoleItem() { BackgroundColor = ConsoleColor.DarkCyan, Value = ContextString.Substring(ContextStringStartIndex, MatchedString.Length) });

                // Context end
                int ContextMatchEndIndex = ContextStringStartIndex + MatchedString.Length;
                ConsoleItemCollection.Add(new ConsoleItem() { Value = ContextString.Substring(ContextMatchEndIndex, ContextString.Length - ContextMatchEndIndex) });
            }
            else
            {
                // Context start
                ConsoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.DarkYellow, Value = ContextString.Substring(0, ContextStringStartIndex) });

                // Context matched
                ConsoleItemCollection.Add(new ConsoleItem() { BackgroundColor = ConsoleColor.DarkCyan, Value = MatchedString });

                // Context end
                int ContextMatchEndIndex = ContextStringStartIndex + MatchedString.Length;
                ConsoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.DarkYellow, Value = ContextString.Substring(ContextMatchEndIndex, ContextString.Length - ContextMatchEndIndex) });
            }

            // Empty buffer
            ConsoleItemCollection.Add(new ConsoleItem() { Value = Environment.NewLine });

            return ConsoleItemCollection;
        }
        #endregion ToConsoleItemCollection

        public string ToString(char separator)
        {
            string Result = string.Empty;

            string LineNumberString = string.Empty;
            if (LineNumber > -1)
            {
                LineNumberString = $"Line {LineNumber}";
            }

            Result = Scope == ResultScope.FileName ? SourceFile : $"{SourceFile}{separator}{LineNumberString}{separator}{ContextString}";
            return Result;
        }
        #endregion Methods..
    }
}
