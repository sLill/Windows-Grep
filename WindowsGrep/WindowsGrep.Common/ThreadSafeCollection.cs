using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

namespace WindowsGrep.Common
{
    public class ThreadSafeCollection<T> : List<T>
    {
        #region Member Variables..
        private object _LockObject = new object();
        #endregion Member Variables..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        #region ThreadSafeCollection
        public ThreadSafeCollection() { }
        #endregion ThreadSafeCollection

        #region ThreadSafeCollection
        public ThreadSafeCollection(IEnumerable<T> collection) 
           : base(collection) { }
        #endregion ThreadSafeCollection
        #endregion Constructors..

        #region Methods..
        #region AddItem
        public virtual void AddItem(T item)
        {
            lock(_LockObject)
            {
                this.Add(item);
            }
        }
        #endregion AddItem
        #endregion Methods..    
    }
}
