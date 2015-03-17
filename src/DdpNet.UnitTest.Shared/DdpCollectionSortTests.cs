namespace DdpNet.UnitTest.Shared
{
    using System;
    using System.Collections.Generic;
    using Collections.TestObjects;
    using DdpNet.Collections;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Newtonsoft.Json.Linq;

    [TestClass]
    public class DdpCollectionSortTests
    {
        private const int Iterations = 1000;
        [TestMethod]
        public void DdpCollection_Sort_ManyInsertsSmallRange()
        {
            var collection = this.GetCollection();
            var sortedCollection = this.GetSortedCollection(collection);

            this.RunAddTests(collection, sortedCollection, Iterations, 0, 100);

            Assert.AreEqual(Iterations, collection.Count);
            Assert.AreEqual(Iterations, sortedCollection.Count);
            this.AssertCollectionSorted(sortedCollection);
        }

        [TestMethod]
        public void DdpCollection_Sort_ManyInsertsLargeRangeRange()
        {
            var collection = this.GetCollection();
            var sortedCollection = this.GetSortedCollection(collection);

            this.RunAddTests(collection, sortedCollection, Iterations, 0, 1000000);

            Assert.AreEqual(Iterations, collection.Count);
            Assert.AreEqual(Iterations, sortedCollection.Count);
            this.AssertCollectionSorted(sortedCollection);
        }

        [TestMethod]
        public void DdpCollection_Sort_ManyChangesSmallRange()
        {
            var collection = this.GetCollection();
            var sortedCollection = this.GetSortedCollection(collection);

            this.RunAddTests(collection, sortedCollection, Iterations, 0, 100);
            this.RunChangeTests(collection, sortedCollection, Iterations, 0, 100);

            Assert.AreEqual(Iterations, collection.Count);
            Assert.AreEqual(Iterations, sortedCollection.Count);
            this.AssertCollectionSorted(sortedCollection);
        }

        [TestMethod]
        public void DdpCollection_Sort_ManyChangesLargeRange()
        {
            var collection = this.GetCollection();
            var sortedCollection = this.GetSortedCollection(collection);

            this.RunAddTests(collection, sortedCollection, Iterations, 0, 100000);
            this.RunChangeTests(collection, sortedCollection, Iterations, 0, 100000);

            Assert.AreEqual(Iterations, collection.Count);
            Assert.AreEqual(Iterations, sortedCollection.Count);
            this.AssertCollectionSorted(sortedCollection);
        }

        [TestMethod]
        public void DdpCollection_Sort_ManyChangesSmallCollection()
        {
            var collection = this.GetCollection();
            var sortedCollection = this.GetSortedCollection(collection);

            this.RunAddTests(collection, sortedCollection, 5, 0, 10);
            this.RunChangeTests(collection, sortedCollection, Iterations, 0, 10);

            Assert.AreEqual(5, collection.Count);
            Assert.AreEqual(5, sortedCollection.Count);
            this.AssertCollectionSorted(sortedCollection);
        }

        private DdpCollection<TestDdpObject> GetCollection()
        {
            var remoteMethodCall = new Mock<IDdpRemoteMethodCall>();

            return new DdpCollection<TestDdpObject>(remoteMethodCall.Object, "TestCollection");
        }

        private DdpFilteredCollection<TestDdpObject> GetSortedCollection(DdpCollection<TestDdpObject> collection)
        {
            return collection.Filter(sortFilter: (x, y) => x.integerField - y.integerField);
        }

        private void RunAddTests(DdpCollection<TestDdpObject> collection, DdpFilteredCollection<TestDdpObject> sortedCollection, int numberOfAdds, int minRange, int maxRange)
        {
            Random random = new Random();
            for (int i = 0; i < numberOfAdds; i++)
            {
                var newValue = random.Next(minRange, maxRange);

                this.AddObject(collection, newValue);

                this.AssertCollectionSorted(sortedCollection);
            }
        }

        private void RunChangeTests(DdpCollection<TestDdpObject> collection, DdpFilteredCollection<TestDdpObject> sortedCollection, int numberOfChanges, int minRange,
            int maxRange)
        {
            Random random = new Random();
            for (int i = 0; i < numberOfChanges; i++)
            {
                var indexToChange = random.Next(0, collection.Count);
                var newValue = random.Next(minRange, maxRange);

                this.ChangeObject(collection, collection[indexToChange].Id, newValue);

                this.AssertCollectionSorted(sortedCollection);
            }
        }

        private void AddObject(DdpCollection<TestDdpObject> collection, int value)
        {
            var id = Utilities.GenerateId();

            ((IDdpCollection)collection).Added(id, JObject.FromObject(new TestDdpObject() { integerField = value}));
        }

        private void ChangeObject(DdpCollection<TestDdpObject> collection, string id, int newValue)
        {
            ((IDdpCollection)collection).Changed(id, new Dictionary<string, JToken>() { { "integerField", newValue } }, new string[0]);
        }

        private void AssertCollectionSorted(DdpFilteredCollection<TestDdpObject> collection)
        {
            for (int i = 0; i < collection.Count - 1; i++)
            {
                Assert.IsTrue(collection[i].integerField <= collection[i+1].integerField);
            }
        }
    }
}
