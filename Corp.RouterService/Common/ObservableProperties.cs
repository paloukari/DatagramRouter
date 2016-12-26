using System;
using System.ComponentModel;

namespace Corp.RouterService.Common
{
    class ObservableProperty : INotifyPropertyChanged
    {
        internal void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
