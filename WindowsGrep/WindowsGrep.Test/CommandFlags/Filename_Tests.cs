namespace WindowsGrep.Test;

public class Filename_Tests : TestBase
{
    #region Methods..
    [Theory]
    [InlineData("-k 'TestData_One' '{0}'")]
    public async Task Filename_Enabled(string command)
    {
        try
        {
            command = string.Format(command, TestDataDirectory);

            var windowsGrep = ServiceProvider.GetRequiredService<WindowsGrep>();
            var grepService = ServiceProvider.GetRequiredService<GrepService>();

            await windowsGrep.RunGrepCommandAsync(grepService, command, new CancellationTokenSource());

            Assert.True(windowsGrep.Results.Count == 1);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Unexpected exception: {ex.Message}");
        }
    }

    [Theory]
    [InlineData("'TestData_One' '{0}'")]
    public async Task Filename_Disabled(string command)
    {
        try
        {
            command = string.Format(command, TestDataDirectory);

            var windowsGrep = ServiceProvider.GetRequiredService<WindowsGrep>();
            var grepService = ServiceProvider.GetRequiredService<GrepService>();

            await windowsGrep.RunGrepCommandAsync(grepService, command, new CancellationTokenSource());

            Assert.True(windowsGrep.Results.Count == 0);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Unexpected exception: {ex.Message}");
        }
    }
    #endregion Methods..
}