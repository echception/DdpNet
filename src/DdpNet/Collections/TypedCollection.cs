namespace DdpNet.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using Newtonsoft.Json.Linq;

    internal class TypedCollection<T> : IDdpCollection where T: DdpObject
    {
        public string CollectionName { get; private set; }

        private Dictionary<string, T> objects;

        private Dictionary<string, DdpSubscription<T>> subscriptions;

        private ObjectChanger changer;

        public TypedCollection(string collectionName)
        {
            this.CollectionName = collectionName;
            this.objects = new Dictionary<string, T>();
            this.subscriptions = new Dictionary<string, DdpSubscription<T>>();
            this.changer = new ObjectChanger();
        }

        public void Add(string id, JObject jObject)
        {
            var deserializedObject = jObject.ToObject<T>();
            deserializedObject.ID = id;
            
            this.objects.Add(id, deserializedObject);

            foreach (var subscription in this.subscriptions)
            {
                subscription.Value.Add(deserializedObject);
            }
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

        public DdpSubscription<T> GetSubscription(string subscriptionName)
        {
            if (!this.subscriptions.ContainsKey(subscriptionName))
            {
                this.CreateSubscription(subscriptionName);
            }

            return this.subscriptions[subscriptionName];
        }

        private void CreateSubscription(string subscriptionName)
        {
            var newSubscription = new DdpSubscription<T>();

            foreach (var ddpObject in this.objects.Values)
            {
                newSubscription.Add(ddpObject);
            }

            this.subscriptions.Add(subscriptionName, newSubscription);
        }
    }
}
