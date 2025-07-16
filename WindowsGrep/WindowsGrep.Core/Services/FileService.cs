namespace WindowsGrep.Core;

public class FileService 
{
    #region Fields..
    private object _fileLock = new object();
    #endregion Fields..

    #region Properties..
    public string Filepath { get; private set; }
    #endregion Properties..

    #region Constructors..
    #endregion Constructors..

    #region Methods..	
    public void Initialize(string filepath)
    {
        filepath = filepath.Trim(new char[] { '\'', '"' });

        if (string.IsNullOrEmpty(filepath))
            throw new Exception("OutFile parameter cannot be null");
        else
        {
            string? directory = Path.GetDirectoryName(filepath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

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
