using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WindowsGrep.Common;
using WindowsGrep.Core;
using WindowsGrep.Engine;

namespace WindowsGrep.Test.Flags.IgnoreBreaks
{
    public class IgnoreBreaksTests : TestBase
    {
        #region Fields..
        private string _flagDescriptorShort;
        private string _flagDescriptorLong;
        private string _testDataRelativePath = @"Flags\IgnoreBreaks\TestData";
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Setup
        [SetUp]
        public void Setup()
        {
            TestDataDirectory = Path.Combine(TestConfigurationManager.WorkingDirectory, _testDataRelativePath);

            List<string> DescriptionCollection = ConsoleFlag.IgnoreBreaks.GetCustomAttribute<DescriptionCollectionAttribute>()?.Value.OrderBy(x => x.Length).ToList();
            _flagDescriptorShort = DescriptionCollection[0];
            _flagDescriptorLong = DescriptionCollection[1];
        }
        #endregion Setup

        #region Tests..
        #region FlagFirst..
        #region IgnoreBreaks_FlagFirst_FlagShort
        [Test]
        public async Task IgnoreBreaks_FlagFirst_FlagShort()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreBreaks.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "quick.*brown";
            string Command = $"{_flagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion IgnoreBreaks_FlagFirst_FlagShort

        #region IgnoreBreaks_FlagFirst_FlagLong
        [Test]
        public async Task IgnoreBreaks_FlagFirst_FlagLong()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreBreaks.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "quick.*brown";
            string Command = $"{_flagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion IgnoreBreaks_FlagFirst_FlagLong
        #endregion FlagFirst..

        #region FlagMiddle..
        #region IgnoreBreaks_FlagMiddle_FlagShort
        [Test]
        public async Task IgnoreBreaks_FlagMiddle_FlagShort()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreBreaks.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "quick.*brown";
            string Command = $"-f '{TestFilePath}' {_flagDescriptorShort} {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion IgnoreBreaks_FlagMiddle_FlagShort 

        #region IgnoreBreaks_FlagMiddle_FlagLong
        [Test]
        public async Task IgnoreBreaks_FlagMiddle_FlagLong()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreBreaks.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "quick.*brown";
            string Command = $"-f '{TestFilePath}' {_flagDescriptorLong} {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion IgnoreBreaks_FlagMiddle_FlagLong 
        #endregion FlagMiddle..

        #region FlagLast..
        #region IgnoreBreaks_FlagLast_FlagShort
        [Test]
        public async Task IgnoreBreaks_FlagLast_FlagShort()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreBreaks.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "quick.*brown";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorShort}";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion IgnoreBreaks_FlagLast_FlagShort 

        #region IgnoreBreaks_FlagLast_FlagLong
        [Test]
        public async Task IgnoreBreaks_FlagLast_FlagLong()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreBreaks.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "quick.*brown";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorLong}";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion IgnoreBreaks_FlagLast_FlagLong 
        #endregion FlagLast..

        #region IgnoreBreaks_NoFlag
        [Test]
        public async Task IgnoreBreaks_NoFlag()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreBreaks.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "quickbrown";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 0);
        }
        #endregion IgnoreBreaks_NoFlag 

        #region IgnoreBreaks_StartOfLine
        [Test]
        public async Task IgnoreBreaks_StartOfLine()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreBreaks.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "^T";
            string Command = $"-f '{TestFilePath}' {SearchTerm} -b";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion IgnoreBreaks_StartOfLine

        #region IgnoreBreaks_StartOfLine_NoFlag
        [Test]
        public async Task IgnoreBreaks_StartOfLine_NoFlag()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreBreaks.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "^a";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 3);
        }
        #endregion IgnoreBreaks_StartOfLine_NoFlag

        #region IgnoreBreaks_EndOfLine
        [Test]
        public async Task IgnoreBreaks_EndOfLine()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreBreaks.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "d$";
            string Command = $"-f '{TestFilePath}' {SearchTerm} -b";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion IgnoreBreaks_EndOfLine

        #region IgnoreBreaks_EndOfLine_NoFlag
        [Test]
        public async Task IgnoreBreaks_EndOfLine_NoFlag()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreBreaks.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "d$";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 3);
        }
        #endregion IgnoreBreaks_EndOfLine_NoFlag

        #region IgnoreBreaks_StartOfLineToEndOfLine
        [Test]
        public async Task IgnoreBreaks_StartOfLineToEndOfLine()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreBreaks.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "^T.*$";
            string Command = $"-f '{TestFilePath}' {SearchTerm} -b";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion IgnoreBreaks_StartOfLineToEndOfLine

        #region IgnoreBreaks_StartOfLineToEndOfLine_NoFlag
        [Test]
        public async Task IgnoreBreaks_StartOfLineToEndOfLine_NoFlag()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreBreaks.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "^a.*$";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 3);
        }
        #endregion IgnoreBreaks_StartOfLineToEndOfLine_NoFlag
        #endregion Tests..
    }
}
