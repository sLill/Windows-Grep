using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WindowsGrep.Common
{
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        #region Event Handlers..
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Event Handlers..

        #region Methods..
        #region RaisePropertyChanged
        protected void RaisePropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion RaisePropertyChanged 
        #endregion Methods..
    }
}
