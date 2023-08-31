using System;
using System.Collections.Generic;
using WindowsGrep.Common;
using WindowsGrep.Core;

namespace WindowsGrep.Engine
{
    public class GrepResult
    {
        #region Properties..
        public bool Suppressed { get; set; }

        public long FileSize { get; set; }

        public string LeadingContextString { get; set; }

        public int LineNumber { get; set; } = -1;

        public string MatchedString { get; set; }

        public ResultScope Scope { get; set; }

        public string SourceFile { get; set; }

        public string TrailingContextString { get; set; }
        #endregion Properties..

        #region Constructors..
        public GrepResult(string sourceFile, ResultScope resultScope)
        {
            Scope = resultScope;
            SourceFile = sourceFile;
        }
        #endregion Constructors..

        #region Methods..
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
            else if (Scope == ResultScope.FileName)
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
            else if (Scope == ResultScope.FileHash)
            {
                // FileName
                consoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.DarkYellow, Value = $"{SourceFile} " });

                // FileSize
                if (FileSize > -1)
                {
                    var fileSizeReduced = WindowsUtils.GetReducedSize(FileSize, 3, out FileSizeType fileSizeType);
                    consoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.Green, Value = $"{fileSizeReduced} {fileSizeType}(s) " });
                }

                // Context matched
                consoleItemCollection.Add(new ConsoleItem() { BackgroundColor = ConsoleColor.DarkCyan, Value = MatchedString });
            }

            // Empty buffer
            consoleItemCollection.Add(new ConsoleItem() { Value = Environment.NewLine });
            return consoleItemCollection;
        }

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

            result = (Scope) switch
            {
                ResultScope.FileContent => $"{SourceFile}{separator}{fileSizeString}{lineNumberString}{separator}{LeadingContextString}{MatchedString}{TrailingContextString}",
                ResultScope.FileName => SourceFile,
                ResultScope.FileHash => $"{SourceFile}{separator}{MatchedString}"
            };

            return result;
        }
        #endregion Methods..
    }
}
