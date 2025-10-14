namespace WindowsGrep.Test;

public class Integration_Tests : TestBase
{
    #region Methods..
    [Theory]
    [InlineData("-r '^-' '{0}'", 100)]
    public async Task Consistency(string command, int cycles)
    {
        int resultCount = -1;

        try
        {
            var windowsGrep = ServiceProvider.GetRequiredService<WindowsGrep>();

            for (int i = 0; i < cycles; i++)
            {
                var grepService = ServiceProvider.GetRequiredService<GrepService>();

                command = string.Format(command, TestDataDirectory);
                grepService.FilesPerTask = 3;
                windowsGrep.Results.Clear();

                await windowsGrep.RunGrepCommandAsync(grepService, command, new CancellationTokenSource());

                HashSet<string> resultDirectories = new HashSet<string>();

                if (resultCount == -1)
                    resultCount = windowsGrep.Results.Count;
                else if (windowsGrep.Results.Count != resultCount)
                    throw new Exception($"ResultCount: {resultCount}, ActualCount:{windowsGrep.Results.Count}");
            }
        }
        catch (Exception ex)
        {
            Assert.Fail($"Unexpected exception: {ex.Message}");
        }
    }
    #endregion Methods..
}