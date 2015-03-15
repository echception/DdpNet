namespace DdpNet
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Collections;
    using Messages;
    using Newtonsoft.Json.Linq;
    using ParameterObjects;

    public class DdpCollection<T> : ReadOnlyObservableCollection<T>, IDdpCollection where T: DdpObject
    {
        public string CollectionName { get; private set; }

        private readonly IDdpRemoteMethodCall client;

        private readonly SynchronizationContext synchronizationContext;
        private readonly ObjectChanger changer = new ObjectChanger();

        private readonly ObservableCollection<T> internalList;

        private readonly List<DdpFilteredCollection<T>> filteredCollections;

        private readonly object filterLock = new object();

        internal DdpCollection(IDdpRemoteMethodCall client, string collectionName) : this(new ObservableCollection<T>())
        {
            Exceptions.ThrowIfNull(client, "client");
            Exceptions.ThrowIfNullOrWhitespace(collectionName, "collectionName");

            this.CollectionName = collectionName;
            this.client = client;
            this.synchronizationContext = SynchronizationContext.Current;
            this.filteredCollections = new List<DdpFilteredCollection<T>>();
        }

        private DdpCollection(ObservableCollection<T> internalList) : base(internalList)
        {
            this.internalList = internalList;
        }

        public Task AddAsync(T item)
        {
            Exceptions.ThrowIfNull(item, "item");

            if (string.IsNullOrWhiteSpace(item.Id))
            {
                item.Id = Utilities.GenerateID();
            }

            var methodName = string.Format(CultureInfo.InvariantCulture, @"/{0}/insert", this.CollectionName);
            return this.client.Call(methodName, item);
        }

        public Task<bool> RemoveAsync(string id)
        {
            Exceptions.ThrowIfNullOrWhitespace(id, "id");

            var methodName = string.Format(CultureInfo.InvariantCulture, @"/{0}/remove", this.CollectionName);
            return this.CallConvertNumberToBool(methodName, new IdParameter(id));
        }

        public Task<bool> UpdateAsync(string id, object fieldsToUpdate)
        {
            Exceptions.ThrowIfNull(fieldsToUpdate, "fieldsToUpdate");
            Exceptions.ThrowIfNullOrWhitespace(id, "id");

            var methodName = string.Format(CultureInfo.InvariantCulture, @"/{0}/update", this.CollectionName);
            var selector = new IdParameter(id);
            var set = new Set(fieldsToUpdate);

            return this.CallConvertNumberToBool(methodName, selector, set);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public DdpFilteredCollection<T> Filter(Func<T, bool> whereFilter = null, Comparison<T> sortFilter = null)
        {
            if (whereFilter == null && sortFilter == null)
            {
                throw new ArgumentException("whereFilter and sortFilter cannot both be null");
            }

            lock (this.filterLock)
            {
                var filteredCollection = new DdpFilteredCollection<T>(this.CollectionName, this.synchronizationContext,
                    whereFilter, sortFilter);

                this.filteredCollections.Add(filteredCollection);

                foreach (var item in this.internalList)
                {
                    filteredCollection.OnAdded(item);
                }

                return filteredCollection;
            }
        }

        void IDdpCollection.Added(string id, JObject jObject)
        {
            lock (this.filterLock)
            {
                var deserializedObject = jObject.ToObject<T>();
                deserializedObject.OnAdded(id, this.synchronizationContext);

                this.internalList.Add(deserializedObject);

                foreach (var filteredCollection in this.filteredCollections)
                {
                    filteredCollection.OnAdded(deserializedObject);
                }
            }
        }

        void IDdpCollection.Changed(string id, Dictionary<string, JToken> fields, string[] cleared)
        {
            lock (this.filterLock)
            {
                var objectToChange = this.internalList.SingleOrDefault(x => x.Id == id);

                if (objectToChange == null)
                {
                    return;
                }

                this.changer.ChangeObject(objectToChange, fields, cleared);

                foreach (var filteredCollection in this.filteredCollections)
                {
                    filteredCollection.OnChanged(objectToChange);
                }
            }
        }

        void IDdpCollection.Removed(string id)
        {
            lock (this.filterLock)
            {
                var objectToRemove = this.SingleOrDefault(x => x.Id == id);

                if (objectToRemove != null)
                {
                    this.internalList.Remove(objectToRemove);
                }

                foreach (var filteredCollection in this.filteredCollections)
                {
                    filteredCollection.OnRemoved(objectToRemove);
                }
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (SynchronizationContext.Current == this.synchronizationContext)
            {
                // Execute the CollectionChanged event on the current thread
                RaiseCollectionChanged(args);
            }
            else
            {
                // Raises the CollectionChanged event on the creator thread
                this.synchronizationContext.Post(RaiseCollectionChanged, args);
            }
        }

        private void RaiseCollectionChanged(object param)
        {
            base.OnCollectionChanged((NotifyCollectionChangedEventArgs) param);
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            if (SynchronizationContext.Current == this.synchronizationContext)
            {
                // Execute the PropertyChanged event on the current thread
                RaisePropertyChanged(args);
            }
            else
            {
                // Raises the PropertyChanged event on the creator thread
                this.synchronizationContext.Post(RaisePropertyChanged, args);
            }
        }

        private void RaisePropertyChanged(object param)
        {
            base.OnPropertyChanged((PropertyChangedEventArgs)param);
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
    }
}
