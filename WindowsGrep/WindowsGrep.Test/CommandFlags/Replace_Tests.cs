namespace WindowsGrep.Test;

public class Replace_Tests : TestBase
{
    #region Constructors..
    public Replace_Tests() : base()
    {
        File.Copy(Path.Combine(TestDataDirectory, "TestData_One.txt"), Path.Combine(TestDataDirectory, "ReplaceTest_Blue.txt"), true);

        Directory.CreateDirectory(Path.Combine(TestDataDirectory, "ReplaceTest_Green"));
        File.Copy(Path.Combine(TestDataDirectory, "TestData_One.txt"), Path.Combine(TestDataDirectory, "ReplaceTest_Green/ReplaceTest_Green.txt"), true);
    }
    #endregion Constructors..

    #region Methods..
    public override void Dispose()
    {
        Directory.GetFiles(TestDataDirectory, "ReplaceTest_*")
        .ToList().ForEach(file =>
        {
            if (File.Exists(file))
                File.Delete(file);
        });

        Directory.GetDirectories(TestDataDirectory, "ReplaceTest_*")
        .ToList().ForEach(directory =>
        {
            if (Directory.Exists(directory))
                Directory.Delete(directory, true);
        });
    }

    [Theory]
    [InlineData("--replace='text sample is This' 'This is sample text' '{0}'", "text sample is This")]
    public async Task Replace(string command, string expectedText)
    {
        try
        {
            string replaceTestFilePath = Path.Combine(TestDataDirectory, "ReplaceTest_Blue.txt");
            command = string.Format(command, replaceTestFilePath);

            var windowsGrep = ServiceProvider.GetRequiredService<WindowsGrep>();
            var grepService = ServiceProvider.GetRequiredService<GrepService>();

            await windowsGrep.RunGrepCommandAsync(grepService, command, new CancellationTokenSource());

            string testFileContent = await File.ReadAllTextAsync(replaceTestFilePath);
            Assert.Contains(expectedText, testFileContent);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Unexpected exception: {ex.Message}");
        }
    }

    [Theory]
    [InlineData("-k --replace=Red 'Blue' '{0}'", "ReplaceTest_Red.txt")]
    [InlineData(@"-k --replace=Red 'Bl[^\.]*' '{0}'", "ReplaceTest_Red.txt")]
    public async Task Replace_Filename(string command, string expectedPath)
    {
        try
        {
            command = string.Format(command, TestDataDirectory);

            var windowsGrep = ServiceProvider.GetRequiredService<WindowsGrep>();
            var grepService = ServiceProvider.GetRequiredService<GrepService>();

            await windowsGrep.RunGrepCommandAsync(grepService, command, new CancellationTokenSource());

            Assert.True(File.Exists(Path.Combine(TestDataDirectory, expectedPath)));
        }
        catch (Exception ex)
        {
            Assert.Fail($"Unexpected exception: {ex.Message}");
        }
    }

    [Theory]
    [InlineData("-r -k --replace=Red 'Green' '{0}'", "ReplaceTest_Green/ReplaceTest_Red.txt")]
    public async Task Replace_Filename_Subdirectory(string command, string expectedPath)
    {
        try
        {
            command = string.Format(command, TestDataDirectory);

            var windowsGrep = ServiceProvider.GetRequiredService<WindowsGrep>();
            var grepService = ServiceProvider.GetRequiredService<GrepService>();

            await windowsGrep.RunGrepCommandAsync(grepService, command, new CancellationTokenSource());

            Assert.True(File.Exists(Path.Combine(TestDataDirectory, expectedPath)));
        }
        catch (Exception ex)
        {
            Assert.Fail($"Unexpected exception: {ex.Message}");
        }
    }
    #endregion Methods..
}