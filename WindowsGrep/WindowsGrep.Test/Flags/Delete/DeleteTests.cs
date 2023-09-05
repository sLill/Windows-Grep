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
        private string _FlagDescriptorShort;
        private string _FlagDescriptorLong;
        private string _TestDataRelativePath = @"Flags\Delete\TestData";
        private string _TestFilePath;
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Setup
        [SetUp]
        public void Setup()
        {
            TestDataDirectory = Path.Combine(TestConfigurationManager.ProjectDirectory, _TestDataRelativePath);
            _TestFilePath = Path.Combine(TestDataDirectory, "DeleteOutput.txt");

            List<string> DescriptionCollection = ConsoleFlag.Delete.GetCustomAttribute<DescriptionCollectionAttribute>()?.Value.OrderBy(x => x.Length).ToList();
            _FlagDescriptorShort = DescriptionCollection[0];
            _FlagDescriptorLong = DescriptionCollection[1];

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
            string Command = $"{_FlagDescriptorShort} -f '{_TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsFalse(File.Exists(_TestFilePath));
        }
        #endregion Write_FlagFirst_FlagShort 

        #region Write_FlagFirst_FlagLong
        [Test]
        public async Task Write_FlagFirst_FlagLong()
        {
            File.WriteAllText(_TestFilePath, "Delete flag test");

            string SearchTerm = "Delete flag test";
            string Command = $"{_FlagDescriptorLong} -f '{_TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

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
            string Command = $"-f '{_TestFilePath}' {_FlagDescriptorShort} {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsFalse(File.Exists(_TestFilePath));
        }
        #endregion Write_FlagMiddle_FlagShort 

        #region Write_FlagMiddle_FlagLong
        [Test]
        public async Task Write_FlagMiddle_FlagLong()
        {
            File.WriteAllText(_TestFilePath, "Delete flag test");

            string SearchTerm = "Delete flag test";
            string Command = $"-f '{_TestFilePath}' {_FlagDescriptorLong} {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

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
            string Command = $"-f '{_TestFilePath}' {SearchTerm} {_FlagDescriptorShort}";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsFalse(File.Exists(_TestFilePath));
        }
        #endregion Write_FlagLast_FlagShort 

        #region Write_FlagLast_FlagLong
        [Test]
        public async Task Write_FlagLast_FlagLong()
        {
            File.WriteAllText(_TestFilePath, "Delete flag test");

            string SearchTerm = "Delete flag test";
            string Command = $"-f '{_TestFilePath}' {SearchTerm} {_FlagDescriptorLong}";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsFalse(File.Exists(_TestFilePath));
        }
        #endregion Write_FlagLast_FlagLong 
        #endregion FlagLast..
        #endregion Tests..

        #region Methods..
        #endregion Methods..
    }
}
