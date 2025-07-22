namespace WindowsGrep.Test;

public class CommandParser_SearchTerm_Tests : TestBase
{
    [Theory]
    [InlineData("-ri search_term .")]
    [InlineData("-ri search_term")]
    [InlineData("search_term .")]
    [InlineData("search_term")]
    [InlineData("-ri \"search_term\" .")]
    [InlineData("-ri \"search_term\"")]
    [InlineData("\"search_term\" .")]
    [InlineData("\"search_term\"")]
    [InlineData("-ri 'search_term' .")]
    [InlineData("-ri 'search_term'")]
    [InlineData("'search_term' .")]
    [InlineData("'search_term'")]
    public void SearchTerm_Valid(string command)
    {
        try
        {
            IDictionary<CommandFlag, string> commandArgs = WindowsGrepUtils.ParseGrepCommandArgs(command);
            Assert.True(commandArgs.ContainsKey(CommandFlag.SearchTerm));
        }
        catch (Exception ex)
        {
            Assert.Fail($"Unexpected exception: {ex.Message}");
        }
    }
}
