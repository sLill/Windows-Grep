namespace WindowsGrep.Test;

public class Path_Include_Tests : TestBase
{
    [Theory]
    [InlineData("-r -p TestData_0 'This is sample text' '{0}'", new[] { "TestData_0" })]
    [InlineData("-r -p TestData_ 'This is sample text' '{0}'", new[] { "TestData_0" })]
    public async Task Path_Include(string command, string[] includePaths)
    {
        command = string.Format(command, TestDataDirectory);

        var windowsGrep = ServiceProvider.GetRequiredService<WindowsGrep>();
        var grepService = ServiceProvider.GetRequiredService<GrepService>();
        
        await windowsGrep.RunGrepCommandAsync(grepService, command, new CancellationTokenSource());

        Assert.True(windowsGrep.Results.All(x =>
        {
            bool isValidSubdirectory = includePaths.ToList().Any(path => 
            {
                return Path.GetDirectoryName(x.SourceFile.Name).Contains(path);
            });

            return isValidSubdirectory;
        }));
    }
}
