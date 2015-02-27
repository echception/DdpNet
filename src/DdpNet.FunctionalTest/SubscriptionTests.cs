namespace DdpNet.FunctionalTest
{
    using System;
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

            var entry = entryCollection.First();

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

            await entryCollection.RemoveAsync(entryCollection.First());

            Assert.AreEqual(startingCount, entryCollection.Count);
        }
    }
}
