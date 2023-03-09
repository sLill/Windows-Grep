using System.Collections.Generic;

namespace WindowsGrep.Common
{
    public class ThreadSafeCollection<T> : List<T>
    {
        #region Fields..
        private object _lockObject = new object();
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public ThreadSafeCollection() { }

        public ThreadSafeCollection(IEnumerable<T> collection)
           : base(collection) { }
        #endregion Constructors..

        #region Methods..
        #region AddItem
        public virtual void AddItem(T item)
        {
            lock (_lockObject)
            {
                this.Add(item);
            }
        }
        #endregion AddItem

        #region AddItemRange
        public virtual void AddItemRange(IEnumerable<T> item)
        {
            lock (_lockObject)
            {
                this.AddRange(item);
            }
        }
        #endregion AddItemRange
        #endregion Methods..    
    }
}
