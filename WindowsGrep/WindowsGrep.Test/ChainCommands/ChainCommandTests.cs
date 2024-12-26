namespace WindowsGrep.Test.ChainCommands
{
    public class ChainCommandTests : TestBase
    {
        #region Fields..
        private string _testDataRelativePath = @"ChainCommands\TestData";
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Setup
        [SetUp]
        public void Setup()
        {
            TestDataDirectory = Path.Combine(TestConfigurationManager.WorkingDirectory, _testDataRelativePath);
        }
        #endregion Setup

        #region Tests..
        #region ChainCommands_One
        [Test]
        public async Task ChainCommands_One()
        {
            string SearchTerm = "fox jumps over";
            string Command = $"-d '{TestDataDirectory}' -k ChainCommands | -r -i {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 3);
        }
        #endregion ChainCommands_One 

        #region ChainCommands_Two
        [Test]
        public async Task ChainCommands_Two()
        {
            string SearchTerm = "fox jumps over";
            string Command = $"-d '{TestDataDirectory}' -k ChainCommands | -r -i {SearchTerm} | -k Two";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion ChainCommands_Two 
        #endregion Tests..
    }
}
