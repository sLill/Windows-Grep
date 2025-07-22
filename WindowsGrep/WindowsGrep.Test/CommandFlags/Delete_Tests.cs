namespace WindowsGrep.Test;

public class Delete_Tests : TestBase
{
    #region Constructors..
    public Delete_Tests() : base()
    {
        File.WriteAllText(Path.Combine(TestDataDirectory, "DeleteTest_Blue.txt"), "The quick brown fox jumps over the lazy dog");

        Directory.CreateDirectory(Path.Combine(TestDataDirectory, "DeleteTest_Green"));
        File.WriteAllText(Path.Combine(TestDataDirectory, "DeleteTest_Green/DeleteTest_Green.txt"), "The quick brown fox jumps over the lazy dog");
    }
    #endregion Constructors..

    #region Methods..
    public override void Dispose()
    {
        Directory.GetFiles(TestDataDirectory, "DeleteTest_*")
        .ToList().ForEach(file =>
        {
            if (File.Exists(file))
                File.Delete(file);
        });

        Directory.GetDirectories(TestDataDirectory, "DeleteTest_*")
        .ToList().ForEach(directory =>
        {
            if (Directory.Exists(directory))
                Directory.Delete(directory, true);
        });
    }

    [Theory]
    [InlineData("--delete 'quick brown fox' '{0}'")]
    [InlineData("-r --delete 'quick brown fox' '{0}'")]
    public async Task Delete(string command)
    {
        try
        {
            string deleteTestFilePath = Path.Combine(TestDataDirectory, "DeleteTest_Blue.txt");
            command = string.Format(command, deleteTestFilePath);

            var windowsGrep = ServiceProvider.GetRequiredService<WindowsGrep>();
            var grepService = ServiceProvider.GetRequiredService<GrepService>();

            await windowsGrep.RunGrepCommandAsync(grepService, command, new CancellationTokenSource());

            Assert.True(windowsGrep.Results.All(x => !File.Exists(x.SourceFile.Name)));
        }
        catch (Exception ex)
        {
            Assert.Fail($"Unexpected exception: {ex.Message}");
        }
    }

    [Theory]
    [InlineData("-k --delete 'DeleteTest_' '{0}'")]
    [InlineData("-r -k --delete 'DeleteTest_' '{0}'")]
    public async Task Delete_Filename(string command)
    {
        try
        { 
            command = string.Format(command, TestDataDirectory);

            var windowsGrep = ServiceProvider.GetRequiredService<WindowsGrep>();
            var grepService = ServiceProvider.GetRequiredService<GrepService>();

            await windowsGrep.RunGrepCommandAsync(grepService, command, new CancellationTokenSource());

            Assert.True(windowsGrep.Results.All(x => !File.Exists(x.SourceFile.Name)));
        }
        catch (Exception ex)
        {
            Assert.Fail($"Unexpected exception: {ex.Message}");
        }
    }
    #endregion Methods..
}