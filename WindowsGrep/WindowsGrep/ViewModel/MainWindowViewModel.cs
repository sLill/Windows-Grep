using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Interop;
using WindowsGrep.Common;
using WindowsGrep.Engine;

namespace WindowsGrep.ViewModel
{
    public class MainWindowViewModel : ObservableObject
    {
        #region Member Variables..
        private static HwndSource _hwndSource;
        #endregion Member Variables..

        #region Properties..
        #region ConsoleLogCollection
        private ObservableCollection<string> _consoleLogCollection;
        public ObservableCollection<string> ConsoleLogCollection
        {
            get { return _consoleLogCollection; }
            set
            {
                _consoleLogCollection = value;
                RaisePropertyChanged();
            }
        }
        #endregion ConsoleLogCollection
        #endregion Properties..

        #region Constructors..
        #region MainWindowViewModel
        public MainWindowViewModel(string[] args)
        {
            // ReadMe
            //if (args.Length == 0)
            //{
            //    string ReadMe = Properties.Resources.ReadMe;
            //    Console.WriteLine(ReadMe + Environment.NewLine);
            //}

            //do
            //{
            //    string Command = args.Length == 0 ? Console.ReadLine() : string.Join(" ", args);
            //    if (Command.Length > 0)
            //    {
            //        try
            //        {
            //            var CommandResults = GrepEngine.ProcessCommand(Command);
            //        }
            //        catch (Exception ex)
            //        {
            //            Console.WriteLine(ex.Message + Environment.NewLine);
            //        }
            //    }
            //}
            //while (args.Length == 0);
        }
        #endregion MainWindowViewModel
        #endregion Constructors..

        #region Methods..
        #region HwndHook
        /// <summary>
        /// Provides application shortcut functions for shortcut events from Windows
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        public IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:

                    // Ctrl C
                    if (Shortcut.CtrlC.GetCustomAttribute<KeyIdAttribute>().Value == wParam.ToInt32())
                    {
                        int virtualKeyParam = (((int)lParam >> 16) & 0xFFFF);
                        uint keyCode = Convert.ToUInt32(KeyInterop.VirtualKeyFromKey(Enum.Parse<Key>(Shortcut.CtrlC.GetCustomAttribute<KeyAttribute>().Value.ToString())).ToString("X"), 16);

                        if (virtualKeyParam == keyCode)
                        {
                            // Do Something
                        }
                    }

                    handled = true;
                    break;
            }

            return IntPtr.Zero;
        }
        #endregion HwndHook

        #region RegisterShortcuts
        public void RegisterShortcuts(IntPtr windowHandle)
        {
            EnumUtils.GetValues<Shortcut>().ToList().ForEach(shortcut =>
            {
                int keyId = shortcut.GetCustomAttribute<KeyIdAttribute>().Value;
                var keyModifer = shortcut.GetCustomAttribute<KeyModifierAttribute>()?.Value ?? KeyModifier.None;
                int keyCode = (int)shortcut.GetCustomAttribute<KeyAttribute>().Value;
                uint virtualKeyCode = Convert.ToUInt32(KeyInterop.VirtualKeyFromKey(Enum.Parse<Key>(keyCode.ToString())).ToString("X"), 16);

                WindowsApi.RegisterHotKey(windowHandle, virtualKeyCode, keyId, keyModifer);
            });

            // Hooks
            _hwndSource = HwndSource.FromHwnd(windowHandle);
            _hwndSource.AddHook(HwndHook);
        }
        #endregion RegisterShortcuts

        #region ReleaseShortcuts
        public void ReleaseShortcuts(IntPtr windowHandle)
        {
            EnumUtils.GetValues<Shortcut>().ToList().ForEach(shortcut =>
            {
                int keyId = shortcut.GetCustomAttribute<KeyIdAttribute>().Value;
                WindowsApi.UnregisterHotkey(windowHandle, keyId);
            });

            // Hooks
            _hwndSource?.RemoveHook(HwndHook);
            _hwndSource?.Dispose();
        }
        #endregion ReleaseShortcuts
        #endregion Methods..
    }
}
