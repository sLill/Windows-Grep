using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WindowsGrep.Common;
using WindowsGrep.Core;
using WindowsGrep.Engine;

namespace WindowsGrep.Test.Flags.TargetFile
{
    public class TargetFileTests : TestBase
    {
        #region Fields..
        private string _flagDescriptorShort;
        private string _flagDescriptorLong;
        private string _testDataRelativePath = @"Flags\TargetFile\TestData";
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Setup
        [SetUp]
        public void Setup()
        {
            TestDataDirectory = Path.Combine(TestConfigurationManager.WorkingDirectory, _testDataRelativePath);

            List<string> DescriptionCollection = ConsoleFlag.TargetFile.GetCustomAttribute<DescriptionCollectionAttribute>()?.Value.OrderBy(x => x.Length).ToList();
            _flagDescriptorShort = DescriptionCollection[0];
            _flagDescriptorLong = DescriptionCollection[1];
        }
        #endregion Setup

        #region Tests..
        #region FlagFirst..
        #region TargetFile_FlagFirst_FlagShort_SingleQuotes
        [Test]
        public async Task TargetFile_FlagFirst_FlagShort_SingleQuotes()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "TargetFile.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox";
            string Command = $"{_flagDescriptorShort} '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion TargetFile_FlagFirst_FlagShort_SingleQuotes 

        #region TargetFile_FlagFirst_FlagShort_DoubleQuotes
        [Test]
        public async Task TargetFile_FlagFirst_FlagShort_DoubleQuotes()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "TargetFile.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox";
            string Command = $"{_flagDescriptorShort} \"{TestFilePath}\" {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion TargetFile_FlagFirst_FlagShort_DoubleQuotes 

        #region TargetFile_FlagFirst_FlagLong_SingleQuotes
        [Test]
        public async Task TargetFile_FlagFirst_FlagLong_SingleQuotes()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "TargetFile.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox";
            string Command = $"{_flagDescriptorLong} '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion TargetFile_FlagFirst_FlagLong_SingleQuotes

        #region TargetFile_FlagFirst_FlagLong_DoubleQuotes
        [Test]
        public async Task TargetFile_FlagFirst_FlagLong_DoubleQuotes()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "TargetFile.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox";
            string Command = $"{_flagDescriptorLong} \"{TestFilePath}\" {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion TargetFile_FlagFirst_FlagLong_DoubleQuotes
        #endregion FlagFirst..

        #region FlagMiddle..
        #region TargetFile_FlagMiddle_FlagShort_SingleQuotes
        [Test]
        public async Task TargetFile_FlagMiddle_FlagShort_SingleQuotes()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "TargetFile.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox";
            string Command = $"-i {_flagDescriptorShort} '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion TargetFile_FlagMiddle_FlagShort_SingleQuotes 

        #region TargetFile_FlagMiddle_FlagShort_DoubleQuotes
        [Test]
        public async Task TargetFile_FlagMiddle_FlagShort_DoubleQuotes()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "TargetFile.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox";
            string Command = $"-i {_flagDescriptorShort} \"{TestFilePath}\" {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion TargetFile_FlagMiddle_FlagShort_DoubleQuotes

        #region TargetFile_FlagMiddle_FlagLong_SingleQuotes
        [Test]
        public async Task TargetFile_FlagMiddle_FlagLong_SingleQuotes()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "TargetFile.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox";
            string Command = $"-i {_flagDescriptorLong} '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion TargetFile_FlagMiddle_FlagLong_SingleQuotes 

        #region TargetFile_FlagMiddle_FlagLong_DoubleQuotes
        [Test]
        public async Task TargetFile_FlagMiddle_FlagLong_DoubleQuotes()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "TargetFile.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox";
            string Command = $"-i {_flagDescriptorLong} \"{TestFilePath}\" {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion TargetFile_FlagMiddle_FlagLong_DoubleQuotes
        #endregion FlagMiddle..

        #region FlagLast..
        #region TargetFile_FlagLast_FlagShort_SingleQuotes
        [Test]
        public async Task TargetFile_FlagLast_FlagShort_SingleQuotes()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "TargetFile.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox";
            string Command = $"{SearchTerm} {_flagDescriptorShort} '{TestFilePath}'";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion TargetFile_FlagLast_FlagShort_SingleQuotes 

        #region TargetFile_FlagLast_FlagShort_DoubleQuotes
        [Test]
        public async Task TargetFile_FlagLast_FlagShort_DoubleQuotes()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "TargetFile.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox";
            string Command = $"{SearchTerm} {_flagDescriptorShort} \"{TestFilePath}\"";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion TargetFile_FlagLast_FlagShort_DoubleQuotes

        #region TargetFile_FlagLast_FlagLong_SingleQuotes
        [Test]
        public async Task TargetFile_FlagLast_FlagLong_SingleQuotes()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "TargetFile.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox";
            string Command = $"{SearchTerm} {_flagDescriptorLong} '{TestFilePath}'";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion TargetFile_FlagLast_FlagLong_SingleQuotes 

        #region TargetFile_FlagLast_FlagLong_DoubleQuotes
        [Test]
        public async Task TargetFile_FlagLast_FlagLong_DoubleQuotes()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "TargetFile.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox";
            string Command = $"{SearchTerm} {_flagDescriptorLong} \"{TestFilePath}\"";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion TargetFile_FlagLast_FlagLong_DoubleQuotes
        #endregion FlagLast..
        #endregion Tests..
    }
}
