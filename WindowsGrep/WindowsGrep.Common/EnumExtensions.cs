using System;
using System.Linq;

namespace WindowsGrep.Common
{
    public static class EnumExtensions
    {
        #region Methods..
        public static TAttribute GetCustomAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
        {
            var EnumType = value.GetType();
            var Name = Enum.GetName(value.GetType(), value);
            return EnumType.GetField(Name).GetCustomAttributes(false).OfType<TAttribute>().SingleOrDefault();
        }
        #endregion Methods..
    }
}
