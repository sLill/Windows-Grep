using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WindowsGrep.Common;
using WindowsGrep.Core;
using WindowsGrep.Engine;

namespace WindowsGrep.Test.Flags.FileTypeFilter
{
    public class FileTypeFilterTests : TestBase
    {
        #region Fields..
        private string _flagDescriptorShort;
        private string _flagDescriptorLong;
        private string _testDataRelativePath = @"Flags\FileTypeFilter\TestData";
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Setup
        [SetUp]
        public void Setup()
        {
            TestDataDirectory = Path.Combine(TestConfigurationManager.WorkingDirectory, _testDataRelativePath);

            List<string> DescriptionCollection = ConsoleFlag.FileTypeFilter.GetCustomAttribute<DescriptionCollectionAttribute>()?.Value.OrderBy(x => x.Length).ToList();
            _flagDescriptorShort = DescriptionCollection[0];
            _flagDescriptorLong = DescriptionCollection[1];
        }
        #endregion Setup

        #region Tests..
        #region FlagFirst..
        #region FileTypeFilter_FlagFirst_FlagShort_SingleType
        [Test]
        public async Task FileTypeFilter_FlagFirst_FlagShort_SingleType()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".cpp";
            string Command = $"{_flagDescriptorShort} {FlagParameter} -d '{TestDataDirectory}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion FileTypeFilter_FlagFirst_FlagShort_SingleType 

        #region FileTypeFilter_FlagFirst_FlagShort_MultiType_CommaDelimited
        [Test]
        public async Task FileTypeFilter_FlagFirst_FlagShort_MultiType_CommaDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".cpp,.txt,.php";
            string Command = $"{_flagDescriptorShort} {FlagParameter} -d '{TestDataDirectory}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 4);
        }
        #endregion FileTypeFilter_FlagFirst_FlagShort_MultiType_CommaDelimited 

        #region FileTypeFilter_FlagFirst_FlagShort_MultiType_SemiColonDelimited
        [Test]
        public async Task FileTypeFilter_FlagFirst_FlagShort_MultiType_SemiColonDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".cpp;.txt";
            string Command = $"{_flagDescriptorShort} {FlagParameter} -d '{TestDataDirectory}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 2);
        }
        #endregion FileTypeFilter_FlagFirst_FlagShort_MultiType_SemiColonDelimited 

        #region FileTypeFilter_FlagFirst_FlagLong_SingleType
        [Test]
        public async Task FileTypeFilter_FlagFirst_FlagLong_SingleType()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".txt";
            string Command = $"{_flagDescriptorLong} {FlagParameter} -d '{TestDataDirectory}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion FileTypeFilter_FlagFirst_FlagLong_SingleType 

        #region FileTypeFilter_FlagFirst_FlagLong_MultiType_SemiColonDelimited
        [Test]
        public async Task FileTypeFilter_FlagFirst_FlagLong_MultiType_SemiColonDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".txt;.php";
            string Command = $"{_flagDescriptorLong} {FlagParameter} -d '{TestDataDirectory}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 3);
        }
        #endregion FileTypeFilter_FlagFirst_FlagLong_MultiType_SemiColonDelimited 
        #endregion FlagFirst..

        #region FlagMiddle..
        #region FileTypeFilter_FlagMiddle_FlagShort_SingleType
        [Test]
        public async Task FileTypeFilter_FlagMiddle_FlagShort_SingleType()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".html";
            string Command = $"-d '{TestDataDirectory}' {_flagDescriptorShort} {FlagParameter} {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion FileTypeFilter_FlagMiddle_FlagShort_SingleType 

        #region FileTypeFilter_FlagMiddle_FlagShort_MultiType_CommaDelimited
        [Test]
        public async Task FileTypeFilter_FlagMiddle_FlagShort_MultiType_CommaDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".html,.txt";
            string Command = $"-d '{TestDataDirectory}' {_flagDescriptorShort} {FlagParameter} {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 2);
        }
        #endregion FileTypeFilter_FlagMiddle_FlagShort_MultiType_CommaDelimited 

        #region FileTypeFilter_FlagMiddle_FlagShort_MultiType_SemiColonDelimited
        [Test]
        public async Task FileTypeFilter_FlagMiddle_FlagShort_MultiType_SemiColonDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".html;.txt";
            string Command = $"-d '{TestDataDirectory}' {_flagDescriptorShort} {FlagParameter} {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 2);
        }
        #endregion FileTypeFilter_FlagMiddle_FlagShort_MultiType_SemiColonDelimited 

        #region FileTypeFilter_FlagMiddle_FlagLong_SingleType
        [Test]
        public async Task FileTypeFilter_FlagMiddle_FlagLong_SingleType()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".php";
            string Command = $"-d '{TestDataDirectory}' {_flagDescriptorLong} {FlagParameter} {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 2);
        }
        #endregion FileTypeFilter_FlagMiddle_FlagLong_SingleType 

        #region FileTypeFilter_FlagMiddle_FlagLong_MultiType_CommaDelimited
        [Test]
        public async Task FileTypeFilter_FlagMiddle_FlagLong_MultiType_CommaDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".php,.wg";
            string Command = $"-d '{TestDataDirectory}' {_flagDescriptorLong} {FlagParameter} {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 3);
        }
        #endregion FileTypeFilter_FlagMiddle_FlagLong_MultiType_CommaDelimited 

        #region FileTypeFilter_FlagMiddle_FlagLong_MultiType_SemiColonDelimited
        [Test]
        public async Task FileTypeFilter_FlagMiddle_FlagLong_MultiType_SemiColonDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".php;.wg";
            string Command = $"-d '{TestDataDirectory}' {_flagDescriptorLong} {FlagParameter} {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 3);
        }
        #endregion FileTypeFilter_FlagMiddle_FlagLong_MultiType_SemiColonDelimited 
        #endregion FlagMiddle..

        #region FlagLast..
        #region FileTypeFilter_FlagLast_FlagShort_SingleType
        [Test]
        public async Task FileTypeFilter_FlagLast_FlagShort_SingleType()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".cpp";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_flagDescriptorShort} {FlagParameter}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion FileTypeFilter_FlagLast_FlagShort_SingleType 

        #region FileTypeFilter_FlagLast_FlagShort_MultiType_CommaDelimited
        [Test]
        public async Task FileTypeFilter_FlagLast_FlagShort_MultiType_CommaDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".cpp,.html";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_flagDescriptorShort} {FlagParameter}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 2);
        }
        #endregion FileTypeFilter_FlagLast_FlagShort_MultiType_CommaDelimited 

        #region FileTypeFilter_FlagLast_FlagShort_MultiType_SemiColonDelimited
        [Test]
        public async Task FileTypeFilter_FlagLast_FlagShort_MultiType_SemiColonDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".cpp;.html";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_flagDescriptorShort} {FlagParameter}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 2);
        }
        #endregion FileTypeFilter_FlagLast_FlagShort_MultiType_SemiColonDelimited 

        #region FileTypeFilter_FlagLast_FlagLong_SingleType
        [Test]
        public async Task FileTypeFilter_FlagLast_FlagLong_SingleType()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".txt";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_flagDescriptorLong} {FlagParameter}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion FileTypeFilter_FlagLast_FlagLong_SingleType 

        #region FileTypeFilter_FlagLast_FlagLong_MultiType_CommaDelimited
        [Test]
        public async Task FileTypeFilter_FlagLast_FlagLong_MultiType_CommaDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".txt,.wg";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_flagDescriptorLong} {FlagParameter}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 2);
        }
        #endregion FileTypeFilter_FlagLast_FlagLong_MultiType_CommaDelimited 

        #region FileTypeFilter_FlagLast_FlagLong_MultiType_SemiColonDelimited
        [Test]
        public async Task FileTypeFilter_FlagLast_FlagLong_MultiType_SemiColonDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".txt;.wg";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_flagDescriptorLong} {FlagParameter}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 2);
        }
        #endregion FileTypeFilter_FlagLast_FlagLong_MultiType_SemiColonDelimited 
        #endregion FlagLast..

        #region FileTypeFilter_FileTypeNotExists
        [Test]
        public async Task FileTypeFilter_FileTypeNotExists()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".tig";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_flagDescriptorLong} {FlagParameter}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 0);
        }
        #endregion FileTypeFilter_FileTypeNotExists 
        #endregion Tests..
    }
}
