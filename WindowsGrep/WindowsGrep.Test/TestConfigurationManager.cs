using System;

namespace WindowsGrep.Test
{
    public static class TestConfigurationManager
    {
        #region Fields..
        #endregion Fields..

        #region Properties..
        public static string WorkingDirectory => Environment.CurrentDirectory;

        public static string ProjectDirectory => System.IO.Directory.GetParent(WorkingDirectory).Parent.Parent.FullName;
        #endregion Properties..

        #region Constructors..
        #endregion Constructors..

        #region Methods..
        #endregion Methods..
    }
}
