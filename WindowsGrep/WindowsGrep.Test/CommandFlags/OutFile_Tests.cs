namespace WindowsGrep.Test;

public class OutFile_Tests : TestBase
{
    [Theory]
    [InlineData("-o output.txt 'This is sample text' '{0}'", "output.txt")]
    [InlineData("-o 'output.txt' 'This is sample text' '{0}'", "output.txt")]
    [InlineData("-o './OutFile_Test/output.txt' 'This is sample text' '{0}'", "./OutFile_Test/output.txt")]
    public async Task OutFile_Enabled(string command, string outputPath)
    {
        command = string.Format(command, TestDataDirectory);

        var windowsGrep = ServiceProvider.GetRequiredService<WindowsGrep>();
        var grepService = ServiceProvider.GetRequiredService<GrepService>();

        await windowsGrep.RunGrepCommandAsync(grepService, command, new CancellationTokenSource());

        Assert.True(File.Exists(outputPath));
        Assert.True(new FileInfo(outputPath).Length > 0);

        if (File.Exists(outputPath))
            File.Delete(outputPath);

        string? subdirectory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(subdirectory) && Directory.Exists(subdirectory))
            Directory.Delete(subdirectory, true);
    }
}