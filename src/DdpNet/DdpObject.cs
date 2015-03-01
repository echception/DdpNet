namespace DdpNet
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Annotations;
    using Newtonsoft.Json;

    public abstract class DdpObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [JsonProperty(PropertyName = "_id")]
        public string ID { get; internal set; }

        [JsonIgnore] 
        internal bool SerializeId { get; set; }

        protected DdpObject()
        {
            this.SerializeId = true;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
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
