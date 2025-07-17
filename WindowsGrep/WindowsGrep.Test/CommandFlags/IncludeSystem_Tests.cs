namespace WindowsGrep.Test;

public class IncludeSystem_Tests : TestBase
{
    #region Methods..
    [Theory]
    [InlineData("--include-system 'This is a system file' '{0}'")]
    public async Task IncludeSystem_Enabled(string command)
    {
        command = string.Format(command, TestDataDirectory);

        var windowsGrep = ServiceProvider.GetRequiredService<WindowsGrep>();
        var grepService = ServiceProvider.GetRequiredService<GrepService>();

        await windowsGrep.RunGrepCommandAsync(grepService, command, new CancellationTokenSource());

        Assert.True(windowsGrep.Results.Count > 0);
    }

    [Theory]
    [InlineData("'This is a system file' '{0}'")]
    public async Task IncludeSystem_Disabled(string command)
    {
        command = string.Format(command, TestDataDirectory);

        var windowsGrep = ServiceProvider.GetRequiredService<WindowsGrep>();
        var grepService = ServiceProvider.GetRequiredService<GrepService>();

        await windowsGrep.RunGrepCommandAsync(grepService, command, new CancellationTokenSource());

        Assert.True(windowsGrep.Results.Count == 0);
    }

    [Theory]
    [InlineData("-k --include-system '_System' '{0}'")]
    public async Task IncludeSystem_Filename_Enabled(string command)
    {
        command = string.Format(command, TestDataDirectory);

        var windowsGrep = ServiceProvider.GetRequiredService<WindowsGrep>();
        var grepService = ServiceProvider.GetRequiredService<GrepService>();

        await windowsGrep.RunGrepCommandAsync(grepService, command, new CancellationTokenSource());

        Assert.True(windowsGrep.Results.Count > 0);
    }

    [Theory]
    [InlineData("-k '_System' '{0}'")]
    public async Task IncludeSystem_Filename_Disabled(string command)
    {
        command = string.Format(command, TestDataDirectory);

        var windowsGrep = ServiceProvider.GetRequiredService<WindowsGrep>();
        var grepService = ServiceProvider.GetRequiredService<GrepService>();

        await windowsGrep.RunGrepCommandAsync(grepService, command, new CancellationTokenSource());

        Assert.True(windowsGrep.Results.Count == 0);
    }
    #endregion Methods..
}