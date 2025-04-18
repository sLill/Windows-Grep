namespace WindowsGrep.Engine;

public class SearchMetrics
{
    #region Properties..
    public int TotalFilesMatchedCount { get; set; }

    public int DeleteSuccessCount { get; set; }

    public int ReplacedSuccessCount { get; set; }

    public List<(string Name, bool IsDirectory)> FailedReadFiles { get; set; } = new List<(string Name, bool IsDirectory)>();

    public List<(string Name, bool IsDirectory)> FailedWriteFiles { get; set; } = new List<(string Name, bool IsDirectory)>();
    #endregion Properties..
}