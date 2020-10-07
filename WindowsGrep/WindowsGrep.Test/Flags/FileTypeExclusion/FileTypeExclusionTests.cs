using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WindowsGrep.Common;
using WindowsGrep.Engine;

namespace WindowsGrep.Test.Flags.FileTypeExclusion
{
    public class FileTypeExclusionTests : TestBase
    {
        #region Member Variables..
        private string _FlagDescriptorShort;
        private string _FlagDescriptorLong;
        private string _TestDataRelativePath = @"Flags\FileTypeExclusion\TestData";
        #endregion Member Variables..

        #region Properties..
        #endregion Properties..

        #region Setup
        [SetUp]
        public void Setup()
        {
            TestDataDirectory = Path.Combine(TestConfigurationManager.ProjectDirectory, _TestDataRelativePath);

            List<string> DescriptionCollection = ConsoleFlag.FileTypeExclusions.GetCustomAttribute<DescriptionCollectionAttribute>()?.Value.OrderBy(x => x.Length).ToList();
            _FlagDescriptorShort = DescriptionCollection[0];
            _FlagDescriptorLong = DescriptionCollection[1];
        }
        #endregion Setup

        #region Tests..
        #region FlagFirst..
        #region FileTypeExclusion_FlagFirst_FlagShort_SingleType
        [Test]
        public void FileTypeExclusion_FlagFirst_FlagShort_SingleType()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".cpp";
            string Command = $"{_FlagDescriptorShort} {FlagParameter} -d '{TestDataDirectory}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 6);
        }
        #endregion FileTypeExclusion_FlagFirst_FlagShort_SingleType 

        #region FileTypeExclusion_FlagFirst_FlagShort_MultiType_CommaDelimited
        [Test]
        public void FileTypeExclusion_FlagFirst_FlagShort_MultiType_CommaDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".cpp,.txt,.php";
            string Command = $"{_FlagDescriptorShort} {FlagParameter} -d '{TestDataDirectory}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 3);
        }
        #endregion FileTypeExclusion_FlagFirst_FlagShort_MultiType_CommaDelimited 

        #region FileTypeExclusion_FlagFirst_FlagShort_MultiType_SemiColonDelimited
        [Test]
        public void FileTypeExclusion_FlagFirst_FlagShort_MultiType_SemiColonDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".cpp;.txt";
            string Command = $"{_FlagDescriptorShort} {FlagParameter} -d '{TestDataDirectory}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 5);
        }
        #endregion FileTypeExclusion_FlagFirst_FlagShort_MultiType_SemiColonDelimited 

        #region FileTypeExclusion_FlagFirst_FlagLong_SingleType
        [Test]
        public void FileTypeExclusion_FlagFirst_FlagLong_SingleType()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".txt";
            string Command = $"{_FlagDescriptorLong} {FlagParameter} -d '{TestDataDirectory}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 6);
        }
        #endregion FileTypeExclusion_FlagFirst_FlagLong_SingleType 

        #region FileTypeExclusion_FlagFirst_FlagLong_MultiType_CommaDelimited
        [Test]
        public void FileTypeExclusion_FlagFirst_FlagLong_MultiType_CommaDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".txt,.php";
            string Command = $"{_FlagDescriptorLong} {FlagParameter} -d '{TestDataDirectory}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 4);
        }
        #endregion FileTypeExclusion_FlagFirst_FlagLong_MultiType_CommaDelimited 

        #region FileTypeExclusion_FlagFirst_FlagLong_MultiType_SemiColonDelimited
        [Test]
        public void FileTypeExclusion_FlagFirst_FlagLong_MultiType_SemiColonDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".txt;.php";
            string Command = $"{_FlagDescriptorLong} {FlagParameter} -d '{TestDataDirectory}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 4);
        }
        #endregion FileTypeExclusion_FlagFirst_FlagLong_MultiType_SemiColonDelimited 
        #endregion FlagFirst..

        #region FlagMiddle..
        #region FileTypeExclusion_FlagMiddle_FlagShort_SingleType
        [Test]
        public void FileTypeExclusion_FlagMiddle_FlagShort_SingleType()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".html";
            string Command = $"-d '{TestDataDirectory}' {_FlagDescriptorShort} {FlagParameter} {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 6);
        }
        #endregion FileTypeExclusion_FlagMiddle_FlagShort_SingleType 

        #region FileTypeExclusion_FlagMiddle_FlagShort_MultiType_CommaDelimited
        [Test]
        public void FileTypeExclusion_FlagMiddle_FlagShort_MultiType_CommaDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".html,.txt";
            string Command = $"-d '{TestDataDirectory}' {_FlagDescriptorShort} {FlagParameter} {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 5);
        }
        #endregion FileTypeExclusion_FlagMiddle_FlagShort_MultiType_CommaDelimited 

        #region FileTypeExclusion_FlagMiddle_FlagShort_MultiType_SemiColonDelimited
        [Test]
        public void FileTypeExclusion_FlagMiddle_FlagShort_MultiType_SemiColonDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".html;.txt";
            string Command = $"-d '{TestDataDirectory}' {_FlagDescriptorShort} {FlagParameter} {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 5);
        }
        #endregion FileTypeExclusion_FlagMiddle_FlagShort_MultiType_SemiColonDelimited 

        #region FileTypeExclusion_FlagMiddle_FlagLong_SingleType
        [Test]
        public void FileTypeExclusion_FlagMiddle_FlagLong_SingleType()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".php";
            string Command = $"-d '{TestDataDirectory}' {_FlagDescriptorLong} {FlagParameter} {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 5);
        }
        #endregion FileTypeExclusion_FlagMiddle_FlagLong_SingleType 

        #region FileTypeExclusion_FlagMiddle_FlagLong_MultiType_CommaDelimited
        [Test]
        public void FileTypeExclusion_FlagMiddle_FlagLong_MultiType_CommaDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".php,.wg";
            string Command = $"-d '{TestDataDirectory}' {_FlagDescriptorLong} {FlagParameter} {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 4);
        }
        #endregion FileTypeExclusion_FlagMiddle_FlagLong_MultiType_CommaDelimited 

        #region FileTypeExclusion_FlagMiddle_FlagLong_MultiType_SemiColonDelimited
        [Test]
        public void FileTypeExclusion_FlagMiddle_FlagLong_MultiType_SemiColonDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".php;.wg";
            string Command = $"-d '{TestDataDirectory}' {_FlagDescriptorLong} {FlagParameter} {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 4);
        }
        #endregion FileTypeExclusion_FlagMiddle_FlagLong_MultiType_SemiColonDelimited 
        #endregion FlagMiddle..

        #region FlagLast..
        #region FileTypeExclusion_FlagLast_FlagShort_SingleType
        [Test]
        public void FileTypeExclusion_FlagLast_FlagShort_SingleType()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".cpp";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_FlagDescriptorShort} {FlagParameter}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 6);
        }
        #endregion FileTypeExclusion_FlagLast_FlagShort_SingleType 

        #region FileTypeExclusion_FlagLast_FlagShort_MultiType_CommaDelimited
        [Test]
        public void FileTypeExclusion_FlagLast_FlagShort_MultiType_CommaDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".cpp,.html";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_FlagDescriptorShort} {FlagParameter}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 5);
        }
        #endregion FileTypeExclusion_FlagLast_FlagShort_MultiType_CommaDelimited 

        #region FileTypeExclusion_FlagLast_FlagShort_MultiType_SemiColonDelimited
        [Test]
        public void FileTypeExclusion_FlagLast_FlagShort_MultiType_SemiColonDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".cpp;.html";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_FlagDescriptorShort} {FlagParameter}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 5);
        }
        #endregion FileTypeExclusion_FlagLast_FlagShort_MultiType_SemiColonDelimited 

        #region FileTypeExclusion_FlagLast_FlagLong_SingleType
        [Test]
        public void FileTypeExclusion_FlagLast_FlagLong_SingleType()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".txt";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_FlagDescriptorLong} {FlagParameter}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 6);
        }
        #endregion FileTypeExclusion_FlagLast_FlagLong_SingleType 

        #region FileTypeExclusion_FlagLast_FlagLong_MultiType_CommaDelimited
        [Test]
        public void FileTypeExclusion_FlagLast_FlagLong_MultiType_CommaDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".txt,.wg";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_FlagDescriptorLong} {FlagParameter}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 5);
        }
        #endregion FileTypeExclusion_FlagLast_FlagLong_MultiType_CommaDelimited 

        #region FileTypeExclusion_FlagLast_FlagLong_MultiType_SemiColonDelimited
        [Test]
        public void FileTypeExclusion_FlagLast_FlagLong_MultiType_SemiColonDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".txt;.wg";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_FlagDescriptorLong} {FlagParameter}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 5);
        }
        #endregion FileTypeExclusion_FlagLast_FlagLong_MultiType_SemiColonDelimited 
        #endregion FlagLast..

        #region FileTypeExclusion_FileTypeNotExists
        [Test]
        public void FileTypeExclusion_FileTypeNotExists()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".tig";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_FlagDescriptorLong} {FlagParameter}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 7);
        }
        #endregion FileTypeExclusion_FileTypeNotExists 
        #endregion Tests..
    }
}
