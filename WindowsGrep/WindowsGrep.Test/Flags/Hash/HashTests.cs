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
            TestDataDirectory = Path.Combine(TestConfigurationManager.WorkingDirectory, _testDataRelativePath);

            List<string> DescriptionCollection = ConsoleFlag.FileHashes.GetCustomAttribute<DescriptionCollectionAttribute>()?.Value.OrderBy(x => x.Length).ToList();
            _flagDescriptor = DescriptionCollection[0];
        }
        #endregion Setup

        #region Tests..
        #region Hash_MD5_SingleResult
        [Test]
        public async Task Hash_MD5_SingleResult()
        {
            string testFilePath = Path.Combine(TestDataDirectory, "Normal.txt");
            Assert.IsTrue(File.Exists(testFilePath));

            string hash = "25a1568def38f50387dd63338d066f9e";
            string command = $"{_flagDescriptor}1 -f '{testFilePath}' {hash}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion Hash_MD5_SingleResult       

        #region Hash_SHA256_SingleResult
        [Test]
        public async Task Hash_SHA256_SingleResult()
        {
            string testFilePath = Path.Combine(TestDataDirectory, "Normal.txt");
            Assert.IsTrue(File.Exists(testFilePath));

            string hash = "aa50d2761939f9211b3d2966fa4397f75f7878b6f46a0a392076b4ae357e803d";
            string command = $"{_flagDescriptor}0 -f '{testFilePath}' {hash}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion Hash_SHA256_SingleResult

        #region Hash_FlagFirst
        [Test]
        public async Task Hash_FlagFirst()
        {
            string testFilePath = Path.Combine(TestDataDirectory, "Normal.txt");
            Assert.IsTrue(File.Exists(testFilePath));

            string hash = "aa50d2761939f9211b3d2966fa4397f75f7878b6f46a0a392076b4ae357e803d";
            string command = $"{_flagDescriptor}0 -f '{testFilePath}' {hash}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Any());
        }
        #endregion Hash_FlagFirst

        #region Hash_FlagMiddle
        [Test]
        public async Task Hash_FlagMiddle()
        {
            string testFilePath = Path.Combine(TestDataDirectory, "Normal.txt");
            Assert.IsTrue(File.Exists(testFilePath));

            string hash = "aa50d2761939f9211b3d2966fa4397f75f7878b6f46a0a392076b4ae357e803d";
            string command = $"-f '{testFilePath}' {_flagDescriptor}0 {hash}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Any());
        }
        #endregion Hash_FlagMiddle

        #region Hash_FlagLast
        [Test]
        public async Task Hash_FlagLast()
        {
            string testFilePath = Path.Combine(TestDataDirectory, "Normal.txt");
            Assert.IsTrue(File.Exists(testFilePath));

            string hash = "aa50d2761939f9211b3d2966fa4397f75f7878b6f46a0a392076b4ae357e803d";
            string command = $"-f '{testFilePath}' {hash} {_flagDescriptor}0";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommand(command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Any());
        }
        #endregion Hash_FlagLast
        #endregion Tests..
    }
}
