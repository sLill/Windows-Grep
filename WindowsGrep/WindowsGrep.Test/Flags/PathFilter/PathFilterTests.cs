public class PathFilterTests : TestBase
{
    #region Fields..
    private string _flagDescriptorShort;
    private string _flagDescriptorLong;
    private string _testDataRelativePath = @"Flags\PathFilter\TestData";
    #endregion Fields..

    #region Properties..
    #endregion Properties..

    #region Setup
    [SetUp]
    public void Setup()
    {
        TestDataDirectory = Path.Combine(TestConfigurationManager.WorkingDirectory, _testDataRelativePath);

        List<string> DescriptionCollection = ConsoleFlag.FileTypeFilter.GetCustomAttribute<DescriptionCollectionAttribute>()?.Value.OrderBy(x => x.Length).ToList();
        _flagDescriptorShort = DescriptionCollection[0];
        _flagDescriptorLong = DescriptionCollection[1];
    }
    #endregion Setup

    #region Tests..
    #endregion Tests..
}