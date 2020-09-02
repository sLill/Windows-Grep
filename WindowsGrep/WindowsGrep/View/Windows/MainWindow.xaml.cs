using System;
using System.Windows;
using System.Windows.Interop;
using WindowsGrep.ViewModel;

namespace WindowsGrep.View.Windows
{
    public partial class MainWindow : Window
    {
        #region Properties..
        #region MainWindowViewModel
        private MainWindowViewModel _mainWindowViewModel;
        public MainWindowViewModel MainWindowViewModel
        {
            get { return _mainWindowViewModel; }
            set { _mainWindowViewModel = value; }
        }
        #endregion MainWindowViewModel
        #endregion Properties..

        #region Constructors..
        #region MainWindow
        public MainWindow(string[] args)
        {
            InitializeComponent();

            _mainWindowViewModel = new MainWindowViewModel(args);
            DataContext = _mainWindowViewModel;
        }
        #endregion MainWindow
        #endregion Constructors..

        #region Methods..
        #region EventHandlers..
        #region Window_Closing
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ReleaseShortcuts();
        }
        #endregion Window_Closing

        #region Window_Loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RegisterShortcuts();
        }
        #endregion Window_Loaded
        #endregion EventHandlers..

        #region RegisterShortcuts
        private void RegisterShortcuts()
        {
            var windowInteropHelper = new WindowInteropHelper(this);
            IntPtr windowHandle = windowInteropHelper.Handle;
            MainWindowViewModel.RegisterShortcuts(windowHandle);
        }
        #endregion RegisterShortcuts

        #region ReleaseShortcuts
        private void ReleaseShortcuts()
        {
            var windowInteropHelper = new WindowInteropHelper(this);
            IntPtr windowHandle = windowInteropHelper.Handle;
            MainWindowViewModel.ReleaseShortcuts(windowHandle);
        }
        #endregion ReleaseShortcuts

        #endregion Methods..
    }
}
