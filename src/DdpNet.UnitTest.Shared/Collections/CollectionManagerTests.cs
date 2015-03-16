﻿namespace DdpNet.UnitTest.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using DdpNet.Collections;
    using Messages;
    using PCLTesting;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Newtonsoft.Json.Linq;
    using TestObjects;

    [TestClass]
    public class CollectionManagerTests
    {
        [TestMethod]
        public void CollectionManager_Added_PresentInTypedCollection()
        {
            var remoteMethodCall = new Mock<IDdpRemoteMethodCall>();

            var collectionManager = new CollectionManager(remoteMethodCall.Object);

            var objectToAdd = new TestDdpObject {integerField = 101, StringProperty = "addedTest"};

            collectionManager.Added(new Added {Collection = "Test", Fields = JObject.FromObject(objectToAdd), Id = "1"});

            var collection = collectionManager.GetCollection<TestDdpObject>("Test");

            Assert.AreEqual(1, collection.Count);

            objectToAdd.Id = "1";
            AssertDdpObjectsEqual(objectToAdd, collection.First());
        }

        [TestMethod]
        public void CollectionManager_Removed_AddedRemovedFromUntypedCollection()
        {
            var remoteMethodCall = new Mock<IDdpRemoteMethodCall>();

            var collectionManager = new CollectionManager(remoteMethodCall.Object);

            var objectToAdd = new TestDdpObject { integerField = 101, StringProperty = "addedTest"};

            collectionManager.Added(new Added { Collection = "Test", Fields = JObject.FromObject(objectToAdd), Id = "1" });
            collectionManager.Removed(new Removed() { Collection = "Test", Id="1"});

            var collection = collectionManager.GetCollection<TestDdpObject>("Test");

            Assert.AreEqual(0, collection.Count);
        }

        [TestMethod]
        public void CollectionManager_Changed_ChangeUntypedObject()
        {
            var remoteMethodCall = new Mock<IDdpRemoteMethodCall>();

            var collectionManager = new CollectionManager(remoteMethodCall.Object);

            var objectToAdd = new TestDdpObject { integerField = 101, StringProperty = "addedTest"};

            collectionManager.Added(new Added { Collection = "Test", Fields = JObject.FromObject(objectToAdd), Id = "1" });

            var changed = new Changed
            {
                Cleared = null,
                Collection = "Test",
                Fields = new Dictionary<string, JToken>() {{"StringProperty", JToken.FromObject("changed!")}},
                ID = "1"
            };

            collectionManager.Changed(changed);

            var collection = collectionManager.GetCollection<TestDdpObject>("Test");

            Assert.AreEqual(1, collection.Count);

            objectToAdd.StringProperty = "changed!";
            objectToAdd.Id = "1";
            AssertDdpObjectsEqual(objectToAdd, collection.First());
        }

        [TestMethod]
        public void CollectionManager_Added_MultipleCollections()
        {
            var remoteMethodCall = new Mock<IDdpRemoteMethodCall>();

            var collectionManager = new CollectionManager(remoteMethodCall.Object);

            var objectToAdd1 = new TestDdpObject { integerField = 101, StringProperty = "addedTest"};
            var objectToAdd2 = new TestDdpObject { integerField = 101, StringProperty = "addedTest"};
            var objectToAdd3 = new TestDdpObject { integerField = 101, StringProperty = "addedTest"};

            collectionManager.Added(new Added { Collection = "Test1", Fields = JObject.FromObject(objectToAdd1), Id = "1" });
            collectionManager.Added(new Added { Collection = "Test2", Fields = JObject.FromObject(objectToAdd2), Id = "2" });
            collectionManager.Added(new Added { Collection = "Test3", Fields = JObject.FromObject(objectToAdd3), Id = "3" });

            var collection1 = collectionManager.GetCollection<TestDdpObject>("Test1");
            var collection2 = collectionManager.GetCollection<TestDdpObject>("Test2");
            var collection3 = collectionManager.GetCollection<TestDdpObject>("Test3");

            Assert.AreEqual(1, collection1.Count);
            Assert.AreEqual(1, collection2.Count);
            Assert.AreEqual(1, collection3.Count);

            objectToAdd1.Id = "1";
            objectToAdd2.Id = "2";
            objectToAdd3.Id = "3";

            AssertDdpObjectsEqual(objectToAdd1, collection1.First());
            AssertDdpObjectsEqual(objectToAdd2, collection2.First());
            AssertDdpObjectsEqual(objectToAdd3, collection3.First());
        }

        [TestMethod]
        public void CollectionManager_GetCollection_RetypeCollection()
        {
            var remoteMethodCall = new Mock<IDdpRemoteMethodCall>();

            var collectionManager = new CollectionManager(remoteMethodCall.Object);

            var collection1 = collectionManager.GetCollection<TestDdpObject>("Test");

            ExceptionAssert.Throws<InvalidCollectionTypeException>(
                () => { var collection2 = collectionManager.GetCollection<SimpleDdpObject>("Test"); });
        }

        [TestMethod]
        public void CollectionManager_Added_ToTypedCollection()
        {
            var remoteMethodCall = new Mock<IDdpRemoteMethodCall>();

            var collectionManager = new CollectionManager(remoteMethodCall.Object);

            var collection = collectionManager.GetCollection<TestDdpObject>("Test");

            Assert.AreEqual(0, collection.Count);

            var objectToAdd = new TestDdpObject { integerField = 101, StringProperty = "addedTest" };

            collectionManager.Added(new Added { Collection = "Test", Fields = JObject.FromObject(objectToAdd), Id = "1" });

            Assert.AreEqual(1, collection.Count);

            objectToAdd.Id = "1";
            AssertDdpObjectsEqual(objectToAdd, collection.First());
        }

        [TestMethod]
        public void CollectionManager_Removed_AddedRemovedFromTypedCollection()
        {
            var remoteMethodCall = new Mock<IDdpRemoteMethodCall>();

            var collectionManager = new CollectionManager(remoteMethodCall.Object);

            var collection = collectionManager.GetCollection<TestDdpObject>("Test");

            var objectToAdd = new TestDdpObject { integerField = 101, StringProperty = "addedTest" };

            collectionManager.Added(new Added { Collection = "Test", Fields = JObject.FromObject(objectToAdd), Id = "1" });

            Assert.AreEqual(1, collection.Count);

            collectionManager.Removed(new Removed() { Collection = "Test", Id = "1" });

            Assert.AreEqual(0, collection.Count);
        }

        [TestMethod]
        public void CollectionManager_Changed_ChangeTypedObject()
        {
            var remoteMethodCall = new Mock<IDdpRemoteMethodCall>();

            var collectionManager = new CollectionManager(remoteMethodCall.Object);

            var collection = collectionManager.GetCollection<TestDdpObject>("Test");

            var objectToAdd = new TestDdpObject { integerField = 101, StringProperty = "addedTest" };

            collectionManager.Added(new Added { Collection = "Test", Fields = JObject.FromObject(objectToAdd), Id = "1" });

            Assert.AreEqual(1, collection.Count);

            var changed = new Changed
            {
                Cleared = null,
                Collection = "Test",
                Fields = new Dictionary<string, JToken>() { { "StringProperty", JToken.FromObject("changed!") } },
                ID = "1"
            };

            collectionManager.Changed(changed);

            Assert.AreEqual(1, collection.Count);

            objectToAdd.StringProperty = "changed!";
            objectToAdd.Id = "1";
            AssertDdpObjectsEqual(objectToAdd, collection.First());
        }

        [TestMethod]
        public void CollectionManager_Added_MultipleTypedCollections()
        {
            var remoteMethodCall = new Mock<IDdpRemoteMethodCall>();

            var collectionManager = new CollectionManager(remoteMethodCall.Object);

            var collection1 = collectionManager.GetCollection<TestDdpObject>("Test1");
            var collection2 = collectionManager.GetCollection<TestDdpObject>("Test2");
            var collection3 = collectionManager.GetCollection<TestDdpObject>("Test3");

            var objectToAdd1 = new TestDdpObject { integerField = 101, StringProperty = "addedTest" };
            var objectToAdd2 = new TestDdpObject { integerField = 101, StringProperty = "addedTest" };
            var objectToAdd3 = new TestDdpObject { integerField = 101, StringProperty = "addedTest" };

            collectionManager.Added(new Added { Collection = "Test1", Fields = JObject.FromObject(objectToAdd1), Id = "1" });
            collectionManager.Added(new Added { Collection = "Test2", Fields = JObject.FromObject(objectToAdd2), Id = "2" });
            collectionManager.Added(new Added { Collection = "Test3", Fields = JObject.FromObject(objectToAdd3), Id = "3" });

            Assert.AreEqual(1, collection1.Count);
            Assert.AreEqual(1, collection2.Count);
            Assert.AreEqual(1, collection3.Count);

            objectToAdd1.Id = "1";
            objectToAdd2.Id = "2";
            objectToAdd3.Id = "3";

            AssertDdpObjectsEqual(objectToAdd1, collection1.First());
            AssertDdpObjectsEqual(objectToAdd2, collection2.First());
            AssertDdpObjectsEqual(objectToAdd3, collection3.First());
        }

        [TestMethod]
        public void CollectionManager_Changed_NotExist()
        {
            var collectionManager = new CollectionManager(null);

            var changed = new Changed
            {
                Cleared = null,
                Collection = "Test",
                Fields = new Dictionary<string, JToken>() { { "StringProperty", JToken.FromObject("changed!") } },
                ID = "1"
            };

            ExceptionAssert.Throws<InvalidOperationException>(() => collectionManager.Changed(changed));
        }

        [TestMethod]
        public void CollectionManager_Removed_NotExist()
        {
            var collectionManager = new CollectionManager(null);

            ExceptionAssert.Throws<InvalidOperationException>(() => collectionManager.Removed(new Removed() { Collection = "Test", Id = "1" }));
        }

        private void AssertDdpObjectsEqual(TestDdpObject expected, TestDdpObject actual)
        {
            Assert.AreNotEqual(expected, actual);
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.StringProperty, actual.StringProperty);
            Assert.AreEqual(expected.integerField, actual.integerField);
        }
    }
}
