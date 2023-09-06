using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WindowsGrep.Common;
using WindowsGrep.Core;
using WindowsGrep.Engine;

namespace WindowsGrep.Test.NativeCommands.List
{
    public class ListTests : TestBase
    {
        #region Fields..
        private string _flagDescriptor;
        private string _testDataRelativePath = @"NativeCommands\List\TestData";
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Setup
        [SetUp]
        public void Setup()
        {
            TestDataDirectory = Path.Combine(TestConfigurationManager.WorkingDirectory, _testDataRelativePath);

            List<string> DescriptionCollection = NativeCommandType.List.GetCustomAttribute<DescriptionCollectionAttribute>()?.Value.OrderBy(x => x.Length).ToList();
            _flagDescriptor = DescriptionCollection[0];
        }
        #endregion Setup

        #region Tests..
        #region List_Empty
        [Test]
        public async Task List_Empty()
        {
            string emptyDirectory = Path.Combine(TestDataDirectory, "Empty");
            Directory.CreateDirectory(emptyDirectory);

            string command = $"cd {emptyDirectory} | {_flagDescriptor}";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(!commandResultCollection.Any());
        }
        #endregion List_Empty

        #region List_NotEmpty
        [Test]
        public async Task List_NotEmpty()
        {
            string command = $"cd {TestDataDirectory} | {_flagDescriptor}";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Any());
        }
        #endregion List_NotEmpty
        #endregion Tests..
    }
}
