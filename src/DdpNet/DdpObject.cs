namespace DdpNet
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Annotations;

    public abstract class DdpObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string ID { get; internal set; }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
