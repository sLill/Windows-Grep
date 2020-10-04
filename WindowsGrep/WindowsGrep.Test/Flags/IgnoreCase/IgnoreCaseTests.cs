using NUnit.Framework;
using System.IO;
using System.Linq;
using WindowsGrep.Common;
using WindowsGrep.Engine;

namespace WindowsGrep.Test.Flags.IgnoreCase
{
    public class IgnoreCaseTests : TestBase
    {
        #region Member Variables..
        private string _testDataRelativePath = @"Flags\IgnoreCase\TestData";
        #endregion Member Variables..

        #region Properties..
        #endregion Properties..

        #region Setup
        [SetUp]
        public void Setup()
        {
            TestDataDirectory = Path.Combine(TestConfigurationManager.ProjectDirectory, _testDataRelativePath);
        }
        #endregion Setup

        #region Tests..
        #region FlagFirst..
        #region IgnoreCase_FlagFirst_FlagShort
        [Test]
        public void IgnoreCase_FlagFirst_FlagShort()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreCase.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox";
            string Command = $"-i -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion IgnoreCase_FlagFirst_FlagShort 

        #region IgnoreCase_FlagFirst_FlagShort
        [Test]
        public void IgnoreCase_FlagFirst_FlagLong()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreCase.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox";
            string Command = $"--ignore-case -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion IgnoreCase_FlagFirst_FlagShort 
        #endregion FlagFirst..
        #endregion Tests..
    }
}
