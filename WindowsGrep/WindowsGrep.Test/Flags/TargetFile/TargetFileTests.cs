using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WindowsGrep.Common;
using WindowsGrep.Core;
using WindowsGrep.Engine;

namespace WindowsGrep.Test.Flags.TargetFile
{
    public class TargetFileTests : TestBase
    {
        #region Fields..
        private string _FlagDescriptorShort;
        private string _FlagDescriptorLong;
        private string _TestDataRelativePath = @"Flags\TargetFile\TestData";
        #endregion Fields..

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
        #region TargetFile_FlagFirst_FlagShort_SingleQuotes
        [Test]
        public void TargetFile_FlagFirst_FlagShort_SingleQuotes()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "TargetFile.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox";
            string Command = $"{_FlagDescriptorShort} '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            WindowsGrep.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion TargetFile_FlagFirst_FlagShort_SingleQuotes 

        #region TargetFile_FlagFirst_FlagShort_DoubleQuotes
        [Test]
        public void TargetFile_FlagFirst_FlagShort_DoubleQuotes()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "TargetFile.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox";
            string Command = $"{_FlagDescriptorShort} \"{TestFilePath}\" {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            WindowsGrep.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion TargetFile_FlagFirst_FlagShort_DoubleQuotes 

        #region TargetFile_FlagFirst_FlagLong_SingleQuotes
        [Test]
        public void TargetFile_FlagFirst_FlagLong_SingleQuotes()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "TargetFile.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox";
            string Command = $"{_FlagDescriptorLong} '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            WindowsGrep.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion TargetFile_FlagFirst_FlagLong_SingleQuotes

        #region TargetFile_FlagFirst_FlagLong_DoubleQuotes
        [Test]
        public void TargetFile_FlagFirst_FlagLong_DoubleQuotes()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "TargetFile.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox";
            string Command = $"{_FlagDescriptorLong} \"{TestFilePath}\" {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            WindowsGrep.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion TargetFile_FlagFirst_FlagLong_DoubleQuotes
        #endregion FlagFirst..

        #region FlagMiddle..
        #region TargetFile_FlagMiddle_FlagShort_SingleQuotes
        [Test]
        public void TargetFile_FlagMiddle_FlagShort_SingleQuotes()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "TargetFile.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox";
            string Command = $"-i {_FlagDescriptorShort} '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            WindowsGrep.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion TargetFile_FlagMiddle_FlagShort_SingleQuotes 

        #region TargetFile_FlagMiddle_FlagShort_DoubleQuotes
        [Test]
        public void TargetFile_FlagMiddle_FlagShort_DoubleQuotes()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "TargetFile.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox";
            string Command = $"-i {_FlagDescriptorShort} \"{TestFilePath}\" {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            WindowsGrep.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion TargetFile_FlagMiddle_FlagShort_DoubleQuotes

        #region TargetFile_FlagMiddle_FlagLong_SingleQuotes
        [Test]
        public void TargetFile_FlagMiddle_FlagLong_SingleQuotes()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "TargetFile.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox";
            string Command = $"-i {_FlagDescriptorLong} '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            WindowsGrep.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion TargetFile_FlagMiddle_FlagLong_SingleQuotes 

        #region TargetFile_FlagMiddle_FlagLong_DoubleQuotes
        [Test]
        public void TargetFile_FlagMiddle_FlagLong_DoubleQuotes()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "TargetFile.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox";
            string Command = $"-i {_FlagDescriptorLong} \"{TestFilePath}\" {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            WindowsGrep.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion TargetFile_FlagMiddle_FlagLong_DoubleQuotes
        #endregion FlagMiddle..

        #region FlagLast..
        #region TargetFile_FlagLast_FlagShort_SingleQuotes
        [Test]
        public void TargetFile_FlagLast_FlagShort_SingleQuotes()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "TargetFile.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox";
            string Command = $"{SearchTerm} {_FlagDescriptorShort} '{TestFilePath}'";

            var GrepResultCollection = new GrepResultCollection();
            WindowsGrep.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion TargetFile_FlagLast_FlagShort_SingleQuotes 

        #region TargetFile_FlagLast_FlagShort_DoubleQuotes
        [Test]
        public void TargetFile_FlagLast_FlagShort_DoubleQuotes()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "TargetFile.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox";
            string Command = $"{SearchTerm} {_FlagDescriptorShort} \"{TestFilePath}\"";

            var GrepResultCollection = new GrepResultCollection();
            WindowsGrep.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion TargetFile_FlagLast_FlagShort_DoubleQuotes

        #region TargetFile_FlagLast_FlagLong_SingleQuotes
        [Test]
        public void TargetFile_FlagLast_FlagLong_SingleQuotes()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "TargetFile.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox";
            string Command = $"{SearchTerm} {_FlagDescriptorLong} '{TestFilePath}'";

            var GrepResultCollection = new GrepResultCollection();
            WindowsGrep.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion TargetFile_FlagLast_FlagLong_SingleQuotes 

        #region TargetFile_FlagLast_FlagLong_DoubleQuotes
        [Test]
        public void TargetFile_FlagLast_FlagLong_DoubleQuotes()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "TargetFile.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox";
            string Command = $"{SearchTerm} {_FlagDescriptorLong} \"{TestFilePath}\"";

            var GrepResultCollection = new GrepResultCollection();
            WindowsGrep.RunCommandAsync(Command, GrepResultCollection, new System.Threading.CancellationToken());

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion TargetFile_FlagLast_FlagLong_DoubleQuotes
        #endregion FlagLast..
        #endregion Tests..
    }
}
