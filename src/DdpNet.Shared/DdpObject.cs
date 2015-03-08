namespace DdpNet
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Newtonsoft.Json;

    public abstract class DdpObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private SynchronizationContext SynchronizationContext { get; set; }

        [JsonProperty(PropertyName = "_id")]
        public string ID { get; internal set; }

        [JsonIgnore] 
        internal bool SerializeId { get; set; }

        protected DdpObject()
        {
            this.SerializeId = true;
        }

        internal void OnAdded(string id, SynchronizationContext synchronizationContext)
        {
            Exceptions.ThrowIfNullOrWhitespace(id, "id");
            Exceptions.ThrowIfNull(synchronizationContext, "synchronizationContext");

            this.ID = id;
            this.SynchronizationContext = synchronizationContext;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var args = new PropertyChangedEventArgs(propertyName);
            if (SynchronizationContext.Current == this.SynchronizationContext)
            {
                RaisePropertyChanged(args);
            }
            else
            {
                this.SynchronizationContext.Post(this.RaisePropertyChanged, args);
            }
        }

        private void RaisePropertyChanged(object param)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, (PropertyChangedEventArgs)param);
        }

        public bool ShouldSerializeID()
        {
            if (this.SerializeId == false)
            {
                return false;
            }

            return !string.IsNullOrWhiteSpace(this.ID);
        }
    }
}
