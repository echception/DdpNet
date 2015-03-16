namespace DdpNet.UnitTest.MessageHandlers
{
    using System.Runtime.CompilerServices;
    using Collections.TestObjects;
    using Connection;
    using DdpNet.Collections;
    using DdpNet.MessageHandlers;
    using DdpNet.Results;
    using Messages;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Newtonsoft.Json;
    using Results;

    [TestClass]
    public class PingHandlerTests
    {
        [TestMethod]
        public void PingHandler_HandleMessage_SendsPongReply()
        {
            var connectionMock = new Mock<IDdpConnectionSender>();
            var collectionMock = new Mock<ICollectionManager>();
            var resultHandlerMock = new Mock<IResultHandler>();

            var pingMessage = new Ping {Id = "testPing"};

            var handler = new PingHandler();

            handler.HandleMessage(connectionMock.Object, collectionMock.Object, resultHandlerMock.Object,
                JsonConvert.SerializeObject(pingMessage));

            connectionMock.Verify(connection => connection.SendObject(It.IsAny<Pong>()));
        }

        [TestMethod]
        public void PingHandler_CanHandle_ValidMessage()
        {
            var handler = new PingHandler();

            Assert.IsTrue(handler.CanHandle("ping"));
        }

        [TestMethod]
        public void PingHandler_CanHandle_InvalidMessageType()
        {
            var handler = new PingHandler();

            Assert.IsFalse(handler.CanHandle("invalid"));
        }
    }
}
