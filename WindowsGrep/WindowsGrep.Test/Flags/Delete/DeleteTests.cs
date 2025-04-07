using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WindowsGrep.Common;
using WindowsGrep.Core;
using WindowsGrep.Engine;

namespace WindowsGrep.Test.Flags.Delete
{
    public class DeleteTests : TestBase
    {
        #region Fields..
        private string _flagDescriptorShort;
        private string _flagDescriptorLong;
        private string _testDataRelativePath = @"Flags\Delete\TestData";
        private string _TestFilePath;
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Setup
        [SetUp]
        public void Setup()
        {
            TestDataDirectory = Path.Combine(TestConfigurationManager.WorkingDirectory, _testDataRelativePath);
            _TestFilePath = Path.Combine(TestDataDirectory, "DeleteOutput.txt");

            List<string> DescriptionCollection = ConsoleFlag.Delete.GetCustomAttribute<DescriptionCollectionAttribute>()?.Value.OrderBy(x => x.Length).ToList();
            _flagDescriptorShort = DescriptionCollection[0];
            _flagDescriptorLong = DescriptionCollection[1];

            // Build TestData directory if it doesn't exist yet
            System.IO.Directory.CreateDirectory(TestDataDirectory);
        }
        #endregion Setup

        #region Tests..
        #region FlagFirst..
        #region Write_FlagFirst_FlagShort
        [Test]
        public async Task Write_FlagFirst_FlagShort()
        {
            File.WriteAllText(_TestFilePath, "Delete flag test");

            string SearchTerm = "Delete flag test";
            string Command = $"{_flagDescriptorShort} -f '{_TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsFalse(File.Exists(_TestFilePath));
        }
        #endregion Write_FlagFirst_FlagShort 

        #region Write_FlagFirst_FlagLong
        [Test]
        public async Task Write_FlagFirst_FlagLong()
        {
            File.WriteAllText(_TestFilePath, "Delete flag test");

            string SearchTerm = "Delete flag test";
            string Command = $"{_flagDescriptorLong} -f '{_TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsFalse(File.Exists(_TestFilePath));
        }
        #endregion Write_FlagFirst_FlagLong 
        #endregion FlagFirst..

        #region FlagMiddle..
        #region Write_FlagMiddle_FlagShort
        [Test]
        public async Task Write_FlagMiddle_FlagShort()
        {
            File.WriteAllText(_TestFilePath, "Delete flag test");

            string SearchTerm = "Delete flag test";
            string Command = $"-f '{_TestFilePath}' {_flagDescriptorShort} {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsFalse(File.Exists(_TestFilePath));
        }
        #endregion Write_FlagMiddle_FlagShort 

        #region Write_FlagMiddle_FlagLong
        [Test]
        public async Task Write_FlagMiddle_FlagLong()
        {
            File.WriteAllText(_TestFilePath, "Delete flag test");

            string SearchTerm = "Delete flag test";
            string Command = $"-f '{_TestFilePath}' {_flagDescriptorLong} {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsFalse(File.Exists(_TestFilePath));
        }
        #endregion Write_FlagMiddle_FlagLong 
        #endregion FlagMiddle..

        #region FlagLast..
        #region Write_FlagLast_FlagShort
        [Test]
        public async Task Write_FlagLast_FlagShort()
        {
            File.WriteAllText(_TestFilePath, "Delete flag test");

            string SearchTerm = "Delete flag test";
            string Command = $"-f '{_TestFilePath}' {SearchTerm} {_flagDescriptorShort}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsFalse(File.Exists(_TestFilePath));
        }
        #endregion Write_FlagLast_FlagShort 

        #region Write_FlagLast_FlagLong
        [Test]
        public async Task Write_FlagLast_FlagLong()
        {
            File.WriteAllText(_TestFilePath, "Delete flag test");

            string SearchTerm = "Delete flag test";
            string Command = $"-f '{_TestFilePath}' {SearchTerm} {_flagDescriptorLong}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsFalse(File.Exists(_TestFilePath));
        }
        #endregion Write_FlagLast_FlagLong 
        #endregion FlagLast..
        #endregion Tests..

        #region Methods..
        #endregion Methods..
    }
}
