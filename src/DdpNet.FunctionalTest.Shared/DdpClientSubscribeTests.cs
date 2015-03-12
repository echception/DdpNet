﻿namespace DdpNet.FunctionalTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using DataObjects;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DdpClientSubscribeTests
    {
        [TestMethod]
        [TestCategory("Functional")]
        public async Task DdpClient_Subscribe_SubscriptionNotExist()
        {
            var meteorClient = TestEnvironment.GetClient();
            await meteorClient.ConnectAsync();

            await ExceptionAssert.AssertThrowsWithMessage<DdpServerException>(async 
                () => await meteorClient.Subscribe("foobar"), "404", "Subscription not found");
        }

        [TestMethod]
        [TestCategory("Functional")]
        public async Task DdpClient_Subscribe_ExecutedTwice()
        {
            var meteorClient = TestEnvironment.GetClient();
            await meteorClient.ConnectAsync();

            var collection = meteorClient.GetCollection<Entry>("entries");

            await meteorClient.Subscribe("entries");
            await meteorClient.Subscribe("entries");
        }

        [TestMethod]
        [TestCategory("Functional")]
        public async Task DdpClient_Subscribe_CaseSensitive()
        {
            var meteorClient = TestEnvironment.GetClient();
            await meteorClient.ConnectAsync();

            var collection = meteorClient.GetCollection<Entry>("entries");

            await ExceptionAssert.AssertDdpServerExceptionThrown(async () => await meteorClient.Subscribe("Entries"), "404");
            await ExceptionAssert.AssertDdpServerExceptionThrown(async () => await meteorClient.Subscribe("Entries"), "404");
        }

        [TestMethod]
        [TestCategory("Functional")]
        public async Task DdpClient_SubscribeWithParameters_LimitedCollection()
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

        [TestMethod]
        [TestCategory("Functional")]
        public async Task DdpClient_Subscribe_MultipleSubscriptionsSameCollection()
        {
            var allEntriesClient = TestEnvironment.GetClient();
            await allEntriesClient.ConnectAsync();

            var multipleSubscriptionsClient = TestEnvironment.GetClient();
            await multipleSubscriptionsClient.ConnectAsync();

            var allEntriesCollection = allEntriesClient.GetCollection<Entry>("entries");
            var multipleSubscriptionsCollection = multipleSubscriptionsClient.GetCollection<Entry>("entries");

            await allEntriesClient.Subscribe("entries");

            for (int i = 0; i < 10; i++)
            {
                await allEntriesCollection.AddAsync(Entry.Random());
            }

            int numberActive = allEntriesCollection.Count(x => x.IsActive);

            await multipleSubscriptionsClient.Subscribe("activeEntries");

            Assert.AreEqual(numberActive, multipleSubscriptionsCollection.Count);

            await multipleSubscriptionsClient.Subscribe("inactiveEntries");

            Assert.AreEqual(allEntriesCollection.Count, multipleSubscriptionsCollection.Count);
        }

        [TestMethod]
        [TestCategory("Functional")]
        public async Task DdpClient_SubscribeWithParameters_SameSubscriptionDifferentParameters()
        {
            TestEnvironment.Cleanup();

            var meteorClient = TestEnvironment.GetClient();
            await meteorClient.ConnectAsync();

            var collection = meteorClient.GetCollection<Entry>("entries");

            for (int i = 0; i < 10; i++)
            {
                await collection.AddAsync(new Entry() { Count = 5, IsActive = true, Name = "TestName" });
                await collection.AddAsync(new Entry() { Count = 5, IsActive = false, Name = "TestName" });
            }

            await meteorClient.Subscribe("entiresWithFilterActive", true);

            Assert.AreEqual(10, collection.Count);

            foreach (var entry in collection)
            {
                Assert.AreEqual(true, entry.IsActive);
            }

            await meteorClient.Subscribe("entiresWithFilterActive", false);

            Assert.AreEqual(20, collection.Count);

            for (int i = 10; i < 20; i++)
            {
                Assert.AreEqual(false, collection[i].IsActive);
            }
        }

        [TestMethod]
        [TestCategory("Functional")]
        public async Task DdpClient_Unsubscribe_ValidSubscription()
        {
            var meteorClient = TestEnvironment.GetClient();
            await meteorClient.ConnectAsync();

            var collection = meteorClient.GetCollection<Entry>("entries");

            await collection.AddAsync(Entry.Random());

            var subscription = await meteorClient.Subscribe("entries");

            Assert.IsTrue(collection.Count > 0);
            await meteorClient.Unsubscribe(subscription);

            Assert.IsTrue(collection.Count == 0);
        }

        [TestMethod]
        [TestCategory("Functional")]
        public async Task DdpClient_Unsubscribe_ExecutedTwice()
        {
            var meteorClient = TestEnvironment.GetClient();
            await meteorClient.ConnectAsync();

            var collection = meteorClient.GetCollection<Entry>("entries");

            await collection.AddAsync(Entry.Random());

            var subscription = await meteorClient.Subscribe("entries");

            Assert.IsTrue(collection.Count > 0);
            await meteorClient.Unsubscribe(subscription);
            await meteorClient.Unsubscribe(subscription);

            Assert.IsTrue(collection.Count == 0);
        }
    }
}
