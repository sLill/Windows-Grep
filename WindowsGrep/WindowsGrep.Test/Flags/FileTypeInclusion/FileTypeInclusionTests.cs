using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WindowsGrep.Common;
using WindowsGrep.Engine;

namespace WindowsGrep.Test.Flags.FileTypeInclusion
{
    public class FileTypeInclusionTests : TestBase
    {
        #region Member Variables..
        private string _FlagDescriptorShort;
        private string _FlagDescriptorLong;
        private string _TestDataRelativePath = @"Flags\FileTypeInclusion\TestData";
        #endregion Member Variables..

        #region Properties..
        #endregion Properties..

        #region Setup
        [SetUp]
        public void Setup()
        {
            TestDataDirectory = Path.Combine(TestConfigurationManager.ProjectDirectory, _TestDataRelativePath);

            List<string> DescriptionCollection = ConsoleFlag.FileTypeInclusions.GetCustomAttribute<DescriptionCollectionAttribute>()?.Value.OrderBy(x => x.Length).ToList();
            _FlagDescriptorShort = DescriptionCollection[0];
            _FlagDescriptorLong = DescriptionCollection[1];
        }
        #endregion Setup

        #region Tests..
        #region FlagFirst..
        #region FileTypeInclusion_FlagFirst_FlagShort_SingleType
        [Test]
        public void FileTypeInclusion_FlagFirst_FlagShort_SingleType()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".cpp";
            string Command = $"{_FlagDescriptorShort} {FlagParameter} -d '{TestDataDirectory}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FileTypeInclusion_FlagFirst_FlagShort_SingleType 

        #region FileTypeInclusion_FlagFirst_FlagShort_MultiType_CommaDelimited
        [Test]
        public void FileTypeInclusion_FlagFirst_FlagShort_MultiType_CommaDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".cpp,.txt,.php";
            string Command = $"{_FlagDescriptorShort} {FlagParameter} -d '{TestDataDirectory}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 4);
        }
        #endregion FileTypeInclusion_FlagFirst_FlagShort_MultiType_CommaDelimited 

        #region FileTypeInclusion_FlagFirst_FlagShort_MultiType_SemiColonDelimited
        [Test]
        public void FileTypeInclusion_FlagFirst_FlagShort_MultiType_SemiColonDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".cpp;.txt";
            string Command = $"{_FlagDescriptorShort} {FlagParameter} -d '{TestDataDirectory}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FileTypeInclusion_FlagFirst_FlagShort_MultiType_SemiColonDelimited 

        #region FileTypeInclusion_FlagFirst_FlagLong_SingleType
        [Test]
        public void FileTypeInclusion_FlagFirst_FlagLong_SingleType()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".txt";
            string Command = $"{_FlagDescriptorLong} {FlagParameter} -d '{TestDataDirectory}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FileTypeInclusion_FlagFirst_FlagLong_SingleType 

        #region FileTypeInclusion_FlagFirst_FlagLong_MultiType_SemiColonDelimited
        [Test]
        public void FileTypeInclusion_FlagFirst_FlagLong_MultiType_SemiColonDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".txt;.php";
            string Command = $"{_FlagDescriptorLong} {FlagParameter} -d '{TestDataDirectory}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 3);
        }
        #endregion FileTypeInclusion_FlagFirst_FlagLong_MultiType_SemiColonDelimited 
        #endregion FlagFirst..

        #region FlagMiddle..
        #region FileTypeInclusion_FlagMiddle_FlagShort_SingleType
        [Test]
        public void FileTypeInclusion_FlagMiddle_FlagShort_SingleType()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".html";
            string Command = $"-d '{TestDataDirectory}' {_FlagDescriptorShort} {FlagParameter} {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FileTypeInclusion_FlagMiddle_FlagShort_SingleType 

        #region FileTypeInclusion_FlagMiddle_FlagShort_MultiType_CommaDelimited
        [Test]
        public void FileTypeInclusion_FlagMiddle_FlagShort_MultiType_CommaDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".html,.txt";
            string Command = $"-d '{TestDataDirectory}' {_FlagDescriptorShort} {FlagParameter} {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FileTypeInclusion_FlagMiddle_FlagShort_MultiType_CommaDelimited 

        #region FileTypeInclusion_FlagMiddle_FlagShort_MultiType_SemiColonDelimited
        [Test]
        public void FileTypeInclusion_FlagMiddle_FlagShort_MultiType_SemiColonDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".html;.txt";
            string Command = $"-d '{TestDataDirectory}' {_FlagDescriptorShort} {FlagParameter} {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FileTypeInclusion_FlagMiddle_FlagShort_MultiType_SemiColonDelimited 

        #region FileTypeInclusion_FlagMiddle_FlagLong_SingleType
        [Test]
        public void FileTypeInclusion_FlagMiddle_FlagLong_SingleType()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".php";
            string Command = $"-d '{TestDataDirectory}' {_FlagDescriptorLong} {FlagParameter} {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FileTypeInclusion_FlagMiddle_FlagLong_SingleType 

        #region FileTypeInclusion_FlagMiddle_FlagLong_MultiType_CommaDelimited
        [Test]
        public void FileTypeInclusion_FlagMiddle_FlagLong_MultiType_CommaDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".php,.wg";
            string Command = $"-d '{TestDataDirectory}' {_FlagDescriptorLong} {FlagParameter} {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 3);
        }
        #endregion FileTypeInclusion_FlagMiddle_FlagLong_MultiType_CommaDelimited 

        #region FileTypeInclusion_FlagMiddle_FlagLong_MultiType_SemiColonDelimited
        [Test]
        public void FileTypeInclusion_FlagMiddle_FlagLong_MultiType_SemiColonDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".php;.wg";
            string Command = $"-d '{TestDataDirectory}' {_FlagDescriptorLong} {FlagParameter} {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 3);
        }
        #endregion FileTypeInclusion_FlagMiddle_FlagLong_MultiType_SemiColonDelimited 
        #endregion FlagMiddle..

        #region FlagLast..
        #region FileTypeInclusion_FlagLast_FlagShort_SingleType
        [Test]
        public void FileTypeInclusion_FlagLast_FlagShort_SingleType()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".cpp";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_FlagDescriptorShort} {FlagParameter}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FileTypeInclusion_FlagLast_FlagShort_SingleType 

        #region FileTypeInclusion_FlagLast_FlagShort_MultiType_CommaDelimited
        [Test]
        public void FileTypeInclusion_FlagLast_FlagShort_MultiType_CommaDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".cpp,.html";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_FlagDescriptorShort} {FlagParameter}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FileTypeInclusion_FlagLast_FlagShort_MultiType_CommaDelimited 

        #region FileTypeInclusion_FlagLast_FlagShort_MultiType_SemiColonDelimited
        [Test]
        public void FileTypeInclusion_FlagLast_FlagShort_MultiType_SemiColonDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".cpp;.html";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_FlagDescriptorShort} {FlagParameter}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FileTypeInclusion_FlagLast_FlagShort_MultiType_SemiColonDelimited 

        #region FileTypeInclusion_FlagLast_FlagLong_SingleType
        [Test]
        public void FileTypeInclusion_FlagLast_FlagLong_SingleType()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".txt";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_FlagDescriptorLong} {FlagParameter}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FileTypeInclusion_FlagLast_FlagLong_SingleType 

        #region FileTypeInclusion_FlagLast_FlagLong_MultiType_CommaDelimited
        [Test]
        public void FileTypeInclusion_FlagLast_FlagLong_MultiType_CommaDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".txt,.wg";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_FlagDescriptorLong} {FlagParameter}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FileTypeInclusion_FlagLast_FlagLong_MultiType_CommaDelimited 

        #region FileTypeInclusion_FlagLast_FlagLong_MultiType_SemiColonDelimited
        [Test]
        public void FileTypeInclusion_FlagLast_FlagLong_MultiType_SemiColonDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".txt;.wg";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_FlagDescriptorLong} {FlagParameter}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FileTypeInclusion_FlagLast_FlagLong_MultiType_SemiColonDelimited 
        #endregion FlagLast..

        #region FileTypeInclusion_FileTypeNotExists
        [Test]
        public void FileTypeInclusion_FileTypeNotExists()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".tig";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_FlagDescriptorLong} {FlagParameter}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 0);
        }
        #endregion FileTypeInclusion_FileTypeNotExists 
        #endregion Tests..
    }
}
