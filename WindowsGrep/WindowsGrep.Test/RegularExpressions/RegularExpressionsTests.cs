using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Threading;
using WindowsGrep.Engine;

namespace WindowsGrep.Test.RegularExpressions
{
    public class RegularExpressionsTests : TestBase
    {
        #region Fields..
        private string _TestDataRelativePath = @"RegularExpressions\TestData";
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
        #region Characters..
        #region RegularExpressions_Characters_Digit
        [Test]
        public void RegularExpressions_Characters_Digit()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "RegularExpressionsOne.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"\d\d\d\d";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion RegularExpressions_Characters_Digit

        #region RegularExpressions_Characters_WordCharacter
        [Test]
        public void RegularExpressions_Characters_WordCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "RegularExpressionsOne.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"not a story the Jed\w would tell you";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion RegularExpressions_Characters_WordCharacter 

        #region RegularExpressions_Characters_SpaceCharacter
        [Test]
        public void RegularExpressions_Characters_SpaceCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "RegularExpressionsOne.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"not\sa\sstory\sthe\sJedi\swould\stell\syou";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion RegularExpressions_Characters_SpaceCharacter 

        #region RegularExpressions_Characters_NonDigit
        [Test]
        public void RegularExpressions_Characters_NonDigit()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "RegularExpressionsOne.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"Palpat\Dne 2005";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion RegularExpressions_Characters_NonDigit 

        #region RegularExpressions_Characters_NonWordCharacter
        [Test]
        public void RegularExpressions_Characters_NonWordCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "RegularExpressionsOne.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"became so powerful.\W\W the only";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion RegularExpressions_Characters_NonWordCharacter 

        #region RegularExpressions_Characters_NonSpaceCharacter
        [Test]
        public void RegularExpressions_Characters_NonSpaceCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "RegularExpressionsOne.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"so po\Ser\Sul and so wi\Se";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion RegularExpressions_Characters_NonSpaceCharacter 

        #region RegularExpressions_Characters_AnyCharacter
        [Test]
        public void RegularExpressions_Characters_AnyCharacter()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "RegularExpressionsOne.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"power, which eventually. of co.rse, h..did";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion RegularExpressions_Characters_AnyCharacter 

        #region RegularExpressions_Characters_Tab
        [Test]
        public void RegularExpressions_Characters_Tab()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "RegularExpressionsOne.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"\t'Did you ever";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion RegularExpressions_Characters_Tab 

        #region RegularExpressions_Characters_NewLine
        [Test]
        public void RegularExpressions_Characters_NewLine()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "RegularExpressionsOne.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"story the Jedi would tell you\.[\r\n\s]*It's a Sith";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion RegularExpressions_Characters_NewLine 
        #endregion Characters..

        #region Character Classes..
        #region RegularExpressions_CharacterClasses_Characters
        [Test]
        public void RegularExpressions_CharacterClasses_Characters()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "RegularExpressionsOne.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"so powerful[\s\w\W]*wise";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion RegularExpressions_CharacterClasses_Characters

        #region RegularExpressions_CharacterClasses_CharacterRange
        [Test]
        public void RegularExpressions_CharacterClasses_CharacterRange()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "RegularExpressionsOne.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            // 1
            string SearchTerm = @"the only [a-z\s]* afraid";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());
            Assert.IsTrue(commandResultCollection.Count == 1);

            // 2
            SearchTerm = @"so powerful[a-z\s]* he was afraid";
            Command = $"-f '{TestFilePath}' {SearchTerm}";

            commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());
            Assert.IsTrue(commandResultCollection.Count == 0);
        }
        #endregion RegularExpressions_CharacterClasses_CharacterRange

        #region RegularExpressions_CharacterClasses_Characters_Negative
        [Test]
        public void RegularExpressions_CharacterClasses_Characters_Negative()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "RegularExpressionsOne.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"so powerful[^\d]*wise";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion RegularExpressions_CharacterClasses_Characters_Negative

        #region RegularExpressions_CharacterClasses_CharacterRange_Negative
        [Test]
        public void RegularExpressions_CharacterClasses_CharacterRange_Negative()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "RegularExpressionsOne.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"Palpatine [^a-z]*";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion RegularExpressions_CharacterClasses_CharacterRange_Negative
        #endregion Character Classes..

        #region Quantifiers..
        #region RegularExpressions_Quantifiers_OneOrMore
        [Test]
        public void RegularExpressions_Quantifiers_OneOrMore()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "RegularExpressionsOne.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"He became so powerful[\.]+ the only thing";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion RegularExpressions_Quantifiers_OneOrMore 

        #region RegularExpressions_Quantifiers_OneOrMoreLazy
        [Test]
        public void RegularExpressions_Quantifiers_OneOrMoreLazy()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "RegularExpressionsOne.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            // 1
            string SearchTerm = @"He became so powerful[\.]+? the only thing";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());
            Assert.IsTrue(commandResultCollection.Count == 1);

            // 2
            SearchTerm = @"He became so powerful[\.]+?";
            Command = $"-f '{TestFilePath}' {SearchTerm}";

            commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());
            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == @"He became so powerful.") == 1);
        }
        #endregion RegularExpressions_Quantifiers_OneOrMoreLazy 

        #region RegularExpressions_Quantifiers_nTimes
        [Test]
        public void RegularExpressions_Quantifiers_nTimes()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "RegularExpressionsOne.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"He became so powerful[\.]{3} the only thing";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion RegularExpressions_Quantifiers_nTimes 

        #region RegularExpressions_Quantifiers_Range
        [Test]
        public void RegularExpressions_Quantifiers_Range()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "RegularExpressionsOne.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"He became so powerful[\.]{1,3} the only thing";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion RegularExpressions_Quantifiers_Range 

        #region RegularExpressions_Quantifiers_nOrMore
        [Test]
        public void RegularExpressions_Quantifiers_nOrMore()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "RegularExpressionsOne.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"He became so powerful[\.]{2,} the only thing";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion RegularExpressions_Quantifiers_nOrMore 

        #region RegularExpressions_Quantifiers_ZeroOrMore
        [Test]
        public void RegularExpressions_Quantifiers_ZeroOrMore()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "RegularExpressionsOne.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"He became so powerful\.\.\.[Z]* the only thing";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion RegularExpressions_Quantifiers_ZeroOrMore 

        #region RegularExpressions_Quantifiers_ZeroOrMore Lazy
        [Test]
        public void RegularExpressions_Quantifiers_ZeroOrMoreLazy()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "RegularExpressionsOne.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            // 1
            string SearchTerm = @"He became so powerful[\.]*? the only thing";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());
            Assert.IsTrue(commandResultCollection.Count == 1);

            // 2
            SearchTerm = @"He became so powerful[\.]*?";
            Command = $"-f '{TestFilePath}' {SearchTerm}";

            commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());
            Assert.IsTrue(commandResultCollection.Count(x => ((GrepCommandResult)x).MatchedString == @"He became so powerful") == 1);
        }
        #endregion RegularExpressions_Quantifiers_ZeroOrMoreLazy

        #region RegularExpressions_Quantifiers_OnceOrNone
        [Test]
        public void RegularExpressions_Quantifiers_OnceOrNone()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "RegularExpressionsOne.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            // Postive test
            string SearchTerm = @"He became so power[f]?ul\.\.\. the only thing";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());
            Assert.IsTrue(commandResultCollection.Count == 1);

            // Negative test
            SearchTerm = @"He became so powerful[\.]? the only thing";
            Command = $"-f '{TestFilePath}' {SearchTerm}";

            commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());
            Assert.IsTrue(commandResultCollection.Count == 0);
        }
        #endregion RegularExpressions_Quantifiers_OnceOrNone
        #endregion Quantifiers..

        #region Logic..
        #region RegularExpressions_Logic_OR
        [Test]
        public void RegularExpressions_Logic_OR()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "RegularExpressionsOne.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"I(t's| thought) not";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 2);
        }
        #endregion RegularExpressions_Logic_OR

        #region RegularExpressions_Logic_NonCapturingGroup
        [Test]
        public void RegularExpressions_Logic_NonCapturingGroup()
        {
            Assert.True(true);
        }
        #endregion RegularExpressions_Logic_NonCapturingGroup

        #region RegularExpressions_Logic_BackReferenceByIndex
        [Test]
        public void RegularExpressions_Logic_BackReferenceByIndex()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "RegularExpressionsOne.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"I thought not([\.]).*\1";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion RegularExpressions_Logic_BackReferenceByIndex

        #region RegularExpressions_Logic_BackReferenceByName
        [Test]
        public void RegularExpressions_Logic_BackReferenceByName()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "RegularExpressionsOne.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"I thought not(?<Cap>\.).*\k<Cap>";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion RegularExpressions_Logic_BackReferenceByName
        #endregion Logic..

        #region Boundaries..
        #region RegularExpressions_Boundaries_StartOfString
        [Test]
        public void RegularExpressions_Boundaries_StartOfString()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "RegularExpressionsOne.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"^.*Tragedy of";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion RegularExpressions_Boundaries_StartOfString

        #region RegularExpressions_Boundaries_Word
        [Test]
        public void RegularExpressions_Boundaries_Word()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "RegularExpressionsOne.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"\blosing\b";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion RegularExpressions_Boundaries_Word

        #region RegularExpressions_Boundaries_Word_Negative
        [Test]
        public void RegularExpressions_Boundaries_Word_Negative()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "RegularExpressionsOne.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"power\B";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 2);
        }
        #endregion RegularExpressions_Boundaries_Word_Negative
        #endregion Boundaries..

        #region LookArounds..
        #region RegularExpressions_LookArounds_PositiveLookAhead_AfterMatch
        [Test]
        public void RegularExpressions_LookArounds_PositiveLookAhead_AfterMatch()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "RegularExpressionsTwo.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"\d+(?= dollars)";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion RegularExpressions_LookArounds_PositiveLookAhead_AfterMatch

        #region RegularExpressions_LookArounds_PositiveLookAhead_BeforeMatch
        [Test]
        public void RegularExpressions_LookArounds_PositiveLookAhead_BeforeMatch()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "RegularExpressionsTwo.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"(?=\d+ dollars)\d+";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion RegularExpressions_LookArounds_PositiveLookAhead_BeforeMatch

        #region RegularExpressions_LookArounds_PositiveLookBehind_AfterMatch
        [Test]
        public void RegularExpressions_LookArounds_PositiveLookBehind_AfterMatch()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "RegularExpressionsTwo.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"\d{3}(?<=USD\d{3})";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion RegularExpressions_LookArounds_PositiveLookBehind_AfterMatch

        #region RegularExpressions_LookArounds_PositiveLookBehind_BeforeMatch
        [Test]
        public void RegularExpressions_LookArounds_PositiveLookBehind_BeforeMatch()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "RegularExpressionsTwo.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"(?<=USD)\d{3}";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Count == 1);
        }
        #endregion RegularExpressions_LookArounds_PositiveLookBehind_BeforeMatch

        #region RegularExpressions_LookArounds_NegativeLookAhead_AfterMatch
        [Test]
        public void RegularExpressions_LookArounds_NegativeLookAhead_AfterMatch()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "RegularExpressionsTwo.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"\d+(?!\d| dollars)";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Any());
        }
        #endregion RegularExpressions_LookArounds_NegativeLookAhead_AfterMatch

        #region RegularExpressions_LookArounds_NegativeLookAhead_BeforeMatch
        [Test]
        public void RegularExpressions_LookArounds_NegativeLookAhead_BeforeMatch()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "RegularExpressionsTwo.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"(?!\d+ dollars)\d+";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Any());
        }
        #endregion RegularExpressions_LookArounds_NegativeLookAhead_BeforeMatch

        #region RegularExpressions_LookArounds_NegativeLookBehind_AfterMatch
        [Test]
        public void RegularExpressions_LookArounds_NegativeLookBehind_AfterMatch()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "RegularExpressionsTwo.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"\d{3}(?<!USD\d{3})";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Any());
        }
        #endregion RegularExpressions_LookArounds_NegativeLookBehind_AfterMatch

        #region RegularExpressions_LookArounds_NegativeLookBehind_BeforeMatch
        [Test]
        public void RegularExpressions_LookArounds_NegativeLookBehind_BeforeMatch()
        {
            string TestFilePath = Path.Combine(TestDataDirectory, "RegularExpressionsTwo.txt");
            Assert.IsTrue(File.Exists(TestFilePath));

            string SearchTerm = @"(?<!USD)\d{3}";
            string Command = $"-f '{TestFilePath}' {SearchTerm}";

            var commandResultCollection = new CommandResultCollection();
            WindowsGrep.RunCommandAsync(Command, commandResultCollection, new CancellationToken());

            Assert.IsTrue(commandResultCollection.Any());
        }
        #endregion RegularExpressions_LookArounds_NegativeLookBehind_BeforeMatch
        #endregion LookArounds..
        #endregion Tests..
    }
}
