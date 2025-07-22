namespace WindowsGrep.Test;

public class NoFlags_Tests : TestBase
{
    #region Methods..
    [Theory]
    [InlineData("'This is sample text' '{0}'")]
    [InlineData("sample '{0}'")]
    public async Task NoFlag(string command)
    {
        try
        {
            command = string.Format(command, TestDataDirectory);

            var windowsGrep = ServiceProvider.GetRequiredService<WindowsGrep>();
            var grepService = ServiceProvider.GetRequiredService<GrepService>();

            await windowsGrep.RunGrepCommandAsync(grepService, command, new CancellationTokenSource());

            Assert.True(windowsGrep.Results.Count > 0);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Unexpected exception: {ex.Message}");
        }
    }
    #endregion Methods..
}