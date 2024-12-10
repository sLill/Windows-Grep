using System.Collections.Generic;

namespace WindowsGrep.Engine
{
    public class SearchMetrics
    {
        #region Properties..
        public int TotalFilesMatchedCount { get; set; }

        public int DeleteSuccessCount { get; set; }

        public int ReplacedSuccessCount { get; set; }

        public List<string> FailedReadFiles { get; set; } = new List<string>();

        public List<string> FailedWriteFiles { get; set; } = new List<string>();
        #endregion Properties..
    }
}