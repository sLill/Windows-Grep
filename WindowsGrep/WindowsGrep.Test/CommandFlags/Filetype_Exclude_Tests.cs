namespace WindowsGrep.Test;

public class Filetype_Exclude_Tests : TestBase
{
    [Theory]
    [InlineData("-T .cpp 'Hello, World!' '{0}'", new[] { ".cpp" })]
    [InlineData("-T .cpp,.go 'Hello, World!' '{0}'", new[] { ".cpp", ".go" })]
    [InlineData("-T '.cpp,.go' 'Hello, World!' '{0}'", new[] { ".cpp", ".go" })]
    [InlineData("-T \".cpp,.go\" 'Hello, World!' '{0}'", new[] { ".cpp", ".go" })]
    [InlineData("-T cpp 'Hello, World!' '{0}'", new[] { ".cpp" })]
    public async Task Filetype_Exclude(string command, string[] invalidFiletypes)
    {
        command = string.Format(command, TestDataDirectory);

        var windowsGrep = ServiceProvider.GetRequiredService<WindowsGrep>();
        var grepService = ServiceProvider.GetRequiredService<GrepService>();

        await windowsGrep.RunGrepCommandAsync(grepService, command, new CancellationTokenSource());
        
        Assert.True(windowsGrep.Results.All(x => !invalidFiletypes.Contains(Path.GetExtension(x.SourceFile.Name))));
    }
}
