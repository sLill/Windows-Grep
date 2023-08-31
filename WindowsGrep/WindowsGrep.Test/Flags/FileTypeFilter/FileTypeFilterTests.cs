using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WindowsGrep.Common;
using WindowsGrep.Core;
using WindowsGrep.Engine;

namespace WindowsGrep.Test.Flags.FileTypeFilter
{
    public class FileTypeFilterTests : TestBase
    {
        #region Fields..
        private string _FlagDescriptorShort;
        private string _FlagDescriptorLong;
        private string _TestDataRelativePath = @"Flags\FileTypeFilter\TestData";
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Setup
        [SetUp]
        public void Setup()
        {
            TestDataDirectory = Path.Combine(TestConfigurationManager.ProjectDirectory, _TestDataRelativePath);

            List<string> DescriptionCollection = ConsoleFlag.FileTypeFilter.GetCustomAttribute<DescriptionCollectionAttribute>()?.Value.OrderBy(x => x.Length).ToList();
            _FlagDescriptorShort = DescriptionCollection[0];
            _FlagDescriptorLong = DescriptionCollection[1];
        }
        #endregion Setup

        #region Tests..
        #region FlagFirst..
        #region FileTypeFilter_FlagFirst_FlagShort_SingleType
        [Test]
        public void FileTypeFilter_FlagFirst_FlagShort_SingleType()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".cpp";
            string Command = $"{_FlagDescriptorShort} {FlagParameter} -d '{TestDataDirectory}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            WindowsGrep.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FileTypeFilter_FlagFirst_FlagShort_SingleType 

        #region FileTypeFilter_FlagFirst_FlagShort_MultiType_CommaDelimited
        [Test]
        public void FileTypeFilter_FlagFirst_FlagShort_MultiType_CommaDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".cpp,.txt,.php";
            string Command = $"{_FlagDescriptorShort} {FlagParameter} -d '{TestDataDirectory}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            WindowsGrep.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count == 4);
        }
        #endregion FileTypeFilter_FlagFirst_FlagShort_MultiType_CommaDelimited 

        #region FileTypeFilter_FlagFirst_FlagShort_MultiType_SemiColonDelimited
        [Test]
        public void FileTypeFilter_FlagFirst_FlagShort_MultiType_SemiColonDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".cpp;.txt";
            string Command = $"{_FlagDescriptorShort} {FlagParameter} -d '{TestDataDirectory}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            WindowsGrep.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FileTypeFilter_FlagFirst_FlagShort_MultiType_SemiColonDelimited 

        #region FileTypeFilter_FlagFirst_FlagLong_SingleType
        [Test]
        public void FileTypeFilter_FlagFirst_FlagLong_SingleType()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".txt";
            string Command = $"{_FlagDescriptorLong} {FlagParameter} -d '{TestDataDirectory}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            WindowsGrep.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FileTypeFilter_FlagFirst_FlagLong_SingleType 

        #region FileTypeFilter_FlagFirst_FlagLong_MultiType_SemiColonDelimited
        [Test]
        public void FileTypeFilter_FlagFirst_FlagLong_MultiType_SemiColonDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".txt;.php";
            string Command = $"{_FlagDescriptorLong} {FlagParameter} -d '{TestDataDirectory}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            WindowsGrep.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count == 3);
        }
        #endregion FileTypeFilter_FlagFirst_FlagLong_MultiType_SemiColonDelimited 
        #endregion FlagFirst..

        #region FlagMiddle..
        #region FileTypeFilter_FlagMiddle_FlagShort_SingleType
        [Test]
        public void FileTypeFilter_FlagMiddle_FlagShort_SingleType()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".html";
            string Command = $"-d '{TestDataDirectory}' {_FlagDescriptorShort} {FlagParameter} {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            WindowsGrep.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FileTypeFilter_FlagMiddle_FlagShort_SingleType 

        #region FileTypeFilter_FlagMiddle_FlagShort_MultiType_CommaDelimited
        [Test]
        public void FileTypeFilter_FlagMiddle_FlagShort_MultiType_CommaDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".html,.txt";
            string Command = $"-d '{TestDataDirectory}' {_FlagDescriptorShort} {FlagParameter} {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            WindowsGrep.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FileTypeFilter_FlagMiddle_FlagShort_MultiType_CommaDelimited 

        #region FileTypeFilter_FlagMiddle_FlagShort_MultiType_SemiColonDelimited
        [Test]
        public void FileTypeFilter_FlagMiddle_FlagShort_MultiType_SemiColonDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".html;.txt";
            string Command = $"-d '{TestDataDirectory}' {_FlagDescriptorShort} {FlagParameter} {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            WindowsGrep.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FileTypeFilter_FlagMiddle_FlagShort_MultiType_SemiColonDelimited 

        #region FileTypeFilter_FlagMiddle_FlagLong_SingleType
        [Test]
        public void FileTypeFilter_FlagMiddle_FlagLong_SingleType()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".php";
            string Command = $"-d '{TestDataDirectory}' {_FlagDescriptorLong} {FlagParameter} {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            WindowsGrep.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FileTypeFilter_FlagMiddle_FlagLong_SingleType 

        #region FileTypeFilter_FlagMiddle_FlagLong_MultiType_CommaDelimited
        [Test]
        public void FileTypeFilter_FlagMiddle_FlagLong_MultiType_CommaDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".php,.wg";
            string Command = $"-d '{TestDataDirectory}' {_FlagDescriptorLong} {FlagParameter} {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            WindowsGrep.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count == 3);
        }
        #endregion FileTypeFilter_FlagMiddle_FlagLong_MultiType_CommaDelimited 

        #region FileTypeFilter_FlagMiddle_FlagLong_MultiType_SemiColonDelimited
        [Test]
        public void FileTypeFilter_FlagMiddle_FlagLong_MultiType_SemiColonDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".php;.wg";
            string Command = $"-d '{TestDataDirectory}' {_FlagDescriptorLong} {FlagParameter} {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            WindowsGrep.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count == 3);
        }
        #endregion FileTypeFilter_FlagMiddle_FlagLong_MultiType_SemiColonDelimited 
        #endregion FlagMiddle..

        #region FlagLast..
        #region FileTypeFilter_FlagLast_FlagShort_SingleType
        [Test]
        public void FileTypeFilter_FlagLast_FlagShort_SingleType()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".cpp";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_FlagDescriptorShort} {FlagParameter}";

            var GrepResultCollection = new GrepResultCollection();
            WindowsGrep.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FileTypeFilter_FlagLast_FlagShort_SingleType 

        #region FileTypeFilter_FlagLast_FlagShort_MultiType_CommaDelimited
        [Test]
        public void FileTypeFilter_FlagLast_FlagShort_MultiType_CommaDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".cpp,.html";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_FlagDescriptorShort} {FlagParameter}";

            var GrepResultCollection = new GrepResultCollection();
            WindowsGrep.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FileTypeFilter_FlagLast_FlagShort_MultiType_CommaDelimited 

        #region FileTypeFilter_FlagLast_FlagShort_MultiType_SemiColonDelimited
        [Test]
        public void FileTypeFilter_FlagLast_FlagShort_MultiType_SemiColonDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".cpp;.html";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_FlagDescriptorShort} {FlagParameter}";

            var GrepResultCollection = new GrepResultCollection();
            WindowsGrep.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FileTypeFilter_FlagLast_FlagShort_MultiType_SemiColonDelimited 

        #region FileTypeFilter_FlagLast_FlagLong_SingleType
        [Test]
        public void FileTypeFilter_FlagLast_FlagLong_SingleType()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".txt";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_FlagDescriptorLong} {FlagParameter}";

            var GrepResultCollection = new GrepResultCollection();
            WindowsGrep.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FileTypeFilter_FlagLast_FlagLong_SingleType 

        #region FileTypeFilter_FlagLast_FlagLong_MultiType_CommaDelimited
        [Test]
        public void FileTypeFilter_FlagLast_FlagLong_MultiType_CommaDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".txt,.wg";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_FlagDescriptorLong} {FlagParameter}";

            var GrepResultCollection = new GrepResultCollection();
            WindowsGrep.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FileTypeFilter_FlagLast_FlagLong_MultiType_CommaDelimited 

        #region FileTypeFilter_FlagLast_FlagLong_MultiType_SemiColonDelimited
        [Test]
        public void FileTypeFilter_FlagLast_FlagLong_MultiType_SemiColonDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".txt;.wg";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_FlagDescriptorLong} {FlagParameter}";

            var GrepResultCollection = new GrepResultCollection();
            WindowsGrep.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FileTypeFilter_FlagLast_FlagLong_MultiType_SemiColonDelimited 
        #endregion FlagLast..

        #region FileTypeFilter_FileTypeNotExists
        [Test]
        public void FileTypeFilter_FileTypeNotExists()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".tig";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_FlagDescriptorLong} {FlagParameter}";

            var GrepResultCollection = new GrepResultCollection();
            WindowsGrep.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count == 0);
        }
        #endregion FileTypeFilter_FileTypeNotExists 
        #endregion Tests..
    }
}
