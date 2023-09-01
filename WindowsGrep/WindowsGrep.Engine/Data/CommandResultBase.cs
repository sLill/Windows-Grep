using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WindowsGrep.Common;
using WindowsGrep.Core;

namespace WindowsGrep.Engine
{
    public abstract class CommandResultBase
    {
        #region Fields..
        #endregion Fields..

        #region Properties..
        public bool Suppressed { get; set; }

        public long FileSize { get; set; }

        public string SourceFile { get; set; }
        #endregion Properties..

        #region Constructors..
        public CommandResultBase(string sourceFile)
        {
            SourceFile = sourceFile;
        }
        #endregion Constructors..

        #region Methods..
        public abstract List<ConsoleItem> ToConsoleItemCollection();

        protected List<ConsoleItem> GetFileAttributeConsoleItems()
        {
            List<ConsoleItem> consoleItemCollection = new List<ConsoleItem>();

            var fileAttributes = File.GetAttributes(SourceFile);
            if ((fileAttributes & FileAttributes.System) == FileAttributes.System)
                consoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = $"[System]" });
            if ((fileAttributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                consoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.DarkCyan, Value = $"[Hidden]" });

            if (consoleItemCollection.Any())
                consoleItemCollection.Insert(0, new ConsoleItem() { Value = " " });

            return consoleItemCollection;
        }

        protected List<ConsoleItem> GetFileSizeConsoleItems()
        {
            List<ConsoleItem> consoleItemCollection = new List<ConsoleItem>();

            if (FileSize > -1)
            {
                var fileSizeReduced = WindowsUtils.GetReducedSize(FileSize, 3, out FileSizeType fileSizeType);
                consoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.Green, Value = $" {fileSizeReduced} {fileSizeType}(s) " });
            }

            return consoleItemCollection;
        }

        public virtual string ToString(char separator)
        {
            return string.Empty;
        }
        #endregion Methods..
    }
}
