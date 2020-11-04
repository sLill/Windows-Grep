using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WindowsGrep.Common;
using WindowsGrep.Engine;

namespace WindowsGrep.Test.Flags.FixedStrings
{
    public class FixedStringsTests : TestBase
    {
        #region Member Variables..
        private string _FlagDescriptorShort;
        private string _FlagDescriptorLong;
        private string _TestDataRelativePath = @"Flags\FixedStrings\TestData";
        #endregion Member Variables..

        #region Properties..
        #endregion Properties..

        #region Setup
        [SetUp]
        public void Setup()
        {
            TestDataDirectory = Path.Combine(TestConfigurationManager.ProjectDirectory, _TestDataRelativePath);

            List<string> DescriptionCollection = ConsoleFlag.FixedStrings.GetCustomAttribute<DescriptionCollectionAttribute>()?.Value.OrderBy(x => x.Length).ToList();
            _FlagDescriptorShort = DescriptionCollection[0];
            _FlagDescriptorLong = DescriptionCollection[1];
        }
        #endregion Setup

        #region Tests..
        #region FlagFirst..
        #region FixedStrings_FlagFirst_FlagShort_SingleLine_SingleResult_Alpha
        [Test]
        public void FixedStrings_FlagFirst_FlagShort_SingleLine_SingleResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "brown fox";
            string Command = $"{_FlagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_SingleLine_SingleResult_Alpha

        #region FixedStrings_FlagFirst_FlagShort_SingleLine_SingleResult_Numeric
        [Test]
        public void FixedStrings_FlagFirst_FlagShort_SingleLine_SingleResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "4";
            string Command = $"{_FlagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_SingleLine_SingleResult_Numeric

        #region FixedStrings_FlagFirst_FlagShort_SingleLine_SingleResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagFirst_FlagShort_SingleLine_SingleResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "the 4 lazy";
            string Command = $"{_FlagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_SingleLine_SingleResult_Alphanumeric

        #region FixedStrings_FlagFirst_FlagShort_SingleLine_SingleResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagFirst_FlagShort_SingleLine_SingleResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"\s";
            string Command = $"{_FlagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_SingleLine_SingleResult_SpecialCharacter

        #region FixedStrings_FlagFirst_FlagShort_SingleLine_MultiResult_Alpha
        [Test]
        public void FixedStrings_FlagFirst_FlagShort_SingleLine_MultiResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "lazy";
            string Command = $"{_FlagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 3);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_SingleLine_MultiResult_Alpha

        #region FixedStrings_FlagFirst_FlagShort_SingleLine_MultiResult_Numeric
        [Test]
        public void FixedStrings_FlagFirst_FlagShort_SingleLine_MultiResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10";
            string Command = $"{_FlagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_SingleLine_MultiResult_Numeric

        #region FixedStrings_FlagFirst_FlagShort_SingleLine_MultiResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagFirst_FlagShort_SingleLine_MultiResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10 lazy";
            string Command = $"{_FlagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_SingleLine_MultiResult_Alphanumeric

        #region FixedStrings_FlagFirst_FlagShort_SingleLine_MultiResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagFirst_FlagShort_SingleLine_MultiResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = ".*";
            string Command = $"{_FlagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_SingleLine_MultiResult_SpecialCharacter

        #region FixedStrings_FlagFirst_FlagShort_MultiLine_SingleResult_Alpha
        [Test]
        public void FixedStrings_FlagFirst_FlagShort_MultiLine_SingleResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "brown fox";
            string Command = $"{_FlagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_MultiLine_SingleResult_Alpha

        #region FixedStrings_FlagFirst_FlagShort_MultiLine_SingleResult_Numeric
        [Test]
        public void FixedStrings_FlagFirst_FlagShort_MultiLine_SingleResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "4";
            string Command = $"{_FlagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_MultiLine_SingleResult_Numeric

        #region FixedStrings_FlagFirst_FlagShort_MultiLine_SingleResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagFirst_FlagShort_MultiLine_SingleResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox hops";
            string Command = $"{_FlagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_MultiLine_SingleResult_Alphanumeric

        #region FixedStrings_FlagFirst_FlagShort_MultiLine_SingleResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagFirst_FlagShort_MultiLine_SingleResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = ".?";
            string Command = $"{_FlagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_MultiLine_SingleResult_SpecialCharacter

        #region FixedStrings_FlagFirst_FlagShort_MultiLine_MultiResult_Alpha
        [Test]
        public void FixedStrings_FlagFirst_FlagShort_MultiLine_MultiResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "lazy";
            string Command = $"{_FlagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 6);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_MultiLine_MultiResult_Alpha

        #region FixedStrings_FlagFirst_FlagShort_MultiLine_MultiResult_Numeric
        [Test]
        public void FixedStrings_FlagFirst_FlagShort_MultiLine_MultiResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10";
            string Command = $"{_FlagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 5);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_MultiLine_MultiResult_Numeric

        #region FixedStrings_FlagFirst_FlagShort_MultiLine_MultiResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagFirst_FlagShort_MultiLine_MultiResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10 lazy";
            string Command = $"{_FlagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 4);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_MultiLine_MultiResult_Alphanumeric

        #region FixedStrings_FlagFirst_FlagShort_MultiLine_MultiResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagFirst_FlagShort_MultiLine_MultiResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"\s";
            string Command = $"{_FlagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_MultiLine_MultiResult_SpecialCharacter

        #region FixedStrings_FlagFirst_FlagLong_SingleLine_SingleResult_Alpha
        [Test]
        public void FixedStrings_FlagFirst_FlagLong_SingleLine_SingleResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "brown fox";
            string Command = $"{_FlagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_SingleLine_SingleResult_Alpha

        #region FixedStrings_FlagFirst_FlagLong_SingleLine_SingleResult_Numeric
        [Test]
        public void FixedStrings_FlagFirst_FlagLong_SingleLine_SingleResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "4";
            string Command = $"{_FlagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_SingleLine_SingleResult_Numeric

        #region FixedStrings_FlagFirst_FlagLong_SingleLine_SingleResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagFirst_FlagLong_SingleLine_SingleResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "the 4 lazy";
            string Command = $"{_FlagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_SingleLine_SingleResult_Alphanumeric

        #region FixedStrings_FlagFirst_FlagLong_SingleLine_SingleResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagFirst_FlagLong_SingleLine_SingleResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"\s";
            string Command = $"{_FlagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_SingleLine_SingleResult_SpecialCharacter

        #region FixedStrings_FlagFirst_FlagLong_SingleLine_MultiResult_Alpha
        [Test]
        public void FixedStrings_FlagFirst_FlagLong_SingleLine_MultiResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "lazy";
            string Command = $"{_FlagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 3);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_SingleLine_MultiResult_Alpha

        #region FixedStrings_FlagFirst_FlagLong_SingleLine_MultiResult_Numeric
        [Test]
        public void FixedStrings_FlagFirst_FlagLong_SingleLine_MultiResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10";
            string Command = $"{_FlagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_SingleLine_MultiResult_Numeric

        #region FixedStrings_FlagFirst_FlagLong_SingleLine_MultiResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagFirst_FlagLong_SingleLine_MultiResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10 lazy";
            string Command = $"{_FlagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_SingleLine_MultiResult_Alphanumeric

        #region FixedStrings_FlagFirst_FlagLong_SingleLine_MultiResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagFirst_FlagLong_SingleLine_MultiResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = ".*";
            string Command = $"{_FlagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_SingleLine_MultiResult_SpecialCharacter

        #region FixedStrings_FlagFirst_FlagLong_MultiLine_SingleResult_Alpha
        [Test]
        public void FixedStrings_FlagFirst_FlagLong_MultiLine_SingleResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "brown fox";
            string Command = $"{_FlagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_MultiLine_SingleResult_Alpha

        #region FixedStrings_FlagFirst_FlagLong_MultiLine_SingleResult_Numeric
        [Test]
        public void FixedStrings_FlagFirst_FlagLong_MultiLine_SingleResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "4";
            string Command = $"{_FlagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_MultiLine_SingleResult_Numeric

        #region FixedStrings_FlagFirst_FlagLong_MultiLine_SingleResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagFirst_FlagLong_MultiLine_SingleResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox hops";
            string Command = $"{_FlagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_MultiLine_SingleResult_Alphanumeric

        #region FixedStrings_FlagFirst_FlagLong_MultiLine_SingleResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagFirst_FlagLong_MultiLine_SingleResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = ".?";
            string Command = $"{_FlagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_MultiLine_SingleResult_SpecialCharacter

        #region FixedStrings_FlagFirst_FlagLong_MultiLine_MultiResult_Alpha
        [Test]
        public void FixedStrings_FlagFirst_FlagLong_MultiLine_MultiResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "lazy";
            string Command = $"{_FlagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 6);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_MultiLine_MultiResult_Alpha

        #region FixedStrings_FlagFirst_FlagLong_MultiLine_MultiResult_Numeric
        [Test]
        public void FixedStrings_FlagFirst_FlagLong_MultiLine_MultiResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10";
            string Command = $"{_FlagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 5);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_MultiLine_MultiResult_Numeric

        #region FixedStrings_FlagFirst_FlagLong_MultiLine_MultiResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagFirst_FlagLong_MultiLine_MultiResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10 lazy";
            string Command = $"{_FlagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 4);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_MultiLine_MultiResult_Alphanumeric 

        #region FixedStrings_FlagFirst_FlagLong_MultiLine_MultiResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagFirst_FlagLong_MultiLine_MultiResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"\s";
            string Command = $"{_FlagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_MultiLine_MultiResult_SpecialCharacter 
        #endregion FlagFirst..

        #region FlagMiddle..
        #region FixedStrings_FlagMiddle_FlagShort_SingleLine_SingleResult_Alpha
        [Test]
        public void FixedStrings_FlagMiddle_FlagShort_SingleLine_SingleResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "brown fox";
            string Command = $"-i {_FlagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_SingleLine_SingleResult_Alpha

        #region FixedStrings_FlagMiddle_FlagShort_SingleLine_SingleResult_Numeric
        [Test]
        public void FixedStrings_FlagMiddle_FlagShort_SingleLine_SingleResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "4";
            string Command = $"-i {_FlagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_SingleLine_SingleResult_Numeric

        #region FixedStrings_FlagMiddle_FlagShort_SingleLine_SingleResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagMiddle_FlagShort_SingleLine_SingleResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "the 4 lazy";
            string Command = $"-i {_FlagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_SingleLine_SingleResult_Alphanumeric

        #region FixedStrings_FlagMiddle_FlagShort_SingleLine_SingleResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagMiddle_FlagShort_SingleLine_SingleResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"\s";
            string Command = $"-i {_FlagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_SingleLine_SingleResult_SpecialCharacter

        #region FixedStrings_FlagMiddle_FlagShort_SingleLine_MultiResult_Alpha
        [Test]
        public void FixedStrings_FlagMiddle_FlagShort_SingleLine_MultiResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "lazy";
            string Command = $"-i {_FlagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 3);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_SingleLine_MultiResult_Alpha

        #region FixedStrings_FlagMiddle_FlagShort_SingleLine_MultiResult_Numeric
        [Test]
        public void FixedStrings_FlagMiddle_FlagShort_SingleLine_MultiResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10";
            string Command = $"-i {_FlagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_SingleLine_MultiResult_Numeric

        #region FixedStrings_FlagMiddle_FlagShort_SingleLine_MultiResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagMiddle_FlagShort_SingleLine_MultiResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10 lazy";
            string Command = $"-i {_FlagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_SingleLine_MultiResult_Alphanumeric

        #region FixedStrings_FlagMiddle_FlagShort_SingleLine_MultiResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagMiddle_FlagShort_SingleLine_MultiResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = ".*";
            string Command = $"-i {_FlagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_SingleLine_MultiResult_SpecialCharacter

        #region FixedStrings_FlagMiddle_FlagShort_MultiLine_SingleResult_Alpha
        [Test]
        public void FixedStrings_FlagMiddle_FlagShort_MultiLine_SingleResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "brown fox";
            string Command = $"-i {_FlagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_MultiLine_SingleResult_Alpha

        #region FixedStrings_FlagMiddle_FlagShort_MultiLine_SingleResult_Numeric
        [Test]
        public void FixedStrings_FlagMiddle_FlagShort_MultiLine_SingleResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "4";
            string Command = $"-i {_FlagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_MultiLine_SingleResult_Numeric

        #region FixedStrings_FlagMiddle_FlagShort_MultiLine_SingleResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagMiddle_FlagShort_MultiLine_SingleResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox hops";
            string Command = $"-i {_FlagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_MultiLine_SingleResult_Alphanumeric

        #region FixedStrings_FlagMiddle_FlagShort_MultiLine_SingleResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagMiddle_FlagShort_MultiLine_SingleResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = ".?";
            string Command = $"-i {_FlagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_MultiLine_SingleResult_SpecialCharacter

        #region FixedStrings_FlagMiddle_FlagShort_MultiLine_MultiResult_Alpha
        [Test]
        public void FixedStrings_FlagMiddle_FlagShort_MultiLine_MultiResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "lazy";
            string Command = $"-i {_FlagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 6);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_MultiLine_MultiResult_Alpha

        #region FixedStrings_FlagMiddle_FlagShort_MultiLine_MultiResult_Numeric
        [Test]
        public void FixedStrings_FlagMiddle_FlagShort_MultiLine_MultiResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10";
            string Command = $"-i {_FlagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 5);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_MultiLine_MultiResult_Numeric

        #region FixedStrings_FlagMiddle_FlagShort_MultiLine_MultiResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagMiddle_FlagShort_MultiLine_MultiResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10 lazy";
            string Command = $"-i {_FlagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 4);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_MultiLine_MultiResult_Alphanumeric

        #region FixedStrings_FlagMiddle_FlagShort_MultiLine_MultiResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagMiddle_FlagShort_MultiLine_MultiResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"\s";
            string Command = $"-i {_FlagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_MultiLine_MultiResult_SpecialCharacter

        #region FixedStrings_FlagMiddle_FlagLong_SingleLine_SingleResult_Alpha
        [Test]
        public void FixedStrings_FlagMiddle_FlagLong_SingleLine_SingleResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "brown fox";
            string Command = $"-i {_FlagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_SingleLine_SingleResult_Alpha

        #region FixedStrings_FlagMiddle_FlagLong_SingleLine_SingleResult_Numeric
        [Test]
        public void FixedStrings_FlagMiddle_FlagLong_SingleLine_SingleResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "4";
            string Command = $"-i {_FlagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_SingleLine_SingleResult_Numeric

        #region FixedStrings_FlagMiddle_FlagLong_SingleLine_SingleResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagMiddle_FlagLong_SingleLine_SingleResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "the 4 lazy";
            string Command = $"-i {_FlagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_SingleLine_SingleResult_Alphanumeric

        #region FixedStrings_FlagMiddle_FlagLong_SingleLine_SingleResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagMiddle_FlagLong_SingleLine_SingleResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"\s";
            string Command = $"-i {_FlagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_SingleLine_SingleResult_SpecialCharacter

        #region FixedStrings_FlagMiddle_FlagLong_SingleLine_MultiResult_Alpha
        [Test]
        public void FixedStrings_FlagMiddle_FlagLong_SingleLine_MultiResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "lazy";
            string Command = $"-i {_FlagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 3);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_SingleLine_MultiResult_Alpha

        #region FixedStrings_FlagMiddle_FlagLong_SingleLine_MultiResult_Numeric
        [Test]
        public void FixedStrings_FlagMiddle_FlagLong_SingleLine_MultiResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10";
            string Command = $"-i {_FlagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_SingleLine_MultiResult_Numeric

        #region FixedStrings_FlagMiddle_FlagLong_SingleLine_MultiResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagMiddle_FlagLong_SingleLine_MultiResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10 lazy";
            string Command = $"-i {_FlagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_SingleLine_MultiResult_Alphanumeric

        #region FixedStrings_FlagMiddle_FlagLong_SingleLine_MultiResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagMiddle_FlagLong_SingleLine_MultiResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = ".*";
            string Command = $"-i {_FlagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_SingleLine_MultiResult_SpecialCharacter

        #region FixedStrings_FlagMiddle_FlagLong_MultiLine_SingleResult_Alpha
        [Test]
        public void FixedStrings_FlagMiddle_FlagLong_MultiLine_SingleResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "brown fox";
            string Command = $"-i {_FlagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_MultiLine_SingleResult_Alpha

        #region FixedStrings_FlagMiddle_FlagLong_MultiLine_SingleResult_Numeric
        [Test]
        public void FixedStrings_FlagMiddle_FlagLong_MultiLine_SingleResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "4";
            string Command = $"-i {_FlagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_MultiLine_SingleResult_Numeric

        #region FixedStrings_FlagMiddle_FlagLong_MultiLine_SingleResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagMiddle_FlagLong_MultiLine_SingleResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox hops";
            string Command = $"-i {_FlagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_MultiLine_SingleResult_Alphanumeric

        #region FixedStrings_FlagMiddle_FlagLong_MultiLine_SingleResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagMiddle_FlagLong_MultiLine_SingleResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = ".?";
            string Command = $"-i {_FlagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_MultiLine_SingleResult_SpecialCharacter

        #region FixedStrings_FlagMiddle_FlagLong_MultiLine_MultiResult_Alpha
        [Test]
        public void FixedStrings_FlagMiddle_FlagLong_MultiLine_MultiResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "lazy";
            string Command = $"-i {_FlagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 6);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_MultiLine_MultiResult_Alpha

        #region FixedStrings_FlagMiddle_FlagLong_MultiLine_MultiResult_Numeric
        [Test]
        public void FixedStrings_FlagMiddle_FlagLong_MultiLine_MultiResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10";
            string Command = $"-i {_FlagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 5);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_MultiLine_MultiResult_Numeric

        #region FixedStrings_FlagMiddle_FlagLong_MultiLine_MultiResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagMiddle_FlagLong_MultiLine_MultiResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10 lazy";
            string Command = $"-i {_FlagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 4);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_MultiLine_MultiResult_Alphanumeric 

        #region FixedStrings_FlagMiddle_FlagLong_MultiLine_MultiResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagMiddle_FlagLong_MultiLine_MultiResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"\s";
            string Command = $"-i {_FlagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_MultiLine_MultiResult_SpecialCharacter 
        #endregion FlagMiddle..

        #region FlagLast..  
        #region FixedStrings_FlagLast_FlagShort_SingleLine_SingleResult_Alpha
        [Test]
        public void FixedStrings_FlagLast_FlagShort_SingleLine_SingleResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "brown fox";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorShort}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagLast_FlagShort_SingleLine_SingleResult_Alpha

        #region FixedStrings_FlagLast_FlagShort_SingleLine_SingleResult_Numeric
        [Test]
        public void FixedStrings_FlagLast_FlagShort_SingleLine_SingleResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "4";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorShort}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagLast_FlagShort_SingleLine_SingleResult_Numeric

        #region FixedStrings_FlagLast_FlagShort_SingleLine_SingleResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagLast_FlagShort_SingleLine_SingleResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "the 4 lazy";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorShort}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagLast_FlagShort_SingleLine_SingleResult_Alphanumeric

        #region FixedStrings_FlagLast_FlagShort_SingleLine_SingleResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagLast_FlagShort_SingleLine_SingleResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"\s";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorShort}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagLast_FlagShort_SingleLine_SingleResult_SpecialCharacter

        #region FixedStrings_FlagLast_FlagShort_SingleLine_MultiResult_Alpha
        [Test]
        public void FixedStrings_FlagLast_FlagShort_SingleLine_MultiResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "lazy";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorShort}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 3);
        }
        #endregion FixedStrings_FlagLast_FlagShort_SingleLine_MultiResult_Alpha

        #region FixedStrings_FlagLast_FlagShort_SingleLine_MultiResult_Numeric
        [Test]
        public void FixedStrings_FlagLast_FlagShort_SingleLine_MultiResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorShort}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagLast_FlagShort_SingleLine_MultiResult_Numeric

        #region FixedStrings_FlagLast_FlagShort_SingleLine_MultiResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagLast_FlagShort_SingleLine_MultiResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10 lazy";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorShort}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagLast_FlagShort_SingleLine_MultiResult_Alphanumeric

        #region FixedStrings_FlagLast_FlagShort_SingleLine_MultiResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagLast_FlagShort_SingleLine_MultiResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = ".*";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorShort}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagLast_FlagShort_SingleLine_MultiResult_SpecialCharacter

        #region FixedStrings_FlagLast_FlagShort_MultiLine_SingleResult_Alpha
        [Test]
        public void FixedStrings_FlagLast_FlagShort_MultiLine_SingleResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "brown fox";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorShort}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagLast_FlagShort_MultiLine_SingleResult_Alpha

        #region FixedStrings_FlagLast_FlagShort_MultiLine_SingleResult_Numeric
        [Test]
        public void FixedStrings_FlagLast_FlagShort_MultiLine_SingleResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "4";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorShort}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagLast_FlagShort_MultiLine_SingleResult_Numeric

        #region FixedStrings_FlagLast_FlagShort_MultiLine_SingleResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagLast_FlagShort_MultiLine_SingleResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox hops";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorShort}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagLast_FlagShort_MultiLine_SingleResult_Alphanumeric

        #region FixedStrings_FlagLast_FlagShort_MultiLine_SingleResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagLast_FlagShort_MultiLine_SingleResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = ".?";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorShort}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagLast_FlagShort_MultiLine_SingleResult_SpecialCharacter

        #region FixedStrings_FlagLast_FlagShort_MultiLine_MultiResult_Alpha
        [Test]
        public void FixedStrings_FlagLast_FlagShort_MultiLine_MultiResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "lazy";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorShort}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 6);
        }
        #endregion FixedStrings_FlagLast_FlagShort_MultiLine_MultiResult_Alpha

        #region FixedStrings_FlagLast_FlagShort_MultiLine_MultiResult_Numeric
        [Test]
        public void FixedStrings_FlagLast_FlagShort_MultiLine_MultiResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorShort}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 5);
        }
        #endregion FixedStrings_FlagLast_FlagShort_MultiLine_MultiResult_Numeric

        #region FixedStrings_FlagLast_FlagShort_MultiLine_MultiResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagLast_FlagShort_MultiLine_MultiResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10 lazy";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorShort}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 4);
        }
        #endregion FixedStrings_FlagLast_FlagShort_MultiLine_MultiResult_Alphanumeric

        #region FixedStrings_FlagLast_FlagShort_MultiLine_MultiResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagLast_FlagShort_MultiLine_MultiResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"\s";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorShort}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagLast_FlagShort_MultiLine_MultiResult_SpecialCharacter

        #region FixedStrings_FlagLast_FlagLong_SingleLine_SingleResult_Alpha
        [Test]
        public void FixedStrings_FlagLast_FlagLong_SingleLine_SingleResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "brown fox";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorLong}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagLast_FlagLong_SingleLine_SingleResult_Alpha

        #region FixedStrings_FlagLast_FlagLong_SingleLine_SingleResult_Numeric
        [Test]
        public void FixedStrings_FlagLast_FlagLong_SingleLine_SingleResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "4";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorLong}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagLast_FlagLong_SingleLine_SingleResult_Numeric

        #region FixedStrings_FlagLast_FlagLong_SingleLine_SingleResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagLast_FlagLong_SingleLine_SingleResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "the 4 lazy";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorLong}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagLast_FlagLong_SingleLine_SingleResult_Alphanumeric

        #region FixedStrings_FlagLast_FlagLong_SingleLine_SingleResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagLast_FlagLong_SingleLine_SingleResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"\s";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorLong}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagLast_FlagLong_SingleLine_SingleResult_SpecialCharacter

        #region FixedStrings_FlagLast_FlagLong_SingleLine_MultiResult_Alpha
        [Test]
        public void FixedStrings_FlagLast_FlagLong_SingleLine_MultiResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "lazy";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorLong}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 3);
        }
        #endregion FixedStrings_FlagLast_FlagLong_SingleLine_MultiResult_Alpha

        #region FixedStrings_FlagLast_FlagLong_SingleLine_MultiResult_Numeric
        [Test]
        public void FixedStrings_FlagLast_FlagLong_SingleLine_MultiResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorLong}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagLast_FlagLong_SingleLine_MultiResult_Numeric

        #region FixedStrings_FlagLast_FlagLong_SingleLine_MultiResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagLast_FlagLong_SingleLine_MultiResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10 lazy";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorLong}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagLast_FlagLong_SingleLine_MultiResult_Alphanumeric

        #region FixedStrings_FlagLast_FlagLong_SingleLine_MultiResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagLast_FlagLong_SingleLine_MultiResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = ".*";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorLong}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagLast_FlagLong_SingleLine_MultiResult_SpecialCharacter

        #region FixedStrings_FlagLast_FlagLong_MultiLine_SingleResult_Alpha
        [Test]
        public void FixedStrings_FlagLast_FlagLong_MultiLine_SingleResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "brown fox";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorLong}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagLast_FlagLong_MultiLine_SingleResult_Alpha

        #region FixedStrings_FlagLast_FlagLong_MultiLine_SingleResult_Numeric
        [Test]
        public void FixedStrings_FlagLast_FlagLong_MultiLine_SingleResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "4";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorLong}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagLast_FlagLong_MultiLine_SingleResult_Numeric

        #region FixedStrings_FlagLast_FlagLong_MultiLine_SingleResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagLast_FlagLong_MultiLine_SingleResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox hops";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorLong}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagLast_FlagLong_MultiLine_SingleResult_Alphanumeric

        #region FixedStrings_FlagLast_FlagLong_MultiLine_SingleResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagLast_FlagLong_MultiLine_SingleResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = ".?";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorLong}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagLast_FlagLong_MultiLine_SingleResult_SpecialCharacter

        #region FixedStrings_FlagLast_FlagLong_MultiLine_MultiResult_Alpha
        [Test]
        public void FixedStrings_FlagLast_FlagLong_MultiLine_MultiResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "lazy";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorLong}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 6);
        }
        #endregion FixedStrings_FlagLast_FlagLong_MultiLine_MultiResult_Alpha

        #region FixedStrings_FlagLast_FlagLong_MultiLine_MultiResult_Numeric
        [Test]
        public void FixedStrings_FlagLast_FlagLong_MultiLine_MultiResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorLong}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 5);
        }
        #endregion FixedStrings_FlagLast_FlagLong_MultiLine_MultiResult_Numeric

        #region FixedStrings_FlagLast_FlagLong_MultiLine_MultiResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagLast_FlagLong_MultiLine_MultiResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10 lazy";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorLong}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 4);
        }
        #endregion FixedStrings_FlagLast_FlagLong_MultiLine_MultiResult_Alphanumeric 

        #region FixedStrings_FlagLast_FlagLong_MultiLine_MultiResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagLast_FlagLong_MultiLine_MultiResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"\s";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_FlagDescriptorLong}";

            var GrepResultCollection = new GrepResultCollection();
            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count(x => x.MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagLast_FlagLong_MultiLine_MultiResult_SpecialCharacter 
        #endregion FlagLast..
        #endregion Tests..
    }
}
