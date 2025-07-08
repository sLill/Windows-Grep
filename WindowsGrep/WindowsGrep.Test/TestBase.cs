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

        var services = new ServiceCollection();
        services.AddSingleton<ConsoleService>()
                .AddSingleton<FileService>()
                .AddScoped<WindowsGrep>()
                .AddScoped<GrepService>()
                .AddScoped<NativeService>()
                .AddScoped<PublisherService>();

        ServiceProvider = services.BuildServiceProvider();
    }
    #endregion Methods..
}
