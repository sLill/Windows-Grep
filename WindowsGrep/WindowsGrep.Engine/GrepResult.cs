namespace WindowsGrep.Engine
{
    public class GrepResult
    {
        #region Properties..
        #region ContextString
        public string ContextString { get; set; }
        #endregion ContextString

        #region MatchedString
        public string MatchedString { get; set; }
        #endregion MatchedString

        #region SourceFile
        public string SourceFile { get; set; }
        #endregion SourceFile
        #endregion Properties..

        #region Constructors..
        #region GrepResult
        public GrepResult(string sourceFile)
        {
            SourceFile = sourceFile;
        }
        #endregion GrepResult
        #endregion Constructors..
    }
}
