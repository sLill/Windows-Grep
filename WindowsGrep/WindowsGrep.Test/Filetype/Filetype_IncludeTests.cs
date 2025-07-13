namespace WindowsGrep.Test;

public class Filetype_IncludeTests : TestBase
{
    [Theory]
    [InlineData("-t .cpp 'Hello, World!' '{0}'", new[] { ".cpp" })]
    [InlineData("-t .cpp,.go 'Hello, World!' '{0}'", new[] { ".cpp", ".go" })]
    [InlineData("-t cpp 'Hello, World!' '{0}'", new[] { ".cpp" })]
    [InlineData("-t '.cpp,.go' 'Hello, World!' '{0}'", new[] { ".cpp", ".go" })]
    [InlineData("-t \".cpp,.go\" 'Hello, World!' '{0}'", new[] { ".cpp", ".go" })]
    public async Task Filetype_Include(string command, string[] validFiletypes)
    {
        command = string.Format(command, TestDataDirectory);

        var windowsGrep = ServiceProvider.GetRequiredService<WindowsGrep>();
        await windowsGrep.RunAsync(new[] { command }, new CancellationTokenSource());
        
        Assert.True(windowsGrep.Results.All(x => validFiletypes.Contains(Path.GetExtension(x.SourceFile.Name))));
    }
}
