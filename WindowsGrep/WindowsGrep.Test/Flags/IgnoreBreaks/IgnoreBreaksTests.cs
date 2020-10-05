using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WindowsGrep.Common;
using WindowsGrep.Engine;

namespace WindowsGrep.Test.Flags.IgnoreBreaks
{
    public class IgnoreBreaksTests : TestBase
    {
        #region Member Variables..
        private string _FlagDescriptorShort;
        private string _FlagDescriptorLong;
        private string _TestDataRelativePath = @"Flags\IgnoreBreaks\TestData";
        #endregion Member Variables..

        #region Properties..
        #endregion Properties..

        #region Setup
        [SetUp]
        public void Setup()
        {
            TestDataDirectory = Path.Combine(TestConfigurationManager.ProjectDirectory, _TestDataRelativePath);

            List<string> DescriptionCollection = ConsoleFlag.IgnoreBreaks.GetCustomAttribute<DescriptionCollectionAttribute>()?.Value.OrderBy(x => x.Length).ToList();
            _FlagDescriptorShort = DescriptionCollection[0];
            _FlagDescriptorLong = DescriptionCollection[1];
        }
        #endregion Setup

        #region Tests..
        #region FlagFirst..
        #region IgnoreBreaks_FlagFirst_FlagShort_NullReplace
        [Test]
        public void IgnoreBreaks_FlagFirst_FlagShort_NullReplace()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreBreaks.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "quickbrown";
            string Command = $"{_FlagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion IgnoreBreaks_FlagFirst_FlagShort_NullReplace 

        #region IgnoreBreaks_FlagFirst_FlagLong_NullReplace
        [Test]
        public void IgnoreBreaks_FlagFirst_FlagLong_NullReplace()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreBreaks.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "quickbrown";
            string Command = $"{_FlagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion IgnoreBreaks_FlagFirst_FlagLong_NullReplace

        #region IgnoreBreaks_FlagFirst_FlagShort_SpaceReplace
        [Test]
        public void IgnoreBreaks_FlagFirst_FlagShort_SpaceReplace()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreBreaks.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "quick brown";
            string Command = $"{_FlagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion IgnoreBreaks_FlagFirst_FlagShort_SpaceReplace 

        #region IgnoreBreaks_FlagFirst_FlagLong_SpaceReplace
        [Test]
        public void IgnoreBreaks_FlagFirst_FlagLong_SpaceReplace()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreBreaks.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "quick brown";
            string Command = $"{_FlagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion IgnoreBreaks_FlagFirst_FlagLong_SpaceReplace
        #endregion FlagFirst..

        #region FlagMiddle..
        #region IgnoreBreaks_FlagMiddle_FlagShort_NullReplace
        [Test]
        public void IgnoreBreaks_FlagMiddle_FlagShort_NullReplace()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreBreaks.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "quickbrown";
            string Command = $"-f '{TestFilePath}' {_FlagDescriptorShort} {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion IgnoreBreaks_FlagMiddle_FlagShort_NullReplace 

        #region IgnoreBreaks_FlagMiddle_FlagLong_NullReplace
        [Test]
        public void IgnoreBreaks_FlagMiddle_FlagLong_NullReplace()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreBreaks.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "quickbrown";
            string Command = $"-f '{TestFilePath}' {_FlagDescriptorLong} {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion IgnoreBreaks_FlagMiddle_FlagLong_NullReplace 

        #region IgnoreBreaks_FlagMiddle_FlagShort_SpaceReplace
        [Test]
        public void IgnoreBreaks_FlagMiddle_FlagShort_SpaceReplace()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreBreaks.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "quick brown";
            string Command = $"-f '{TestFilePath}' {_FlagDescriptorShort} {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion IgnoreBreaks_FlagMiddle_FlagShort_SpaceReplace 

        #region IgnoreBreaks_FlagMiddle_FlagLong_SpaceReplace
        [Test]
        public void IgnoreBreaks_FlagMiddle_FlagLong_SpaceReplace()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreBreaks.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "quick brown";
            string Command = $"-f '{TestFilePath}' {_FlagDescriptorLong} {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion IgnoreBreaks_FlagMiddle_FlagLong_SpaceReplace 
        #endregion FlagMiddle..

        #region FlagLast..
        #region IgnoreBreaks_FlagLast_FlagShort_NullReplace
        [Test]
        public void IgnoreBreaks_FlagLast_FlagShort_NullReplace()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreBreaks.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "quickbrown";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorShort}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion IgnoreBreaks_FlagLast_FlagShort_NullReplace 

        #region IgnoreBreaks_FlagLast_FlagLong_NullReplace
        [Test]
        public void IgnoreBreaks_FlagLast_FlagLong_NullReplace()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreBreaks.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "quickbrown";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorLong}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion IgnoreBreaks_FlagLast_FlagLong_NullReplace 

        #region IgnoreBreaks_FlagLast_FlagShort_SpaceReplace
        [Test]
        public void IgnoreBreaks_FlagLast_FlagShort_SpaceReplace()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreBreaks.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "quick brown";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorShort}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion IgnoreBreaks_FlagLast_FlagShort_SpaceReplace 

        #region IgnoreBreaks_FlagLast_FlagLong_SpaceReplace
        [Test]
        public void IgnoreBreaks_FlagLast_FlagLong_SpaceReplace()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreBreaks.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "quick brown";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorLong}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion IgnoreBreaks_FlagLast_FlagLong_SpaceReplace 
        #endregion FlagLast..

        #region IgnoreBreaks_NoFlag
        [Test]
        public void IgnoreBreaks_NoFlag()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreBreaks.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "quickbrown";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 0);
        }
        #endregion IgnoreBreaks_NoFlag 
        #endregion Tests..
    }
}
