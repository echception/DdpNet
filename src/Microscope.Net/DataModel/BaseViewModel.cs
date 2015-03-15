namespace Microscope.Net.DataModel
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using DdpNet;

    public class BaseViewModel : INotifyPropertyChanged
    {
        private MeteorClient client;

        public BaseViewModel()
        {
            this.Client = App.Current.Client;
        }

        public MeteorClient Client
        {
            get { return client; }
            set
            {
                this.client = value;
                this.OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
