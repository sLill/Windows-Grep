using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WindowsGrep.UI
{
    public partial class MainWindow : Window
    {
        #region Constructors..
        #region MainWindow
        public MainWindow(string[] args)
        {
            InitializeComponent();

            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

            // ReadMe
            if (args.Length == 0)
            {
                string ReadMe = Properties.Resources.ReadMe;
                Console.WriteLine(ReadMe + Environment.NewLine);
            }

            do
            {
                string Command = args.Length == 0 ? Console.ReadLine() : string.Join(" ", args);
                if (Command.Length > 0)
                {
                    try
                    {
                        var CommandResults = GrepEngine.ProcessCommand(Command);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message + Environment.NewLine);
                    }
                }
            }
            while (args.Length == 0);
        }
        #endregion MainWindow
        #endregion Constructors..

        #region Methods..
        #region EventHandlers..
        #region CurrentDomain_ProcessExit
        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            // Unregister shortcuts

            // Unsubscribe from events
            AppDomain.CurrentDomain.ProcessExit -= CurrentDomain_ProcessExit;
        }
        #endregion CurrentDomain_ProcessExit
        #endregion EventHandlers..
        #region RegisterShortcuts
        private void RegisterShortcuts()
        {
            Convert.ToUInt32(KeyInterop.VirtualKeyFromKey(key).ToString("X"), 16);
        }
        #endregion RegisterShortcuts

        #region UnregisterShortcuts
        private void UnregisterShortcuts()
        {

        }
        #endregion UnregisterShortcuts
        #endregion Methods..
    }
}
