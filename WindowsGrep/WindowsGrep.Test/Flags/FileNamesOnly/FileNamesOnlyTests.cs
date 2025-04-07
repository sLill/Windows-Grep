using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WindowsGrep.Common;
using WindowsGrep.Core;
using WindowsGrep.Engine;

namespace WindowsGrep.Test.Flags.FileNamesOnly
{
    public class FileNamesOnlyTests : TestBase
    {
        #region Fields..
        private string _flagDescriptorShort;
        private string _flagDescriptorLong;
        private string _testDataRelativePath = @"Flags\FileNamesOnly\TestData";
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Setup
        [SetUp]
        public void Setup()
        {
            TestDataDirectory = Path.Combine(TestConfigurationManager.WorkingDirectory, _testDataRelativePath);

            List<string> DescriptionCollection = ConsoleFlag.FileNamesOnly.GetCustomAttribute<DescriptionCollectionAttribute>()?.Value.OrderBy(x => x.Length).ToList();
            _flagDescriptorShort = DescriptionCollection[0];
            _flagDescriptorLong = DescriptionCollection[1];
        }
        #endregion Setup

        #region Tests..
        #region FlagFirst..
        #region FileNamesOnly_FlagFirst_FlagShort_SingleResult
        [Test]
        public async Task FileNamesOnly_FlagFirst_FlagShort_SingleResult()
        {
            string SearchTerm = "One";
            string Command = $"{_flagDescriptorShort} -d '{TestDataDirectory}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion FileNamesOnly_FlagFirst_FlagShort_SingleResult 

        #region FileNamesOnly_FlagFirst_FlagShort_MultiResult
        [Test]
        public async Task FileNamesOnly_FlagFirst_FlagShort_MultiResult()
        {
            string SearchTerm = "FileNamesOnly";
            string Command = $"{_flagDescriptorShort} -d '{TestDataDirectory}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 4);
        }
        #endregion FileNamesOnly_FlagFirst_FlagShort_MultiResult 

        #region FileNamesOnly_FlagFirst_FlagLong_SingleResult
        [Test]
        public async Task FileNamesOnly_FlagFirst_FlagLong_SingleResult()
        {
            string SearchTerm = "One";
            string Command = $"{_flagDescriptorLong} -d '{TestDataDirectory}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion FileNamesOnly_FlagFirst_FlagLong_SingleResult

        #region FileNamesOnly_FlagFirst_FlagLong_MultiResult
        [Test]
        public async Task FileNamesOnly_FlagFirst_FlagLong_MultiResult()
        {
            string SearchTerm = "FileNamesOnly";
            string Command = $"{_flagDescriptorLong} -d '{TestDataDirectory}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 4);
        }
        #endregion FileNamesOnly_FlagFirst_FlagLong_MultiResult
        #endregion FlagFirst..

        #region FlagMiddle..
        #region FileNamesOnly_FlagMiddle_FlagShort_SingleResult
        [Test]
        public async Task FileNamesOnly_FlagMiddle_FlagShort_SingleResult()
        {
            string SearchTerm = "One";
            string Command = $"-i {_flagDescriptorShort} -d '{TestDataDirectory}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion FileNamesOnly_FlagMiddle_FlagShort_SingleResult 

        #region FileNamesOnly_FlagMiddle_FlagShort_MultiResult
        [Test]
        public async Task FileNamesOnly_FlagMiddle_FlagShort_MultiResult()
        {
            string SearchTerm = "FileNamesOnly";
            string Command = $"-i {_flagDescriptorShort} -d '{TestDataDirectory}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 5);
        }
        #endregion FileNamesOnly_FlagMiddle_FlagShort_MultiResult 

        #region FileNamesOnly_FlagMiddle_FlagLong_SingleResult
        [Test]
        public async Task FileNamesOnly_FlagMiddle_FlagLong_SingleResult()
        {
            string SearchTerm = "One";
            string Command = $"-i {_flagDescriptorLong} -d '{TestDataDirectory}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion FileNamesOnly_FlagMiddle_FlagLong_SingleResult

        #region FileNamesOnly_FlagMiddle_FlagLong_MultiResult
        [Test]
        public async Task FileNamesOnly_FlagMiddle_FlagLong_MultiResult()
        {
            string SearchTerm = "FileNamesOnly";
            string Command = $"-i {_flagDescriptorLong} -d '{TestDataDirectory}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 5);
        }
        #endregion FileNamesOnly_FlagMiddle_FlagLong_MultiResult
        #endregion FlagMiddle..

        #region FlagLast..
        #region FileNamesOnly_FlagLast_FlagShort_SingleResult
        [Test]
        public async Task FileNamesOnly_FlagLast_FlagShort_SingleResult()
        {
            string SearchTerm = "One";
            string Command = $"{SearchTerm} {_flagDescriptorShort} -d '{TestDataDirectory}'";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion FileNamesOnly_FlagLast_FlagShort_SingleResult 

        #region FileNamesOnly_FlagLast_FlagShort_MultiResult
        [Test]
        public async Task FileNamesOnly_FlagLast_FlagShort_MultiResult()
        {
            string SearchTerm = "FileNamesOnly";
            string Command = $"{SearchTerm} {_flagDescriptorShort} -d '{TestDataDirectory}'";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 4);
        }
        #endregion FileNamesOnly_FlagLast_FlagShort_MultiResult 

        #region FileNamesOnly_FlagLast_FlagLong_SingleResult
        [Test]
        public async Task FileNamesOnly_FlagLast_FlagLong_SingleResult()
        {
            string SearchTerm = "One";
            string Command = $"{SearchTerm} {_flagDescriptorLong} -d '{TestDataDirectory}'";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion FileNamesOnly_FlagLast_FlagLong_SingleResult

        #region FileNamesOnly_FlagLast_FlagLong_MultiResult
        [Test]
        public async Task FileNamesOnly_FlagLast_FlagLong_MultiResult()
        {
            string SearchTerm = "FileNamesOnly";
            string Command = $"{SearchTerm} {_flagDescriptorLong} -d '{TestDataDirectory}'";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 4);
        }
        #endregion FileNamesOnly_FlagLast_FlagLong_MultiResult
        #endregion FlagLast..
        #endregion Tests..
    }
}
