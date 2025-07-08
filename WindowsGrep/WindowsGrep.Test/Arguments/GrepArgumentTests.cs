namespace WindowsGrep.Test;

public class GrepArgumentTests : TestBase
{
    [Fact]
    public void GrepArguments_Basic()
    {
        string testText = "This is sample text";
        string command = $"-rk -i --max-depth=\"3\" \"{testText}\" \"{TestDataDirectory}\"";

        IDictionary<ConsoleFlag, string> commandArgs = WindowsGrep.ParseGrepCommandArgs(command);
        
        Assert.True(commandArgs.ContainsKey(ConsoleFlag.Recursive));
        Assert.True(commandArgs.ContainsKey(ConsoleFlag.IgnoreCase));
        Assert.True(commandArgs.ContainsKey(ConsoleFlag.SearchTerm));
        Assert.True(commandArgs.ContainsKey(ConsoleFlag.Directory));

        Assert.True(commandArgs[ConsoleFlag.SearchTerm] == testText);
        Assert.True(commandArgs[ConsoleFlag.Directory] == TestDataDirectory);
    }
}
