namespace DdpNet.UnitTest.Collections
{
    using System.Collections.Generic;
    using DdpNet.Collections;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json.Linq;
    using ObjectChangerTestObjects;

    [TestClass]
    public class UntypedCollectionTests
    {
        private const string collName = "TestCollection";

        [TestMethod]
        public void UntypedCollection_Constructor_CollectionNameSet()
        {
            var collection = new UntypedCollection(collName);

            Assert.AreEqual(collName, collection.CollectionName);
        }

        [TestMethod]
        public void UntypedCollection_Constructor_CollectionStartesActive()
        {
            var collection = new UntypedCollection(collName);

            collection.ThrowIfInactive();
        }

        [TestMethod]
        [ExpectedException(typeof(InactiveCollectionException))]
        public void UntypedCollection_ThrowIfInactive_Throws()
        {
            var collection = new UntypedCollection(collName);

            collection.SetInactive();

            collection.ThrowIfInactive();
        }

        [TestMethod]
        [ExpectedException(typeof (InactiveCollectionException))]
        public void UntypedCollection_Added_ThrowsWhenInactive()
        {
            var collection = new UntypedCollection(collName);

            collection.SetInactive();

            collection.Added("1", JObject.FromObject(new SimpleField()));
        }

        [TestMethod]
        [ExpectedException(typeof(InactiveCollectionException))]
        public void UntypedCollection_Changed_ThrowsWhenInactive()
        {
            var collection = new UntypedCollection(collName);

            collection.SetInactive();

            collection.Changed("1", null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(InactiveCollectionException))]
        public void UntypedCollection_Removed_ThrowsWhenInactive()
        {
            var collection = new UntypedCollection(collName);

            collection.SetInactive();

            collection.Removed("1");
        }

        [TestMethod]
        public void UntypedCollection_Added_AddsValue()
        {
            var objectToAdd = new SimpleField() {integerField = 10};

            var collection = new UntypedCollection(collName);

            collection.Added("1", JObject.FromObject(objectToAdd));

            Assert.AreEqual(1, collection.Objects.Count);

            var objectAdded = collection.Objects["1"].ToObject<SimpleField>();

            Assert.AreEqual(10, objectAdded.integerField);
        }

        [TestMethod]
        public void UntypedCollection_Changed_ChangesValue()
        {
            var objectToAdd = new SimpleField() { integerField = 10 };

            var collection = new UntypedCollection(collName);

            collection.Added("1", JObject.FromObject(objectToAdd));

            var change = new Dictionary<string, JToken>()
            {
                {"integerField", JToken.FromObject(999)}
            };

            collection.Changed("1", change, null);

            var objectAdded = collection.Objects["1"].ToObject<SimpleField>();

            Assert.AreEqual(999, objectAdded.integerField);
        }

        [TestMethod]
        public void UntypedCollection_Changed_InvalidObjectDoesNothing()
        {
            var objectToAdd = new SimpleField() { integerField = 10 };

            var collection = new UntypedCollection(collName);

            collection.Added("1", JObject.FromObject(objectToAdd));

            var change = new Dictionary<string, JToken>()
            {
                {"integerField", JToken.FromObject(999)}
            };

            collection.Changed("2", change, null);

            var objectAdded = collection.Objects["1"].ToObject<SimpleField>();

            Assert.AreEqual(10, objectAdded.integerField);
        }

        [TestMethod]
        public void UntypedCollection_Removed_RemovesObject()
        {
            var objectToAdd = new SimpleField() { integerField = 10 };

            var collection = new UntypedCollection(collName);

            collection.Added("1", JObject.FromObject(objectToAdd));

            collection.Removed("1");

            Assert.AreEqual(0, collection.Objects.Count);
        }

        [TestMethod]
        public void UntypedCollection_Removed_InvalidObjectDoesNothing()
        {
            var objectToAdd = new SimpleField() { integerField = 10 };

            var collection = new UntypedCollection(collName);

            collection.Added("1", JObject.FromObject(objectToAdd));

            collection.Removed("2");

            Assert.AreEqual(1, collection.Objects.Count);
        }
    }
}
