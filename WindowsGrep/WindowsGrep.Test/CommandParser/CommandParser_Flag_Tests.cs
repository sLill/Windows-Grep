namespace WindowsGrep.Test;

public class CommandParser_Flag_Tests : TestBase
{
    [Theory]
    [InlineData("-r -i search_term .", new[] { CommandFlag.Recursive, CommandFlag.IgnoreCase })]
    [InlineData("-r -k -i search_term", new[] { CommandFlag.Recursive, CommandFlag.IgnoreCase, CommandFlag.FileNamesOnly })]
    [InlineData("-rk -i search_term .", new[] { CommandFlag.Recursive, CommandFlag.IgnoreCase, CommandFlag.FileNamesOnly })]
    [InlineData("-rk -i search_term", new[] { CommandFlag.Recursive, CommandFlag.IgnoreCase, CommandFlag.FileNamesOnly })]
    [InlineData("-r --ignore-breaks -k -i search_term", new[] { CommandFlag.Recursive, CommandFlag.IgnoreCase, CommandFlag.FileNamesOnly, CommandFlag.IgnoreBreaks })]
    [InlineData("-r --ignore-breaks -k -i search_term .", new[] { CommandFlag.Recursive, CommandFlag.IgnoreCase, CommandFlag.FileNamesOnly, CommandFlag.IgnoreBreaks })]
    public void ShortDescriptors_Basic_Valid(string command, CommandFlag[] expectedArgs)
    {
        try
        {
            IDictionary<CommandFlag, string> commandArgs = WindowsGrepUtils.ParseGrepCommandArgs(command);
            expectedArgs.ToList().ForEach(x => Assert.True(commandArgs.ContainsKey(x)));
        }
        catch (Exception ex)
        {
            Assert.Fail($"Unexpected exception: {ex.Message}");
        }
    }

    [Theory]
    [InlineData("-r -t '.cpp;.txt' search_term .", new[] { CommandFlag.Recursive, CommandFlag.FileTypeIncludeFilter })]
    [InlineData("-r -t .cpp;.txt -i search_term .", new[] { CommandFlag.Recursive, CommandFlag.IgnoreCase, CommandFlag.FileTypeIncludeFilter })]
    [InlineData("-r -t .cpp,.txt search_term .", new[] { CommandFlag.Recursive, CommandFlag.FileTypeIncludeFilter })]
    [InlineData("-r -i -c 20 search_term .", new[] { CommandFlag.Recursive, CommandFlag.IgnoreCase, CommandFlag.Context })]
    [InlineData("-r -c 20 -i search_term", new[] { CommandFlag.Recursive, CommandFlag.IgnoreCase, CommandFlag.Context })]
    [InlineData("-c 20 -rk -i search_term .", new[] { CommandFlag.Context, CommandFlag.Recursive, CommandFlag.IgnoreCase, CommandFlag.FileNamesOnly })]
    [InlineData("-c 20 --ignore-breaks -k -i search_term", new[] { CommandFlag.Context, CommandFlag.IgnoreBreaks, CommandFlag.IgnoreCase, CommandFlag.FileNamesOnly })]
    public void ShortDescriptors_Parameters_Valid(string command, CommandFlag[] expectedArgs)
    {
        try
        {
            IDictionary<CommandFlag, string> commandArgs = WindowsGrepUtils.ParseGrepCommandArgs(command);
            expectedArgs.ToList().ForEach(x =>
            {
                Assert.True(commandArgs.ContainsKey(x));

                bool expectsParameter = x.GetCustomAttribute<ExpectsParameterAttribute>()?.Value ?? false;
                if (expectsParameter)
                    Assert.True(!string.IsNullOrEmpty(commandArgs[x]), $"Expected parameter for {x} but got empty/null string.");
            });
        }
        catch (Exception ex)
        {
            Assert.Fail($"Unexpected exception: {ex.Message}");
        }
    }

    [Theory]
    [InlineData("-r search_term -k")]
    [InlineData("-r search_term -k .")]
    [InlineData("-z search_term")]
    public void ShortDescriptors_Invalid(string command)
    {
        Assert.Throws<Exception>(() => WindowsGrepUtils.ParseGrepCommandArgs(command));
    }
}
