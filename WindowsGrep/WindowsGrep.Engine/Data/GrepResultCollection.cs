using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WindowsGrep.Common;

namespace WindowsGrep.Engine
{
    public class GrepResultCollection : ThreadSafeCollection<GrepResult>
    {
        #region Events..
        public event EventHandler ItemsAdded;
        #endregion Events..

        #region Methods..
        #region AddItem
        public override void AddItem(GrepResult item)
        {
            base.AddItem(item);
            ItemsAdded?.Invoke(new List<GrepResult>() { item }, EventArgs.Empty);
        }
        #endregion AddItem

        #region AddItemRange
        public override void AddItemRange(IEnumerable<GrepResult> itemCollection)
        {
            base.AddItemRange(itemCollection);
            ItemsAdded?.Invoke(itemCollection, EventArgs.Empty);
        }
        #endregion AddItemRange

        #region Write
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
        #endregion Write
        #endregion Methods..
    }
}
