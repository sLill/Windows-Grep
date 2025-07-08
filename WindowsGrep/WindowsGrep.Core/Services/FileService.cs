namespace WindowsGrep.Core;

public class FileService 
{
    #region Fields..
    private readonly ILogger<FileService> _logger;
    private object _fileLock = new object();
    #endregion Fields..

    #region Properties..
    public string Filepath { get; private set; }
    #endregion Properties..

    #region Constructors..
    public FileService(ILogger<FileService> logger)
    {
        _logger = logger;
    }
    #endregion Constructors..

    #region Methods..	
    public void Initialize(string filepath)
    {
        if (string.IsNullOrEmpty(filepath))
            throw new Exception("OutFile parameter cannot be null");
        else if (!Directory.Exists(Path.GetDirectoryName(filepath)))
            throw new Exception("OutFile directory does not exist");
        else
        {
            Filepath = filepath;

            if (File.Exists(Filepath))
                File.Delete(Filepath);
        }
    }

    public void Write(ConsoleItem consoleItem)
    {
        lock (_fileLock)
            File.AppendAllText(Filepath, consoleItem.Value);
    }
    #endregion Methods..
}
