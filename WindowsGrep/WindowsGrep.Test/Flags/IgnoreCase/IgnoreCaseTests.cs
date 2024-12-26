namespace WindowsGrep.Test;

public class IgnoreCaseTests : TestBase
{
    #region Fields..
    private string _flagDescriptorShort;
    private string _flagDescriptorLong;
    private string _testDataRelativePath = @"Flags\IgnoreCase\TestData";
    #endregion Fields..

    #region Properties..
    #endregion Properties..

    #region Setup
    [SetUp]
    public void Setup()
    {
        TestDataDirectory = Path.Combine(TestConfigurationManager.WorkingDirectory, _testDataRelativePath);

        List<string> DescriptionCollection = ConsoleFlag.IgnoreCase.GetCustomAttribute<DescriptionCollectionAttribute>()?.Value.OrderBy(x => x.Length).ToList();
        _flagDescriptorShort = DescriptionCollection[0];
        _flagDescriptorLong = DescriptionCollection[1];
    }
    #endregion Setup

    #region Tests..
    #region FlagFirst..
    #region IgnoreCase_FlagFirst_FlagShort_SingleResult
    [Test]
    public async Task IgnoreCase_FlagFirst_FlagShort_SingleResult()
    {
        string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreCase.txt");
        Assert.IsTrue(File.Exists(TestFilePath));

        string SearchTerm = "fox";
        string Command = $"{_flagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

        var commandResultCollection = new CommandResultCollection();
        await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

        Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
    }
    #endregion IgnoreCase_FlagFirst_FlagShort_SingleResult 

    #region IgnoreCase_FlagFirst_FlagShort_MultiResult
    [Test]
    public async Task IgnoreCase_FlagFirst_FlagShort_MultiResult()
    {
        string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreCase.txt");
        Assert.IsTrue(File.Exists(TestFilePath));

        string SearchTerm = "lazy";
        string Command = $"{_flagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

        var commandResultCollection = new CommandResultCollection();
        await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

        Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 3);
    }
    #endregion IgnoreCase_FlagFirst_FlagShort_MultiResult 

    #region IgnoreCase_FlagFirst_FlagLong_SingleResult
    [Test]
    public async Task IgnoreCase_FlagFirst_FlagLong_SingleResult()
    {
        string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreCase.txt");
        Assert.IsTrue(File.Exists(TestFilePath));

        string SearchTerm = "fox";
        string Command = $"{_flagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

        var commandResultCollection = new CommandResultCollection();
        await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

        Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
    }
    #endregion IgnoreCase_FlagFirst_FlagLong_SingleResult 

    #region IgnoreCase_FlagFirst_FlagLong_MultiResult
    [Test]
    public async Task IgnoreCase_FlagFirst_FlagLong_MultiResult()
    {
        string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreCase.txt");
        Assert.IsTrue(File.Exists(TestFilePath));

        string SearchTerm = "lazy";
        string Command = $"{_flagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

        var commandResultCollection = new CommandResultCollection();
        await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

        Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 3);
    }
    #endregion IgnoreCase_FlagFirst_FlagLong_MultiResult 
    #endregion FlagFirst..

    #region FlagMiddle..
    #region IgnoreCase_FlagMiddle_FlagShort_SingleResult
    [Test]
    public async Task IgnoreCase_FlagMiddle_FlagShort_SingleResult()
    {
        string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreCase.txt");
        Assert.IsTrue(File.Exists(TestFilePath));

        string SearchTerm = "fox";
        string Command = $"-f '{TestFilePath}' {_flagDescriptorShort} {SearchTerm}";

        var commandResultCollection = new CommandResultCollection();
        await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

        Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
    }
    #endregion IgnoreCase_FlagMiddle_FlagShort_SingleResult 

    #region IgnoreCase_FlagMiddle_FlagShort_MultiResult
    [Test]
    public async Task IgnoreCase_FlagMiddle_FlagShort_MultiResult()
    {
        string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreCase.txt");
        Assert.IsTrue(File.Exists(TestFilePath));

        string SearchTerm = "lazy";
        string Command = $"-f '{TestFilePath}' {_flagDescriptorShort} {SearchTerm}";

        var commandResultCollection = new CommandResultCollection();
        await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

        Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 3);
    }
    #endregion IgnoreCase_FlagMiddle_FlagShort_MultiResult 

    #region IgnoreCase_FlagMiddle_FlagLong_SingleResult
    [Test]
    public async Task IgnoreCase_FlagMiddle_FlagLong_SingleResult()
    {
        string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreCase.txt");
        Assert.IsTrue(File.Exists(TestFilePath));

        string SearchTerm = "fox";
        string Command = $"-f '{TestFilePath}' {_flagDescriptorLong} {SearchTerm}";

        var commandResultCollection = new CommandResultCollection();
        await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

        Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
    }
    #endregion IgnoreCase_FlagMiddle_FlagLong_SingleResult 

    #region IgnoreCase_FlagMiddle_FlagLong_MultiResult
    [Test]
    public async Task IgnoreCase_FlagMiddle_FlagLong_MultiResult()
    {
        string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreCase.txt");
        Assert.IsTrue(File.Exists(TestFilePath));

        string SearchTerm = "lazy";
        string Command = $"-f '{TestFilePath}' {_flagDescriptorLong} {SearchTerm}";

        var commandResultCollection = new CommandResultCollection();
        await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

        Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 3);
    }
    #endregion IgnoreCase_FlagMiddle_FlagLong_MultiResult 
    #endregion FlagMiddle..

    #region FlagLast..
    #region IgnoreCase_FlagLast_FlagShort_SingleResult
    [Test]
    public async Task IgnoreCase_FlagLast_FlagShort_SingleResult()
    {
        string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreCase.txt");
        Assert.IsTrue(File.Exists(TestFilePath));

        string SearchTerm = "fox";
        string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorShort}";

        var commandResultCollection = new CommandResultCollection();
        await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

        Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
    }
    #endregion IgnoreCase_FlagLast_FlagShort_SingleResult 

    #region IgnoreCase_FlagLast_FlagShort_MultiResult
    [Test]
    public async Task IgnoreCase_FlagLast_FlagShort_MultiResult()
    {
        string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreCase.txt");
        Assert.IsTrue(File.Exists(TestFilePath));

        string SearchTerm = "lazy";
        string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorShort}";

        var commandResultCollection = new CommandResultCollection();
        await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

        Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 3);
    }
    #endregion IgnoreCase_FlagLast_FlagShort_MultiResult 

    #region IgnoreCase_FlagLast_FlagLong_SingleResult
    [Test]
    public async Task IgnoreCase_FlagLast_FlagLong_SingleResult()
    {
        string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreCase.txt");
        Assert.IsTrue(File.Exists(TestFilePath));

        string SearchTerm = "fox";
        string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorLong}";

        var commandResultCollection = new CommandResultCollection();
        await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

        Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
    }
    #endregion IgnoreCase_FlagLast_FlagLong_SingleResult 

    #region IgnoreCase_FlagLast_FlagLong_MultiResult
    [Test]
    public async Task IgnoreCase_FlagLast_FlagLong_MultiResult()
    {
        string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreCase.txt");
        Assert.IsTrue(File.Exists(TestFilePath));

        string SearchTerm = "lazy";
        string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorLong}";

        var commandResultCollection = new CommandResultCollection();
        await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

        Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 3);
    }
    #endregion IgnoreCase_FlagLast_FlagLong_MultiResult 
    #endregion FlagLast..
    #endregion Tests..
}
