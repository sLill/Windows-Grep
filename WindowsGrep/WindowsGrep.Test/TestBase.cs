namespace WindowsGrep.Test;

public class TestBase
{
    #region Fields..
    #endregion Fields..

    #region Properties..
    public string TestDataDirectory { get; protected set; }
    #endregion Properties..

    #region Constructors..
    public TestBase()
    {
        ConfigurationManager.Instance.LoadDefaultConfiguration();
    }
    #endregion Constructors..

    #region Methods..
    #endregion Methods..
}
