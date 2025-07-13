namespace WindowsGrep.Test;

public abstract class TestBase
{
    #region Properties..
    protected string TestDataDirectory { get; private set; }
    protected IServiceProvider ServiceProvider { get; private set; }
    #endregion Properties..

    #region Constructors..
    protected TestBase()
    {
        Initialize();
    }
    #endregion Constructors..

    #region Methods..
    private void Initialize()
    {
        TestDataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Properties/Resources/TestData");

        var publisherMock = new Mock<PublisherService>();
        publisherMock.Setup(x => x.RemoveAllSubscribers()).Callback(() => { });

        var services = new ServiceCollection();
        services.AddSingleton<ConsoleService>()
                .AddSingleton<FileService>()
                .AddSingleton<ILogger>(_ => NullLogger.Instance)
                .AddScoped<PublisherService>(_ => publisherMock.Object)
                .AddScoped<WindowsGrep>()
                .AddScoped<GrepService>()
                .AddScoped<NativeService>();

        ServiceProvider = services.BuildServiceProvider();
    }
    #endregion Methods..
}
