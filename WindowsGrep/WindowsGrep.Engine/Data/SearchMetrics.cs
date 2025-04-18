namespace WindowsGrep.Engine;

public class SearchMetrics
{
    #region Properties..
    public int TotalFilesMatchedCount { get; set; }

    public int DeleteSuccessCount { get; set; }

    public int ReplacedSuccessCount { get; set; }

    public List<FileItem> FailedReadFiles { get; set; } = new List<FileItem>();

    public List<FileItem> FailedWriteFiles { get; set; } = new List<FileItem>();
    #endregion Properties..
}