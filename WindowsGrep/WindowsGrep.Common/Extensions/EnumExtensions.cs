using System;
using System.Linq;

namespace WindowsGrep.Common
{
    public static class EnumExtensions
    {
        #region Methods..
        public static TAttribute GetCustomAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
        {
            var enumType = value.GetType();
            var name = Enum.GetName(value.GetType(), value);
            return enumType.GetField(name).GetCustomAttributes(false).OfType<TAttribute>().SingleOrDefault();
        }
        #endregion Methods..
    }
}
