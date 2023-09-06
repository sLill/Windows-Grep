using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WindowsGrep.Common;
using WindowsGrep.Core;
using WindowsGrep.Engine;

namespace WindowsGrep.Test.Flags.Recursive
{
    public class ContextTests : TestBase
    {
        #region Fields..
        private string _flagDescriptorShort;
        private string _flagDescriptorLong;
        private string _testDataRelativePath = @"Flags\Recursive\TestData";
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Setup
        [SetUp]
        public void Setup()
        {
            TestDataDirectory = Path.Combine(TestConfigurationManager.WorkingDirectory, _testDataRelativePath);

            List<string> DescriptionCollection = ConsoleFlag.Recursive.GetCustomAttribute<DescriptionCollectionAttribute>()?.Value.OrderBy(x => x.Length).ToList();
            _flagDescriptorShort = DescriptionCollection[0];
            _flagDescriptorLong = DescriptionCollection[1];
        }
        #endregion Setup

        #region Tests..
        #region FlagFirst..
        #region Recursive_FlagFirst_FlagShort
        [Test]
        public async Task Recursive_FlagFirst_FlagShort()
        {
            string SearchTerm = "quick brown fox";
            string Command = $"{_flagDescriptorShort} -d '{TestDataDirectory}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 2);
        }
        #endregion Recursive_FlagFirst_FlagShort 

        #region Recursive_FlagFirst_FlagLong
        [Test]
        public async Task IgnoreCase_FlagFirst_FlagLong()
        {
            string SearchTerm = "quick brown fox";
            string Command = $"{_flagDescriptorLong} -d '{TestDataDirectory}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 2);
        }
        #endregion Recursive_FlagFirst_FlagLong 
        #endregion FlagFirst..

        #region FlagMiddle..
        #region Recursive_FlagMiddle_FlagShort
        [Test]
        public async Task Recursive_FlagMiddle_FlagShort()
        {
            string SearchTerm = "quick brown fox";
            string Command = $"-d '{TestDataDirectory}' {_flagDescriptorShort} {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 2);
        }
        #endregion Recursive_FlagMiddle_FlagShort 

        #region Recursive_FlagMiddle_FlagLong
        [Test]
        public async Task IgnoreCase_FlagMiddle_FlagLong()
        {
            string SearchTerm = "quick brown fox";
            string Command = $"-d '{TestDataDirectory}' {_flagDescriptorLong} {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 2);
        }
        #endregion Recursive_FlagMiddle_FlagLong 
        #endregion FlagMiddle..

        #region FlagLast..
        #region Recursive_FlagLast_FlagShort
        [Test]
        public async Task Recursive_FlagLast_FlagShort()
        {
            string SearchTerm = "quick brown fox";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_flagDescriptorShort}";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 2);
        }
        #endregion Recursive_FlagLast_FlagShort 

        #region Recursive_FlagLast_FlagLong
        [Test]
        public async Task IgnoreCase_FlagLast_FlagLong()
        {
            string SearchTerm = "quick brown fox";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm} {_flagDescriptorLong}";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 2);
        }
        #endregion Recursive_FlagLast_FlagLong 
        #endregion FlagLast..

        #region Recursive_NoFlag
        [Test]
        public async Task Recursive_NoFlag()
        {
            string SearchTerm = "quick brown fox";
            string Command = $"-d '{TestDataDirectory}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString.EqualsIgnoreCase(SearchTerm)) == 1);
        }
        #endregion Recursive_NoFlag 
        #endregion Tests..
    }
}
