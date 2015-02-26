namespace DdpNet.Collections
{
    using System;
    using System.Collections.Generic;
    using Messages;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Manages all the collections the client knows about.
    /// This class can be called from two different threads; the receive thread will call Added, Changed and Removed when it receives the respective message from the server.
    /// The GetCollection method can be called from the user thread, to get an instance of a collection
    /// 
    /// Some notes about the class:
    /// The typed DdpCollection is only ever created on the user thread, when they call GetCollection
    /// The UntypedCollection can be modified on either the user or the receive thread though, so there is a potential for conflicts
    /// All modifications to the UntypedCollection utilize double checked locking to ensure only one thread can update it at a time.
    /// This also ensures that when the collection is converted to a typed collection, the write operations will be going to the correct collection
    /// </summary>
    internal class CollectionManager : ICollectionManager
    {
        /// <summary>
        /// The untyped collections being managed
        /// </summary>
        private readonly Dictionary<string, UntypedCollection> untypedCollections;

        /// <summary>
        /// The typed collections being managed
        /// </summary>
        private readonly Dictionary<string, IDdpCollection> typedCollections;

        /// <summary>
        /// The DdpClient used to communicate with the server
        /// </summary>
        private IDdpRemoteMethodCall client;

        /// <summary>
        /// Sync object to lock around critical sections
        /// </summary>
        private object collectionsSyncObject = new object();

        /// <summary>
        /// Creates a new CollectionManager
        /// </summary>
        /// <param name="client">The DdpClient to use</param>
        public CollectionManager(IDdpRemoteMethodCall client)
        {
            this.typedCollections = new Dictionary<string, IDdpCollection>();
            this.untypedCollections = new Dictionary<string, UntypedCollection>();
            this.client = client;
        }

        /// <summary>
        /// Adds an object to a collection
        /// If the collection does not yet exist, an UntypedCollection will be created to store the object in
        /// </summary>
        /// <param name="message">The Added message to process</param>
        public void Added(Added message)
        {
            IDdpCollection collection;

            if (this.typedCollections.TryGetValue(message.Collection, out collection))
            {
                collection.Added(message.ID, message.Fields);
            }
            else
            {
                lock (this.collectionsSyncObject)
                {
                    // Its possible we got the lock after the untyped collection was converted to a typed collection,
                    // so we need to check the typed collections again
                    if (this.typedCollections.TryGetValue(message.Collection, out collection))
                    {
                        collection.Added(message.ID, message.Fields);
                    }
                    else
                    {
                        UntypedCollection untypedCollection;
                        if (!this.untypedCollections.TryGetValue(message.Collection, out untypedCollection))
                        {
                            untypedCollection = new UntypedCollection(message.Collection);
                            this.untypedCollections.Add(message.Collection, untypedCollection);
                        }

                        untypedCollection.Added(message.ID, message.Fields);
                    }
                }
            }
        }

        /// <summary>
        /// Changes an object in a collection
        /// If the collection does not exist, will throw InvalidOperationException
        /// </summary>
        /// <param name="message">The Changed message to process</param>
        public void Changed(Changed message)
        {
            IDdpCollection collection;

            if (this.typedCollections.TryGetValue(message.Collection, out collection))
            {
                collection.Changed(message.ID, message.Fields, message.Cleared);
            }
            else
            {
                lock (this.collectionsSyncObject)
                {
                    // Its possible we got the lock after the untyped collection was converted to a typed collection,
                    // so we need to check the typed collections again
                    if (this.typedCollections.TryGetValue(message.Collection, out collection))
                    {
                        collection.Changed(message.ID, message.Fields, message.Cleared);
                    }
                    else
                    {
                        UntypedCollection untypedCollection;
                        if (!this.untypedCollections.TryGetValue(message.Collection, out untypedCollection))
                        {
                            throw new InvalidOperationException("Collection to change was not present");
                        }

                        untypedCollection.Changed(message.ID, message.Fields, message.Cleared);
                    }
                }
            }
        }

        /// <summary>
        /// Removes an object from a collection
        /// If the collection does not exist, throws an InvalidOperationException
        /// </summary>
        /// <param name="message">The Removed message to process</param>
        public void Removed(Removed message)
        {
            IDdpCollection collection;

            if (this.typedCollections.TryGetValue(message.Collection, out collection))
            {
                collection.Removed(message.ID);
            }
            else
            {
                lock (this.collectionsSyncObject)
                {
                    // Its possible we got the lock after the untyped collection was converted to a typed collection,
                    // so we need to check the typed collections again
                    if (this.typedCollections.TryGetValue(message.Collection, out collection))
                    {
                        collection.Removed(message.ID);
                    }
                    else
                    {
                        UntypedCollection untypedCollection;
                        if (!this.untypedCollections.TryGetValue(message.Collection, out untypedCollection))
                        {
                            throw new InvalidOperationException("Collection to change was not present");
                        }

                        untypedCollection.Removed(message.ID);
                    }
                }
            }            
        }

        /// <summary>
        /// Gets a typed instance of a collection.
        /// Only one instance is kept of each collection, so repeated calls to this with the same parameters will return the same object
        /// If it is called again with the same collectionName, but a different type, an error will be thrown
        /// If the collection does not exist, a new one will be created
        /// If it exists, but is untyped, it will be converted to a typed collection
        /// If a typed collection exists, it will be returned directly
        /// </summary>
        /// <typeparam name="T">The type of the collection</typeparam>
        /// <param name="collectionName">The name of the collection</param>
        /// <returns>A DdpCollection</returns>
        public DdpCollection<T> GetCollection<T>(string collectionName) where T: DdpObject
        {
            // Prevent any UntypedCollections from being modfified, in case we need to convert them to a typed collection
            lock (this.collectionsSyncObject)
            {
                IDdpCollection collection;
                if (!this.typedCollections.TryGetValue(collectionName, out collection))
                {
                    UntypedCollection untypedCollection;
                    if (!this.untypedCollections.TryGetValue(collectionName, out untypedCollection))
                    {
                        collection = new DdpCollection<T>(this.client, collectionName);
                        this.typedCollections.Add(collectionName, collection);
                    }
                    else
                    {
                        var convertedCollection = this.ConvertToTypedCollection<T>(collectionName, untypedCollection);
                        collection = convertedCollection;
                    }
                }

                if (!(collection is DdpCollection<T>))
                {
                    throw new InvalidCollectionTypeException(collectionName, collection.GetType(), typeof(DdpCollection<T>));
                }
                var typedCollection = (DdpCollection<T>) collection;

                return typedCollection;
            }
        }

        /// <summary>
        /// Converts the given untyped collection to a typed collection, and updates the respective properties
        /// </summary>
        /// <typeparam name="T">The type of the new typed collection</typeparam>
        /// <param name="collectionName">The name of the collection</param>
        /// <param name="collection">The untyped collection to convert</param>
        /// <returns>The new typed collection</returns>
        private DdpCollection<T> ConvertToTypedCollection<T>(string collectionName, UntypedCollection collection) where T: DdpObject
        {
            var typedCollection = new DdpCollection<T>(this.client, collection.CollectionName);

            var ddpCollection = (IDdpCollection) typedCollection;

            foreach (var item in collection.Objects)
            {
                ddpCollection.Added(item.Key, item.Value);
            }

            this.typedCollections.Add(collectionName, typedCollection);
            this.untypedCollections.Remove(collectionName);

            return typedCollection;
        }
    }
}
