namespace WindowsGrep.Test;

public class Context_Tests : TestBase
{
    [Theory]
    [InlineData("-c 1 'hitespac' '{0}'", 1)]
    [InlineData("'hitespac' '{0}'", 0)]
    public async Task Context(string command, int contextLength)
    {
        command = string.Format(command, TestDataDirectory);

        var windowsGrep = ServiceProvider.GetRequiredService<WindowsGrep>();
        var grepService = ServiceProvider.GetRequiredService<GrepService>();

        await windowsGrep.RunGrepCommandAsync(grepService, command, new CancellationTokenSource());

        Assert.True(windowsGrep.Results.All(x => ((GrepResult)x).LeadingContextString.Trim().Length == contextLength && ((GrepResult)x).TrailingContextString.Trim().Length == contextLength));
    }
}