namespace WindowsGrep.Test;

public class Filetype_IncludeTests : TestBase
{
    #region Methods..
    [Theory]
    [InlineData("-r -i -k -t \'txt\' \".*\" '{0}'", new[] { ".txt" })]
    [InlineData("-t .cpp 'Hello, World!' '{0}'", new[] { ".cpp" })]
    [InlineData("-t .cpp,.go 'Hello, World!' '{0}'", new[] { ".cpp", ".go" })]
    [InlineData("-t cpp 'Hello, World!' '{0}'", new[] { ".cpp" })]
    [InlineData("-t '.cpp,.go' 'Hello, World!' '{0}'", new[] { ".cpp", ".go" })]
    [InlineData("-t \".cpp,.go\" 'Hello, World!' '{0}'", new[] { ".cpp", ".go" })]
    public async Task Filetype_Include(string command, string[] validFiletypes)
    {
        try
        {
            command = string.Format(command, TestDataDirectory);

            var windowsGrep = ServiceProvider.GetRequiredService<WindowsGrep>();
            var grepService = ServiceProvider.GetRequiredService<GrepService>();

            await windowsGrep.RunGrepCommandAsync(grepService, command, new CancellationTokenSource());

            Assert.True(windowsGrep.Results.All(x => validFiletypes.Contains(Path.GetExtension(x.SourceFile.Name))));
        }
        catch (Exception ex)
        {
            Assert.Fail($"Unexpected exception: {ex.Message}");
        }
    }
    #endregion Methods..
}
