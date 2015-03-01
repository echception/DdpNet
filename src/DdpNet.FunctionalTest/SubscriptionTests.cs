namespace DdpNet.FunctionalTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DataObjects;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SubscriptionTests
    {
        [TestMethod]
        [TestCategory("Functional")]
        public async Task SubscriptionTests_CreateReadDelete()
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
        public async Task SubscriptionTests_Remove()
        {
            var meteorClient = TestEnvironment.GetClient();
            await meteorClient.ConnectAsync();

            var entryCollection = meteorClient.GetCollection<Entry>("entries");
            await meteorClient.Subscribe("entries");

            int startingCount = entryCollection.Count;

            var firstEntry = new Entry() { Count = 12, IsActive = false, Name = "FirstEntry" };

            await entryCollection.AddAsync(firstEntry);

            Assert.AreEqual(startingCount + 1, entryCollection.Count);

            await entryCollection.RemoveAsync(firstEntry);

            Assert.AreEqual(startingCount, entryCollection.Count);
        }

        [TestMethod]
        [TestCategory("Functional")]
        public async Task SubscriptionTests_Update()
        {
            var meteorClient = TestEnvironment.GetClient();
            await meteorClient.ConnectAsync();

            var entryCollection = meteorClient.GetCollection<Entry>("entries");
            await meteorClient.Subscribe("entries");

            var entry = new Entry() {Count = 101, IsActive = true, Name = "Entry Name"};

            await entryCollection.AddAsync(entry);

            var update = new Entry() {Count = 102, IsActive = false, Name = "New Name"};

            await entryCollection.UpdateAsync(entry.ID, update);

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
        public async Task SubscriptionTests_AddedRemoveLargeNumberOfObjects()
        {
            var meteorClient = TestEnvironment.GetClient();
            await meteorClient.ConnectAsync();

            var entryCollection = meteorClient.GetCollection<Entry>("entries");
            await meteorClient.Subscribe("entries");

            var inserts = 1000;
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
                startedTasks.Add(entryCollection.RemoveAsync(entry));
            }

            Task.WaitAll(startedTasks.ToArray());

            Assert.AreEqual(0, entryCollection.Count);
        }
    }
}
