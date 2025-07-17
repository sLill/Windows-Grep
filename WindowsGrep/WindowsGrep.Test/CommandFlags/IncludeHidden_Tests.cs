namespace WindowsGrep.Test;

public class IncludeHidden_Tests : TestBase
{
    #region Methods..
    [Theory]
    [InlineData("--include-hidden 'This is a hidden file' '{0}'")]
    public async Task IncludeHidden_Enabled(string command)
    {
        command = string.Format(command, TestDataDirectory);

        var windowsGrep = ServiceProvider.GetRequiredService<WindowsGrep>();
        var grepService = ServiceProvider.GetRequiredService<GrepService>();

        await windowsGrep.RunGrepCommandAsync(grepService, command, new CancellationTokenSource());

        Assert.True(windowsGrep.Results.Count > 0);
    }

    [Theory]
    [InlineData("'This is a hidden file' '{0}'")]
    public async Task IncludeHidden_Disabled(string command)
    {
        command = string.Format(command, TestDataDirectory);

        var windowsGrep = ServiceProvider.GetRequiredService<WindowsGrep>();
        var grepService = ServiceProvider.GetRequiredService<GrepService>();

        await windowsGrep.RunGrepCommandAsync(grepService, command, new CancellationTokenSource());

        Assert.True(windowsGrep.Results.Count == 0);
    }

    [Theory]
    [InlineData("-k --include-hidden '_Hidden' '{0}'")]
    public async Task IncludeHidden_Filename_Enabled(string command)
    {
        command = string.Format(command, TestDataDirectory);

        var windowsGrep = ServiceProvider.GetRequiredService<WindowsGrep>();
        var grepService = ServiceProvider.GetRequiredService<GrepService>();

        await windowsGrep.RunGrepCommandAsync(grepService, command, new CancellationTokenSource());

        Assert.True(windowsGrep.Results.Count > 0);
    }

    [Theory]
    [InlineData("-k '_Hidden' '{0}'")]
    public async Task IncludeHidden_Filename_Disabled(string command)
    {
        command = string.Format(command, TestDataDirectory);

        var windowsGrep = ServiceProvider.GetRequiredService<WindowsGrep>();
        var grepService = ServiceProvider.GetRequiredService<GrepService>();

        await windowsGrep.RunGrepCommandAsync(grepService, command, new CancellationTokenSource());

        Assert.True(windowsGrep.Results.Count == 0);
    }
    #endregion Methods..
}