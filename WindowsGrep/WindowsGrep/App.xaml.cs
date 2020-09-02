using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WindowsGrep.View.Windows;

namespace WindowsGrep
{
    public partial class App : Application
    {
        #region Application_Startup
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(e.Args);
            mainWindow.Show();
        }
        #endregion Application_Startup
    }
}
