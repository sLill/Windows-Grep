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
        #region Recursive_FlagFirst_FlagShort_SingleType
        [Test]
        public void Recursive_FlagFirst_FlagShort_SingleType()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".cpp";
            string Command = $"{_FlagDescriptorShort} {FlagParameter} -d '{TestDataDirectory}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion Recursive_FlagFirst_FlagShort_SingleType 

        #region Recursive_FlagFirst_FlagShort_MultiType_CommaDelimited
        [Test]
        public void Recursive_FlagFirst_FlagShort_MultiType_CommaDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".cpp,.txt,.php";
            string Command = $"{_FlagDescriptorShort} {FlagParameter} -d '{TestDataDirectory}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 4);
        }
        #endregion Recursive_FlagFirst_FlagShort_MultiType_CommaDelimited 

        #region Recursive_FlagFirst_FlagShort_MultiType_SemiColonDelimited
        [Test]
        public void Recursive_FlagFirst_FlagShort_MultiType_SemiColonDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".cpp;.txt";
            string Command = $"{_FlagDescriptorShort} {FlagParameter} -d '{TestDataDirectory}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion Recursive_FlagFirst_FlagShort_MultiType_SemiColonDelimited 

        #region Recursive_FlagFirst_FlagLong_SingleType
        [Test]
        public void IgnoreCase_FlagFirst_FlagLong_SingleType()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".txt";
            string Command = $"{_FlagDescriptorLong} {FlagParameter} -d '{TestDataDirectory}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion Recursive_FlagFirst_FlagLong_SingleType 

        #region IgnoreCase_FlagFirst_FlagLong_MultiType_CommaDelimited
        [Test]
        public void IgnoreCase_FlagFirst_FlagLong_MultiType_CommaDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".txt,.php";
            string Command = $"{_FlagDescriptorLong} {FlagParameter} -d '{TestDataDirectory}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 3);
        }
        #endregion IgnoreCase_FlagFirst_FlagLong_MultiType_CommaDelimited 

        #region IgnoreCase_FlagFirst_FlagLong_MultiType_SemiColonDelimited
        [Test]
        public void IgnoreCase_FlagFirst_FlagLong_MultiType_SemiColonDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".txt;.php";
            string Command = $"{_FlagDescriptorLong} {FlagParameter} -d '{TestDataDirectory}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 3);
        }
        #endregion IgnoreCase_FlagFirst_FlagLong_MultiType_SemiColonDelimited 
        #endregion FlagFirst..

        #region FlagMiddle..
        #region Recursive_FlagMiddle_FlagShort_SingleType
        [Test]
        public void Recursive_FlagMiddle_FlagShort_SingleType()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".html";
            string Command = $"-d '{TestDataDirectory}' {_FlagDescriptorShort} {FlagParameter} {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion Recursive_FlagMiddle_FlagShort_SingleType 

        #region Recursive_FlagMiddle_FlagShort_MultiType_CommaDelimited
        [Test]
        public void Recursive_FlagMiddle_FlagShort_MultiType_CommaDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".html,.txt";
            string Command = $"-d '{TestDataDirectory}' {_FlagDescriptorShort} {FlagParameter} {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion Recursive_FlagMiddle_FlagShort_MultiType_CommaDelimited 

        #region Recursive_FlagMiddle_FlagShort_MultiType_SemiColonDelimited
        [Test]
        public void Recursive_FlagMiddle_FlagShort_MultiType_SemiColonDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".html;.txt";
            string Command = $"-d '{TestDataDirectory}' {_FlagDescriptorShort} {FlagParameter} {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion Recursive_FlagMiddle_FlagShort_MultiType_SemiColonDelimited 

        #region Recursive_FlagMiddle_FlagLong_SingleType
        [Test]
        public void IgnoreCase_FlagMiddle_FlagLong_SingleType()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".php";
            string Command = $"-d '{TestDataDirectory}' {_FlagDescriptorLong} {FlagParameter} {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion Recursive_FlagMiddle_FlagLong_SingleType 

        #region IgnoreCase_FlagMiddle_FlagLong_MultiType_CommaDelimited
        [Test]
        public void IgnoreCase_FlagMiddle_FlagLong_MultiType_CommaDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".php,.wg";
            string Command = $"-d '{TestDataDirectory}' {_FlagDescriptorLong} {FlagParameter} {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 3);
        }
        #endregion IgnoreCase_FlagMiddle_FlagLong_MultiType_CommaDelimited 

        #region IgnoreCase_FlagMiddle_FlagLong_MultiType_SemiColonDelimited
        [Test]
        public void IgnoreCase_FlagMiddle_FlagLong_MultiType_SemiColonDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".php;.wg";
            string Command = $"-d '{TestDataDirectory}' {_FlagDescriptorLong} {FlagParameter} {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 3);
        }
        #endregion IgnoreCase_FlagMiddle_FlagLong_MultiType_SemiColonDelimited 
        #endregion FlagMiddle..

        #region FlagLast..
        #region Recursive_FlagLast_FlagShort_SingleType
        [Test]
        public void Recursive_FlagLast_FlagShort_SingleType()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".cpp";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_FlagDescriptorShort} {FlagParameter}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion Recursive_FlagLast_FlagShort_SingleType 

        #region Recursive_FlagLast_FlagShort_MultiType_CommaDelimited
        [Test]
        public void Recursive_FlagLast_FlagShort_MultiType_CommaDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".cpp,.html";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_FlagDescriptorShort} {FlagParameter}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion Recursive_FlagLast_FlagShort_MultiType_CommaDelimited 

        #region Recursive_FlagLast_FlagShort_MultiType_SemiColonDelimited
        [Test]
        public void Recursive_FlagLast_FlagShort_MultiType_SemiColonDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".cpp;.html";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_FlagDescriptorShort} {FlagParameter}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion Recursive_FlagLast_FlagShort_MultiType_SemiColonDelimited 

        #region Recursive_FlagLast_FlagLong_SingleType
        [Test]
        public void IgnoreCase_FlagLast_FlagLong_SingleType()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".txt";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_FlagDescriptorLong} {FlagParameter}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion Recursive_FlagLast_FlagLong_SingleType 

        #region IgnoreCase_FlagLast_FlagLong_MultiType_CommaDelimited
        [Test]
        public void IgnoreCase_FlagLast_FlagLong_MultiType_CommaDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".txt,.wg";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_FlagDescriptorLong} {FlagParameter}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion IgnoreCase_FlagLast_FlagLong_MultiType_CommaDelimited 

        #region IgnoreCase_FlagLast_FlagLong_MultiType_SemiColonDelimited
        [Test]
        public void IgnoreCase_FlagLast_FlagLong_MultiType_SemiColonDelimited()
        {
            string SearchTerm = "quick brown fox";
            string FlagParameter = ".txt;.wg";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_FlagDescriptorLong} {FlagParameter}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion IgnoreCase_FlagLast_FlagLong_MultiType_SemiColonDelimited 
        #endregion FlagLast..
        #endregion Tests..
    }
}
