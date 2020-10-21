using System;
using System.Collections.Generic;
using System.Text;
using WindowsGrep.Common;

namespace WindowsGrep.Engine
{
    public class GrepResult
    {
        #region Properties..
        #region LeadingContextString
        public string LeadingContextString { get; set; }
        #endregion LeadingContextString

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

        #region TrailingContextString
        public string TrailingContextString { get; set; }
        #endregion TrailingContextString
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
                ConsoleItemCollection.Add(new ConsoleItem() { Value = LeadingContextString });

                // Context matched
                ConsoleItemCollection.Add(new ConsoleItem() { BackgroundColor = ConsoleColor.DarkCyan, Value = MatchedString });

                // Context end
                ConsoleItemCollection.Add(new ConsoleItem() { Value = TrailingContextString });
            }
            else
            {
                // Context start
                ConsoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.DarkYellow, Value = LeadingContextString });

                // Context matched
                ConsoleItemCollection.Add(new ConsoleItem() { BackgroundColor = ConsoleColor.DarkCyan, Value = MatchedString });

                // Context end
                ConsoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.DarkYellow, Value = TrailingContextString });
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

            Result = Scope == ResultScope.FileName ? SourceFile : $"{SourceFile}{separator}{LineNumberString}{separator}{LeadingContextString}";
            return Result;
        }
        #endregion Methods..
    }
}
