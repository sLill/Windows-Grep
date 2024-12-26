namespace WindowsGrep.Common;

/// <summary>
/// Members of this collection are used to sanitize user arguments
/// </summary>
public class FilterCharacterCollectionAttribute : Attribute
{
    #region Properties..
    public char[] Value { get; private set; }
    #endregion Properties..

    #region Constructors..
    public FilterCharacterCollectionAttribute(params char[] filterchars)
    {
        this.Value = filterchars;
    }
    #endregion Constructors..
}
