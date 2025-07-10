namespace WindowsGrep.Test;

public class CommandParser_PathTests : TestBase
{
    [Theory]
    [InlineData("-ri search_term {0}")]
    [InlineData("search_term {0}")]
    [InlineData("search_term .")]
    public void Path_Valid(string command)
    {
        command = string.Format(command, Environment.CurrentDirectory);
        IDictionary<CommandFlag, string> commandArgs = WindowsGrepUtils.ParseGrepCommandArgs(command);
        commandArgs.ContainsKey(CommandFlag.Path);
    }

    [Theory]
    [InlineData("search_term U:/Path/Does/Not/Exist")]
    public void Path_Invalid(string command)
    {
        Assert.Throws<Exception>(() => WindowsGrepUtils.ParseGrepCommandArgs(command));
    }
}
