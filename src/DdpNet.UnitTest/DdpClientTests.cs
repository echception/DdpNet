namespace DdpNet.UnitTest
{
    using System;
    using Messages;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;

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
    }
}
