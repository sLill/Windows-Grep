namespace WindowsGrep.Test;

public class MaxDepth_Tests : TestBase
{
    #region Methods..
    [Theory]
    [InlineData("-r --max-depth=0 'This is sample text' '{0}'", 0)]
    [InlineData("-r --max-depth=1 'This is sample text' '{0}'", 1)]
    public async Task MaxDepth_Enabled(string command, int maxDepth)
    {
        try
        {
            command = string.Format(command, TestDataDirectory);

            var windowsGrep = ServiceProvider.GetRequiredService<WindowsGrep>();
            var grepService = ServiceProvider.GetRequiredService<GrepService>();

            await windowsGrep.RunGrepCommandAsync(grepService, command, new CancellationTokenSource());

            Assert.True(windowsGrep.Results.Any(x =>
            {
                string relativePath = Path.GetRelativePath(TestDataDirectory, Path.GetDirectoryName(x.SourceFile.Name));
                int depth = relativePath == "." ? 0 : relativePath.Split(Path.DirectorySeparatorChar).Length;

                return depth <= maxDepth;
            }));
        }
        catch (Exception ex)
        {
            Assert.Fail($"Unexpected exception: {ex.Message}");
        }
    }

    [Theory]
    [InlineData("-r 'This is sample text' '{0}'")]
    public async Task MaxDepth_Disabled(string command)
    {
        try
        {
            command = string.Format(command, TestDataDirectory);

            var windowsGrep = ServiceProvider.GetRequiredService<WindowsGrep>();
            var grepService = ServiceProvider.GetRequiredService<GrepService>();

            await windowsGrep.RunGrepCommandAsync(grepService, command, new CancellationTokenSource());

            Assert.True(windowsGrep.Results.Any(x =>
            {
                string relativePath = Path.GetRelativePath(TestDataDirectory, Path.GetDirectoryName(x.SourceFile.Name));
                int depth = relativePath == "." ? 0 : relativePath.Split(Path.DirectorySeparatorChar).Length;

                return depth > 0;
            }));
        }
        catch (Exception ex)
        {
            Assert.Fail($"Unexpected exception: {ex.Message}");
        }
    }
    #endregion Methods..
}