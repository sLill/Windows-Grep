namespace WindowsGrep.Test;

public class IgnoreCase_Tests : TestBase
{
    [Theory]
    [InlineData("-i 'THIS IS SAMPLE TEXT' '{0}'")]
    public async Task IgnoreCase_Enabled(string command)
    {
        command = string.Format(command, TestDataDirectory);

        var windowsGrep = ServiceProvider.GetRequiredService<WindowsGrep>();
        var grepService = ServiceProvider.GetRequiredService<GrepService>();

        await windowsGrep.RunGrepCommandAsync(grepService, command, new CancellationTokenSource());

        Assert.True(windowsGrep.Results.Count > 0);
    }

    [Theory]
    [InlineData("'THIS IS SAMPLE TEXT' '{0}'")]
    public async Task IgnoreCase_Disabled(string command)
    {
        command = string.Format(command, TestDataDirectory);

        var windowsGrep = ServiceProvider.GetRequiredService<WindowsGrep>();
        var grepService = ServiceProvider.GetRequiredService<GrepService>();

        await windowsGrep.RunGrepCommandAsync(grepService, command, new CancellationTokenSource());

        Assert.True(windowsGrep.Results.Count == 0);
    }
}