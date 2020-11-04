using System.Collections.Generic;

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
            lock (_LockObject)
            {
                this.Add(item);
            }
        }
        #endregion AddItem

        #region AddItemRange
        public virtual void AddItemRange(IEnumerable<T> item)
        {
            lock (_LockObject)
            {
                this.AddRange(item);
            }
        }
        #endregion AddItemRange
        #endregion Methods..    
    }
}
