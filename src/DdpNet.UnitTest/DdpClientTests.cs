namespace DdpNet.UnitTest
{
    using System;
    using Collections.TestObjects;
    using Connection;
    using Messages;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    [TestClass]
    public class DdpClientTests
    {
        [TestMethod]
        public void DdpClient_ConnectAsync_CalledMultipleTimes()
        {
            var connection = new InMemoryConnection();

            var client = new DdpClient(connection);

            connection.Reply(
                JsonConvert.SerializeObject(new Connected() {Session = "TestSession"}));

            client.ConnectAsync(true).Wait();

            bool exceptionCaught = false;

            try
            {
                client.ConnectAsync(true).Wait();
            }
            catch (AggregateException e)
            {
                Assert.IsTrue(e.InnerException is InvalidOperationException);
                exceptionCaught = true;
            }

            Assert.IsTrue(exceptionCaught);
        }

        [TestMethod]
        public void DdpClient_Call_ServerMethodInvoked()
        {
            var connection = new InMemoryConnection();

            var client = new DdpClient(connection);

            this.Connect(client, connection);

            this.ClearSentMessages(connection);

            var task = client.Call("TestMethod");

            var methodString = connection.GetSentMessage();

            var method = JsonConvert.DeserializeObject<Method>(methodString);

            connection.Reply(JsonConvert.SerializeObject(new Result() {ID = method.ID}));
            connection.Reply(JsonConvert.SerializeObject(new Updated() {Methods = new[] {method.ID}}));

            task.Wait();

            Assert.AreEqual("TestMethod", method.MethodName);
            Assert.AreEqual(0, method.Parameters.Count);
        }

        [TestMethod]
        public void DdpClient_Call_GetResult()
        {
            var connection = new InMemoryConnection();

            var client = new DdpClient(connection);

            this.Connect(client, connection);

            this.ClearSentMessages(connection);

            var task = client.Call<SimpleField>("TestMethod");

            var methodString = connection.GetSentMessage();

            var method = JsonConvert.DeserializeObject<Method>(methodString);

            connection.Reply(
                JsonConvert.SerializeObject(new Result()
                {
                    ID = method.ID,
                    ResultObject = JToken.FromObject(new SimpleField() {integerField = 11, boolField = false, stringField = "ReturnObject"})
                }));
            connection.Reply(JsonConvert.SerializeObject(new Updated() { Methods = new[] { method.ID } }));

            task.Wait();

            Assert.AreEqual("TestMethod", method.MethodName);
            Assert.AreEqual(0, method.Parameters.Count);

            var result = task.Result;

            Assert.AreEqual(11, result.integerField);
            Assert.AreEqual(false, result.boolField);
            Assert.AreEqual("ReturnObject", result.stringField);
        }

        [TestMethod]
        public void DdpClient_Call_ErrorReturned()
        {
            var connection = new InMemoryConnection();

            var client = new DdpClient(connection);

            this.Connect(client, connection);

            this.ClearSentMessages(connection);

            var task = client.Call("TestMethod");

            var methodString = connection.GetSentMessage();

            var method = JsonConvert.DeserializeObject<Method>(methodString);

            connection.Reply(
                JsonConvert.SerializeObject(new Result()
                {
                    ID = method.ID,
                    Error = new Error() {Details = "Failed", ErrorMessage = "TestFailed", Reason = "ReasonFailed"}
                }));
            connection.Reply(JsonConvert.SerializeObject(new Updated() { Methods = new[] { method.ID } }));

            ExceptionAssert.ExpectAggregateException(() => task.Wait(), typeof(InvalidOperationException));

            Assert.AreEqual("TestMethod", method.MethodName);
            Assert.AreEqual(0, method.Parameters.Count);

            Assert.IsNotNull(task.Exception);
            Assert.IsTrue(task.IsFaulted);
        }

        [TestMethod]
        public void DdpClient_Subscribe_ServerMethodCalled()
        {
            var connection = new InMemoryConnection();

            var client = new DdpClient(connection);

            this.Connect(client, connection);

            this.ClearSentMessages(connection);

            var task = client.Subscribe("TestSubscription");

            var subscribeString = connection.GetSentMessage();
            var subscribe = JsonConvert.DeserializeObject<Subscribe>(subscribeString);

            connection.Reply(JsonConvert.SerializeObject(new Ready() { SubscriptionsReady = new []{subscribe.ID}}));

            task.Wait();

            Assert.IsTrue(task.IsCompleted);
            Assert.AreEqual("TestSubscription", subscribe.Name);
        }

        [TestMethod]
        public void DdpClient_GetCollection_GetsTypedCollection()
        {
            var connection = new InMemoryConnection();

            var client = new DdpClient(connection);

            this.Connect(client, connection);

            var collection = client.GetCollection<SimpleDdpObject>("Test");

            Assert.IsNotNull(collection);
        }

        private void Connect(DdpClient client, InMemoryConnection connection)
        {
            connection.Reply(
                JsonConvert.SerializeObject(new Connected() { Session = "TestSession" }));

            client.ConnectAsync(true).Wait();
        }

        private void ClearSentMessages(InMemoryConnection connection)
        {
            while (!string.IsNullOrWhiteSpace(connection.GetSentMessage())) { }
        }
    }
}
