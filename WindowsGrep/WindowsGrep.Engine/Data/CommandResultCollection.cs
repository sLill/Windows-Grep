using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WindowsGrep.Common;
using WindowsGrep.Core;

namespace WindowsGrep.Engine
{
    public class CommandResultCollection : ThreadSafeCollection<CommandResultBase>
    {
        #region Events..
        public event EventHandler ItemsAdded;
        #endregion Events..

        #region Methods..
        public override void AddItem(CommandResultBase item)
        {
            base.AddItem(item);
            ItemsAdded?.Invoke(new List<CommandResultBase>() { item }, EventArgs.Empty);
        }

        public override void AddItemRange(List<CommandResultBase> itemCollection)
        {
            base.AddItemRange(itemCollection);
            ItemsAdded?.Invoke(itemCollection, EventArgs.Empty);
        }

        public void Write(string fileName)
        {
            try
            {
                var fileFormat = FileFormat.PlainText;

                string extension = Path.GetExtension(fileName);
                switch (extension.ToUpper())
                {
                    case ".CSV":
                    case ".XLSX":
                    case ".XLS":
                        fileFormat = FileFormat.CommaSeparatedValues;
                        break;
                    default:
                        fileFormat = FileFormat.PlainText;
                        break;
                }

                char separatorCharacter = fileFormat == FileFormat.CommaSeparatedValues ? ',' : ' ';
                string fileContent = string.Join(Environment.NewLine, this.OrderBy(x => x.SourceFile).Select(x => x.ToString(separatorCharacter)));
                File.WriteAllText(fileName, fileContent);
            }
            catch (Exception ex)
            {
                string writeException = $"[Error writing to file: {ex.Message}]";
                ConsoleUtils.WriteConsoleItem(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = writeException });
            }
        }
        #endregion Methods..
    }
}
