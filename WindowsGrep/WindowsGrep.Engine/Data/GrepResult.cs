using System;
using System.Collections.Generic;
using System.IO;
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

            switch(Scope)
            {
                case ResultScope.FileContent:
                    BuildFileContentConsoleItemCollection(consoleItemCollection);
                    break;

                case ResultScope.FileName:
                    BuildFileNameConsoleItemCollection(consoleItemCollection);
                    break;

                case ResultScope.FileHash:
                    BuildFileHashConsoleItemCollection(consoleItemCollection);
                    break;
            }

            // Empty buffer
            consoleItemCollection.Add(new ConsoleItem() { Value = Environment.NewLine });
            return consoleItemCollection;
        }

        private void BuildFileContentConsoleItemCollection(List<ConsoleItem> consoleItemCollection)
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

        private void BuildFileNameConsoleItemCollection(List<ConsoleItem> consoleItemCollection)
        {
            // Context start
            consoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.DarkYellow, Value = LeadingContextString });

            // Context matched
            consoleItemCollection.Add(new ConsoleItem() { BackgroundColor = ConsoleColor.DarkCyan, Value = MatchedString });

            // Context end
            consoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.DarkYellow, Value = TrailingContextString });

            // File attributes
            consoleItemCollection.AddRange(GetFileAttributeConsoleItems());

            // FileSize
            consoleItemCollection.AddRange(GetFileSizeConsoleItems());
        }

        private void BuildFileHashConsoleItemCollection(List<ConsoleItem> consoleItemCollection)
        {
            // FileName
            consoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.DarkYellow, Value = $"{SourceFile} " });

            // File attributes
            consoleItemCollection.AddRange(GetFileAttributeConsoleItems());

            // FileSize
            consoleItemCollection.AddRange(GetFileSizeConsoleItems());

            // Context matched
            consoleItemCollection.Add(new ConsoleItem() { BackgroundColor = ConsoleColor.DarkCyan, Value = MatchedString });
        }

        private List<ConsoleItem> GetFileAttributeConsoleItems()
        {
            List<ConsoleItem> consoleItemCollection = new List<ConsoleItem>();

            var fileAttributes = File.GetAttributes(SourceFile);
            if ((fileAttributes & FileAttributes.System) == FileAttributes.System)
                consoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = $" [System]" });
            if ((fileAttributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                consoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.White, Value = $" [Hidden]" });

            return consoleItemCollection;
        }

        private List<ConsoleItem> GetFileSizeConsoleItems()
        {
            List<ConsoleItem> consoleItemCollection = new List<ConsoleItem>();

            if (FileSize > -1)
            {
                var fileSizeReduced = WindowsUtils.GetReducedSize(FileSize, 3, out FileSizeType fileSizeType);
                consoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.Green, Value = $"{fileSizeReduced} {fileSizeType}(s) " });
            }

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
