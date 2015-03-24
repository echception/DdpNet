﻿namespace DdpNet.FunctionalTest
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using DataObjects;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DdpCollectionTests
    {
        [TestMethod]
        [TestCategory("Functional")]
        public async Task DdpCollection_Add_ObjectReadable()
        {
            var meteorClient = TestEnvironment.GetClient();
            await meteorClient.ConnectAsync();

            var entryCollection = meteorClient.GetCollection<Entry>("entries");
            await meteorClient.Subscribe("entries");

            var startingCount = entryCollection.Count;

            var firstEntry = new Entry() { Count = 12, IsActive = false, Name = "FirstEntry" };

            await entryCollection.AddAsync(firstEntry);

            Assert.AreEqual(startingCount + 1, entryCollection.Count);

            var entry = entryCollection.Single(x => x.Id == firstEntry.Id);

            Entry.AssertAreEqual(firstEntry, entry);
        }

        [TestMethod]
        [TestCategory("Functional")]
        public async Task DdpCollection_Add_ReturnsItemID()
        {
            var meteorClient = TestEnvironment.GetClient();
            await meteorClient.ConnectAsync();

            var entryCollection = meteorClient.GetCollection<Entry>("entries");
            await meteorClient.Subscribe("entries");

            var firstEntry = new Entry() { Count = 12, IsActive = false, Name = "FirstEntry" };

            string id = await entryCollection.AddAsync(firstEntry);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(id));

            var itemAdded = entryCollection.Single(x => x.Id == id);

            Assert.IsNotNull(itemAdded);
        }

        [TestMethod]
        [TestCategory("Functional")]
        public async Task DdpCollection_Remove_ValidObject()
        {
            var meteorClient = TestEnvironment.GetClient();
            await meteorClient.ConnectAsync();

            var entryCollection = meteorClient.GetCollection<Entry>("entries");
            await meteorClient.Subscribe("entries");

            int startingCount = entryCollection.Count;

            var firstEntry = new Entry() { Count = 12, IsActive = false, Name = "FirstEntry" };

            await entryCollection.AddAsync(firstEntry);

            Assert.AreEqual(startingCount + 1, entryCollection.Count);

            var removed = await entryCollection.RemoveAsync(firstEntry.Id);

            Assert.IsTrue(removed);

            Assert.AreEqual(startingCount, entryCollection.Count);
        }

        [TestMethod]
        [TestCategory("Functional")]
        public async Task DdpCollection_Update_ValidObject()
        {
            var meteorClient = TestEnvironment.GetClient();
            await meteorClient.ConnectAsync();

            var entryCollection = meteorClient.GetCollection<Entry>("entries");
            await meteorClient.Subscribe("entries");

            var entry = new Entry() { Count = 101, IsActive = true, Name = "Entry Name" };

            await entryCollection.AddAsync(entry);

            var update = new Entry() { Count = 102, IsActive = false, Name = "New Name" };

            bool wasUpdated = await entryCollection.UpdateAsync(entry.Id, update);

            Assert.IsTrue(wasUpdated);

            var updatedEntry = entryCollection.Single(x => x.Id == entry.Id);

            Entry.AssertAreEqual(update, updatedEntry);
        }

        [TestMethod]
        [TestCategory("Functional")]
        public async Task DdpCollection_Update_DictionaryObject()
        {
            var meteorClient = TestEnvironment.GetClient();
            await meteorClient.ConnectAsync();

            var entryCollection = meteorClient.GetCollection<Entry>("entries");
            await meteorClient.Subscribe("entries");

            var entry = new Entry() { Count = 101, IsActive = true, Name = "Entry Name" };

            await entryCollection.AddAsync(entry);

            Dictionary<string, object> fieldsToUpdate = new Dictionary<string, object>()
                                                            {
                                                                { "Count", 102 },
                                                                { "IsActive", false },
                                                                { "Name", "New Name" }
                                                            };

            bool wasUpdated = await entryCollection.UpdateAsync(entry.Id, fieldsToUpdate);

            Assert.IsTrue(wasUpdated);

            var updatedEntry = entryCollection.Single(x => x.Id == entry.Id);

            Assert.AreEqual(fieldsToUpdate["Count"], updatedEntry.Count);
            Assert.AreEqual(fieldsToUpdate["IsActive"], updatedEntry.IsActive);
            Assert.AreEqual(fieldsToUpdate["Name"], updatedEntry.Name);
        }

        [TestMethod]
        [TestCategory("Functional")]
        public async Task DdpCollection_Added_RaisesCollectionChangedEvent()
        {
            var meteorClient = TestEnvironment.GetClient();
            await meteorClient.ConnectAsync();

            var entryCollection = meteorClient.GetCollection<Entry>("entries");
            await meteorClient.Subscribe("entries");

            var entry = new Entry() { Count = 101, IsActive = true, Name = "Entry Name" };

            bool collectionChangedCalled = false;
            bool propertyChanged = false;
            ((INotifyCollectionChanged)entryCollection).CollectionChanged += (sender, args) => collectionChangedCalled = true;
            ((INotifyPropertyChanged)entryCollection).PropertyChanged += (sender, args) =>
            {
                propertyChanged = true;
            };

            await entryCollection.AddAsync(entry);

            Assert.IsTrue(collectionChangedCalled);
            Assert.IsTrue(propertyChanged);
        }

        [TestMethod]
        [TestCategory("Functional")]
        public async Task DdpCollection_AddRemove_LargeNumberOfObjects()
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
                startedTasks.Add(entryCollection.RemoveAsync(entry.Id));
            }

            Task.WaitAll(startedTasks.ToArray());

            Assert.AreEqual(0, entryCollection.Count);
        }

        [TestMethod]
        [TestCategory("Functional")]
        public async Task DdpCollection_Update_ItemNotExist()
        {
            var meteorClient = TestEnvironment.GetClient();
            await meteorClient.ConnectAsync();

            var collection = meteorClient.GetCollection<Entry>("entries");

            await meteorClient.Subscribe("entries");

            bool wasUpdated = await collection.UpdateAsync("NOT_EXIST", new Entry() { Count = 10 });

            Assert.IsFalse(wasUpdated);
        }

        [TestMethod]
        [TestCategory("Functional")]
        public async Task DdpCollection_Remove_ItemNotExist()
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
        public async Task DdpCollection_Add_NoPermission()
        {
            var meteorClient = TestEnvironment.GetClient();
            await meteorClient.ConnectAsync();

            var collection = meteorClient.GetCollection<Entry>("denyAll");

            await ExceptionAssert.AssertThrowsWithMessage<DdpServerException>(async () => await collection.AddAsync(Entry.Random()), "403", "Access denied");
        }

        [TestMethod]
        [TestCategory("Functional")]
        public async Task DdpCollection_Update_NoPermission()
        {
            var meteorClient = TestEnvironment.GetClient();
            await meteorClient.ConnectAsync();

            var entry = Entry.Random();

            await TestEnvironment.AddDenyAllEntry(meteorClient, entry);

            var collection = meteorClient.GetCollection<Entry>("denyAll");

            await meteorClient.Subscribe("denyAll");

            Assert.IsTrue(collection.Count > 0);

            var entryToUpdate = collection.First();

            await ExceptionAssert.AssertThrowsWithMessage<DdpServerException>(async () => await collection.UpdateAsync(entryToUpdate.Id, Entry.Random()), "403", "Access denied");
        }

        [TestMethod]
        [TestCategory("Functional")]
        public async Task DdpCollection_Remove_NoPermission()
        {
            var meteorClient = TestEnvironment.GetClient();
            await meteorClient.ConnectAsync();

            var entry = Entry.Random();

            await TestEnvironment.AddDenyAllEntry(meteorClient, entry);

            var collection = meteorClient.GetCollection<Entry>("denyAll");

            await meteorClient.Subscribe("denyAll");

            Assert.IsTrue(collection.Count > 0);

            var entryToRemove = collection.First();

            await ExceptionAssert.AssertThrowsWithMessage<DdpServerException>(async () => await collection.RemoveAsync(entryToRemove.Id), "403", "Access denied");
        }
    }
}
