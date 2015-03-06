namespace DdpNet.UnitTest
{
    using System;
    using Messages;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;

    [TestClass]
    public class HeartbeatsTest
    {
        private DdpClient client;
        private InMemoryConnection testConnection;

        [TestInitialize]
        public void TestInitialize()
        {
            this.testConnection = new InMemoryConnection();
            this.client = new DdpClient(this.testConnection);
        }

        [TestMethod]
        public void DdpClient_SendPingMessage_ReturnsPongMessage()
        {
            var ping = new Ping();
            this.testConnection.Reply(JsonConvert.SerializeObject(ping));

            this.client.ReceiveAsync().Wait();

            var sentMessage = this.testConnection.GetSentMessage();

            var pongReply = JsonConvert.DeserializeObject<Pong>(sentMessage);

            Assert.AreEqual("pong", pongReply.MessageType);
            Assert.IsTrue(String.IsNullOrEmpty(pongReply.ID));
        }

        [TestMethod]
        public void DdpClient_SendPingMessageWithId_ReturnsPongMessageWithId()
        {
            var ping = new Ping {ID = "TestID"};
            this.testConnection.Reply(JsonConvert.SerializeObject(ping));

            this.client.ReceiveAsync().Wait();

            var sentMessage = this.testConnection.GetSentMessage();

            var pongReply = JsonConvert.DeserializeObject<Pong>(sentMessage);

            Assert.AreEqual("pong", pongReply.MessageType);
            Assert.AreEqual("TestID", pongReply.ID);
        }
    }
}
