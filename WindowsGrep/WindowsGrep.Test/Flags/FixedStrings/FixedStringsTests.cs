using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WindowsGrep.Common;
using WindowsGrep.Core;
using WindowsGrep.Engine;

namespace WindowsGrep.Test.Flags.FixedStrings
{
    public class FixedStringsTests : TestBase
    {
        #region Fields..
        private string _flagDescriptorShort;
        private string _flagDescriptorLong;
        private string _testDataRelativePath = @"Flags\FixedStrings\TestData";
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Setup
        [SetUp]
        public void Setup()
        {
            TestDataDirectory = Path.Combine(TestConfigurationManager.WorkingDirectory, _testDataRelativePath);

            List<string> DescriptionCollection = ConsoleFlag.FixedStrings.GetCustomAttribute<DescriptionCollectionAttribute>()?.Value.OrderBy(x => x.Length).ToList();
            _flagDescriptorShort = DescriptionCollection[0];
            _flagDescriptorLong = DescriptionCollection[1];
        }
        #endregion Setup

        #region Tests..
        #region FlagFirst..
        #region FixedStrings_FlagFirst_FlagShort_SingleLine_SingleResult_Alpha
        [Test]
        public async Task FixedStrings_FlagFirst_FlagShort_SingleLine_SingleResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "brown fox";
            string Command = $"{_flagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_SingleLine_SingleResult_Alpha

        #region FixedStrings_FlagFirst_FlagShort_SingleLine_SingleResult_Numeric
        [Test]
        public async Task FixedStrings_FlagFirst_FlagShort_SingleLine_SingleResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "4";
            string Command = $"{_flagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_SingleLine_SingleResult_Numeric

        #region FixedStrings_FlagFirst_FlagShort_SingleLine_SingleResult_Alphanumeric
        [Test]
        public async Task FixedStrings_FlagFirst_FlagShort_SingleLine_SingleResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "the 4 lazy";
            string Command = $"{_flagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_SingleLine_SingleResult_Alphanumeric

        #region FixedStrings_FlagFirst_FlagShort_SingleLine_SingleResult_SpecialCharacter
        [Test]
        public async Task FixedStrings_FlagFirst_FlagShort_SingleLine_SingleResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"\s";
            string Command = $"{_flagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_SingleLine_SingleResult_SpecialCharacter

        #region FixedStrings_FlagFirst_FlagShort_SingleLine_MultiResult_Alpha
        [Test]
        public async Task FixedStrings_FlagFirst_FlagShort_SingleLine_MultiResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "lazy";
            string Command = $"{_flagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 3);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_SingleLine_MultiResult_Alpha

        #region FixedStrings_FlagFirst_FlagShort_SingleLine_MultiResult_Numeric
        [Test]
        public async Task FixedStrings_FlagFirst_FlagShort_SingleLine_MultiResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10";
            string Command = $"{_flagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_SingleLine_MultiResult_Numeric

        #region FixedStrings_FlagFirst_FlagShort_SingleLine_MultiResult_Alphanumeric
        [Test]
        public async Task FixedStrings_FlagFirst_FlagShort_SingleLine_MultiResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10 lazy";
            string Command = $"{_flagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_SingleLine_MultiResult_Alphanumeric

        #region FixedStrings_FlagFirst_FlagShort_SingleLine_MultiResult_SpecialCharacter
        [Test]
        public async Task FixedStrings_FlagFirst_FlagShort_SingleLine_MultiResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = ".*";
            string Command = $"{_flagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_SingleLine_MultiResult_SpecialCharacter

        #region FixedStrings_FlagFirst_FlagShort_MultiLine_SingleResult_Alpha
        [Test]
        public async Task FixedStrings_FlagFirst_FlagShort_MultiLine_SingleResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "brown fox";
            string Command = $"{_flagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_MultiLine_SingleResult_Alpha

        #region FixedStrings_FlagFirst_FlagShort_MultiLine_SingleResult_Numeric
        [Test]
        public async Task FixedStrings_FlagFirst_FlagShort_MultiLine_SingleResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "4";
            string Command = $"{_flagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_MultiLine_SingleResult_Numeric

        #region FixedStrings_FlagFirst_FlagShort_MultiLine_SingleResult_Alphanumeric
        [Test]
        public async Task FixedStrings_FlagFirst_FlagShort_MultiLine_SingleResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox hops";
            string Command = $"{_flagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_MultiLine_SingleResult_Alphanumeric

        #region FixedStrings_FlagFirst_FlagShort_MultiLine_SingleResult_SpecialCharacter
        [Test]
        public async Task FixedStrings_FlagFirst_FlagShort_MultiLine_SingleResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = ".?";
            string Command = $"{_flagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_MultiLine_SingleResult_SpecialCharacter

        #region FixedStrings_FlagFirst_FlagShort_MultiLine_MultiResult_Alpha
        [Test]
        public async Task FixedStrings_FlagFirst_FlagShort_MultiLine_MultiResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "lazy";
            string Command = $"{_flagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 6);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_MultiLine_MultiResult_Alpha

        #region FixedStrings_FlagFirst_FlagShort_MultiLine_MultiResult_Numeric
        [Test]
        public async Task FixedStrings_FlagFirst_FlagShort_MultiLine_MultiResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10";
            string Command = $"{_flagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 5);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_MultiLine_MultiResult_Numeric

        #region FixedStrings_FlagFirst_FlagShort_MultiLine_MultiResult_Alphanumeric
        [Test]
        public async Task FixedStrings_FlagFirst_FlagShort_MultiLine_MultiResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10 lazy";
            string Command = $"{_flagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 4);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_MultiLine_MultiResult_Alphanumeric

        #region FixedStrings_FlagFirst_FlagShort_MultiLine_MultiResult_SpecialCharacter
        [Test]
        public async Task FixedStrings_FlagFirst_FlagShort_MultiLine_MultiResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"\s";
            string Command = $"{_flagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagFirst_FlagShort_MultiLine_MultiResult_SpecialCharacter

        #region FixedStrings_FlagFirst_FlagLong_SingleLine_SingleResult_Alpha
        [Test]
        public async Task FixedStrings_FlagFirst_FlagLong_SingleLine_SingleResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "brown fox";
            string Command = $"{_flagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_SingleLine_SingleResult_Alpha

        #region FixedStrings_FlagFirst_FlagLong_SingleLine_SingleResult_Numeric
        [Test]
        public async Task FixedStrings_FlagFirst_FlagLong_SingleLine_SingleResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "4";
            string Command = $"{_flagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_SingleLine_SingleResult_Numeric

        #region FixedStrings_FlagFirst_FlagLong_SingleLine_SingleResult_Alphanumeric
        [Test]
        public async Task FixedStrings_FlagFirst_FlagLong_SingleLine_SingleResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "the 4 lazy";
            string Command = $"{_flagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_SingleLine_SingleResult_Alphanumeric

        #region FixedStrings_FlagFirst_FlagLong_SingleLine_SingleResult_SpecialCharacter
        [Test]
        public async Task FixedStrings_FlagFirst_FlagLong_SingleLine_SingleResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"\s";
            string Command = $"{_flagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_SingleLine_SingleResult_SpecialCharacter

        #region FixedStrings_FlagFirst_FlagLong_SingleLine_MultiResult_Alpha
        [Test]
        public async Task FixedStrings_FlagFirst_FlagLong_SingleLine_MultiResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "lazy";
            string Command = $"{_flagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 3);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_SingleLine_MultiResult_Alpha

        #region FixedStrings_FlagFirst_FlagLong_SingleLine_MultiResult_Numeric
        [Test]
        public async Task FixedStrings_FlagFirst_FlagLong_SingleLine_MultiResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10";
            string Command = $"{_flagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_SingleLine_MultiResult_Numeric

        #region FixedStrings_FlagFirst_FlagLong_SingleLine_MultiResult_Alphanumeric
        [Test]
        public async Task FixedStrings_FlagFirst_FlagLong_SingleLine_MultiResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10 lazy";
            string Command = $"{_flagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_SingleLine_MultiResult_Alphanumeric

        #region FixedStrings_FlagFirst_FlagLong_SingleLine_MultiResult_SpecialCharacter
        [Test]
        public async Task FixedStrings_FlagFirst_FlagLong_SingleLine_MultiResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = ".*";
            string Command = $"{_flagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_SingleLine_MultiResult_SpecialCharacter

        #region FixedStrings_FlagFirst_FlagLong_MultiLine_SingleResult_Alpha
        [Test]
        public async Task FixedStrings_FlagFirst_FlagLong_MultiLine_SingleResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "brown fox";
            string Command = $"{_flagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_MultiLine_SingleResult_Alpha

        #region FixedStrings_FlagFirst_FlagLong_MultiLine_SingleResult_Numeric
        [Test]
        public async Task FixedStrings_FlagFirst_FlagLong_MultiLine_SingleResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "4";
            string Command = $"{_flagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_MultiLine_SingleResult_Numeric

        #region FixedStrings_FlagFirst_FlagLong_MultiLine_SingleResult_Alphanumeric
        [Test]
        public async Task FixedStrings_FlagFirst_FlagLong_MultiLine_SingleResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox hops";
            string Command = $"{_flagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_MultiLine_SingleResult_Alphanumeric

        #region FixedStrings_FlagFirst_FlagLong_MultiLine_SingleResult_SpecialCharacter
        [Test]
        public async Task FixedStrings_FlagFirst_FlagLong_MultiLine_SingleResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = ".?";
            string Command = $"{_flagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_MultiLine_SingleResult_SpecialCharacter

        #region FixedStrings_FlagFirst_FlagLong_MultiLine_MultiResult_Alpha
        [Test]
        public async Task FixedStrings_FlagFirst_FlagLong_MultiLine_MultiResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "lazy";
            string Command = $"{_flagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 6);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_MultiLine_MultiResult_Alpha

        #region FixedStrings_FlagFirst_FlagLong_MultiLine_MultiResult_Numeric
        [Test]
        public async Task FixedStrings_FlagFirst_FlagLong_MultiLine_MultiResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10";
            string Command = $"{_flagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 5);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_MultiLine_MultiResult_Numeric

        #region FixedStrings_FlagFirst_FlagLong_MultiLine_MultiResult_Alphanumeric
        [Test]
        public async Task FixedStrings_FlagFirst_FlagLong_MultiLine_MultiResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10 lazy";
            string Command = $"{_flagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 4);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_MultiLine_MultiResult_Alphanumeric 

        #region FixedStrings_FlagFirst_FlagLong_MultiLine_MultiResult_SpecialCharacter
        [Test]
        public async Task FixedStrings_FlagFirst_FlagLong_MultiLine_MultiResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"\s";
            string Command = $"{_flagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagFirst_FlagLong_MultiLine_MultiResult_SpecialCharacter 
        #endregion FlagFirst..

        #region FlagMiddle..
        #region FixedStrings_FlagMiddle_FlagShort_SingleLine_SingleResult_Alpha
        [Test]
        public async Task FixedStrings_FlagMiddle_FlagShort_SingleLine_SingleResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "brown fox";
            string Command = $"-i {_flagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_SingleLine_SingleResult_Alpha

        #region FixedStrings_FlagMiddle_FlagShort_SingleLine_SingleResult_Numeric
        [Test]
        public async Task FixedStrings_FlagMiddle_FlagShort_SingleLine_SingleResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "4";
            string Command = $"-i {_flagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_SingleLine_SingleResult_Numeric

        #region FixedStrings_FlagMiddle_FlagShort_SingleLine_SingleResult_Alphanumeric
        [Test]
        public async Task FixedStrings_FlagMiddle_FlagShort_SingleLine_SingleResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "the 4 lazy";
            string Command = $"-i {_flagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_SingleLine_SingleResult_Alphanumeric

        #region FixedStrings_FlagMiddle_FlagShort_SingleLine_SingleResult_SpecialCharacter
        [Test]
        public async Task FixedStrings_FlagMiddle_FlagShort_SingleLine_SingleResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"\s";
            string Command = $"-i {_flagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_SingleLine_SingleResult_SpecialCharacter

        #region FixedStrings_FlagMiddle_FlagShort_SingleLine_MultiResult_Alpha
        [Test]
        public async Task FixedStrings_FlagMiddle_FlagShort_SingleLine_MultiResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "lazy";
            string Command = $"-i {_flagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 3);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_SingleLine_MultiResult_Alpha

        #region FixedStrings_FlagMiddle_FlagShort_SingleLine_MultiResult_Numeric
        [Test]
        public async Task FixedStrings_FlagMiddle_FlagShort_SingleLine_MultiResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10";
            string Command = $"-i {_flagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_SingleLine_MultiResult_Numeric

        #region FixedStrings_FlagMiddle_FlagShort_SingleLine_MultiResult_Alphanumeric
        [Test]
        public async Task FixedStrings_FlagMiddle_FlagShort_SingleLine_MultiResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10 lazy";
            string Command = $"-i {_flagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_SingleLine_MultiResult_Alphanumeric

        #region FixedStrings_FlagMiddle_FlagShort_SingleLine_MultiResult_SpecialCharacter
        [Test]
        public async Task FixedStrings_FlagMiddle_FlagShort_SingleLine_MultiResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = ".*";
            string Command = $"-i {_flagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_SingleLine_MultiResult_SpecialCharacter

        #region FixedStrings_FlagMiddle_FlagShort_MultiLine_SingleResult_Alpha
        [Test]
        public async Task FixedStrings_FlagMiddle_FlagShort_MultiLine_SingleResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "brown fox";
            string Command = $"-i {_flagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_MultiLine_SingleResult_Alpha

        #region FixedStrings_FlagMiddle_FlagShort_MultiLine_SingleResult_Numeric
        [Test]
        public async Task FixedStrings_FlagMiddle_FlagShort_MultiLine_SingleResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "4";
            string Command = $"-i {_flagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_MultiLine_SingleResult_Numeric

        #region FixedStrings_FlagMiddle_FlagShort_MultiLine_SingleResult_Alphanumeric
        [Test]
        public async Task FixedStrings_FlagMiddle_FlagShort_MultiLine_SingleResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox hops";
            string Command = $"-i {_flagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_MultiLine_SingleResult_Alphanumeric

        #region FixedStrings_FlagMiddle_FlagShort_MultiLine_SingleResult_SpecialCharacter
        [Test]
        public async Task FixedStrings_FlagMiddle_FlagShort_MultiLine_SingleResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = ".?";
            string Command = $"-i {_flagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_MultiLine_SingleResult_SpecialCharacter

        #region FixedStrings_FlagMiddle_FlagShort_MultiLine_MultiResult_Alpha
        [Test]
        public async Task FixedStrings_FlagMiddle_FlagShort_MultiLine_MultiResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "lazy";
            string Command = $"-i {_flagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 6);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_MultiLine_MultiResult_Alpha

        #region FixedStrings_FlagMiddle_FlagShort_MultiLine_MultiResult_Numeric
        [Test]
        public async Task FixedStrings_FlagMiddle_FlagShort_MultiLine_MultiResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10";
            string Command = $"-i {_flagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 5);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_MultiLine_MultiResult_Numeric

        #region FixedStrings_FlagMiddle_FlagShort_MultiLine_MultiResult_Alphanumeric
        [Test]
        public async Task FixedStrings_FlagMiddle_FlagShort_MultiLine_MultiResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10 lazy";
            string Command = $"-i {_flagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 4);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_MultiLine_MultiResult_Alphanumeric

        #region FixedStrings_FlagMiddle_FlagShort_MultiLine_MultiResult_SpecialCharacter
        [Test]
        public async Task FixedStrings_FlagMiddle_FlagShort_MultiLine_MultiResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"\s";
            string Command = $"-i {_flagDescriptorShort} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagMiddle_FlagShort_MultiLine_MultiResult_SpecialCharacter

        #region FixedStrings_FlagMiddle_FlagLong_SingleLine_SingleResult_Alpha
        [Test]
        public async Task FixedStrings_FlagMiddle_FlagLong_SingleLine_SingleResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "brown fox";
            string Command = $"-i {_flagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_SingleLine_SingleResult_Alpha

        #region FixedStrings_FlagMiddle_FlagLong_SingleLine_SingleResult_Numeric
        [Test]
        public async Task FixedStrings_FlagMiddle_FlagLong_SingleLine_SingleResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "4";
            string Command = $"-i {_flagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_SingleLine_SingleResult_Numeric

        #region FixedStrings_FlagMiddle_FlagLong_SingleLine_SingleResult_Alphanumeric
        [Test]
        public async Task FixedStrings_FlagMiddle_FlagLong_SingleLine_SingleResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "the 4 lazy";
            string Command = $"-i {_flagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_SingleLine_SingleResult_Alphanumeric

        #region FixedStrings_FlagMiddle_FlagLong_SingleLine_SingleResult_SpecialCharacter
        [Test]
        public async Task FixedStrings_FlagMiddle_FlagLong_SingleLine_SingleResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"\s";
            string Command = $"-i {_flagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_SingleLine_SingleResult_SpecialCharacter

        #region FixedStrings_FlagMiddle_FlagLong_SingleLine_MultiResult_Alpha
        [Test]
        public async Task FixedStrings_FlagMiddle_FlagLong_SingleLine_MultiResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "lazy";
            string Command = $"-i {_flagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 3);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_SingleLine_MultiResult_Alpha

        #region FixedStrings_FlagMiddle_FlagLong_SingleLine_MultiResult_Numeric
        [Test]
        public async Task FixedStrings_FlagMiddle_FlagLong_SingleLine_MultiResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10";
            string Command = $"-i {_flagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_SingleLine_MultiResult_Numeric

        #region FixedStrings_FlagMiddle_FlagLong_SingleLine_MultiResult_Alphanumeric
        [Test]
        public async Task FixedStrings_FlagMiddle_FlagLong_SingleLine_MultiResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10 lazy";
            string Command = $"-i {_flagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_SingleLine_MultiResult_Alphanumeric

        #region FixedStrings_FlagMiddle_FlagLong_SingleLine_MultiResult_SpecialCharacter
        [Test]
        public async Task FixedStrings_FlagMiddle_FlagLong_SingleLine_MultiResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = ".*";
            string Command = $"-i {_flagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_SingleLine_MultiResult_SpecialCharacter

        #region FixedStrings_FlagMiddle_FlagLong_MultiLine_SingleResult_Alpha
        [Test]
        public async Task FixedStrings_FlagMiddle_FlagLong_MultiLine_SingleResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "brown fox";
            string Command = $"-i {_flagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_MultiLine_SingleResult_Alpha

        #region FixedStrings_FlagMiddle_FlagLong_MultiLine_SingleResult_Numeric
        [Test]
        public async Task FixedStrings_FlagMiddle_FlagLong_MultiLine_SingleResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "4";
            string Command = $"-i {_flagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_MultiLine_SingleResult_Numeric

        #region FixedStrings_FlagMiddle_FlagLong_MultiLine_SingleResult_Alphanumeric
        [Test]
        public async Task FixedStrings_FlagMiddle_FlagLong_MultiLine_SingleResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox hops";
            string Command = $"-i {_flagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_MultiLine_SingleResult_Alphanumeric

        #region FixedStrings_FlagMiddle_FlagLong_MultiLine_SingleResult_SpecialCharacter
        [Test]
        public async Task FixedStrings_FlagMiddle_FlagLong_MultiLine_SingleResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = ".?";
            string Command = $"-i {_flagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_MultiLine_SingleResult_SpecialCharacter

        #region FixedStrings_FlagMiddle_FlagLong_MultiLine_MultiResult_Alpha
        [Test]
        public async Task FixedStrings_FlagMiddle_FlagLong_MultiLine_MultiResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "lazy";
            string Command = $"-i {_flagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 6);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_MultiLine_MultiResult_Alpha

        #region FixedStrings_FlagMiddle_FlagLong_MultiLine_MultiResult_Numeric
        [Test]
        public async Task FixedStrings_FlagMiddle_FlagLong_MultiLine_MultiResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10";
            string Command = $"-i {_flagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 5);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_MultiLine_MultiResult_Numeric

        #region FixedStrings_FlagMiddle_FlagLong_MultiLine_MultiResult_Alphanumeric
        [Test]
        public async Task FixedStrings_FlagMiddle_FlagLong_MultiLine_MultiResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10 lazy";
            string Command = $"-i {_flagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 4);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_MultiLine_MultiResult_Alphanumeric 

        #region FixedStrings_FlagMiddle_FlagLong_MultiLine_MultiResult_SpecialCharacter
        [Test]
        public async Task FixedStrings_FlagMiddle_FlagLong_MultiLine_MultiResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"\s";
            string Command = $"-i {_flagDescriptorLong} -f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagMiddle_FlagLong_MultiLine_MultiResult_SpecialCharacter 
        #endregion FlagMiddle..

        #region FlagLast..  
        #region FixedStrings_FlagLast_FlagShort_SingleLine_SingleResult_Alpha
        [Test]
        public async Task FixedStrings_FlagLast_FlagShort_SingleLine_SingleResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "brown fox";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorShort}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagLast_FlagShort_SingleLine_SingleResult_Alpha

        #region FixedStrings_FlagLast_FlagShort_SingleLine_SingleResult_Numeric
        [Test]
        public async Task FixedStrings_FlagLast_FlagShort_SingleLine_SingleResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "4";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorShort}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagLast_FlagShort_SingleLine_SingleResult_Numeric

        #region FixedStrings_FlagLast_FlagShort_SingleLine_SingleResult_Alphanumeric
        [Test]
        public async Task FixedStrings_FlagLast_FlagShort_SingleLine_SingleResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "the 4 lazy";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorShort}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagLast_FlagShort_SingleLine_SingleResult_Alphanumeric

        #region FixedStrings_FlagLast_FlagShort_SingleLine_SingleResult_SpecialCharacter
        [Test]
        public async Task FixedStrings_FlagLast_FlagShort_SingleLine_SingleResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"\s";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorShort}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagLast_FlagShort_SingleLine_SingleResult_SpecialCharacter

        #region FixedStrings_FlagLast_FlagShort_SingleLine_MultiResult_Alpha
        [Test]
        public async Task FixedStrings_FlagLast_FlagShort_SingleLine_MultiResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "lazy";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorShort}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 3);
        }
        #endregion FixedStrings_FlagLast_FlagShort_SingleLine_MultiResult_Alpha

        #region FixedStrings_FlagLast_FlagShort_SingleLine_MultiResult_Numeric
        [Test]
        public async Task FixedStrings_FlagLast_FlagShort_SingleLine_MultiResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorShort}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagLast_FlagShort_SingleLine_MultiResult_Numeric

        #region FixedStrings_FlagLast_FlagShort_SingleLine_MultiResult_Alphanumeric
        [Test]
        public async Task FixedStrings_FlagLast_FlagShort_SingleLine_MultiResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10 lazy";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorShort}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagLast_FlagShort_SingleLine_MultiResult_Alphanumeric

        #region FixedStrings_FlagLast_FlagShort_SingleLine_MultiResult_SpecialCharacter
        [Test]
        public async Task FixedStrings_FlagLast_FlagShort_SingleLine_MultiResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = ".*";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorShort}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagLast_FlagShort_SingleLine_MultiResult_SpecialCharacter

        #region FixedStrings_FlagLast_FlagShort_MultiLine_SingleResult_Alpha
        [Test]
        public async Task FixedStrings_FlagLast_FlagShort_MultiLine_SingleResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "brown fox";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorShort}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagLast_FlagShort_MultiLine_SingleResult_Alpha

        #region FixedStrings_FlagLast_FlagShort_MultiLine_SingleResult_Numeric
        [Test]
        public async Task FixedStrings_FlagLast_FlagShort_MultiLine_SingleResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "4";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorShort}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagLast_FlagShort_MultiLine_SingleResult_Numeric

        #region FixedStrings_FlagLast_FlagShort_MultiLine_SingleResult_Alphanumeric
        [Test]
        public async Task FixedStrings_FlagLast_FlagShort_MultiLine_SingleResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox hops";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorShort}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagLast_FlagShort_MultiLine_SingleResult_Alphanumeric

        #region FixedStrings_FlagLast_FlagShort_MultiLine_SingleResult_SpecialCharacter
        [Test]
        public async Task FixedStrings_FlagLast_FlagShort_MultiLine_SingleResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = ".?";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorShort}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagLast_FlagShort_MultiLine_SingleResult_SpecialCharacter

        #region FixedStrings_FlagLast_FlagShort_MultiLine_MultiResult_Alpha
        [Test]
        public async Task FixedStrings_FlagLast_FlagShort_MultiLine_MultiResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "lazy";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorShort}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 6);
        }
        #endregion FixedStrings_FlagLast_FlagShort_MultiLine_MultiResult_Alpha

        #region FixedStrings_FlagLast_FlagShort_MultiLine_MultiResult_Numeric
        [Test]
        public async Task FixedStrings_FlagLast_FlagShort_MultiLine_MultiResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorShort}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 5);
        }
        #endregion FixedStrings_FlagLast_FlagShort_MultiLine_MultiResult_Numeric

        #region FixedStrings_FlagLast_FlagShort_MultiLine_MultiResult_Alphanumeric
        [Test]
        public async Task FixedStrings_FlagLast_FlagShort_MultiLine_MultiResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10 lazy";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorShort}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 4);
        }
        #endregion FixedStrings_FlagLast_FlagShort_MultiLine_MultiResult_Alphanumeric

        #region FixedStrings_FlagLast_FlagShort_MultiLine_MultiResult_SpecialCharacter
        [Test]
        public async Task FixedStrings_FlagLast_FlagShort_MultiLine_MultiResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"\s";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorShort}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagLast_FlagShort_MultiLine_MultiResult_SpecialCharacter

        #region FixedStrings_FlagLast_FlagLong_SingleLine_SingleResult_Alpha
        [Test]
        public async Task FixedStrings_FlagLast_FlagLong_SingleLine_SingleResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "brown fox";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorLong}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagLast_FlagLong_SingleLine_SingleResult_Alpha

        #region FixedStrings_FlagLast_FlagLong_SingleLine_SingleResult_Numeric
        [Test]
        public async Task FixedStrings_FlagLast_FlagLong_SingleLine_SingleResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "4";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorLong}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagLast_FlagLong_SingleLine_SingleResult_Numeric

        #region FixedStrings_FlagLast_FlagLong_SingleLine_SingleResult_Alphanumeric
        [Test]
        public async Task FixedStrings_FlagLast_FlagLong_SingleLine_SingleResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "the 4 lazy";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorLong}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagLast_FlagLong_SingleLine_SingleResult_Alphanumeric

        #region FixedStrings_FlagLast_FlagLong_SingleLine_SingleResult_SpecialCharacter
        [Test]
        public async Task FixedStrings_FlagLast_FlagLong_SingleLine_SingleResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"\s";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorLong}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagLast_FlagLong_SingleLine_SingleResult_SpecialCharacter

        #region FixedStrings_FlagLast_FlagLong_SingleLine_MultiResult_Alpha
        [Test]
        public async Task FixedStrings_FlagLast_FlagLong_SingleLine_MultiResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "lazy";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorLong}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 3);
        }
        #endregion FixedStrings_FlagLast_FlagLong_SingleLine_MultiResult_Alpha

        #region FixedStrings_FlagLast_FlagLong_SingleLine_MultiResult_Numeric
        [Test]
        public async Task FixedStrings_FlagLast_FlagLong_SingleLine_MultiResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorLong}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagLast_FlagLong_SingleLine_MultiResult_Numeric

        #region FixedStrings_FlagLast_FlagLong_SingleLine_MultiResult_Alphanumeric
        [Test]
        public async Task FixedStrings_FlagLast_FlagLong_SingleLine_MultiResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10 lazy";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorLong}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagLast_FlagLong_SingleLine_MultiResult_Alphanumeric

        #region FixedStrings_FlagLast_FlagLong_SingleLine_MultiResult_SpecialCharacter
        [Test]
        public async Task FixedStrings_FlagLast_FlagLong_SingleLine_MultiResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsSingleLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = ".*";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorLong}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagLast_FlagLong_SingleLine_MultiResult_SpecialCharacter

        #region FixedStrings_FlagLast_FlagLong_MultiLine_SingleResult_Alpha
        [Test]
        public async Task FixedStrings_FlagLast_FlagLong_MultiLine_SingleResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "brown fox";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorLong}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagLast_FlagLong_MultiLine_SingleResult_Alpha

        #region FixedStrings_FlagLast_FlagLong_MultiLine_SingleResult_Numeric
        [Test]
        public async Task FixedStrings_FlagLast_FlagLong_MultiLine_SingleResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "4";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorLong}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagLast_FlagLong_MultiLine_SingleResult_Numeric

        #region FixedStrings_FlagLast_FlagLong_MultiLine_SingleResult_Alphanumeric
        [Test]
        public async Task FixedStrings_FlagLast_FlagLong_MultiLine_SingleResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "fox hops";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorLong}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagLast_FlagLong_MultiLine_SingleResult_Alphanumeric

        #region FixedStrings_FlagLast_FlagLong_MultiLine_SingleResult_SpecialCharacter
        [Test]
        public async Task FixedStrings_FlagLast_FlagLong_MultiLine_SingleResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = ".?";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorLong}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 1);
        }
        #endregion FixedStrings_FlagLast_FlagLong_MultiLine_SingleResult_SpecialCharacter

        #region FixedStrings_FlagLast_FlagLong_MultiLine_MultiResult_Alpha
        [Test]
        public async Task FixedStrings_FlagLast_FlagLong_MultiLine_MultiResult_Alpha()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "lazy";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorLong}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 6);
        }
        #endregion FixedStrings_FlagLast_FlagLong_MultiLine_MultiResult_Alpha

        #region FixedStrings_FlagLast_FlagLong_MultiLine_MultiResult_Numeric
        [Test]
        public async Task FixedStrings_FlagLast_FlagLong_MultiLine_MultiResult_Numeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorLong}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 5);
        }
        #endregion FixedStrings_FlagLast_FlagLong_MultiLine_MultiResult_Numeric

        #region FixedStrings_FlagLast_FlagLong_MultiLine_MultiResult_Alphanumeric
        [Test]
        public async Task FixedStrings_FlagLast_FlagLong_MultiLine_MultiResult_Alphanumeric()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = "10 lazy";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorLong}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 4);
        }
        #endregion FixedStrings_FlagLast_FlagLong_MultiLine_MultiResult_Alphanumeric 

        #region FixedStrings_FlagLast_FlagLong_MultiLine_MultiResult_SpecialCharacter
        [Test]
        public async Task FixedStrings_FlagLast_FlagLong_MultiLine_MultiResult_SpecialCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "FixedStringsMultiLine.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"\s";
            string Command = $"-f '{TestFilePath}' {SearchTerm} {_flagDescriptorLong}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == SearchTerm) == 2);
        }
        #endregion FixedStrings_FlagLast_FlagLong_MultiLine_MultiResult_SpecialCharacter 
        #endregion FlagLast..
        #endregion Tests..
    }
}
