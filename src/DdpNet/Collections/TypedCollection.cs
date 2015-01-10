namespace DdpNet.Collections
{
    using System.Collections.Generic;
    using Newtonsoft.Json.Linq;

    internal class TypedCollection<T> : IDdpCollection where T: DdpObject
    {
        public string CollectionName { get; private set; }

        private List<T> objects;

        private Dictionary<string, DdpSubscription<T>> subscriptions; 

        public TypedCollection(string collectionName)
        {
            this.CollectionName = collectionName;
            this.objects = new List<T>();
            this.subscriptions = new Dictionary<string, DdpSubscription<T>>();
        }

        public void Add(string id, JObject jObject)
        {
            var deserializedObject = jObject.ToObject<T>();
            deserializedObject.ID = id;

            this.objects.Add(deserializedObject);

            foreach (var subscription in this.subscriptions)
            {
                subscription.Value.Add(deserializedObject);
            }
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

            foreach (var ddpObject in this.objects)
            {
                newSubscription.Add(ddpObject);
            }

            this.subscriptions.Add(subscriptionName, newSubscription);
        }
    }
}
