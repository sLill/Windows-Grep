namespace WindowsGrep.Test;

public class Path_Include_Tests : TestBase
{
    #region Methods..
    [Theory]
    [InlineData("-r -p TestData_0 'This is sample text' '{0}'", new[] { "TestData_0" })]
    [InlineData("-r -p TestData_ 'This is sample text' '{0}'", new[] { "TestData_0" })]
    [InlineData("-rk -p 'Test Data (x86)' .* '{0}'", new[] { "Test Data (x86)" })]
    [InlineData("-rk -p '(x86),TestData_' .* '{0}'", new[] { "Test Data (x86)", "TestData_0" })]
    [InlineData("-rk -p 'Test Data (x86)',TestData_ .* '{0}'", new[] { "Test Data (x86)", "TestData_0" })]
    [InlineData(@"-rk -p 'Test Data (x86)',""TestData_"" .* '{0}'", new[] { "Test Data (x86)", "TestData_0" })]
    public async Task Path_Include(string command, string[] includePaths)
    {
        try 
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
        catch (Exception ex)
        {
            Assert.Fail($"Unexpected exception: {ex.Message}");
        }
    } 
    #endregion Methods..
}
