using NUnit.Framework;
using System;
using System.IO;
using WindowsGrep.Engine;

namespace WindowsGrep.Test.Flags.FixedStrings
{
    public class FixedStringsTests
    {
        #region Properties..
        private string _testDataRelativePath = @"Flags\FixedStrings\TestData";

        public string TestDataDirectory { get; private set; }
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
        #region FixedStrings_FlagFirst_FlagShort_SingleLine_SingleResult_Alpha
        [Test]
        public void FixedStrings_FlagFirst_FlagShort_SingleLine_SingleResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-F -f '{TestFilePath}' brown fox";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_SingleLine_SingleResult_Alpha

        #region FixedStrings_FlagFirst_FlagShort_SingleLine_SingleResult_Numeric
        [Test]
        public void FixedStrings_FlagFirst_FlagShort_SingleLine_SingleResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-F -f '{TestFilePath}' 4";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_SingleLine_SingleResult_Numeric

        #region FixedStrings_FlagFirst_FlagShort_SingleLine_SingleResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagFirst_FlagShort_SingleLine_SingleResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-F -f '{TestFilePath}' the 4 lazy";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_SingleLine_SingleResult_Alphanumeric

        #region FixedStrings_FlagFirst_FlagShort_SingleLine_SingleResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagFirst_FlagShort_SingleLine_SingleResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-F -f '{TestFilePath}' " + @"\s";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_SingleLine_SingleResult_SpecialCharacter

        #region FixedStrings_FlagFirst_FlagShort_SingleLine_MultiResult_Alpha
        [Test]
        public void FixedStrings_FlagFirst_FlagShort_SingleLine_MultiResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-F -f '{TestFilePath}' lazy";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 3);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_SingleLine_MultiResult_Alpha

        #region FixedStrings_FlagFirst_FlagShort_SingleLine_MultiResult_Numeric
        [Test]
        public void FixedStrings_FlagFirst_FlagShort_SingleLine_MultiResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-F -f '{TestFilePath}' 10";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_SingleLine_MultiResult_Numeric

        #region FixedStrings_FlagFirst_FlagShort_SingleLine_MultiResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagFirst_FlagShort_SingleLine_MultiResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-F -f '{TestFilePath}' 10 lazy";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_SingleLine_MultiResult_Alphanumeric

        #region FixedStrings_FlagFirst_FlagShort_SingleLine_MultiResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagFirst_FlagShort_SingleLine_MultiResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-F -f '{TestFilePath}' .*";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_SingleLine_MultiResult_SpecialCharacter

        #region FixedStrings_FlagFirst_FlagShort_MultiLine_SingleResult_Alpha
        [Test]
        public void FixedStrings_FlagFirst_FlagShort_MultiLine_SingleResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-F -f '{TestFilePath}' brown fox";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_MultiLine_SingleResult_Alpha

        #region FixedStrings_FlagFirst_FlagShort_MultiLine_SingleResult_Numeric
        [Test]
        public void FixedStrings_FlagFirst_FlagShort_MultiLine_SingleResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-F -f '{TestFilePath}' 4";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_MultiLine_SingleResult_Numeric

        #region FixedStrings_FlagFirst_FlagShort_MultiLine_SingleResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagFirst_FlagShort_MultiLine_SingleResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-F -f '{TestFilePath}' fox hops";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_MultiLine_SingleResult_Alphanumeric

        #region FixedStrings_FlagFirst_FlagShort_MultiLine_SingleResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagFirst_FlagShort_MultiLine_SingleResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-F -f '{TestFilePath}' .?";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_MultiLine_SingleResult_SpecialCharacter

        #region FixedStrings_FlagFirst_FlagShort_MultiLine_MultiResult_Alpha
        [Test]
        public void FixedStrings_FlagFirst_FlagShort_MultiLine_MultiResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-F -f '{TestFilePath}' lazy";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 6);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_MultiLine_MultiResult_Alpha

        #region FixedStrings_FlagFirst_FlagShort_MultiLine_MultiResult_Numeric
        [Test]
        public void FixedStrings_FlagFirst_FlagShort_MultiLine_MultiResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-F -f '{TestFilePath}' 10";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 5);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_MultiLine_MultiResult_Numeric

        #region FixedStrings_FlagFirst_FlagShort_MultiLine_MultiResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagFirst_FlagShort_MultiLine_MultiResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-F -f '{TestFilePath}' 10 lazy";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 4);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_MultiLine_MultiResult_Alphanumeric

        #region FixedStrings_FlagFirst_FlagShort_MultiLine_MultiResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagFirst_FlagShort_MultiLine_MultiResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-F -f '{TestFilePath}' " + @"\s";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_MultiLine_MultiResult_SpecialCharacter

        #region FixedStrings_FlagFirst_FlagLong_SingleLine_SingleResult_Alpha
        [Test]
        public void FixedStrings_FlagFirst_FlagLong_SingleLine_SingleResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"--fixed-strings -f '{TestFilePath}' brown fox";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_SingleLine_SingleResult_Alpha

        #region FixedStrings_FlagFirst_FlagLong_SingleLine_SingleResult_Numeric
        [Test]
        public void FixedStrings_FlagFirst_FlagLong_SingleLine_SingleResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"--fixed-strings -f '{TestFilePath}' 4";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_SingleLine_SingleResult_Numeric

        #region FixedStrings_FlagFirst_FlagLong_SingleLine_SingleResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagFirst_FlagLong_SingleLine_SingleResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"--fixed-strings -f '{TestFilePath}' the 4 lazy";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_SingleLine_SingleResult_Alphanumeric

        #region FixedStrings_FlagFirst_FlagLong_SingleLine_SingleResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagFirst_FlagLong_SingleLine_SingleResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"--fixed-strings -f '{TestFilePath}' " + @"\s";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_SingleLine_SingleResult_SpecialCharacter

        #region FixedStrings_FlagFirst_FlagLong_SingleLine_MultiResult_Alpha
        [Test]
        public void FixedStrings_FlagFirst_FlagLong_SingleLine_MultiResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"--fixed-strings -f '{TestFilePath}' lazy";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 3);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_SingleLine_MultiResult_Alpha

        #region FixedStrings_FlagFirst_FlagLong_SingleLine_MultiResult_Numeric
        [Test]
        public void FixedStrings_FlagFirst_FlagLong_SingleLine_MultiResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"--fixed-strings -f '{TestFilePath}' 10";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_SingleLine_MultiResult_Numeric

        #region FixedStrings_FlagFirst_FlagLong_SingleLine_MultiResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagFirst_FlagLong_SingleLine_MultiResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"--fixed-strings -f '{TestFilePath}' 10 lazy";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_SingleLine_MultiResult_Alphanumeric

        #region FixedStrings_FlagFirst_FlagLong_SingleLine_MultiResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagFirst_FlagLong_SingleLine_MultiResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"--fixed-strings -f '{TestFilePath}' .*";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_SingleLine_MultiResult_SpecialCharacter

        #region FixedStrings_FlagFirst_FlagLong_MultiLine_SingleResult_Alpha
        [Test]
        public void FixedStrings_FlagFirst_FlagLong_MultiLine_SingleResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"--fixed-strings -f '{TestFilePath}' brown fox";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_MultiLine_SingleResult_Alpha

        #region FixedStrings_FlagFirst_FlagLong_MultiLine_SingleResult_Numeric
        [Test]
        public void FixedStrings_FlagFirst_FlagLong_MultiLine_SingleResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"--fixed-strings -f '{TestFilePath}' 4";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_MultiLine_SingleResult_Numeric

        #region FixedStrings_FlagFirst_FlagLong_MultiLine_SingleResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagFirst_FlagLong_MultiLine_SingleResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"--fixed-strings -f '{TestFilePath}' fox hops";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_MultiLine_SingleResult_Alphanumeric

        #region FixedStrings_FlagFirst_FlagLong_MultiLine_SingleResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagFirst_FlagLong_MultiLine_SingleResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"--fixed-strings -f '{TestFilePath}' .?";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_MultiLine_SingleResult_SpecialCharacter

        #region FixedStrings_FlagFirst_FlagLong_MultiLine_MultiResult_Alpha
        [Test]
        public void FixedStrings_FlagFirst_FlagLong_MultiLine_MultiResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"--fixed-strings -f '{TestFilePath}' lazy";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 6);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_MultiLine_MultiResult_Alpha

        #region FixedStrings_FlagFirst_FlagLong_MultiLine_MultiResult_Numeric
        [Test]
        public void FixedStrings_FlagFirst_FlagLong_MultiLine_MultiResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"--fixed-strings -f '{TestFilePath}' 10";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 5);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_MultiLine_MultiResult_Numeric

        #region FixedStrings_FlagFirst_FlagLong_MultiLine_MultiResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagFirst_FlagLong_MultiLine_MultiResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"--fixed-strings -f '{TestFilePath}' 10 lazy";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 4);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_MultiLine_MultiResult_Alphanumeric 

        #region FixedStrings_FlagFirst_FlagLong_MultiLine_MultiResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagFirst_FlagLong_MultiLine_MultiResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"--fixed-strings -f '{TestFilePath}' " + @"\s";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
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

            string Command = $"-i -F -f '{TestFilePath}' brown fox";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_SingleLine_SingleResult_Alpha

        #region FixedStrings_FlagMiddle_FlagShort_SingleLine_SingleResult_Numeric
        [Test]
        public void FixedStrings_FlagMiddle_FlagShort_SingleLine_SingleResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-i -F -f '{TestFilePath}' 4";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_SingleLine_SingleResult_Numeric

        #region FixedStrings_FlagMiddle_FlagShort_SingleLine_SingleResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagMiddle_FlagShort_SingleLine_SingleResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-i -F -f '{TestFilePath}' the 4 lazy";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_SingleLine_SingleResult_Alphanumeric

        #region FixedStrings_FlagMiddle_FlagShort_SingleLine_SingleResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagMiddle_FlagShort_SingleLine_SingleResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-i -F -f '{TestFilePath}' " + @"\s";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_SingleLine_SingleResult_SpecialCharacter

        #region FixedStrings_FlagMiddle_FlagShort_SingleLine_MultiResult_Alpha
        [Test]
        public void FixedStrings_FlagMiddle_FlagShort_SingleLine_MultiResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-i -F -f '{TestFilePath}' lazy";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 3);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_SingleLine_MultiResult_Alpha

        #region FixedStrings_FlagMiddle_FlagShort_SingleLine_MultiResult_Numeric
        [Test]
        public void FixedStrings_FlagMiddle_FlagShort_SingleLine_MultiResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-i -F -f '{TestFilePath}' 10";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_SingleLine_MultiResult_Numeric

        #region FixedStrings_FlagMiddle_FlagShort_SingleLine_MultiResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagMiddle_FlagShort_SingleLine_MultiResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-i -F -f '{TestFilePath}' 10 lazy";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_SingleLine_MultiResult_Alphanumeric

        #region FixedStrings_FlagMiddle_FlagShort_SingleLine_MultiResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagMiddle_FlagShort_SingleLine_MultiResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-i -F -f '{TestFilePath}' .*";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_SingleLine_MultiResult_SpecialCharacter

        #region FixedStrings_FlagMiddle_FlagShort_MultiLine_SingleResult_Alpha
        [Test]
        public void FixedStrings_FlagMiddle_FlagShort_MultiLine_SingleResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-i -F -f '{TestFilePath}' brown fox";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_MultiLine_SingleResult_Alpha

        #region FixedStrings_FlagMiddle_FlagShort_MultiLine_SingleResult_Numeric
        [Test]
        public void FixedStrings_FlagMiddle_FlagShort_MultiLine_SingleResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-i -F -f '{TestFilePath}' 4";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_MultiLine_SingleResult_Numeric

        #region FixedStrings_FlagMiddle_FlagShort_MultiLine_SingleResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagMiddle_FlagShort_MultiLine_SingleResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-i -F -f '{TestFilePath}' fox hops";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_MultiLine_SingleResult_Alphanumeric

        #region FixedStrings_FlagMiddle_FlagShort_MultiLine_SingleResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagMiddle_FlagShort_MultiLine_SingleResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-i -F -f '{TestFilePath}' .?";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_MultiLine_SingleResult_SpecialCharacter

        #region FixedStrings_FlagMiddle_FlagShort_MultiLine_MultiResult_Alpha
        [Test]
        public void FixedStrings_FlagMiddle_FlagShort_MultiLine_MultiResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-i -F -f '{TestFilePath}' lazy";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 6);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_MultiLine_MultiResult_Alpha

        #region FixedStrings_FlagMiddle_FlagShort_MultiLine_MultiResult_Numeric
        [Test]
        public void FixedStrings_FlagMiddle_FlagShort_MultiLine_MultiResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-i -F -f '{TestFilePath}' 10";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 5);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_MultiLine_MultiResult_Numeric

        #region FixedStrings_FlagMiddle_FlagShort_MultiLine_MultiResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagMiddle_FlagShort_MultiLine_MultiResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-i -F -f '{TestFilePath}' 10 lazy";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 4);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_MultiLine_MultiResult_Alphanumeric

        #region FixedStrings_FlagMiddle_FlagShort_MultiLine_MultiResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagMiddle_FlagShort_MultiLine_MultiResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-i -F -f '{TestFilePath}' " + @"\s";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_MultiLine_MultiResult_SpecialCharacter

        #region FixedStrings_FlagMiddle_FlagLong_SingleLine_SingleResult_Alpha
        [Test]
        public void FixedStrings_FlagMiddle_FlagLong_SingleLine_SingleResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-i --fixed-strings -f '{TestFilePath}' brown fox";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_SingleLine_SingleResult_Alpha

        #region FixedStrings_FlagMiddle_FlagLong_SingleLine_SingleResult_Numeric
        [Test]
        public void FixedStrings_FlagMiddle_FlagLong_SingleLine_SingleResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-i --fixed-strings -f '{TestFilePath}' 4";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_SingleLine_SingleResult_Numeric

        #region FixedStrings_FlagMiddle_FlagLong_SingleLine_SingleResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagMiddle_FlagLong_SingleLine_SingleResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-i --fixed-strings -f '{TestFilePath}' the 4 lazy";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_SingleLine_SingleResult_Alphanumeric

        #region FixedStrings_FlagMiddle_FlagLong_SingleLine_SingleResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagMiddle_FlagLong_SingleLine_SingleResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-i --fixed-strings -f '{TestFilePath}' " + @"\s";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_SingleLine_SingleResult_SpecialCharacter

        #region FixedStrings_FlagMiddle_FlagLong_SingleLine_MultiResult_Alpha
        [Test]
        public void FixedStrings_FlagMiddle_FlagLong_SingleLine_MultiResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-i --fixed-strings -f '{TestFilePath}' lazy";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 3);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_SingleLine_MultiResult_Alpha

        #region FixedStrings_FlagMiddle_FlagLong_SingleLine_MultiResult_Numeric
        [Test]
        public void FixedStrings_FlagMiddle_FlagLong_SingleLine_MultiResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-i --fixed-strings -f '{TestFilePath}' 10";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_SingleLine_MultiResult_Numeric

        #region FixedStrings_FlagMiddle_FlagLong_SingleLine_MultiResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagMiddle_FlagLong_SingleLine_MultiResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-i --fixed-strings -f '{TestFilePath}' 10 lazy";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_SingleLine_MultiResult_Alphanumeric

        #region FixedStrings_FlagMiddle_FlagLong_SingleLine_MultiResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagMiddle_FlagLong_SingleLine_MultiResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-i --fixed-strings -f '{TestFilePath}' .*";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_SingleLine_MultiResult_SpecialCharacter

        #region FixedStrings_FlagMiddle_FlagLong_MultiLine_SingleResult_Alpha
        [Test]
        public void FixedStrings_FlagMiddle_FlagLong_MultiLine_SingleResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-i --fixed-strings -f '{TestFilePath}' brown fox";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_MultiLine_SingleResult_Alpha

        #region FixedStrings_FlagMiddle_FlagLong_MultiLine_SingleResult_Numeric
        [Test]
        public void FixedStrings_FlagMiddle_FlagLong_MultiLine_SingleResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-i --fixed-strings -f '{TestFilePath}' 4";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_MultiLine_SingleResult_Numeric

        #region FixedStrings_FlagMiddle_FlagLong_MultiLine_SingleResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagMiddle_FlagLong_MultiLine_SingleResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-i --fixed-strings -f '{TestFilePath}' fox hops";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_MultiLine_SingleResult_Alphanumeric

        #region FixedStrings_FlagMiddle_FlagLong_MultiLine_SingleResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagMiddle_FlagLong_MultiLine_SingleResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-i --fixed-strings -f '{TestFilePath}' .?";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_MultiLine_SingleResult_SpecialCharacter

        #region FixedStrings_FlagMiddle_FlagLong_MultiLine_MultiResult_Alpha
        [Test]
        public void FixedStrings_FlagMiddle_FlagLong_MultiLine_MultiResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-i --fixed-strings -f '{TestFilePath}' lazy";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 6);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_MultiLine_MultiResult_Alpha

        #region FixedStrings_FlagMiddle_FlagLong_MultiLine_MultiResult_Numeric
        [Test]
        public void FixedStrings_FlagMiddle_FlagLong_MultiLine_MultiResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-i --fixed-strings -f '{TestFilePath}' 10";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 5);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_MultiLine_MultiResult_Numeric

        #region FixedStrings_FlagMiddle_FlagLong_MultiLine_MultiResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagMiddle_FlagLong_MultiLine_MultiResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-i --fixed-strings -f '{TestFilePath}' 10 lazy";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 4);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_MultiLine_MultiResult_Alphanumeric 

        #region FixedStrings_FlagMiddle_FlagLong_MultiLine_MultiResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagMiddle_FlagLong_MultiLine_MultiResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-i --fixed-strings -f '{TestFilePath}' " + @"\s";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
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

            string Command = $"-f '{TestFilePath}' -F brown fox";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FixedStrings_FlagLast_FlagShort_SingleLine_SingleResult_Alpha

        #region FixedStrings_FlagLast_FlagShort_SingleLine_SingleResult_Numeric
        [Test]
        public void FixedStrings_FlagLast_FlagShort_SingleLine_SingleResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-f '{TestFilePath}' -F 4";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FixedStrings_FlagLast_FlagShort_SingleLine_SingleResult_Numeric

        #region FixedStrings_FlagLast_FlagShort_SingleLine_SingleResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagLast_FlagShort_SingleLine_SingleResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-f '{TestFilePath}' -F the 4 lazy";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FixedStrings_FlagLast_FlagShort_SingleLine_SingleResult_Alphanumeric

        #region FixedStrings_FlagLast_FlagShort_SingleLine_SingleResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagLast_FlagShort_SingleLine_SingleResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-f '{TestFilePath}' -F " + @"\s";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FixedStrings_FlagLast_FlagShort_SingleLine_SingleResult_SpecialCharacter

        #region FixedStrings_FlagLast_FlagShort_SingleLine_MultiResult_Alpha
        [Test]
        public void FixedStrings_FlagLast_FlagShort_SingleLine_MultiResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-f '{TestFilePath}' -F lazy";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 3);
        }
        #endregion FixedStrings_FlagLast_FlagShort_SingleLine_MultiResult_Alpha

        #region FixedStrings_FlagLast_FlagShort_SingleLine_MultiResult_Numeric
        [Test]
        public void FixedStrings_FlagLast_FlagShort_SingleLine_MultiResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-f '{TestFilePath}' -F 10";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FixedStrings_FlagLast_FlagShort_SingleLine_MultiResult_Numeric

        #region FixedStrings_FlagLast_FlagShort_SingleLine_MultiResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagLast_FlagShort_SingleLine_MultiResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-f '{TestFilePath}' -F 10 lazy";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FixedStrings_FlagLast_FlagShort_SingleLine_MultiResult_Alphanumeric

        #region FixedStrings_FlagLast_FlagShort_SingleLine_MultiResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagLast_FlagShort_SingleLine_MultiResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-f '{TestFilePath}' -F .*";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FixedStrings_FlagLast_FlagShort_SingleLine_MultiResult_SpecialCharacter

        #region FixedStrings_FlagLast_FlagShort_MultiLine_SingleResult_Alpha
        [Test]
        public void FixedStrings_FlagLast_FlagShort_MultiLine_SingleResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-f '{TestFilePath}' -F brown fox";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FixedStrings_FlagLast_FlagShort_MultiLine_SingleResult_Alpha

        #region FixedStrings_FlagLast_FlagShort_MultiLine_SingleResult_Numeric
        [Test]
        public void FixedStrings_FlagLast_FlagShort_MultiLine_SingleResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-f '{TestFilePath}' -F 4";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FixedStrings_FlagLast_FlagShort_MultiLine_SingleResult_Numeric

        #region FixedStrings_FlagLast_FlagShort_MultiLine_SingleResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagLast_FlagShort_MultiLine_SingleResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-f '{TestFilePath}' -F fox hops";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FixedStrings_FlagLast_FlagShort_MultiLine_SingleResult_Alphanumeric

        #region FixedStrings_FlagLast_FlagShort_MultiLine_SingleResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagLast_FlagShort_MultiLine_SingleResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-f '{TestFilePath}' -F .?";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FixedStrings_FlagLast_FlagShort_MultiLine_SingleResult_SpecialCharacter

        #region FixedStrings_FlagLast_FlagShort_MultiLine_MultiResult_Alpha
        [Test]
        public void FixedStrings_FlagLast_FlagShort_MultiLine_MultiResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-f '{TestFilePath}' -F lazy";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 6);
        }
        #endregion FixedStrings_FlagLast_FlagShort_MultiLine_MultiResult_Alpha

        #region FixedStrings_FlagLast_FlagShort_MultiLine_MultiResult_Numeric
        [Test]
        public void FixedStrings_FlagLast_FlagShort_MultiLine_MultiResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-f '{TestFilePath}' -F 10";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 5);
        }
        #endregion FixedStrings_FlagLast_FlagShort_MultiLine_MultiResult_Numeric

        #region FixedStrings_FlagLast_FlagShort_MultiLine_MultiResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagLast_FlagShort_MultiLine_MultiResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-f '{TestFilePath}' -F 10 lazy";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 4);
        }
        #endregion FixedStrings_FlagLast_FlagShort_MultiLine_MultiResult_Alphanumeric

        #region FixedStrings_FlagLast_FlagShort_MultiLine_MultiResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagLast_FlagShort_MultiLine_MultiResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-f '{TestFilePath}' -F " + @"\s";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FixedStrings_FlagLast_FlagShort_MultiLine_MultiResult_SpecialCharacter

        #region FixedStrings_FlagLast_FlagLong_SingleLine_SingleResult_Alpha
        [Test]
        public void FixedStrings_FlagLast_FlagLong_SingleLine_SingleResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-f '{TestFilePath}' --fixed-strings brown fox";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FixedStrings_FlagLast_FlagLong_SingleLine_SingleResult_Alpha

        #region FixedStrings_FlagLast_FlagLong_SingleLine_SingleResult_Numeric
        [Test]
        public void FixedStrings_FlagLast_FlagLong_SingleLine_SingleResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-f '{TestFilePath}' --fixed-strings 4";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FixedStrings_FlagLast_FlagLong_SingleLine_SingleResult_Numeric

        #region FixedStrings_FlagLast_FlagLong_SingleLine_SingleResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagLast_FlagLong_SingleLine_SingleResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-f '{TestFilePath}' --fixed-strings the 4 lazy";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FixedStrings_FlagLast_FlagLong_SingleLine_SingleResult_Alphanumeric

        #region FixedStrings_FlagLast_FlagLong_SingleLine_SingleResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagLast_FlagLong_SingleLine_SingleResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-f '{TestFilePath}' --fixed-strings " + @"\s";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FixedStrings_FlagLast_FlagLong_SingleLine_SingleResult_SpecialCharacter

        #region FixedStrings_FlagLast_FlagLong_SingleLine_MultiResult_Alpha
        [Test]
        public void FixedStrings_FlagLast_FlagLong_SingleLine_MultiResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-f '{TestFilePath}' --fixed-strings lazy";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 3);
        }
        #endregion FixedStrings_FlagLast_FlagLong_SingleLine_MultiResult_Alpha

        #region FixedStrings_FlagLast_FlagLong_SingleLine_MultiResult_Numeric
        [Test]
        public void FixedStrings_FlagLast_FlagLong_SingleLine_MultiResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-f '{TestFilePath}' --fixed-strings 10";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FixedStrings_FlagLast_FlagLong_SingleLine_MultiResult_Numeric

        #region FixedStrings_FlagLast_FlagLong_SingleLine_MultiResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagLast_FlagLong_SingleLine_MultiResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-f '{TestFilePath}' --fixed-strings 10 lazy";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FixedStrings_FlagLast_FlagLong_SingleLine_MultiResult_Alphanumeric

        #region FixedStrings_FlagLast_FlagLong_SingleLine_MultiResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagLast_FlagLong_SingleLine_MultiResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-f '{TestFilePath}' --fixed-strings .*";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FixedStrings_FlagLast_FlagLong_SingleLine_MultiResult_SpecialCharacter

        #region FixedStrings_FlagLast_FlagLong_MultiLine_SingleResult_Alpha
        [Test]
        public void FixedStrings_FlagLast_FlagLong_MultiLine_SingleResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-f '{TestFilePath}' --fixed-strings brown fox";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FixedStrings_FlagLast_FlagLong_MultiLine_SingleResult_Alpha

        #region FixedStrings_FlagLast_FlagLong_MultiLine_SingleResult_Numeric
        [Test]
        public void FixedStrings_FlagLast_FlagLong_MultiLine_SingleResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-f '{TestFilePath}' --fixed-strings 4";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FixedStrings_FlagLast_FlagLong_MultiLine_SingleResult_Numeric

        #region FixedStrings_FlagLast_FlagLong_MultiLine_SingleResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagLast_FlagLong_MultiLine_SingleResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-f '{TestFilePath}' --fixed-strings fox hops";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FixedStrings_FlagLast_FlagLong_MultiLine_SingleResult_Alphanumeric

        #region FixedStrings_FlagLast_FlagLong_MultiLine_SingleResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagLast_FlagLong_MultiLine_SingleResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-f '{TestFilePath}' --fixed-strings .?";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 1);
        }
        #endregion FixedStrings_FlagLast_FlagLong_MultiLine_SingleResult_SpecialCharacter

        #region FixedStrings_FlagLast_FlagLong_MultiLine_MultiResult_Alpha
        [Test]
        public void FixedStrings_FlagLast_FlagLong_MultiLine_MultiResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-f '{TestFilePath}' --fixed-strings lazy";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 6);
        }
        #endregion FixedStrings_FlagLast_FlagLong_MultiLine_MultiResult_Alpha

        #region FixedStrings_FlagLast_FlagLong_MultiLine_MultiResult_Numeric
        [Test]
        public void FixedStrings_FlagLast_FlagLong_MultiLine_MultiResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-f '{TestFilePath}' --fixed-strings 10";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 5);
        }
        #endregion FixedStrings_FlagLast_FlagLong_MultiLine_MultiResult_Numeric

        #region FixedStrings_FlagLast_FlagLong_MultiLine_MultiResult_Alphanumeric
        [Test]
        public void FixedStrings_FlagLast_FlagLong_MultiLine_MultiResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-f '{TestFilePath}' --fixed-strings 10 lazy";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 4);
        }
        #endregion FixedStrings_FlagLast_FlagLong_MultiLine_MultiResult_Alphanumeric 

        #region FixedStrings_FlagLast_FlagLong_MultiLine_MultiResult_SpecialCharacter
        [Test]
        public void FixedStrings_FlagLast_FlagLong_MultiLine_MultiResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string Command = $"-f '{TestFilePath}' --fixed-strings " + @"\s";
            var GrepResultCollection = new GrepResultCollection();

            GrepEngine.RunCommand(Command, GrepResultCollection);

            Assert.IsTrue(GrepResultCollection.Count == 2);
        }
        #endregion FixedStrings_FlagLast_FlagLong_MultiLine_MultiResult_SpecialCharacter 
        #endregion FlagLast..
        #endregion Tests..
    }
}
