using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WindowsGrep.Common;
using WindowsGrep.Engine;

namespace WindowsGrep.Test.Flags.TargetFile
{
    public class TargetFileTests : TestBase
    {
        #region Member Variables..
        private string _FlagDescriptorShort;
        private string _FlagDescriptorLong;
        private string _TestDataRelativePath = @"Flags\TargetFile\TestData";
        #endregion Member Variables..

        #region Properties..
        #endregion Properties..

        #region Setup
        [SetUp]
        public void Setup()
        {
            TestDataDirectory = Path.Combine(TestConfigurationManager.ProjectDirectory, _TestDataRelativePath);

            List<string> DescriptionCollection = ConsoleFlag.TargetFile.GetCustomAttribute<DescriptionCollectionAttribute>()?.Value.OrderBy(x => x.Length).ToList();
            _FlagDescriptorShort = DescriptionCollection[0];
            _FlagDescriptorLong = DescriptionCollection[1];
        }
        #endregion Setup

        #region Tests..
        #region FlagFirst..
        #region TargetFile_FlagFirst_FlagShort
        [Test]
        public void TargetFile_FlagFirst_FlagShort()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "TargetFile.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox";
            string Command = $"{_FlagDescriptorShort} '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion TargetFile_FlagFirst_FlagShort 

        #region TargetFile_FlagFirst_FlagLong
        [Test]
        public void TargetFile_FlagFirst_FlagLong()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "TargetFile.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox";
            string Command = $"{_FlagDescriptorLong} '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion TargetFile_FlagFirst_FlagLong 
        #endregion FlagFirst..

        #region FlagMiddle..
        #region TargetFile_FlagMiddle_FlagShort
        [Test]
        public void TargetFile_FlagMiddle_FlagShort()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "TargetFile.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox";
            string Command = $"-i {_FlagDescriptorShort} '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion TargetFile_FlagMiddle_FlagShort 

        #region TargetFile_FlagMiddle_FlagLong
        [Test]
        public void TargetFile_FlagMiddle_FlagLong()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "TargetFile.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox";
            string Command = $"-i {_FlagDescriptorLong} '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion TargetFile_FlagMiddle_FlagLong 
        #endregion FlagMiddle..

        #region FlagLast..
        #region TargetFile_FlagLast_FlagShort
        [Test]
        public void TargetFile_FlagLast_FlagShort()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "TargetFile.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox";
            string Command = $"{SearchTerm} {_FlagDescriptorShort} '{TestFilePath}'";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion TargetFile_FlagLast_FlagShort 

        #region TargetFile_FlagLast_FlagLong
        [Test]
        public void TargetFile_FlagLast_FlagLong()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "TargetFile.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox";
            string Command = $"{SearchTerm} {_FlagDescriptorLong} '{TestFilePath}'";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion TargetFile_FlagLast_FlagLong 
        #endregion FlagLast..
        #endregion Tests..
    }
}
