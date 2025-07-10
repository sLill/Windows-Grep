namespace WindowsGrep.Test;

public class CommandParser_FlagTests : TestBase
{
    [Theory]
    [InlineData("-r -z -i search_term .", new[] { CommandFlag.Recursive, CommandFlag.IgnoreCase })]
    [InlineData("-r -k -i search_term", new[] { CommandFlag.Recursive, CommandFlag.IgnoreCase, CommandFlag.FileNamesOnly })]
    [InlineData("-rk -i search_term .", new[] { CommandFlag.Recursive, CommandFlag.IgnoreCase, CommandFlag.FileNamesOnly })]
    [InlineData("-rk -i search_term", new[] { CommandFlag.Recursive, CommandFlag.IgnoreCase, CommandFlag.FileNamesOnly })]
    [InlineData("-r --ignore-breaks -k -i search_term", new[] { CommandFlag.Recursive, CommandFlag.IgnoreCase, CommandFlag.FileNamesOnly })]
    [InlineData("-r --ignore-breaks -k -i search_term .", new[] { CommandFlag.Recursive, CommandFlag.IgnoreCase, CommandFlag.FileNamesOnly })]
    public void ShortDescriptors_Valid(string command, CommandFlag[] expectedArgs)
    {
        IDictionary<CommandFlag, string> commandArgs = WindowsGrepUtils.ParseGrepCommandArgs(command);
        expectedArgs.ToList().ForEach(x => Assert.True(commandArgs.ContainsKey(x)));
    }

    [Theory]
    [InlineData("-r search_term -k")]
    [InlineData("-r search_term -k .")]
    public void ShortDescriptors_Invalid(string command)
    {
        Assert.Throws<Exception>(() => WindowsGrepUtils.ParseGrepCommandArgs(command));
    }
}
