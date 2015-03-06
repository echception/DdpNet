namespace DdpNet.UnitTest.Results
{
    using DdpNet.Results;
    using Messages;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    [TestClass]
    public class SubscribeResultFilterTests
    {
        [TestMethod]
        public void SubscribeResultFilter_HandleReturnObject_ValidObject()
        {
            var subscribeFilter = new SubscribeResultFilter("subID");

            var ready = new Ready() {SubscriptionsReady = new[] {"subID"}};

            var returnedObject = new ReturnedObject("ready", JObject.FromObject(ready),
                JsonConvert.SerializeObject(ready));

            subscribeFilter.HandleReturnObject(returnedObject);

            Assert.IsTrue(subscribeFilter.IsCompleted());
            Assert.AreEqual(returnedObject, subscribeFilter.GetReturnedObject());
        }

        [TestMethod]
        public void SubscribeResultFilter_HandleReturnObject_InvalidSubscriptionID()
        {
            var subscribeFilter = new SubscribeResultFilter("invalid");

            var ready = new Ready() { SubscriptionsReady = new[] { "subID" } };

            var returnedObject = new ReturnedObject("ready", JObject.FromObject(ready),
                JsonConvert.SerializeObject(ready));

            subscribeFilter.HandleReturnObject(returnedObject);

            Assert.IsFalse(subscribeFilter.IsCompleted());
            Assert.IsNull(subscribeFilter.GetReturnedObject());
        }

        [TestMethod]
        public void SubscribeResultFilter_HandleReturnObject_InvalidReturnedObject()
        {
            var subscribeFilter = new SubscribeResultFilter("invalid");

            var ping = new Ping();

            var returnedObject = new ReturnedObject("ping", JObject.FromObject(ping),
                JsonConvert.SerializeObject(ping));

            subscribeFilter.HandleReturnObject(returnedObject);

            Assert.IsFalse(subscribeFilter.IsCompleted());
            Assert.IsNull(subscribeFilter.GetReturnedObject());
        }
    }
}
