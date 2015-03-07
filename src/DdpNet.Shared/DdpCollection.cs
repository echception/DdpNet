namespace DdpNet
{
    using System;
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
    using ParameterObjects;

    public class DdpCollection<T> : ReadOnlyObservableCollection<T>, IDdpCollection where T: DdpObject
    {
        internal string CollectionName { get; private set; }
        private IDdpRemoteMethodCall client;

        private SynchronizationContext synchronizationContext;
        private ObjectChanger changer = new ObjectChanger();

        private ObservableCollection<T> internalList; 

        internal DdpCollection(IDdpRemoteMethodCall client, string collectionName) : this(new ObservableCollection<T>())
        {
            Exceptions.ThrowIfNull(client, "client");
            Exceptions.ThrowIfNullOrWhitespace(collectionName, "collectionName");

            this.CollectionName = collectionName;
            this.client = client;
            this.synchronizationContext = SynchronizationContext.Current;
        }

        internal DdpCollection(ObservableCollection<T> internalList) : base(internalList)
        {
            this.internalList = internalList;
        }

        public Task AddAsync(T item)
        {
            Exceptions.ThrowIfNull(item, "item");

            if (string.IsNullOrWhiteSpace(item.ID))
            {
                item.ID = Utilities.GenerateID();
            }

            var methodName = string.Format(@"/{0}/insert", this.CollectionName);
            return this.client.Call(methodName, item);
        }

        public Task<bool> RemoveAsync(string id)
        {
            Exceptions.ThrowIfNullOrWhitespace(id, "id");

            var methodName = string.Format(@"/{0}/remove", this.CollectionName);
            return this.CallConvertNumberToBool(methodName, new IdParameter(id));
        }

        public Task<bool> UpdateAsync(string id, T item)
        {
            Exceptions.ThrowIfNull(item, "item");
            Exceptions.ThrowIfNullOrWhitespace(id, "id");

            var methodName = string.Format(@"/{0}/update", this.CollectionName);
            var selector = new IdParameter(id);
            var set = new Set(item);
            using (new ForceSerializeDdpObjectId(item, false))
            {
                return this.CallConvertNumberToBool(methodName, selector, set);
            }
        }

        void IDdpCollection.Added(string id, JObject jObject)
        {
            var deserializedObject = jObject.ToObject<T>();
            deserializedObject.ID = id;

            this.internalList.Add(deserializedObject);
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
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (SynchronizationContext.Current == this.synchronizationContext)
            {
                // Execute the CollectionChanged event on the current thread
                RaiseCollectionChanged(e);
            }
            else
            {
                // Raises the CollectionChanged event on the creator thread
                this.synchronizationContext.Post(RaiseCollectionChanged, e);
            }
        }

        private void RaiseCollectionChanged(object param)
        {
            base.OnCollectionChanged((NotifyCollectionChangedEventArgs) param);
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (SynchronizationContext.Current == this.synchronizationContext)
            {
                // Execute the PropertyChanged event on the current thread
                RaisePropertyChanged(e);
            }
            else
            {
                // Raises the PropertyChanged event on the creator thread
                this.synchronizationContext.Post(RaisePropertyChanged, e);
            }
        }

        private async Task<bool> CallConvertNumberToBool(string methodName, params object[] parameters)
        {
            int numberUpdated = await this.client.Call<int>(methodName, parameters);

            if (numberUpdated == 0)
            {
                return false;
            }
            else if (numberUpdated == 1)
            {
                return true;
            }

            throw new InvalidOperationException("Unexpected number of documents were updated");
        }

        private void RaisePropertyChanged(object param)
        {
            base.OnPropertyChanged((PropertyChangedEventArgs)param);
        }
    }
}
