using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGrep.Common
{
    public class EnumUtils
    {
        #region Methods..
        /// <summary>
        /// Return all enum values of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
        #endregion Methods..
    }
}
