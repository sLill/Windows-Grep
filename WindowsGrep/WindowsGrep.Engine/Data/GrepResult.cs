using System;
using System.Collections.Generic;
using WindowsGrep.Common;

namespace WindowsGrep.Engine
{
    public class GrepResult
    {
        #region Properties..
        #region Suppressed
        public bool Suppressed { get; set; }
        #endregion Suppressed

        #region FileSize
        public long FileSize { get; set; }
        #endregion FileSize

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
        public GrepResult(string sourceFile, ResultScope resultScope)
        {
            Scope = resultScope;
            SourceFile = sourceFile;
        }
        #endregion Constructors..

        #region Methods..
        #region ToConsoleItemCollection
        public List<ConsoleItem> ToConsoleItemCollection()
        {
            List<ConsoleItem> consoleItemCollection = new List<ConsoleItem>();

            if (Scope == ResultScope.FileContent)
            {
                // FileName
                consoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.DarkYellow, Value = $"{SourceFile} " });

                // FileSize
                if (FileSize > -1)
                {
                    var fileSizeReduced = WindowsUtils.GetReducedSize(FileSize, 3, out FileSizeType fileSizeType);
                    consoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.Green, Value = $"{fileSizeReduced} {fileSizeType}(s) " });
                }

                // Line number
                if (LineNumber > -1)
                {
                    consoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.DarkMagenta, Value = $"Line {LineNumber}  " });
                }

                // Context start
                consoleItemCollection.Add(new ConsoleItem() { Value = LeadingContextString });

                // Context matched
                consoleItemCollection.Add(new ConsoleItem() { BackgroundColor = ConsoleColor.DarkCyan, Value = MatchedString });

                // Context end
                consoleItemCollection.Add(new ConsoleItem() { Value = TrailingContextString });
            }
            else
            {
                // Context start
                consoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.DarkYellow, Value = LeadingContextString });

                // Context matched
                consoleItemCollection.Add(new ConsoleItem() { BackgroundColor = ConsoleColor.DarkCyan, Value = MatchedString });

                // Context end
                consoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.DarkYellow, Value = TrailingContextString });

                // FileSize
                if (FileSize > -1)
                {
                    var fileSizeReduced = WindowsUtils.GetReducedSize(FileSize, 3, out FileSizeType fileSizeType);
                    consoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = $" {fileSizeReduced} {fileSizeType}(s)" });
                }
            }

            // Empty buffer
            consoleItemCollection.Add(new ConsoleItem() { Value = Environment.NewLine });

            return consoleItemCollection;
        }
        #endregion ToConsoleItemCollection

        #region ToString
        public string ToString(char separator)
        {
            string result = string.Empty;

            string lineNumberString = LineNumber > -1 ? $"Line {LineNumber}" : string.Empty;

            string fileSizeString = string.Empty;
            if (FileSize > -1)
            {
                var fileSizeReduced = WindowsUtils.GetReducedSize(FileSize, 3, out FileSizeType fileSizeType);
                fileSizeString = $"{fileSizeReduced} {fileSizeType}(s){separator}";
            }

            result = Scope == ResultScope.FileName ? SourceFile : $"{SourceFile}{separator}{fileSizeString}{lineNumberString}{separator}{LeadingContextString}{MatchedString}{TrailingContextString}";
            return result;
        } 
        #endregion ToString
        #endregion Methods..
    }
}
