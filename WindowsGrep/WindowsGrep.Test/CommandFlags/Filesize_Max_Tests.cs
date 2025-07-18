namespace WindowsGrep.Test;

public class Filesize_Max_Tests : TestBase
{
    #region Methods..
    [Theory]
    [InlineData("--filesize-max=5kb 'sample' '{0}'", 5000)]
    [InlineData("--filesize-max=5KB 'sample' '{0}'", 5000)]
    [InlineData("--filesize-max=5Kb 'sample' '{0}'", 5000)]
    [InlineData("--filesize-max=5000 'sample' '{0}'", 5000)]
    public async Task Filesize_Max(string command, long maxSize)
    {
        command = string.Format(command, TestDataDirectory);

        var windowsGrep = ServiceProvider.GetRequiredService<WindowsGrep>();
        var grepService = ServiceProvider.GetRequiredService<GrepService>();

        await windowsGrep.RunGrepCommandAsync(grepService, command, new CancellationTokenSource());

        Assert.True(windowsGrep.Results.All(x =>
        {
            long fileSize = WindowsUtils.GetFileSizeOnDisk(x.SourceFile.Name);
            return fileSize <= maxSize;
        }));
    }
    #endregion Methods..
}