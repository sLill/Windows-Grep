using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WindowsGrep.Common;

namespace WindowsGrep.Engine
{
    public class GrepResultCollection : ThreadSafeCollection<GrepResult>
    {
        #region Member Variables..
        #endregion Member Variables..

        #region Properties..
        #endregion Properties..

        #region Events..
        public event EventHandler ItemAdded;
        #endregion Events..

        #region Constructors..
        #endregion Constructors..

        #region Methods..
        #region AddItem
        public override void AddItem(GrepResult item)
        {
            base.AddItem(item);
            ItemAdded?.Invoke(item, EventArgs.Empty);
        }
        #endregion AddItem

        #region Write
        public void Write(string fileName)
        {
            try
            {
                var fileFormat = FileFormat.PlainText;

                string Extension = Path.GetExtension(fileName);
                switch(Extension.ToUpper())
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

                char SeparatorCharacter = fileFormat == FileFormat.CommaSeparatedValues ? ',' : ' ';
                string fileContent = string.Join(Environment.NewLine, this.OrderBy(x => x.SourceFile).Select(x => x.ToString(SeparatorCharacter)));
                File.WriteAllText(fileName, fileContent);
            }
            catch (Exception ex)
            {
                string WriteException = $"[Error writing to file: {ex.Message}]";
                ConsoleUtils.WriteConsoleItem(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = WriteException });
            }
        }
        #endregion Write
        #endregion Methods..
    }
}
