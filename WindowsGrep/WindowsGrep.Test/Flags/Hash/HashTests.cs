using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WindowsGrep.Common;
using WindowsGrep.Core;
using WindowsGrep.Engine;

namespace WindowsGrep.Test.Flags.Hash
{
    public class HashTests : TestBase
    {
        #region Fields..
        private string _flagDescriptor;
        private string _testDataRelativePath = @"Flags\Hash\TestData";
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Setup
        [SetUp]
        public void Setup()
        {
            TestDataDirectory = Path.Combine(TestConfigurationManager.ProjectDirectory, _testDataRelativePath);

            List<string> DescriptionCollection = ConsoleFlag.FileHashes.GetCustomAttribute<DescriptionCollectionAttribute>()?.Value.OrderBy(x => x.Length).ToList();
            _flagDescriptor = DescriptionCollection[0];
        }
        #endregion Setup

        #region Tests..
        #region Hash_FlagFirst_MD5_SingleResult
        [Test]
        public async Task Hash_FlagFirst_MD5_SingleResult()
        {
            string testFilePath = Path.Combine(TestDataDirectory, "MD5.txt");
            Assert.IsTrue(File.Exists(testFilePath));

            string hash = "25a1568def38f50387dd63338d066f9e";
            string command = $"{_flagDescriptor}1 -f '{testFilePath}' {hash}";

            var commandResultCollection = new CommandResultCollection();
            await WindowsGrep.RunCommandAsync(command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion Hash_FlagFirst_MD5_SingleResult
        #endregion Tests..
    }
}
