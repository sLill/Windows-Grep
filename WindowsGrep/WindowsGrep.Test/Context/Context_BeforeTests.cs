namespace WindowsGrep.Test;

public class Context_BeforeTests : TestBase
{
    [Theory]
    [InlineData("whitespace '{0}'")]
    public async Task Context_None(string command)
    {
        var windowsGrep = ServiceProvider.GetRequiredService<WindowsGrep>();
        var publisherService = ServiceProvider.GetRequiredService<PublisherService>();
        
        command = string.Format(command, Environment.CurrentDirectory);

        List<ConsoleItem> items = new List<ConsoleItem>();
        publisherService.Subscribe<ConsoleItem>(x => items.Add(x));

        await windowsGrep.RunAsync(new[] { command }, new CancellationTokenSource());
        Assert.True(items.Any(x => x.Value.Contains("whitespace")));
    }
}
