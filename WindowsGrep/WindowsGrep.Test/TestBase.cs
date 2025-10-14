namespace WindowsGrep.Test;

public abstract class TestBase : IDisposable
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

        var publisherMock = new Mock<PublisherService>() { CallBase = true };

        var services = new ServiceCollection();
        services.AddSingleton<ConsoleService>()
                .AddSingleton<FileService>()
                .AddScoped<PublisherService>(_ => publisherMock.Object)
                .AddScoped<WindowsGrep>()
                .AddTransient<GrepService>()
                .AddScoped<NativeService>();

        ServiceProvider = services.BuildServiceProvider();
    }

    public virtual void Dispose() { }
    #endregion Methods..
}
