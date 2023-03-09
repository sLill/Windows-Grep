using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WindowsGrep.Common;
using WindowsGrep.Engine;

namespace WindowsGrep.Test.Flags.IgnoreCase
{
    public class IgnoreCaseTests : TestBase
    {
        #region Fields..
        private string _FlagDescriptorShort;
        private string _FlagDescriptorLong;
        private string _TestDataRelativePath = @"Flags\IgnoreCase\TestData";
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Setup
        [SetUp]
        public void Setup()
        {
            TestDataDirectory = Path.Combine(TestConfigurationManager.ProjectDirectory, _TestDataRelativePath);

            List<string> DescriptionCollection = ConsoleFlag.IgnoreCase.GetCustomAttribute<DescriptionCollectionAttribute>()?.Value.OrderBy(x => x.Length).ToList();
            _FlagDescriptorShort = DescriptionCollection[0];
            _FlagDescriptorLong = DescriptionCollection[1];
        }
        #endregion Setup

        #region Tests..
        #region FlagFirst..
        #region IgnoreCase_FlagFirst_FlagShort_SingleResult
        [Test]
        public void IgnoreCase_FlagFirst_FlagShort_SingleResult()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreCase.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox";
            string Command = $"{_FlagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion IgnoreCase_FlagFirst_FlagShort_SingleResult 

        #region IgnoreCase_FlagFirst_FlagShort_MultiResult
        [Test]
        public void IgnoreCase_FlagFirst_FlagShort_MultiResult()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreCase.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "lazy";
            string Command = $"{_FlagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 3);
        }
        #endregion IgnoreCase_FlagFirst_FlagShort_MultiResult 

        #region IgnoreCase_FlagFirst_FlagLong_SingleResult
        [Test]
        public void IgnoreCase_FlagFirst_FlagLong_SingleResult()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreCase.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox";
            string Command = $"{_FlagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion IgnoreCase_FlagFirst_FlagLong_SingleResult 

        #region IgnoreCase_FlagFirst_FlagLong_MultiResult
        [Test]
        public void IgnoreCase_FlagFirst_FlagLong_MultiResult()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreCase.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "lazy";
            string Command = $"{_FlagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 3);
        }
        #endregion IgnoreCase_FlagFirst_FlagLong_MultiResult 
        #endregion FlagFirst..

        #region FlagMiddle..
        #region IgnoreCase_FlagMiddle_FlagShort_SingleResult
        [Test]
        public void IgnoreCase_FlagMiddle_FlagShort_SingleResult()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreCase.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox";
            string Command = $"-f '{TestFilePath}' {_FlagDescriptorShort} {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion IgnoreCase_FlagMiddle_FlagShort_SingleResult 

        #region IgnoreCase_FlagMiddle_FlagShort_MultiResult
        [Test]
        public void IgnoreCase_FlagMiddle_FlagShort_MultiResult()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreCase.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "lazy";
            string Command = $"-f '{TestFilePath}' {_FlagDescriptorShort} {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 3);
        }
        #endregion IgnoreCase_FlagMiddle_FlagShort_MultiResult 

        #region IgnoreCase_FlagMiddle_FlagLong_SingleResult
        [Test]
        public void IgnoreCase_FlagMiddle_FlagLong_SingleResult()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreCase.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox";
            string Command = $"-f '{TestFilePath}' {_FlagDescriptorLong} {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion IgnoreCase_FlagMiddle_FlagLong_SingleResult 

        #region IgnoreCase_FlagMiddle_FlagLong_MultiResult
        [Test]
        public void IgnoreCase_FlagMiddle_FlagLong_MultiResult()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreCase.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "lazy";
            string Command = $"-f '{TestFilePath}' {_FlagDescriptorLong} {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 3);
        }
        #endregion IgnoreCase_FlagMiddle_FlagLong_MultiResult 
        #endregion FlagMiddle..

        #region FlagLast..
        #region IgnoreCase_FlagLast_FlagShort_SingleResult
        [Test]
        public void IgnoreCase_FlagLast_FlagShort_SingleResult()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreCase.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorShort}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion IgnoreCase_FlagLast_FlagShort_SingleResult 

        #region IgnoreCase_FlagLast_FlagShort_MultiResult
        [Test]
        public void IgnoreCase_FlagLast_FlagShort_MultiResult()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreCase.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "lazy";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorShort}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 3);
        }
        #endregion IgnoreCase_FlagLast_FlagShort_MultiResult 

        #region IgnoreCase_FlagLast_FlagLong_SingleResult
        [Test]
        public void IgnoreCase_FlagLast_FlagLong_SingleResult()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreCase.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorLong}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion IgnoreCase_FlagLast_FlagLong_SingleResult 

        #region IgnoreCase_FlagLast_FlagLong_MultiResult
        [Test]
        public void IgnoreCase_FlagLast_FlagLong_MultiResult()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "IgnoreCase.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "lazy";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorLong}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 3);
        }
        #endregion IgnoreCase_FlagLast_FlagLong_MultiResult 
        #endregion FlagLast..
        #endregion Tests..
    }
}
