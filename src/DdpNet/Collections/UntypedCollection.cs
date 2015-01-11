using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DdpNet.Collections
{
    using System.Collections.ObjectModel;
    using Newtonsoft.Json.Linq;

    internal class UntypedCollection : IDdpCollection
    {
        public string CollectionName { get; private set; }

        private Dictionary<string, JObject> objects;

        internal ReadOnlyDictionary<string, JObject> Objects { get {  return new ReadOnlyDictionary<string, JObject>(this.objects);} }

        private ObjectChanger changer;

        internal readonly object syncObject = new object();

        private bool active;

        public UntypedCollection(string collectionName)
        {
            this.CollectionName = collectionName;
            this.objects = new Dictionary<string, JObject>();
            this.changer = new ObjectChanger();
            this.active = true;
        }

        public void Added(string id, JObject value)
        {
            lock (this.syncObject)
            {
                this.ThrowIfInactive();
                this.objects.Add(id, value);
            }
        }

        public void Changed(string id, Dictionary<string, JToken> fields, string[] cleared)
        {
            lock (this.syncObject)
            {
                this.ThrowIfInactive();
                var objectToChange = this.objects[id];

                if (objectToChange == null)
                {
                    return;
                }

                this.changer.ChangeObject(objectToChange, fields, cleared);
            }
        }

        public void Removed(string id)
        {
            lock (this.syncObject)
            {
                this.ThrowIfInactive();
                this.objects.Remove(id);
            }
        }

        internal void ThrowIfInactive()
        {
            if (!this.active)
            {
                throw new InactiveCollectionException();
            }
        }

        internal void SetInactive()
        {
            this.active = false;
        }
    }
}
