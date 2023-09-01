using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using WindowsGrep.Common;
using WindowsGrep.Core;
using WindowsGrep.Engine;

namespace WindowsGrep.Test.Flags.Directory
{
    public class DirectoryTests : TestBase
    {
        #region Fields..
        private string _FlagDescriptorShort;
        private string _FlagDescriptorLong;
        private string _TestDataRelativePath = @"Flags\Directory\TestData";
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Setup
        [SetUp]
        public void Setup()
        {
            TestDataDirectory = Path.Combine(TestConfigurationManager.ProjectDirectory, _TestDataRelativePath);

            List<string> DescriptionCollection = ConsoleFlag.Directory.GetCustomAttribute<DescriptionCollectionAttribute>()?.Value.OrderBy(x => x.Length).ToList();
            _FlagDescriptorShort = DescriptionCollection[0];
            _FlagDescriptorLong = DescriptionCollection[1];
        }
        #endregion Setup

        #region Tests..
        #region FlagFirst..
        #region Directory_FlagFirst_FlagShort_SingleQuotes
        [Test]
        public void Directory_FlagFirst_FlagShort_SingleQuotes()
        {
            string SearchTerm = "fox";
            string Command = $"{_FlagDescriptorShort} '{TestDataDirectory}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 2);
        }
        #endregion Directory_FlagFirst_FlagShort_SingleQuotes 

        #region Directory_FlagFirst_FlagShort_DoubleQuotes
        [Test]
        public void Directory_FlagFirst_FlagShort_DoubleQuotes()
        {
            string SearchTerm = "fox";
            string Command = $"{_FlagDescriptorShort} \"{TestDataDirectory}\" {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 2);
        }
        #endregion Directory_FlagFirst_FlagShort_DoubleQuotes 

        #region Directory_FlagFirst_FlagLong_SingleQuotes
        [Test]
        public void Directory_FlagFirst_FlagLong_SingleQuotes()
        {
            string SearchTerm = "fox";
            string Command = $"{_FlagDescriptorLong} '{TestDataDirectory}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 2);
        }
        #endregion Directory_FlagFirst_FlagLong_SingleQuotes

        #region Directory_FlagFirst_FlagLong_DoubleQuotes
        [Test]
        public void Directory_FlagFirst_FlagLong_DoubleQuotes()
        {
            string SearchTerm = "fox";
            string Command = $"{_FlagDescriptorLong} \"{TestDataDirectory}\" {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 2);
        }
        #endregion Directory_FlagFirst_FlagLong_DoubleQuotes
        #endregion FlagFirst..

        #region FlagMiddle..
        #region Directory_FlagMiddle_FlagShort_SingleQuotes
        [Test]
        public void Directory_FlagMiddle_FlagShort_SingleQuotes()
        {
            string SearchTerm = "fox";
            string Command = $"-i {_FlagDescriptorShort} '{TestDataDirectory}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 2);
        }
        #endregion Directory_FlagMiddle_FlagShort_SingleQuotes 

        #region Directory_FlagMiddle_FlagShort_DoubleQuotes
        [Test]
        public void Directory_FlagMiddle_FlagShort_DoubleQuotes()
        {
            string SearchTerm = "fox";
            string Command = $"-i {_FlagDescriptorShort} \"{TestDataDirectory}\" {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 2);
        }
        #endregion Directory_FlagMiddle_FlagShort_DoubleQuotes

        #region Directory_FlagMiddle_FlagLong_SingleQuotes
        [Test]
        public void Directory_FlagMiddle_FlagLong_SingleQuotes()
        {
            string SearchTerm = "fox";
            string Command = $"-i {_FlagDescriptorLong} '{TestDataDirectory}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 2);
        }
        #endregion Directory_FlagMiddle_FlagLong_SingleQuotes 

        #region Directory_FlagMiddle_FlagLong_DoubleQuotes
        [Test]
        public void Directory_FlagMiddle_FlagLong_DoubleQuotes()
        {
            string SearchTerm = "fox";
            string Command = $"-i {_FlagDescriptorLong} \"{TestDataDirectory}\" {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 2);
        }
        #endregion Directory_FlagMiddle_FlagLong_DoubleQuotes
        #endregion FlagMiddle..

        #region FlagLast..
        #region Directory_FlagLast_FlagShort_SingleQuotes
        [Test]
        public void Directory_FlagLast_FlagShort_SingleQuotes()
        {
            string SearchTerm = "fox";
            string Command = $"{SearchTerm} {_FlagDescriptorShort} '{TestDataDirectory}'";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 2);
        }
        #endregion Directory_FlagLast_FlagShort_SingleQuotes 

        #region Directory_FlagLast_FlagShort_DoubleQuotes
        [Test]
        public void Directory_FlagLast_FlagShort_DoubleQuotes()
        {
            string SearchTerm = "fox";
            string Command = $"{SearchTerm} {_FlagDescriptorShort} \"{TestDataDirectory}\"";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 2);
        }
        #endregion Directory_FlagLast_FlagShort_DoubleQuotes

        #region Directory_FlagLast_FlagLong_SingleQuotes
        [Test]
        public void Directory_FlagLast_FlagLong_SingleQuotes()
        {
            string SearchTerm = "fox";
            string Command = $"{SearchTerm} {_FlagDescriptorLong} '{TestDataDirectory}'";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 2);
        }
        #endregion Directory_FlagLast_FlagLong_SingleQuotes 

        #region Directory_FlagLast_FlagLong_DoubleQuotes
        [Test]
        public void Directory_FlagLast_FlagLong_DoubleQuotes()
        {
            string SearchTerm = "fox";
            string Command = $"{SearchTerm} {_FlagDescriptorLong} \"{TestDataDirectory}\"";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 2);
        }
        #endregion Directory_FlagLast_FlagLong_DoubleQuotes
        #endregion FlagLast..
        #endregion Tests..
    }
}
