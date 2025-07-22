namespace WindowsGrep.Test;

public class Recursive_Tests : TestBase
{
    #region Methods..
    [Theory]
    [InlineData("-r 'sample' '{0}'")]
    public async Task Recursive_Enabled(string command)
    {
        try
        {
            command = string.Format(command, TestDataDirectory);

            var windowsGrep = ServiceProvider.GetRequiredService<WindowsGrep>();
            var grepService = ServiceProvider.GetRequiredService<GrepService>();

            await windowsGrep.RunGrepCommandAsync(grepService, command, new CancellationTokenSource());

            HashSet<string> resultDirectories = new HashSet<string>();
            windowsGrep.Results.ForEach(x => resultDirectories.Add(Path.GetDirectoryName(x.SourceFile.Name)));

            Assert.True(resultDirectories.Count > 1);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Unexpected exception: {ex.Message}");
        }
    }

    [Theory]
    [InlineData("'sample' '{0}'")]
    public async Task Recursive_Disabled(string command)
    {
        try
        {
            command = string.Format(command, TestDataDirectory);

            var windowsGrep = ServiceProvider.GetRequiredService<WindowsGrep>();
            var grepService = ServiceProvider.GetRequiredService<GrepService>();

            await windowsGrep.RunGrepCommandAsync(grepService, command, new CancellationTokenSource());

            HashSet<string> resultDirectories = new HashSet<string>();
            windowsGrep.Results.ForEach(x => resultDirectories.Add(Path.GetDirectoryName(x.SourceFile.Name)));

            Assert.True(resultDirectories.Count == 1);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Unexpected exception: {ex.Message}");
        }
    }
    #endregion Methods..
}