namespace WindowsGrep.Engine
{
    public class SearchMetrics
    {
        #region Properties..
        #region TotalFilesMatchedCount
        public int TotalFilesMatchedCount { get; set; }
        #endregion TotalFilesMatchedCount

        #region DeleteSuccessCount
        public int DeleteSuccessCount { get; set; }
        #endregion DeleteSuccessCount

        #region ReplacedSuccessCount
        public int ReplacedSuccessCount { get; set; }
        #endregion ReplacedSuccessCount

        #region FileReadFailedCount
        public int FileReadFailedCount { get; set; }
        #endregion FileReadFailedCount

        #region FileWriteFailedCount
        public int FileWriteFailedCount { get; set; }
        #endregion FileWriteFailedCount
        #endregion Properties..
    }
}