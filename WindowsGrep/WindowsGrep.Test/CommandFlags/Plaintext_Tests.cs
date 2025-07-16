namespace WindowsGrep.Test;

public class Plaintext_Tests : TestBase
{
    #region Methods..
    [Theory]
    [InlineData("-F '*Markdown' '{0}'")]
    public async Task Plaintext_Enabled(string command)
    {
        command = string.Format(command, TestDataDirectory);

        var windowsGrep = ServiceProvider.GetRequiredService<WindowsGrep>();
        var grepService = ServiceProvider.GetRequiredService<GrepService>();

        await windowsGrep.RunGrepCommandAsync(grepService, command, new CancellationTokenSource());

        Assert.True(windowsGrep.Results.Count > 0);
    }

    [Theory]
    [InlineData("'*Markdown' '{0}'")]
    public async Task Plaintext_Disabled(string command)
    {
        command = string.Format(command, TestDataDirectory);

        var windowsGrep = ServiceProvider.GetRequiredService<WindowsGrep>();
        var grepService = ServiceProvider.GetRequiredService<GrepService>();

        await windowsGrep.RunGrepCommandAsync(grepService, command, new CancellationTokenSource());

        Assert.True(windowsGrep.Results.Count == 0);
    } 
    #endregion Methods..
}