namespace WindowsGrep.Test.Flags.FileTypeExcludeFilter
{
    public class FileTypeExcludeFilterTests : TestBase
    {
        #region Fields..
        private string _flagDescriptorShort;
        private string _flagDescriptorLong;
        private string _testDataRelativePath = @"Flags\FileTypeExcludeFilter\TestData";
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Setup
        [SetUp]
        public void Setup()
        {
            TestDataDirectory = Path.Combine(TestConfigurationManager.WorkingDirectory, _testDataRelativePath);

            List<string> DescriptionCollection = ConsoleFlag.FileTypeExcludeFilter.GetCustomAttribute<DescriptionCollectionAttribute>()?.Value.OrderBy(x => x.Length).ToList();
            _flagDescriptorShort = DescriptionCollection[0];
            _flagDescriptorLong = DescriptionCollection[1];
        }
        #endregion Setup

        #region Tests..
        #region FlagFirst..
        #region FileTypeExcludeFilter_FlagFirst_FlagShort_SingleType
        [Test]
        public async Task FileTypeExcludeFilter_FlagFirst_FlagShort_SingleType()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".cpp";
            string Command = $"{_flagDescriptorShort} {FlagParameter} -d '{TestDataDirectory}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 6);
        }
        #endregion FileTypeExcludeFilter_FlagFirst_FlagShort_SingleType 

        #region FileTypeExcludeFilter_FlagFirst_FlagShort_MultiType_CommaDelimited
        [Test]
        public async Task FileTypeExcludeFilter_FlagFirst_FlagShort_MultiType_CommaDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".cpp,.txt,.php";
            string Command = $"{_flagDescriptorShort} {FlagParameter} -d '{TestDataDirectory}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 3);
        }
        #endregion FileTypeExcludeFilter_FlagFirst_FlagShort_MultiType_CommaDelimited 

        #region FileTypeExcludeFilter_FlagFirst_FlagShort_MultiType_SemiColonDelimited
        [Test]
        public async Task FileTypeExcludeFilter_FlagFirst_FlagShort_MultiType_SemiColonDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".cpp;.txt";
            string Command = $"{_flagDescriptorShort} {FlagParameter} -d '{TestDataDirectory}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 5);
        }
        #endregion FileTypeExcludeFilter_FlagFirst_FlagShort_MultiType_SemiColonDelimited 

        #region FileTypeExcludeFilter_FlagFirst_FlagLong_SingleType
        [Test]
        public async Task FileTypeExcludeFilter_FlagFirst_FlagLong_SingleType()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".txt";
            string Command = $"{_flagDescriptorLong} {FlagParameter} -d '{TestDataDirectory}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 6);
        }
        #endregion FileTypeExcludeFilter_FlagFirst_FlagLong_SingleType 

        #region FileTypeExcludeFilter_FlagFirst_FlagLong_MultiType_CommaDelimited
        [Test]
        public async Task FileTypeExcludeFilter_FlagFirst_FlagLong_MultiType_CommaDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".txt,.php";
            string Command = $"{_flagDescriptorLong} {FlagParameter} -d '{TestDataDirectory}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 4);
        }
        #endregion FileTypeExcludeFilter_FlagFirst_FlagLong_MultiType_CommaDelimited 

        #region FileTypeExcludeFilter_FlagFirst_FlagLong_MultiType_SemiColonDelimited
        [Test]
        public async Task FileTypeExcludeFilter_FlagFirst_FlagLong_MultiType_SemiColonDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".txt;.php";
            string Command = $"{_flagDescriptorLong} {FlagParameter} -d '{TestDataDirectory}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 4);
        }
        #endregion FileTypeExcludeFilter_FlagFirst_FlagLong_MultiType_SemiColonDelimited 
        #endregion FlagFirst..

        #region FlagMiddle..
        #region FileTypeExcludeFilter_FlagMiddle_FlagShort_SingleType
        [Test]
        public async Task FileTypeExcludeFilter_FlagMiddle_FlagShort_SingleType()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".html";
            string Command = $"-d '{TestDataDirectory}' {_flagDescriptorShort} {FlagParameter} {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 6);
        }
        #endregion FileTypeExcludeFilter_FlagMiddle_FlagShort_SingleType 

        #region FileTypeExcludeFilter_FlagMiddle_FlagShort_MultiType_CommaDelimited
        [Test]
        public async Task FileTypeExcludeFilter_FlagMiddle_FlagShort_MultiType_CommaDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".html,.txt";
            string Command = $"-d '{TestDataDirectory}' {_flagDescriptorShort} {FlagParameter} {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 5);
        }
        #endregion FileTypeExcludeFilter_FlagMiddle_FlagShort_MultiType_CommaDelimited 

        #region FileTypeExcludeFilter_FlagMiddle_FlagShort_MultiType_SemiColonDelimited
        [Test]
        public async Task FileTypeExcludeFilter_FlagMiddle_FlagShort_MultiType_SemiColonDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".html;.txt";
            string Command = $"-d '{TestDataDirectory}' {_flagDescriptorShort} {FlagParameter} {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 5);
        }
        #endregion FileTypeExcludeFilter_FlagMiddle_FlagShort_MultiType_SemiColonDelimited 

        #region FileTypeExcludeFilter_FlagMiddle_FlagLong_SingleType
        [Test]
        public async Task FileTypeExcludeFilter_FlagMiddle_FlagLong_SingleType()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".php";
            string Command = $"-d '{TestDataDirectory}' {_flagDescriptorLong} {FlagParameter} {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 5);
        }
        #endregion FileTypeExcludeFilter_FlagMiddle_FlagLong_SingleType 

        #region FileTypeExcludeFilter_FlagMiddle_FlagLong_MultiType_CommaDelimited
        [Test]
        public async Task FileTypeExcludeFilter_FlagMiddle_FlagLong_MultiType_CommaDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".php,.wg";
            string Command = $"-d '{TestDataDirectory}' {_flagDescriptorLong} {FlagParameter} {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 4);
        }
        #endregion FileTypeExcludeFilter_FlagMiddle_FlagLong_MultiType_CommaDelimited 

        #region FileTypeExcludeFilter_FlagMiddle_FlagLong_MultiType_SemiColonDelimited
        [Test]
        public async Task FileTypeExcludeFilter_FlagMiddle_FlagLong_MultiType_SemiColonDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".php;.wg";
            string Command = $"-d '{TestDataDirectory}' {_flagDescriptorLong} {FlagParameter} {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 4);
        }
        #endregion FileTypeExcludeFilter_FlagMiddle_FlagLong_MultiType_SemiColonDelimited 
        #endregion FlagMiddle..

        #region FlagLast..
        #region FileTypeExcludeFilter_FlagLast_FlagShort_SingleType
        [Test]
        public async Task FileTypeExcludeFilter_FlagLast_FlagShort_SingleType()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".cpp";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_flagDescriptorShort} {FlagParameter}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 6);
        }
        #endregion FileTypeExcludeFilter_FlagLast_FlagShort_SingleType 

        #region FileTypeExcludeFilter_FlagLast_FlagShort_MultiType_CommaDelimited
        [Test]
        public async Task FileTypeExcludeFilter_FlagLast_FlagShort_MultiType_CommaDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".cpp,.html";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_flagDescriptorShort} {FlagParameter}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 5);
        }
        #endregion FileTypeExcludeFilter_FlagLast_FlagShort_MultiType_CommaDelimited 

        #region FileTypeExcludeFilter_FlagLast_FlagShort_MultiType_SemiColonDelimited
        [Test]
        public async Task FileTypeExcludeFilter_FlagLast_FlagShort_MultiType_SemiColonDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".cpp;.html";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_flagDescriptorShort} {FlagParameter}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 5);
        }
        #endregion FileTypeExcludeFilter_FlagLast_FlagShort_MultiType_SemiColonDelimited 

        #region FileTypeExcludeFilter_FlagLast_FlagLong_SingleType
        [Test]
        public async Task FileTypeExcludeFilter_FlagLast_FlagLong_SingleType()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".txt";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_flagDescriptorLong} {FlagParameter}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 6);
        }
        #endregion FileTypeExcludeFilter_FlagLast_FlagLong_SingleType 

        #region FileTypeExcludeFilter_FlagLast_FlagLong_MultiType_CommaDelimited
        [Test]
        public async Task FileTypeExcludeFilter_FlagLast_FlagLong_MultiType_CommaDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".txt,.wg";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_flagDescriptorLong} {FlagParameter}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 5);
        }
        #endregion FileTypeExcludeFilter_FlagLast_FlagLong_MultiType_CommaDelimited 

        #region FileTypeExcludeFilter_FlagLast_FlagLong_MultiType_SemiColonDelimited
        [Test]
        public async Task FileTypeExcludeFilter_FlagLast_FlagLong_MultiType_SemiColonDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".txt;.wg";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_flagDescriptorLong} {FlagParameter}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 5);
        }
        #endregion FileTypeExcludeFilter_FlagLast_FlagLong_MultiType_SemiColonDelimited 
        #endregion FlagLast..

        #region FileTypeExcludeFilter_FileTypeNotExists
        [Test]
        public async Task FileTypeExcludeFilter_FileTypeNotExists()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".tig";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_flagDescriptorLong} {FlagParameter}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 7);
        }
        #endregion FileTypeExcludeFilter_FileTypeNotExists 
        #endregion Tests..
    }
}
