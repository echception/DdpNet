namespace DdpNet.UnitTest.Collections.ObjectChangerTestObjects
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Annotations;
    using Newtonsoft.Json;

    public class SimpleProperty : INotifyPropertyChanged
    {
        public string StringProperty { get; set; }

        public int IntegerProperty { get; set; }
        public float FloatProperty { get; set; }

        public bool BoolProperty { get; set; }

        [JsonProperty(PropertyName = "foobar")]
        public int ActualNameInJsonAttribute { get; set; }

        public bool PrivateSetProperty { get; private set; }

        private string readOnlyProperty;

        public string ReadOnlyProperty { get { return this.readOnlyProperty; } }

        private float propertyWithPropertyChanged;

        public float PropertyWithPropertyChanged
        {
            get { return this.propertyWithPropertyChanged; }
            set
            {
                this.propertyWithPropertyChanged = value;
                this.OnPropertyChanged("PropertyWithPropertyChanged");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public SimpleProperty(bool privateSetProperty, string readOnlyProperty, float propertyWithPropertyChanged)
        {
            this.PrivateSetProperty = privateSetProperty;
            this.readOnlyProperty = readOnlyProperty;
            this.propertyWithPropertyChanged = propertyWithPropertyChanged;
        }

        public SimpleProperty CreateCopy(bool? privateSetProperty = null, string readOnlyProperty = null, float? propertyWithPropertyChanged = null)
        {
            var privateSet = privateSetProperty.HasValue ? privateSetProperty.Value : this.PrivateSetProperty;
            var readOnly = readOnlyProperty != null ? readOnlyProperty : this.readOnlyProperty;
            var propertyChanged = propertyWithPropertyChanged.HasValue
                ? propertyWithPropertyChanged.Value
                : this.propertyWithPropertyChanged;

            return new SimpleProperty(privateSet, readOnly, propertyChanged)
            {
                StringProperty = this.StringProperty,
                IntegerProperty = this.IntegerProperty,
                FloatProperty = this.FloatProperty,
                BoolProperty = this.BoolProperty,
                ActualNameInJsonAttribute = this.ActualNameInJsonAttribute
            };
        }
    }
}
