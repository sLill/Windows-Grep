namespace WindowsGrep.Test;

public class CommandParser_Path_Tests : TestBase
{
    [Theory]
    [InlineData("search_term")]
    [InlineData("-ri search_term {0}")]
    [InlineData("search_term {0}")]
    [InlineData("search_term .")]
    [InlineData("-ri search_term \"{0}\"")]
    [InlineData("search_term \"{0}\"")]
    [InlineData("-ri search_term '{0}'")]
    [InlineData("search_term '{0}'")]
    public void Path_Valid(string command)
    {
        command = string.Format(command, TestDataDirectory);
        IDictionary<CommandFlag, string> commandArgs = WindowsGrepUtils.ParseGrepCommandArgs(command);
        Assert.True(commandArgs.ContainsKey(CommandFlag.Path));
    }

    [Theory]
    [InlineData("search_term U:/Path/Does/Not/Exist")]
    public void Path_Invalid(string command)
    {
        Assert.Throws<Exception>(() => WindowsGrepUtils.ParseGrepCommandArgs(command));
    }
}
