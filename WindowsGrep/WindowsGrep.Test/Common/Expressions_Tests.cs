namespace WindowsGrep.Test;

public class Expressions_Tests : TestBase
{
    #region Methods..
    [Theory]
    [InlineData("-rk -p 'Test Data' .* '{0}'")]
    public async Task Expressions(string command)
    {
        command = string.Format(command, TestDataDirectory);

        var windowsGrep = ServiceProvider.GetRequiredService<WindowsGrep>();
        var grepService = ServiceProvider.GetRequiredService<GrepService>();

        await windowsGrep.RunGrepCommandAsync(grepService, command, new CancellationTokenSource());

        Assert.True(windowsGrep.Results.Count > 0);
    }
    #endregion Methods..
}