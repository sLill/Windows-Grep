using System.Collections.Generic;

namespace WindowsGrep.Common
{
    public class ThreadSafeCollection<T> : List<T>
    {
        #region Fields..
        private object _lockObject = new object();
        #endregion Fields..

        #region Constructors..
        public ThreadSafeCollection() { }

        public ThreadSafeCollection(IEnumerable<T> collection)
           : base(collection) { }
        #endregion Constructors..

        #region Methods..
        public virtual void AddItem(T item)
        {
            lock (_lockObject)
            {
                this.Add(item);
            }
        }

        public virtual void AddItemRange(List<T> item)
        {
            lock (_lockObject)
            {
                this.AddRange(item);
            }
        }
        #endregion Methods..    
    }
}
