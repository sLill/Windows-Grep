namespace WindowsGrep.Test.Flags.Replace
{
    public class ReplaceTests : TestBase
    {
        #region Fields..
        private const string _TestFileTemplateText = "The quick brown fox jumps over the lazy dog";
        private const string _TestFileExpectedResultText = "The slow green turtle jumps over the lazy dog";

        private string _flagDescriptorShort;
        private string _flagDescriptorLong;
        private string _testDataRelativePath = @"Flags\Replace\TestData";
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Setup
        [SetUp]
        public void Setup()
        {
            TestDataDirectory = Path.Combine(TestConfigurationManager.WorkingDirectory, _testDataRelativePath);

            List<string> DescriptionCollection = ConsoleFlag.Replace.GetCustomAttribute<DescriptionCollectionAttribute>()?.Value.OrderBy(x => x.Length).ToList();
            _flagDescriptorShort = DescriptionCollection[0];
            _flagDescriptorLong = DescriptionCollection[1];

            // Build TestData directory if it doesn't exist yet
            System.IO.Directory.CreateDirectory(TestDataDirectory);
        }
        #endregion Setup

        #region Tests..
        #region FlagFirst..
        #region Replace_FlagFirst_FlagShort_SingleQuotes
        [Test]
        public async Task Replace_FlagFirst_FlagShort_SingleQuotes()
        {
            try
            {
                CreateTestFiles();

                string SearchTerm = "quick brown fox";
                string ReplaceText = "slow green turtle";
                string Command = $"{_flagDescriptorShort} '{ReplaceText}' -d '{TestDataDirectory}' {SearchTerm}";

                var commandResultCollection = new CommandResultCollection();
                await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

                // Assert there are no files returned in the grep collection whose text doesn't match the expected text
                Assert.IsFalse(commandResultCollection.Where(result => File.ReadAllText(result.SourceFile) != _TestFileExpectedResultText).Any());
            }
            catch
            {
                throw;
            }
            finally
            {
                DeleteTestFiles();
            }
        }
        #endregion Replace_FlagFirst_FlagShort_SingleQuotes 

        #region Replace_FlagFirst_FlagShort_DoubleQuotes
        [Test]
        public async Task Replace_FlagFirst_FlagShort_DoubleQuotes()
        {
            try
            {
                CreateTestFiles();

                string SearchTerm = "quick brown fox";
                string ReplaceText = "slow green turtle";
                string Command = $"{_flagDescriptorShort} \"{ReplaceText}\" -d '{TestDataDirectory}' {SearchTerm}";

                var commandResultCollection = new CommandResultCollection();
                await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

                // Assert there are no files returned in the grep collection whose text doesn't match the expected text
                Assert.IsFalse(commandResultCollection.Where(result => File.ReadAllText(result.SourceFile) != _TestFileExpectedResultText).Any());
            }
            catch
            {
                throw;
            }
            finally
            {
                DeleteTestFiles();
            }
        }
        #endregion Replace_FlagFirst_FlagShort_DoubleQuotes

        #region Replace_FlagFirst_FlagLong_SingleQuotes
        [Test]
        public async Task Replace_FlagFirst_FlagLong_SingleQuotes()
        {
            try
            {
                CreateTestFiles();

                string SearchTerm = "quick brown fox";
                string ReplaceText = "slow green turtle";
                string Command = $"{_flagDescriptorLong} '{ReplaceText}' -d '{TestDataDirectory}' {SearchTerm}";

                var commandResultCollection = new CommandResultCollection();
                await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

                // Assert there are no files returned in the grep collection whose text doesn't match the expected text
                Assert.IsFalse(commandResultCollection.Where(result => File.ReadAllText(result.SourceFile) != _TestFileExpectedResultText).Any());
            }
            catch
            {
                throw;
            }
            finally
            {
                DeleteTestFiles();
            }
        }
        #endregion Replace_FlagFirst_FlagLong_SingleQuotes 

        #region Replace_FlagFirst_FlagLong_DoubleQuotes
        [Test]
        public async Task Replace_FlagFirst_FlagLong_DoubleQuotes()
        {
            try
            {
                CreateTestFiles();

                string SearchTerm = "quick brown fox";
                string ReplaceText = "slow green turtle";
                string Command = $"{_flagDescriptorLong} \"{ReplaceText}\" -d '{TestDataDirectory}' {SearchTerm}";

                var commandResultCollection = new CommandResultCollection();
                await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

                // Assert there are no files returned in the grep collection whose text doesn't match the expected text
                Assert.IsFalse(commandResultCollection.Where(result => File.ReadAllText(result.SourceFile) != _TestFileExpectedResultText).Any());
            }
            catch
            {
                throw;
            }
            finally
            {
                DeleteTestFiles();
            }
        }
        #endregion Replace_FlagFirst_FlagLong_DoubleQuotes
        #endregion FlagFirst..

        #region FlagMiddle..
        #region Replace_FlagMiddle_FlagShort_SingleQuotes
        [Test]
        public async Task Replace_FlagMiddle_FlagShort_SingleQuotes()
        {
            try
            {
                CreateTestFiles();

                string SearchTerm = "quick brown fox";
                string ReplaceText = "slow green turtle";
                string Command = $"-d '{TestDataDirectory}' {_flagDescriptorShort} '{ReplaceText}' {SearchTerm}";

                var commandResultCollection = new CommandResultCollection();
                await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

                // Assert there are no files returned in the grep collection whose text doesn't match the expected text
                Assert.IsFalse(commandResultCollection.Where(result => File.ReadAllText(result.SourceFile) != _TestFileExpectedResultText).Any());
            }
            catch
            {
                throw;
            }
            finally
            {
                DeleteTestFiles();
            }
        }
        #endregion Replace_FlagMiddle_FlagShort_SingleQuotes 

        #region Replace_FlagMiddle_FlagShort_DoubleQuotes
        [Test]
        public async Task Replace_FlagMiddle_FlagShort_DoubleQuotes()
        {
            try
            {
                CreateTestFiles();

                string SearchTerm = "quick brown fox";
                string ReplaceText = "slow green turtle";
                string Command = $"-d '{TestDataDirectory}' {_flagDescriptorShort} \"{ReplaceText}\" {SearchTerm}";

                var commandResultCollection = new CommandResultCollection();
                await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

                // Assert there are no files returned in the grep collection whose text doesn't match the expected text
                Assert.IsFalse(commandResultCollection.Where(result => File.ReadAllText(result.SourceFile) != _TestFileExpectedResultText).Any());
            }
            catch
            {
                throw;
            }
            finally
            {
                DeleteTestFiles();
            }
        }
        #endregion Replace_FlagMiddle_FlagShort_DoubleQuotes

        #region Replace_FlagMiddle_FlagLong_SingleQuotes
        [Test]
        public async Task Replace_FlagMiddle_FlagLong_SingleQuotes()
        {
            try
            {
                CreateTestFiles();

                string SearchTerm = "quick brown fox";
                string ReplaceText = "slow green turtle";
                string Command = $"-d '{TestDataDirectory}' {_flagDescriptorLong} '{ReplaceText}' {SearchTerm}";

                var commandResultCollection = new CommandResultCollection();
                await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

                // Assert there are no files returned in the grep collection whose text doesn't match the expected text
                Assert.IsFalse(commandResultCollection.Where(result => File.ReadAllText(result.SourceFile) != _TestFileExpectedResultText).Any());
            }
            catch
            {
                throw;
            }
            finally
            {
                DeleteTestFiles();
            }
        }
        #endregion Replace_FlagMiddle_FlagLong_SingleQuotes 

        #region Replace_FlagMiddle_FlagLong_DoubleQuotes
        [Test]
        public async Task Replace_FlagMiddle_FlagLong_DoubleQuotes()
        {
            try
            {
                CreateTestFiles();

                string SearchTerm = "quick brown fox";
                string ReplaceText = "slow green turtle";
                string Command = $"-d '{TestDataDirectory}' {_flagDescriptorLong} \"{ReplaceText}\" {SearchTerm}";

                var commandResultCollection = new CommandResultCollection();
                await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

                // Assert there are no files returned in the grep collection whose text doesn't match the expected text
                Assert.IsFalse(commandResultCollection.Where(result => File.ReadAllText(result.SourceFile) != _TestFileExpectedResultText).Any());
            }
            catch
            {
                throw;
            }
            finally
            {
                DeleteTestFiles();
            }
        }
        #endregion Replace_FlagMiddle_FlagLong_DoubleQuotes
        #endregion FlagMiddle..

        #region FlagLast..
        #region Replace_FlagLast_FlagShort_SingleQuotes
        [Test]
        public async Task Replace_FlagLast_FlagShort_SingleQuotes()
        {
            try
            {
                CreateTestFiles();

                string SearchTerm = "quick brown fox";
                string ReplaceText = "slow green turtle";
                string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_flagDescriptorShort} '{ReplaceText}'";

                var commandResultCollection = new CommandResultCollection();
                await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

                // Assert there are no files returned in the grep collection whose text doesn't match the expected text
                Assert.IsFalse(commandResultCollection.Where(result => File.ReadAllText(result.SourceFile) != _TestFileExpectedResultText).Any());
            }
            catch
            {
                throw;
            }
            finally
            {
                DeleteTestFiles();
            }
        }
        #endregion Replace_FlagLast_FlagShort_SingleQuotes 

        #region Replace_FlagLast_FlagShort_DoubleQuotes
        [Test]
        public async Task Replace_FlagLast_FlagShort_DoubleQuotes()
        {
            try
            {
                CreateTestFiles();

                string SearchTerm = "quick brown fox";
                string ReplaceText = "slow green turtle";
                string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_flagDescriptorShort} \"{ReplaceText}\"";

                var commandResultCollection = new CommandResultCollection();
                await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

                // Assert there are no files returned in the grep collection whose text doesn't match the expected text
                Assert.IsFalse(commandResultCollection.Where(result => File.ReadAllText(result.SourceFile) != _TestFileExpectedResultText).Any());
            }
            catch
            {
                throw;
            }
            finally
            {
                DeleteTestFiles();
            }
        }
        #endregion Replace_FlagLast_FlagShort_DoubleQuotes

        #region Replace_FlagLast_FlagLong_SingleQuotes
        [Test]
        public async Task Replace_FlagLast_FlagLong_SingleQuotes()
        {
            try
            {
                CreateTestFiles();

                string SearchTerm = "quick brown fox";
                string ReplaceText = "slow green turtle";
                string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_flagDescriptorLong} '{ReplaceText}'";

                var commandResultCollection = new CommandResultCollection();
                await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

                // Assert there are no files returned in the grep collection whose text doesn't match the expected text
                Assert.IsFalse(commandResultCollection.Where(result => File.ReadAllText(result.SourceFile) != _TestFileExpectedResultText).Any());
            }
            catch
            {
                throw;
            }
            finally
            {
                DeleteTestFiles();
            }
        }
        #endregion Replace_FlagLast_FlagLong_SingleQuotes 

        #region Replace_FlagLast_FlagLong_DoubleQuotes
        [Test]
        public async Task Replace_FlagLast_FlagLong_DoubleQuotes()
        {
            try
            {
                CreateTestFiles();

                string SearchTerm = "quick brown fox";
                string ReplaceText = "slow green turtle";
                string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_flagDescriptorLong} \"{ReplaceText}\"";

                var commandResultCollection = new CommandResultCollection();
                await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

                // Assert there are no files returned in the grep collection whose text doesn't match the expected text
                Assert.IsFalse(commandResultCollection.Where(result => File.ReadAllText(result.SourceFile) != _TestFileExpectedResultText).Any());
            }
            catch
            {
                throw;
            }
            finally
            {
                DeleteTestFiles();
            }
        }
        #endregion Replace_FlagLast_FlagLong_DoubleQuotes
        #endregion FlagLast..
        #endregion Tests..

        #region Methods..

        #region CreateTestFiles
        /// <summary>
        /// Create a copy of each file in TestData
        /// </summary>
        private async Task CreateTestFiles()
        {
            // File one
            string FilePathOne = Path.Combine(TestDataDirectory, "ReplaceOne.txt");
            File.WriteAllText(FilePathOne, _TestFileTemplateText);
            Assert.IsTrue(File.Exists(FilePathOne));

            // File two
            string FilePathTwo = Path.Combine(TestDataDirectory, "ReplaceTwo.txt");
            File.WriteAllText(FilePathTwo, _TestFileTemplateText);
            Assert.IsTrue(File.Exists(FilePathTwo));
        }
        #endregion CreateTestFiles

        #region DeleteTestFiles
        private async Task DeleteTestFiles()
        {
            // Delete all files in TestData
            var DirectoryInfo = new DirectoryInfo(TestDataDirectory);
            DirectoryInfo.GetFiles().ToList().ForEach(file => file.Delete());
        }
        #endregion DeleteTestFiles
        #endregion Methods..
    }
}
