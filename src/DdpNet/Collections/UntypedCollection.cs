namespace DdpNet.Collections
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Prior to the user calling GetCollection<T>, we don't know the type of a collection
    /// The server can send objects we need to know about, so this class stores data we have received from the server
    /// that does not yet have a strong type. Some assumptions:
    /// 
    /// * This class is never read. There are no members exposed to read the collection data. When exposed to the user,
    /// it will first be converted to a typed collection.
    /// * Added, Changed, and Removed are only called from one thread (the receive thread)
    /// However, the user thread can call GetCollection, which causes this to be converted to a DdpCollection
    /// There can be problems if the receive thread is trying to modify the collection at the same time the user calls GetCollection
    /// To work around this problem, there is a syncObject that is locked before all modification operations, and before converting to a typed collection
    /// After the collection is converted to a typed collection, the UntypedCollection is set to inactive, as there may be a thread waiting on the lock
    /// All the modification operations check this flag prior to modification. If the collection is inactive, it will throw an exception. 
    /// The CollectionManager knows the exception means the UntypedCollection has been typed, and so will retry on the typed collection
    /// </summary>
    internal class UntypedCollection : IDdpCollection
    {
        /// <summary>
        /// The name of the collection
        /// </summary>
        public string CollectionName { get; private set; }

        /// <summary>
        /// The currently stored objects
        /// </summary>
        private Dictionary<string, JObject> objects;

        /// <summary>
        /// Read only view of the contents. Should not lock on syncObject, as this is called when converting to the typed collection,
        /// which already has a lock on the object.
        /// </summary>
        internal ReadOnlyDictionary<string, JObject> Objects { get {  return new ReadOnlyDictionary<string, JObject>(this.objects);} }

        /// <summary>
        /// Responsible for applying changes to an object
        /// </summary>
        private UntypedObjectChanger changer;

        /// <summary>
        /// See the class summary. Sync object for locking
        /// </summary>
        internal readonly object syncObject = new object();

        /// <summary>
        /// True if the collection is active for the CollectionName, false if it is not
        /// </summary>
        private bool active;

        /// <summary>
        /// Creates an instance of UntypedCollection
        /// </summary>
        /// <param name="collectionName">The name of the collection</param>
        public UntypedCollection(string collectionName)
        {
            this.CollectionName = collectionName;
            this.objects = new Dictionary<string, JObject>();
            this.changer = new UntypedObjectChanger();
            this.active = true;
        }

        /// <summary>
        /// Called when an object was added to the collection
        /// </summary>
        /// <param name="id">The id of the object added</param>
        /// <param name="value">The object added</param>
        public void Added(string id, JObject value)
        {
            lock (this.syncObject)
            {
                this.ThrowIfInactive();
                this.objects.Add(id, value);
            }
        }

        /// <summary>
        /// Called when an object was changed.
        /// </summary>
        /// <param name="id">The ID of the object changed</param>
        /// <param name="fields">The fields that changed</param>
        /// <param name="cleared">The fields that were cleared</param>
        public void Changed(string id, Dictionary<string, JToken> fields, string[] cleared)
        {
            lock (this.syncObject)
            {
                this.ThrowIfInactive();
                JObject objectToChange;

                if (this.objects.TryGetValue(id, out objectToChange))
                {
                    this.changer.ChangeObject(objectToChange, fields, cleared);
                }
            }
        }

        /// <summary>
        /// Called when an object was removed from the collection
        /// </summary>
        /// <param name="id">The ID of the object removed</param>
        public void Removed(string id)
        {
            lock (this.syncObject)
            {
                this.ThrowIfInactive();
                this.objects.Remove(id);
            }
        }

        /// <summary>
        /// Throws an InactiveCollectionException if active=false
        /// </summary>
        internal void ThrowIfInactive()
        {
            if (!this.active)
            {
                throw new InactiveCollectionException();
            }
        }

        /// <summary>
        /// Marks the UntypedCollection as inactive. Should be called when converting the collection to a TypedCollection
        /// </summary>
        internal void SetInactive()
        {
            this.active = false;
        }
    }
}
