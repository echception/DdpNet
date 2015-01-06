namespace DdpNet.UnitTest
{
    using System.Linq;
    using Messages;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    [TestClass]
    public class ConnectionTests
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
        public void DdpClient_ConnectAsync_SendsConnectMessage()
        {
            this.client.ConnectAsync(false).Wait();

            var message = this.testConnection.GetSentMessage();

            var connectMessage = JsonConvert.DeserializeObject<Connect>(message);

            Assert.IsNotNull(connectMessage, "Connect should send an object");
            Assert.AreEqual(connectMessage.Version, "1", "Message should support version 1");
            Assert.AreEqual(connectMessage.VersionsSupported.Count(), 1, "Client only supports 1 version");
            Assert.AreEqual(connectMessage.VersionsSupported.First(), "1", "Client only supports 1 version");
        }
    }
}
