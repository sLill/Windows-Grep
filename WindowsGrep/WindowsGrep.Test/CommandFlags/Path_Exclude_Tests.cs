namespace WindowsGrep.Test;

public class Path_Exclude_Tests : TestBase
{
    [Theory]
    [InlineData("-r -P TestData_0 'This is sample text' '{0}'", new[] { "TestData_0" })]
    [InlineData("-r -P TestData_ 'This is sample text' '{0}'", new[] { "TestData_0" })]
    public async Task Path_Exclude(string command, string[] excludePaths)
    {
        command = string.Format(command, TestDataDirectory);

        var windowsGrep = ServiceProvider.GetRequiredService<WindowsGrep>();
        var grepService = ServiceProvider.GetRequiredService<GrepService>();

        await windowsGrep.RunGrepCommandAsync(grepService, command, new CancellationTokenSource());

        Assert.True(windowsGrep.Results.All(x =>
        {
            bool isValidSubdirectory = !excludePaths.ToList().Any(path =>
            {
                return Path.GetDirectoryName(x.SourceFile.Name).Contains(path);
            });

            return isValidSubdirectory;
        }));
    }
}