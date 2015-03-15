namespace Microscope.Net.DataModel
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using DdpNet;

    public class BaseViewModel : INotifyPropertyChanged
    {
        public MeteorClient Client
        {
            get { return App.Current.Client; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
