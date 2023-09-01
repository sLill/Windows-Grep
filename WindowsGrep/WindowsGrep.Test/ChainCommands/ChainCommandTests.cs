using NUnit.Framework;
using System.IO;
using System.Threading;
using WindowsGrep.Engine;

namespace WindowsGrep.Test.ChainCommands
{
    public class ChainCommandTests : TestBase
    {
        #region Fields..
        private string _TestDataRelativePath = @"ChainCommands\TestData";
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Setup
        [SetUp]
        public void Setup()
        {
            TestDataDirectory = Path.Combine(TestConfigurationManager.ProjectDirectory, _TestDataRelativePath);
        }
        #endregion Setup

        #region Tests..
        #region ChainCommands_One
        [Test]
        public void ChainCommands_One()
        {
            string SearchTerm = "fox jumps over";
            string Command = $"-d '{TestDataDirectory}' -k ChainCommands | -r -i {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 3);
        }
        #endregion ChainCommands_One 

        #region ChainCommands_Two
        [Test]
        public void ChainCommands_Two()
        {
            string SearchTerm = "fox jumps over";
            string Command = $"-d '{TestDataDirectory}' -k ChainCommands | -r -i {SearchTerm} | -k Two";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion ChainCommands_Two 
        #endregion Tests..
    }
}
