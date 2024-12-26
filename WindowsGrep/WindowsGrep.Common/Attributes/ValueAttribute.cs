namespace WindowsGrep.Common
{
    public class ValueAttribute : Attribute
    {
        #region Properties..
        public long Value { get; private set; }
        #endregion Properties..

        #region Constructors..
        public ValueAttribute(long value)
        {
            this.Value = value;
        }
        #endregion Constructors..
    }
}
