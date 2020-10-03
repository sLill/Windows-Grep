using System;
using System.Collections.Generic;
using System.Text;

namespace WindowsGrep.Test
{
    public static class TestConfigurationManager
    {
        #region Member Variables..
        #endregion Member Variables..

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
