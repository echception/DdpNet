namespace DdpNet.UnitTest.Results
{
    using DdpNet.Results;
    using Messages;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    [TestClass]
    public class ConnectedResultFilterTests
    {
        [TestMethod]
        public void ConnectedResultFilter_HandleReturnObject_ConnectedMessage()
        {
            var connectedResultFilter = new ConnectedResultFilter();

            var connected = new Connected() {Session = "Test"};

            var returnedObject = new ReturnedObject("connected", JObject.FromObject(connected),
                JsonConvert.SerializeObject(connected));

            connectedResultFilter.HandleReturnObject(returnedObject);

            Assert.IsTrue(connectedResultFilter.IsCompleted());

            Assert.AreEqual(returnedObject, connectedResultFilter.GetReturnedObject());
        }

        [TestMethod]
        public void ConnectedResultFilter_HandleReturnObject_FailedMessage()
        {
            var connectedResultFilter = new ConnectedResultFilter();

            var failed = new Failed() {Version = "1"};

            var returnedObject = new ReturnedObject("failed", JObject.FromObject(failed),
                JsonConvert.SerializeObject(failed));

            connectedResultFilter.HandleReturnObject(returnedObject);

            Assert.IsTrue(connectedResultFilter.IsCompleted());
            Assert.AreEqual(returnedObject, connectedResultFilter.GetReturnedObject());
        }

        [TestMethod]
        public void ConnectedResultFilter_HandleReturnObject_UnknownMessage()
        {
            var connectedResultFilter = new ConnectedResultFilter();

            var ping = new Ping();

            var returnedObject = new ReturnedObject("ping", JObject.FromObject(ping), JsonConvert.SerializeObject(ping));

            connectedResultFilter.HandleReturnObject(returnedObject);

            Assert.IsFalse(connectedResultFilter.IsCompleted());
            Assert.IsNull(connectedResultFilter.GetReturnedObject());
        }
    }
}
