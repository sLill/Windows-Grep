using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WindowsGrep.Common;
using WindowsGrep.Engine;

namespace WindowsGrep.Test.Flags.Write
{
    public class WriteTests : TestBase
    {
        #region Member Variables..
        private string _FlagDescriptorShort;
        private string _FlagDescriptorLong;
        private string _TestDataRelativePath = @"Flags\Write\TestData";
        private string _testOutputFilePath;
        #endregion Member Variables..

        #region Properties..
        #endregion Properties..

        #region Setup
        [SetUp]
        public void Setup()
        {
            TestDataDirectory = Path.Combine(TestConfigurationManager.ProjectDirectory, _TestDataRelativePath);
            _testOutputFilePath = Path.Combine(TestDataDirectory, "WriteOutput.txt");

            List<string> DescriptionCollection = ConsoleFlag.Write.GetCustomAttribute<DescriptionCollectionAttribute>()?.Value.OrderBy(x => x.Length).ToList();
            _FlagDescriptorShort = DescriptionCollection[0];
            _FlagDescriptorLong = DescriptionCollection[1];
        }
        #endregion Setup

        #region Tests..
        #region FlagFirst..
        #region Write_FlagFirst_FlagShort
        [Test]
        public void Write_FlagFirst_FlagShort()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "Write.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "quick brown";
            string Command = $"{_FlagDescriptorShort} '{_testOutputFilePath}' -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(File.Exists(_testOutputFilePath));
            Assert.IsTrue(FileHasData(_testOutputFilePath));

            File.Delete(_testOutputFilePath);
        }
        #endregion Write_FlagFirst_FlagShort 
        #endregion FlagFirst..

        #region FlagMiddle..
        #region Write_FlagMiddle_FlagShort
        [Test]
        public void Write_FlagMiddle_FlagShort()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "Write.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "quick brown";
            string Command = $"-f '{TestFilePath}' {_FlagDescriptorShort} '{_testOutputFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(File.Exists(_testOutputFilePath));
            Assert.IsTrue(FileHasData(_testOutputFilePath));

            File.Delete(_testOutputFilePath);
        }
        #endregion Write_FlagMiddle_FlagShort 
        #endregion FlagMiddle..

        #region FlagLast..
        #region Write_FlagLast_FlagShort
        [Test]
        public void Write_FlagLast_FlagShort()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "Write.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "quick brown";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorShort} '{_testOutputFilePath}'";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(File.Exists(_testOutputFilePath));
            Assert.IsTrue(FileHasData(_testOutputFilePath));

            File.Delete(_testOutputFilePath);
        }
        #endregion Write_FlagLast_FlagShort 
        #endregion FlagLast..
        #endregion Tests..

        #region Methods..
        private bool FileHasData(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            return fileInfo.Length > 0;
        }
        #endregion Methods..
    }
}
