namespace WindowsGrep.Test;

public class Filesize_Min_Tests : TestBase
{
    #region Methods..
    [Theory]
    [InlineData("--filesize-min=5kb 'sample' '{0}'", 5000)]
    [InlineData("--filesize-min=5KB 'sample' '{0}'", 5000)]
    [InlineData("--filesize-min=5Kb 'sample' '{0}'", 5000)]
    [InlineData("--filesize-min=5000 'sample' '{0}'", 5000)]
    public async Task Filesize_Min(string command, long minSize)
    {
        try
        {
            command = string.Format(command, TestDataDirectory);

            var windowsGrep = ServiceProvider.GetRequiredService<WindowsGrep>();
            var grepService = ServiceProvider.GetRequiredService<GrepService>();

            await windowsGrep.RunGrepCommandAsync(grepService, command, new CancellationTokenSource());

            Assert.True(windowsGrep.Results.All(x =>
            {
                long fileSize = WindowsUtils.GetFileSizeOnDisk(x.SourceFile.Name);
                return fileSize >= minSize;
            }));
        }
        catch (Exception ex)
        {
            Assert.Fail($"Unexpected exception: {ex.Message}");
        }
    }
    #endregion Methods..
}