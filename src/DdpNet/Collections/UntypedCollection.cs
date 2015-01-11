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

        public UntypedCollection(string collectionName)
        {
            this.CollectionName = collectionName;
            this.objects = new Dictionary<string, JObject>();
            this.changer = new ObjectChanger();
        }

        public void Add(string id, JObject value)
        {
            this.objects.Add(id, value);
        }

        public void Change(string id, Dictionary<string, JToken> fields, string[] cleared)
        {
            var objectToChange = this.objects[id];

            if (objectToChange == null)
            {
                return;
            }

            this.changer.ChangeObject(objectToChange, fields, cleared);
        }

        public void Remove(string id)
        {
            this.objects.Remove(id);
        }
    }
}
