using System;
using System.Collections.Generic;
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
        public override void AddItem(GrepResult item)
        {
            base.AddItem(item);
            ItemAdded?.Invoke(item, EventArgs.Empty);
        }
        #endregion Methods..
    }
}
