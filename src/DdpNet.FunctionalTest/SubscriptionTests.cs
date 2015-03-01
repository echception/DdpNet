namespace DdpNet.FunctionalTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Remoting;
    using System.Threading.Tasks;
    using DataObjects;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SubscriptionTests
    {
        [TestMethod]
        [TestCategory("Functional")]
        public async Task SubscriptionTests_Add_ObjectReadable()
        {
            var meteorClient = TestEnvironment.GetClient();
            await meteorClient.ConnectAsync();

            var entryCollection = meteorClient.GetCollection<Entry>("entries");
            await meteorClient.Subscribe("entries");

            var startingCount = entryCollection.Count;

            var firstEntry = new Entry() {Count = 12, IsActive = false, Name = "FirstEntry"};

            await entryCollection.AddAsync(firstEntry);

            Assert.AreEqual(startingCount + 1, entryCollection.Count);

            var entry = entryCollection.Single(x => x.ID == firstEntry.ID);

            Entry.AssertAreEqual(firstEntry, entry);
        }

        [TestMethod]
        [TestCategory("Functional")]
        public async Task SubscriptionTests_Remove_ValidObject()
        {
            var meteorClient = TestEnvironment.GetClient();
            await meteorClient.ConnectAsync();

            var entryCollection = meteorClient.GetCollection<Entry>("entries");
            await meteorClient.Subscribe("entries");

            int startingCount = entryCollection.Count;

            var firstEntry = new Entry() { Count = 12, IsActive = false, Name = "FirstEntry" };

            await entryCollection.AddAsync(firstEntry);

            Assert.AreEqual(startingCount + 1, entryCollection.Count);

            var removed = await entryCollection.RemoveAsync(firstEntry.ID);

            Assert.IsTrue(removed);

            Assert.AreEqual(startingCount, entryCollection.Count);
        }

        [TestMethod]
        [TestCategory("Functional")]
        public async Task SubscriptionTests_Update_ValidObject()
        {
            var meteorClient = TestEnvironment.GetClient();
            await meteorClient.ConnectAsync();

            var entryCollection = meteorClient.GetCollection<Entry>("entries");
            await meteorClient.Subscribe("entries");

            var entry = new Entry() {Count = 101, IsActive = true, Name = "Entry Name"};

            await entryCollection.AddAsync(entry);

            var update = new Entry() {Count = 102, IsActive = false, Name = "New Name"};

            bool wasUpdated = await entryCollection.UpdateAsync(entry.ID, update);

            Assert.IsTrue(wasUpdated);

            var updatedEntry = entryCollection.Single(x => x.ID == entry.ID);

            Entry.AssertAreEqual(update, updatedEntry);
        }

        [TestMethod]
        [TestCategory("Functional")]
        public async Task SubscriptionTests_Added_RaisesCollectionChangedEvent()
        {
            var meteorClient = TestEnvironment.GetClient();
            await meteorClient.ConnectAsync();

            var entryCollection = meteorClient.GetCollection<Entry>("entries");
            await meteorClient.Subscribe("entries");

            var entry = new Entry() { Count = 101, IsActive = true, Name = "Entry Name" };

            bool collectionChangedCalled = false;
            bool propertyChanged = false;
            entryCollection.CollectionChanged += (sender, args) => collectionChangedCalled = true;
            entryCollection.PropertyChanged += (sender, args) =>
            {
                Assert.AreEqual("Count", args.PropertyName);
                propertyChanged = true;
            };

            await entryCollection.AddAsync(entry);

            Assert.IsTrue(collectionChangedCalled);
            Assert.IsTrue(propertyChanged);
        }

        [TestMethod]
        [TestCategory("Functional")]
        public async Task SubscriptionTests_AddRemove_LargeNumberOfObjects()
        {
            var meteorClient = TestEnvironment.GetClient();
            await meteorClient.ConnectAsync();

            var entryCollection = meteorClient.GetCollection<Entry>("entries");
            await meteorClient.Subscribe("entries");

            var inserts = 10;
            var currentCount = entryCollection.Count;

            List<Task> startedTasks = new List<Task>();

            for (int i = 0; i < inserts; i++)
            {
                var entry = new Entry()
                {
                    Count = i,
                    IsActive = true,
                    Name = "Item " + i.ToString()
                };

                startedTasks.Add(entryCollection.AddAsync(entry));
            }

            Task.WaitAll(startedTasks.ToArray());

            var newCount = entryCollection.Count;

            Assert.AreEqual(currentCount + inserts, newCount);
            startedTasks.Clear();

            foreach (var entry in entryCollection)
            {
                startedTasks.Add(entryCollection.RemoveAsync(entry.ID));
            }

            Task.WaitAll(startedTasks.ToArray());

            Assert.AreEqual(0, entryCollection.Count);
        }

        [TestMethod]
        [TestCategory("Functional")]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task SubscriptionTests_Subscribe_SubscriptionNotExist()
        {
            var meteorClient = TestEnvironment.GetClient();
            await meteorClient.ConnectAsync();

            await meteorClient.Subscribe("foobar");
        }

        [TestMethod]
        [TestCategory("Functional")]
        public async Task SubscriptionTests_Update_ItemNotExist()
        {
            var meteorClient = TestEnvironment.GetClient();
            await meteorClient.ConnectAsync();

            var collection = meteorClient.GetCollection<Entry>("entries");

            await meteorClient.Subscribe("entries");

            bool wasUpdated = await collection.UpdateAsync("NOT_EXIST", new Entry() {Count = 10});

            Assert.IsFalse(wasUpdated);
        }

        [TestMethod]
        [TestCategory("Functional")]
        public async Task SubscriptionTests_Remove_ItemNotExist()
        {
            var meteorClient = TestEnvironment.GetClient();
            await meteorClient.ConnectAsync();

            var collection = meteorClient.GetCollection<Entry>("entries");

            await meteorClient.Subscribe("entries");

            bool wasRemoved = await collection.RemoveAsync("ID_NOT_EXIST");

            Assert.IsFalse(wasRemoved);
        }

        [TestMethod]
        [TestCategory("Functional")]
        public async Task SubscriptionTests_SubscribeWithParameters_LimitedCollection()
        {
            var meteorClient = TestEnvironment.GetClient();
            await meteorClient.ConnectAsync();

            var collection = meteorClient.GetCollection<Entry>("entries");

            for (int i = 0; i < 10; i++)
            {
                await collection.AddAsync(new Entry() {Count = 5, IsActive = true, Name = "TestName"});
            }

            var entryToFilter = new Entry() {Count = 10001, IsActive = false, Name = "FILTER_ON_NAME_ITEM"};

            await collection.AddAsync(entryToFilter);

            await meteorClient.Subscribe("entriesByName", entryToFilter.Name);

            Assert.AreEqual(1, collection.Count);

            var entry = collection.First();

            Entry.AssertAreEqual(entryToFilter, entry);
        }
    }
}
