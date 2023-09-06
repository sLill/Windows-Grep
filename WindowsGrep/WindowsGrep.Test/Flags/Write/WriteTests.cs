using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WindowsGrep.Common;
using WindowsGrep.Core;
using WindowsGrep.Engine;

namespace WindowsGrep.Test.Flags.Write
{
    public class WriteTests : TestBase
    {
        #region Fields..
        private string _flagDescriptorShort;
        private string _flagDescriptorLong;
        private string _testDataRelativePath = @"Flags\Write\TestData";
        private string _testOutputFilePath;
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Setup
        [SetUp]
        public void Setup()
        {
            TestDataDirectory = Path.Combine(TestConfigurationManager.WorkingDirectory, _testDataRelativePath);
            _testOutputFilePath = Path.Combine(TestDataDirectory, "WriteOutput.txt");

            List<string> DescriptionCollection = ConsoleFlag.Write.GetCustomAttribute<DescriptionCollectionAttribute>()?.Value.OrderBy(x => x.Length).ToList();
            _flagDescriptorShort = DescriptionCollection[0];
            _flagDescriptorLong = DescriptionCollection[1];
        }
        #endregion Setup

        #region Tests..
        #region FlagFirst..
        #region Write_FlagFirst_FlagShort
        [Test]
        public async Task Write_FlagFirst_FlagShort()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "Write.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "quick brown";
            string Command = $"{_flagDescriptorShort} '{_testOutputFilePath}' -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(File.Exists(_testOutputFilePath));
            Assert.IsTrue(FileHasData(_testOutputFilePath));

            File.Delete(_testOutputFilePath);
        }
        #endregion Write_FlagFirst_FlagShort 

        #region Write_FlagFirst_FlagLong
        [Test]
        public async Task Write_FlagFirst_FlagLong()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "Write.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "quick brown";
            string Command = $"{_flagDescriptorLong} '{_testOutputFilePath}' -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(File.Exists(_testOutputFilePath));
            Assert.IsTrue(FileHasData(_testOutputFilePath));

            File.Delete(_testOutputFilePath);
        }
        #endregion Write_FlagFirst_FlagLong 
        #endregion FlagFirst..

        #region FlagMiddle..
        #region Write_FlagMiddle_FlagShort
        [Test]
        public async Task Write_FlagMiddle_FlagShort()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "Write.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "quick brown";
            string Command = $"-f '{TestFilePath}' {_flagDescriptorShort} '{_testOutputFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(File.Exists(_testOutputFilePath));
            Assert.IsTrue(FileHasData(_testOutputFilePath));

            File.Delete(_testOutputFilePath);
        }
        #endregion Write_FlagMiddle_FlagShort 

        #region Write_FlagMiddle_FlagLong
        [Test]
        public async Task Write_FlagMiddle_FlagLong()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "Write.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "quick brown";
            string Command = $"-f '{TestFilePath}' {_flagDescriptorLong} '{_testOutputFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(File.Exists(_testOutputFilePath));
            Assert.IsTrue(FileHasData(_testOutputFilePath));

            File.Delete(_testOutputFilePath);
        }
        #endregion Write_FlagMiddle_FlagLong 
        #endregion FlagMiddle..

        #region FlagLast..
        #region Write_FlagLast_FlagShort
        [Test]
        public async Task Write_FlagLast_FlagShort()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "Write.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "quick brown";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorShort} '{_testOutputFilePath}'";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(File.Exists(_testOutputFilePath));
            Assert.IsTrue(FileHasData(_testOutputFilePath));

            File.Delete(_testOutputFilePath);
        }
        #endregion Write_FlagLast_FlagShort 

        #region Write_FlagLast_FlagLong
        [Test]
        public async Task Write_FlagLast_FlagLong()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "Write.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "quick brown";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorLong} '{_testOutputFilePath}'";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(File.Exists(_testOutputFilePath));
            Assert.IsTrue(FileHasData(_testOutputFilePath));

            File.Delete(_testOutputFilePath);
        }
        #endregion Write_FlagLast_FlagLong 
        #endregion FlagLast..
        #endregion Tests..

        #region Methods..
        private bool FileHasData(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            return fileInfo.Length > 0;
        }
        #endregion Methods..
    }
}
