namespace WindowsGrep.Test.Flags.Directory
{
    public class DirectoryTests : TestBase
    {
        #region Fields..
        private string _flagDescriptorShort;
        private string _flagDescriptorLong;
        private string _testDataRelativePath = @"Flags\Directory\TestData";
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Setup
        [SetUp]
        public void Setup()
        {
            TestDataDirectory = Path.Combine(TestConfigurationManager.WorkingDirectory, _testDataRelativePath);

            List<string> DescriptionCollection = ConsoleFlag.Directory.GetCustomAttribute<DescriptionCollectionAttribute>()?.Value.OrderBy(x => x.Length).ToList();
            _flagDescriptorShort = DescriptionCollection[0];
            _flagDescriptorLong = DescriptionCollection[1];
        }
        #endregion Setup

        #region Tests..
        #region FlagFirst..
        #region Directory_FlagFirst_FlagShort_SingleQuotes
        [Test]
        public async Task Directory_FlagFirst_FlagShort_SingleQuotes()
        {
            string SearchTerm = "fox";
            string Command = $"{_flagDescriptorShort} '{TestDataDirectory}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 2);
        }
        #endregion Directory_FlagFirst_FlagShort_SingleQuotes 

        #region Directory_FlagFirst_FlagShort_DoubleQuotes
        [Test]
        public async Task Directory_FlagFirst_FlagShort_DoubleQuotes()
        {
            string SearchTerm = "fox";
            string Command = $"{_flagDescriptorShort} \"{TestDataDirectory}\" {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 2);
        }
        #endregion Directory_FlagFirst_FlagShort_DoubleQuotes 

        #region Directory_FlagFirst_FlagLong_SingleQuotes
        [Test]
        public async Task Directory_FlagFirst_FlagLong_SingleQuotes()
        {
            string SearchTerm = "fox";
            string Command = $"{_flagDescriptorLong} '{TestDataDirectory}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 2);
        }
        #endregion Directory_FlagFirst_FlagLong_SingleQuotes

        #region Directory_FlagFirst_FlagLong_DoubleQuotes
        [Test]
        public async Task Directory_FlagFirst_FlagLong_DoubleQuotes()
        {
            string SearchTerm = "fox";
            string Command = $"{_flagDescriptorLong} \"{TestDataDirectory}\" {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 2);
        }
        #endregion Directory_FlagFirst_FlagLong_DoubleQuotes
        #endregion FlagFirst..

        #region FlagMiddle..
        #region Directory_FlagMiddle_FlagShort_SingleQuotes
        [Test]
        public async Task Directory_FlagMiddle_FlagShort_SingleQuotes()
        {
            string SearchTerm = "fox";
            string Command = $"-i {_flagDescriptorShort} '{TestDataDirectory}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 2);
        }
        #endregion Directory_FlagMiddle_FlagShort_SingleQuotes 

        #region Directory_FlagMiddle_FlagShort_DoubleQuotes
        [Test]
        public async Task Directory_FlagMiddle_FlagShort_DoubleQuotes()
        {
            string SearchTerm = "fox";
            string Command = $"-i {_flagDescriptorShort} \"{TestDataDirectory}\" {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 2);
        }
        #endregion Directory_FlagMiddle_FlagShort_DoubleQuotes

        #region Directory_FlagMiddle_FlagLong_SingleQuotes
        [Test]
        public async Task Directory_FlagMiddle_FlagLong_SingleQuotes()
        {
            string SearchTerm = "fox";
            string Command = $"-i {_flagDescriptorLong} '{TestDataDirectory}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 2);
        }
        #endregion Directory_FlagMiddle_FlagLong_SingleQuotes 

        #region Directory_FlagMiddle_FlagLong_DoubleQuotes
        [Test]
        public async Task Directory_FlagMiddle_FlagLong_DoubleQuotes()
        {
            string SearchTerm = "fox";
            string Command = $"-i {_flagDescriptorLong} \"{TestDataDirectory}\" {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 2);
        }
        #endregion Directory_FlagMiddle_FlagLong_DoubleQuotes
        #endregion FlagMiddle..

        #region FlagLast..
        #region Directory_FlagLast_FlagShort_SingleQuotes
        [Test]
        public async Task Directory_FlagLast_FlagShort_SingleQuotes()
        {
            string SearchTerm = "fox";
            string Command = $"{SearchTerm} {_flagDescriptorShort} '{TestDataDirectory}'";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 2);
        }
        #endregion Directory_FlagLast_FlagShort_SingleQuotes 

        #region Directory_FlagLast_FlagShort_DoubleQuotes
        [Test]
        public async Task Directory_FlagLast_FlagShort_DoubleQuotes()
        {
            string SearchTerm = "fox";
            string Command = $"{SearchTerm} {_flagDescriptorShort} \"{TestDataDirectory}\"";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 2);
        }
        #endregion Directory_FlagLast_FlagShort_DoubleQuotes

        #region Directory_FlagLast_FlagLong_SingleQuotes
        [Test]
        public async Task Directory_FlagLast_FlagLong_SingleQuotes()
        {
            string SearchTerm = "fox";
            string Command = $"{SearchTerm} {_flagDescriptorLong} '{TestDataDirectory}'";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 2);
        }
        #endregion Directory_FlagLast_FlagLong_SingleQuotes 

        #region Directory_FlagLast_FlagLong_DoubleQuotes
        [Test]
        public async Task Directory_FlagLast_FlagLong_DoubleQuotes()
        {
            string SearchTerm = "fox";
            string Command = $"{SearchTerm} {_flagDescriptorLong} \"{TestDataDirectory}\"";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 2);
        }
        #endregion Directory_FlagLast_FlagLong_DoubleQuotes
        #endregion FlagLast..
        #endregion Tests..
    }
}
