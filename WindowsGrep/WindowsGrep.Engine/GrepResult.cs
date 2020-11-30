using System;
using System.Collections.Generic;
using WindowsGrep.Common;

namespace WindowsGrep.Engine
{
    public class GrepResult
    {
        #region Properties..
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

                // FileSize
                if (FileSize > -1)
                {
                    var FileSizeReduced = WindowsUtils.GetReducedSize(FileSize, 3, out FileSizeType fileSizeType);
                    ConsoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.Green, Value = $"{FileSizeReduced} {fileSizeType}(s) " });
                }

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

                // FileSize
                if (FileSize > -1)
                {
                    var FileSizeReduced = WindowsUtils.GetReducedSize(FileSize, 3, out FileSizeType fileSizeType);
                    ConsoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = $" {FileSizeReduced} {fileSizeType}(s)" });
                }
            }

            // Empty buffer
            ConsoleItemCollection.Add(new ConsoleItem() { Value = Environment.NewLine });

            return ConsoleItemCollection;
        }
        #endregion ToConsoleItemCollection

        public string ToString(char separator)
        {
            string Result = string.Empty;

            string LineNumberString = LineNumber > -1 ? $"Line {LineNumber}" : string.Empty;

            string FileSizeString = string.Empty;
            if (FileSize > -1)
            {
                var FileSizeReduced = WindowsUtils.GetReducedSize(FileSize, 3, out FileSizeType fileSizeType);
                FileSizeString = $"{FileSizeReduced} {fileSizeType}(s){separator}";
            }

            Result = Scope == ResultScope.FileName ? SourceFile : $"{SourceFile}{separator}{FileSizeString}{LineNumberString}{separator}{LeadingContextString}";
            return Result;
        }
        #endregion Methods..
    }
}
