using System;
using System.Collections.Generic;
using WindowsGrep.Core;

namespace WindowsGrep.Engine.Data
{
    public class NativeCommandResult : CommandResultBase
    {
        #region Fields..
        private NativeCommandType _commandType;
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public NativeCommandResult(string sourceFile, NativeCommandType nativeCommandType)
            : base(sourceFile)
        {
            _commandType = nativeCommandType;
        }
        #endregion Constructors..

        #region Methods..
        public override List<ConsoleItem> ToConsoleItemCollection()
        {
            List<ConsoleItem> consoleItemCollection = new List<ConsoleItem>();

            switch (_commandType)
            {
                case NativeCommandType.List:
                    BuildFileNameConsoleItemCollection(consoleItemCollection);
                    break;

                case NativeCommandType.ClearConsole:
                case NativeCommandType.ChangeDirectory:
                    break;
            }

            // Empty buffer
            consoleItemCollection.Add(new ConsoleItem() { Value = Environment.NewLine });
            return consoleItemCollection;
        }

        private void BuildFileNameConsoleItemCollection(List<ConsoleItem> consoleItemCollection)
        {
            // Filename
            consoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.DarkYellow, Value = SourceFile });

            // File attributes
            consoleItemCollection.AddRange(GetFileAttributeConsoleItems());

            // FileSize
            consoleItemCollection.AddRange(GetFileSizeConsoleItems());
        }
        #endregion Methods..
    }
}
