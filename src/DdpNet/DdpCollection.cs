namespace DdpNet
{
    using System.Collections;
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

    public class DdpCollection<T> : IEnumerable<T>, INotifyCollectionChanged, INotifyPropertyChanged, IDdpCollection where T: DdpObject
    {
        public int Count { get { return this.internalList.Count; } }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        internal string CollectionName { get; private set; }
        private DdpClient client;

        private SynchronizationContext synchronizationContext;
        private ObjectChanger changer = new ObjectChanger();

        private List<T> internalList; 

        internal DdpCollection(DdpClient client, string collectionName)
        {
            this.CollectionName = collectionName;
            this.client = client;
            this.synchronizationContext = SynchronizationContext.Current;
            this.internalList = new List<T>();
        }

        public Task AddAsync(T item)
        {
            var methodName = string.Format(@"/{0}/insert", this.CollectionName);
            return this.client.Call(methodName, new List<object>() {item});
        }

        internal void AddInternal(T item)
        {
            this.internalList.Add(item);
        }

        void IDdpCollection.Added(string id, JObject jObject)
        {
            var deserializedObject = jObject.ToObject<T>();
            deserializedObject.ID = id;

            this.AddInternal(deserializedObject);
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, deserializedObject));
            this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
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
                this.internalList.Remove(objectToRemove);
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, objectToRemove));
                this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            }
        }

        protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (SynchronizationContext.Current == this.synchronizationContext)
            {
                // Execute the CollectionChanged event on the current thread
                RaiseCollectionChanged(e);
            }
            else
            {
                // Raises the CollectionChanged event on the creator thread
                this.synchronizationContext.Send(RaiseCollectionChanged, e);
            }
        }

        private void RaiseCollectionChanged(object param)
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, (NotifyCollectionChangedEventArgs) param);
            }
        }

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (SynchronizationContext.Current == this.synchronizationContext)
            {
                // Execute the PropertyChanged event on the current thread
                RaisePropertyChanged(e);
            }
            else
            {
                // Raises the PropertyChanged event on the creator thread
                this.synchronizationContext.Send(RaisePropertyChanged, e);
            }
        }

        private void RaisePropertyChanged(object param)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, (PropertyChangedEventArgs)param);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            var snapShot = this.internalList.ToList();

            return snapShot.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            var snapShot = this.internalList.ToList();

            return snapShot.GetEnumerator();
        }
    }
}
