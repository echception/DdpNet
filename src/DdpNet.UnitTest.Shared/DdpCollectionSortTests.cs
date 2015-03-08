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
        [TestMethod]
        public void DdpCollection_Sort_ManyInsertsSmallRange()
        {
            var collection = this.GetSortedCollection();

            this.RunAddTests(collection, 10000, 0, 100);

            Assert.AreEqual(10000, collection.Count);
            this.AssertCollectionSorted(collection);
        }

        [TestMethod]
        public void DdpCollection_Sort_ManyInsertsLargeRangeRange()
        {
            var collection = this.GetSortedCollection();

            this.RunAddTests(collection, 10000, 0, 1000000);

            Assert.AreEqual(10000, collection.Count);
            this.AssertCollectionSorted(collection);
        }

        [TestMethod]
        public void DdpCollection_Sort_ManyChangesSmallRange()
        {
            var collection = this.GetSortedCollection();

            this.RunAddTests(collection, 10000, 0, 100);
            this.RunChangeTests(collection, 10000, 0, 100);

            Assert.AreEqual(10000, collection.Count);
            this.AssertCollectionSorted(collection);
        }

        [TestMethod]
        public void DdpCollection_Sort_ManyChangesLargeRange()
        {
            var collection = this.GetSortedCollection();

            this.RunAddTests(collection, 10000, 0, 100000);
            this.RunChangeTests(collection, 10000, 0, 100000);

            Assert.AreEqual(10000, collection.Count);
            this.AssertCollectionSorted(collection);
        }

        [TestMethod]
        public void DdpCollection_Sort_ManyChangesSmallCollection()
        {
            var collection = this.GetSortedCollection();

            this.RunAddTests(collection, 5, 0, 10);
            this.RunChangeTests(collection, 10000, 0, 10);

            Assert.AreEqual(5, collection.Count);
            this.AssertCollectionSorted(collection);
        }

        private DdpCollection<TestDdpObject> GetSortedCollection()
        {
            var remoteMethodCall = new Mock<IDdpRemoteMethodCall>();

            var collection = new DdpCollection<TestDdpObject>(remoteMethodCall.Object, "TestCollection");
            this.SortCollectionByIntegerField(collection);

            return collection;
        }

        private void SortCollectionByIntegerField(DdpCollection<TestDdpObject> collection)
        {
            collection.Sort((x, y) => x.integerField - y.integerField);
        }

        private void RunAddTests(DdpCollection<TestDdpObject> collection, int numberOfAdds, int minRange, int maxRange)
        {
            Random random = new Random();
            for (int i = 0; i < numberOfAdds; i++)
            {
                var newValue = random.Next(minRange, maxRange);

                this.AddObject(collection, newValue);

                this.AssertCollectionSorted(collection);
            }
        }

        private void RunChangeTests(DdpCollection<TestDdpObject> collection, int numberOfChanges, int minRange,
            int maxRange)
        {
            Random random = new Random();
            for (int i = 0; i < numberOfChanges; i++)
            {
                var indexToChange = random.Next(0, collection.Count);
                var newValue = random.Next(minRange, maxRange);

                this.ChangeObject(collection, collection[indexToChange].ID, newValue);

                this.AssertCollectionSorted(collection);
            }
        }

        private void AddObject(DdpCollection<TestDdpObject> collection, int value)
        {
            var id = Utilities.GenerateID();

            ((IDdpCollection)collection).Added(id, JObject.FromObject(new TestDdpObject() { integerField = value}));
        }

        private void ChangeObject(DdpCollection<TestDdpObject> collection, string id, int newValue)
        {
            ((IDdpCollection)collection).Changed(id, new Dictionary<string, JToken>() { { "integerField", newValue } }, new string[0]);
        }

        private void AssertCollectionSorted(DdpCollection<TestDdpObject> collection)
        {
            for (int i = 0; i < collection.Count - 1; i++)
            {
                Assert.IsTrue(collection[i].integerField <= collection[i+1].integerField);
            }
        }
    }
}
