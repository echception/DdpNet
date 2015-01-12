namespace DdpNet
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Collections;
    using Messages;
    using Newtonsoft.Json.Linq;

    public class DdpCollection<T> : ObservableCollection<T>, IDdpCollection where T: DdpObject
    {
        internal string CollectionName { get; private set; }
        private DdpClient client;

        private SynchronizationContext _synchronizationContext = SynchronizationContext.Current;
        private ObjectChanger changer = new ObjectChanger();

        internal DdpCollection(DdpClient client, string collectionName)
        {
            this.CollectionName = collectionName;
            this.client = client;
        }

        public Task InsertAsync(T item)
        {
            var methodName = string.Format(@"/{0}/insert", this.CollectionName);
            return this.client.Call(methodName, new List<object>() {item});
        }

        protected override void InsertItem(int index, T item)
        {
            this.InsertAsync(item).Wait();
        }

        void IDdpCollection.Added(string id, JObject jObject)
        {
            var deserializedObject = jObject.ToObject<T>();
            deserializedObject.ID = id;

            this.Add(deserializedObject);
        }

        void IDdpCollection.Changed(string id, Dictionary<string, JToken> fields, string[] cleared)
        {
            var objectToChange = this.SingleOrDefault(x => x.ID == id);

            if (objectToChange == null)
            {
                return;
            }

            this.changer.ChangeObject(objectToChange, fields, cleared);
        }

        void IDdpCollection.Removed(string id)
        {
            var objectToRemove = this.SingleOrDefault(x => x.ID == id);

            if (objectToRemove != null)
            {
                this.Remove(objectToRemove);
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (SynchronizationContext.Current == _synchronizationContext)
            {
                // Execute the CollectionChanged event on the current thread
                RaiseCollectionChanged(e);
            }
            else
            {
                // Raises the CollectionChanged event on the creator thread
                _synchronizationContext.Send(RaiseCollectionChanged, e);
            }
        }

        private void RaiseCollectionChanged(object param)
        {
            // We are in the creator thread, call the base implementation directly
            base.OnCollectionChanged((NotifyCollectionChangedEventArgs)param);
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (SynchronizationContext.Current == _synchronizationContext)
            {
                // Execute the PropertyChanged event on the current thread
                RaisePropertyChanged(e);
            }
            else
            {
                // Raises the PropertyChanged event on the creator thread
                _synchronizationContext.Send(RaisePropertyChanged, e);
            }
        }

        private void RaisePropertyChanged(object param)
        {
            // We are in the creator thread, call the base implementation directly
            base.OnPropertyChanged((PropertyChangedEventArgs)param);
        }
    }
}
