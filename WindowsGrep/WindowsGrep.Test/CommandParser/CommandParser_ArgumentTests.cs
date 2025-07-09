namespace WindowsGrep.Test;

public class CommandParser_ArgumentTests : TestBase
{
    [Theory]
    [InlineData("-r -z -i search_term", new[] { ConsoleFlag.Recursive, ConsoleFlag.IgnoreCase })]
    [InlineData("-r -k -i search_term", new[] { ConsoleFlag.Recursive, ConsoleFlag.IgnoreCase, ConsoleFlag.FileNamesOnly })]
    [InlineData("-rk -i search_term", new[] { ConsoleFlag.Recursive, ConsoleFlag.IgnoreCase, ConsoleFlag.FileNamesOnly })]
    [InlineData("-r --ignore-breaks -k -i search_term", new[] { ConsoleFlag.Recursive, ConsoleFlag.IgnoreCase, ConsoleFlag.FileNamesOnly })]
    public void ShortDescriptors_Valid(string command, ConsoleFlag[] expectedArgs)
    {
        IDictionary<ConsoleFlag, string> commandArgs = WindowsGrepUtils.ParseGrepCommandArgs(command);
        expectedArgs.ToList().ForEach(x => Assert.True(commandArgs.ContainsKey(x)));
    }

    [Theory]
    [InlineData("-r search_term -k")]
    public void ShortDescriptors_Invalid(string command)
    {
        Assert.Throws<Exception>(() => WindowsGrepUtils.ParseGrepCommandArgs(command));
    }

    [Theory]
    [InlineData("search_term {0}")]
    public void Path_Valid(string command)
    {
        command = string.Format(command, Environment.CurrentDirectory);
        IDictionary<ConsoleFlag, string> commandArgs = WindowsGrepUtils.ParseGrepCommandArgs(command);

        Assert.True(commandArgs.ContainsKey(ConsoleFlag.Path));
    }

    [Theory]
    [InlineData("search_term U:/Path/Does/Not/Exist")]
    public void Path_Invalid(string command)
    {
        Assert.Throws<Exception>(() => WindowsGrepUtils.ParseGrepCommandArgs(command));
    }
}
