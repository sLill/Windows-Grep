namespace WindowsGrep.Engine
{
    public class SearchMetrics
    {
        #region Properties..
        public int TotalFilesMatchedCount { get; set; }

        public int DeleteSuccessCount { get; set; }

        public int ReplacedSuccessCount { get; set; }

        public int FileReadFailedCount { get; set; }

        public int FileWriteFailedCount { get; set; }
        #endregion Properties..
    }
}