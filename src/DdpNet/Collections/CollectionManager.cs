namespace DdpNet.Collections
{
    using System;
    using System.Collections.Generic;
    using Messages;
    using Newtonsoft.Json.Linq;

    internal class CollectionManager
    {
        private readonly Dictionary<string, IDdpCollection> collections;

        private DdpClient client;

        public CollectionManager(DdpClient client)
        {
            this.collections = new Dictionary<string, IDdpCollection>();
            this.client = client;
        }

        internal void Added(Added message)
        {
            if (!this.collections.ContainsKey(message.Collection))
            {
                this.collections.Add(message.Collection, new UntypedCollection(message.Collection));
            }

            try
            {
                this.collections[message.Collection].Added(message.ID, message.Fields);
            }
            catch (InactiveCollectionException)
            {
                this.collections[message.Collection].Added(message.ID, message.Fields);
            }
            
        }

        internal void Changed(Changed message)
        {
            if (!this.collections.ContainsKey(message.Collection))
            {
                throw new InvalidOperationException("Object to change was not present");
            }

            try
            {
                this.collections[message.Collection].Changed(message.ID, message.Fields, message.Cleared);
            }
            catch (InactiveCollectionException)
            {
                this.collections[message.Collection].Changed(message.ID, message.Fields, message.Cleared);
            }
            
        }

        internal void Removed(Removed message)
        {
            if (!this.collections.ContainsKey(message.Collection))
            {
                throw new InvalidOperationException("Object to remove was not present");
            }

            try
            {
                this.collections[message.Collection].Removed(message.ID);
            }
            catch (InactiveCollectionException)
            {
                this.collections[message.Collection].Removed(message.ID);
            }
            
        }

        public DdpCollection<T> GetCollection<T>(string collectionName) where T: DdpObject
        {
            IDdpCollection collection;

            if (!this.collections.TryGetValue(collectionName, out collection))
            {
                collection = new DdpCollection<T>(this.client, collectionName);
                this.collections.Add(collectionName, collection);
            }
            else if (collection is UntypedCollection)
            {
                var convertedCollection = this.ConvertToTypedCollection<T>(collectionName, (UntypedCollection) collection);
                collection = convertedCollection;
            }

            var typedCollection = (DdpCollection<T>) collection;

            return typedCollection;
        }

        private DdpCollection<T> ConvertToTypedCollection<T>(string collectionName, UntypedCollection collection) where T: DdpObject
        {
            lock (collection.syncObject)
            {
                var typedCollection = new DdpCollection<T>(this.client, collection.CollectionName);
                this.collections[collectionName] = typedCollection;

                var ddpCollection = (IDdpCollection) typedCollection;

                foreach (var item in collection.Objects)
                {
                    ddpCollection.Added(item.Key, item.Value);
                }

                return typedCollection;
            }
        }
    }
}
