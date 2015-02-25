namespace DdpNet.UnitTest.Results
{
    using DdpNet.Results;
    using Messages;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    [TestClass]
    public class MethodCallResultFilterTests
    {
        [TestMethod]
        public void MethodCallResultFilter_HandleReturnObject_ValidMessage()
        {
            var resultFilter = new MethodCallResultFilter("10");

            var message = new Result() {ID = "10"};
            var updated = new Updated() {Methods = new[] {"10"}};

            var resultReturnedObject = new ReturnedObject("result", JObject.FromObject(message),
                JsonConvert.SerializeObject(message));

            var updatedReturnedObject = new ReturnedObject("updated", JObject.FromObject(updated),
                JsonConvert.SerializeObject(updated));

            resultFilter.HandleReturnObject(resultReturnedObject);

            Assert.IsFalse(resultFilter.IsCompleted());

            resultFilter.HandleReturnObject(updatedReturnedObject);

            Assert.IsTrue(resultFilter.IsCompleted());
            Assert.AreEqual(resultReturnedObject, resultFilter.GetReturnedObject());
        }

        [TestMethod]
        public void MethodCallResultFilter_HandleReturnObject_UpdatedFirst()
        {
            var resultFilter = new MethodCallResultFilter("10");

            var message = new Result() { ID = "10" };
            var updated = new Updated() { Methods = new[] { "10" } };

            var resultReturnedObject = new ReturnedObject("result", JObject.FromObject(message),
                JsonConvert.SerializeObject(message));

            var updatedReturnedObject = new ReturnedObject("updated", JObject.FromObject(updated),
                JsonConvert.SerializeObject(updated));

            resultFilter.HandleReturnObject(updatedReturnedObject);

            Assert.IsFalse(resultFilter.IsCompleted());

            resultFilter.HandleReturnObject(resultReturnedObject);

            Assert.IsTrue(resultFilter.IsCompleted());
            Assert.AreEqual(resultReturnedObject, resultFilter.GetReturnedObject());
        }

        [TestMethod]
        public void MethodCallResultFilter_HandleReturnObject_NotHandledID()
        {
            var resultFilter = new MethodCallResultFilter("11");

            var message = new Result() { ID = "10" };
            var updated = new Updated() { Methods = new[] { "10" } };

            var resultReturnedObject = new ReturnedObject("result", JObject.FromObject(message),
                JsonConvert.SerializeObject(message));

            var updatedReturnedObject = new ReturnedObject("updated", JObject.FromObject(updated),
                JsonConvert.SerializeObject(updated));

            resultFilter.HandleReturnObject(updatedReturnedObject);

            Assert.IsFalse(resultFilter.IsCompleted());

            resultFilter.HandleReturnObject(resultReturnedObject);

            Assert.IsFalse(resultFilter.IsCompleted());
            Assert.IsNull(resultFilter.GetReturnedObject());
        }
    }
}
