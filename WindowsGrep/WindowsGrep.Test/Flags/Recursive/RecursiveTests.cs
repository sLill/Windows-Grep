using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using WindowsGrep.Common;
using WindowsGrep.Core;
using WindowsGrep.Engine;

namespace WindowsGrep.Test.Flags.Recursive
{
    public class ContextTests : TestBase
    {
        #region Fields..
        private string _FlagDescriptorShort;
        private string _FlagDescriptorLong;
        private string _TestDataRelativePath = @"Flags\Recursive\TestData";
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Setup
        [SetUp]
        public void Setup()
        {
            TestDataDirectory = Path.Combine(TestConfigurationManager.ProjectDirectory, _TestDataRelativePath);

            List<string> DescriptionCollection = ConsoleFlag.Recursive.GetCustomAttribute<DescriptionCollectionAttribute>()?.Value.OrderBy(x => x.Length).ToList();
            _FlagDescriptorShort = DescriptionCollection[0];
            _FlagDescriptorLong = DescriptionCollection[1];
        }
        #endregion Setup

        #region Tests..
        #region FlagFirst..
        #region Recursive_FlagFirst_FlagShort
        [Test]
        public void Recursive_FlagFirst_FlagShort()
        {
            string SearchTerm = "quick brown fox";
            string Command = $"{_FlagDescriptorShort} -d '{TestDataDirectory}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 2);
        }
        #endregion Recursive_FlagFirst_FlagShort 

        #region Recursive_FlagFirst_FlagLong
        [Test]
        public void IgnoreCase_FlagFirst_FlagLong()
        {
            string SearchTerm = "quick brown fox";
            string Command = $"{_FlagDescriptorLong} -d '{TestDataDirectory}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 2);
        }
        #endregion Recursive_FlagFirst_FlagLong 
        #endregion FlagFirst..

        #region FlagMiddle..
        #region Recursive_FlagMiddle_FlagShort
        [Test]
        public void Recursive_FlagMiddle_FlagShort()
        {
            string SearchTerm = "quick brown fox";
            string Command = $"-d '{TestDataDirectory}' {_FlagDescriptorShort} {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 2);
        }
        #endregion Recursive_FlagMiddle_FlagShort 

        #region Recursive_FlagMiddle_FlagLong
        [Test]
        public void IgnoreCase_FlagMiddle_FlagLong()
        {
            string SearchTerm = "quick brown fox";
            string Command = $"-d '{TestDataDirectory}' {_FlagDescriptorLong} {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 2);
        }
        #endregion Recursive_FlagMiddle_FlagLong 
        #endregion FlagMiddle..

        #region FlagLast..
        #region Recursive_FlagLast_FlagShort
        [Test]
        public void Recursive_FlagLast_FlagShort()
        {
            string SearchTerm = "quick brown fox";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_FlagDescriptorShort}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 2);
        }
        #endregion Recursive_FlagLast_FlagShort 

        #region Recursive_FlagLast_FlagLong
        [Test]
        public void IgnoreCase_FlagLast_FlagLong()
        {
            string SearchTerm = "quick brown fox";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_FlagDescriptorLong}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 2);
        }
        #endregion Recursive_FlagLast_FlagLong 
        #endregion FlagLast..

        #region Recursive_NoFlag
        [Test]
        public void Recursive_NoFlag()
        {
            string SearchTerm = "quick brown fox";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion Recursive_NoFlag 
        #endregion Tests..
    }
}
