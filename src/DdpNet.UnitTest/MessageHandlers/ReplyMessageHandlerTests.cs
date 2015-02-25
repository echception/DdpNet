namespace DdpNet.UnitTest.MessageHandlers
{
    using Collections.TestObjects;
    using Connection;
    using DdpNet.Collections;
    using DdpNet.MessageHandlers;
    using DdpNet.Results;
    using Messages;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Newtonsoft.Json;
    using Results;

    [TestClass]
    public class ReplyMessageHandlerTests
    {
        [TestMethod]
        public void ReplyMessageHandler_HandleMessage_HandlesConnected()
        {
            var connectionMock = new Mock<IDdpConnectionSender>();
            var collectionMock = new Mock<ICollectionManager>();
            var resultHandlerMock = new Mock<IResultHandler>();

            var connected = new Connected() {Session = "1"};

            var handler = new ReplyMessageHandler();

            handler.HandleMessage(connectionMock.Object, collectionMock.Object, resultHandlerMock.Object,
                JsonConvert.SerializeObject(connected));

            Assert.IsTrue(handler.CanHandle(connected.MessageType));

            resultHandlerMock.Verify(resultHandler => resultHandler.AddResult(It.IsAny<ReturnedObject>()));
        }

        [TestMethod]
        public void ReplyMessageHandler_HandleMessage_HandlesFailed()
        {
            var connectionMock = new Mock<IDdpConnectionSender>();
            var collectionMock = new Mock<ICollectionManager>();
            var resultHandlerMock = new Mock<IResultHandler>();

            var failed = new Failed() {Version = "2"};

            var handler = new ReplyMessageHandler();

            handler.HandleMessage(connectionMock.Object, collectionMock.Object, resultHandlerMock.Object,
                JsonConvert.SerializeObject(failed));

            Assert.IsTrue(handler.CanHandle(failed.MessageType));

            resultHandlerMock.Verify(resultHandler => resultHandler.AddResult(It.IsAny<ReturnedObject>()));
        }

        [TestMethod]
        public void ReplyMessageHandler_HandleMessage_HandlesResult()
        {
            var connectionMock = new Mock<IDdpConnectionSender>();
            var collectionMock = new Mock<ICollectionManager>();
            var resultHandlerMock = new Mock<IResultHandler>();

            var result = new Result();

            var handler = new ReplyMessageHandler();

            handler.HandleMessage(connectionMock.Object, collectionMock.Object, resultHandlerMock.Object,
                JsonConvert.SerializeObject(result));

            Assert.IsTrue(handler.CanHandle(result.MessageType));

            resultHandlerMock.Verify(resultHandler => resultHandler.AddResult(It.IsAny<ReturnedObject>()));
        }

        [TestMethod]
        public void ReplyMessageHandler_HandleMessage_HandlesReady()
        {
            var connectionMock = new Mock<IDdpConnectionSender>();
            var collectionMock = new Mock<ICollectionManager>();
            var resultHandlerMock = new Mock<IResultHandler>();

            var ready = new Ready();

            var handler = new ReplyMessageHandler();

            handler.HandleMessage(connectionMock.Object, collectionMock.Object, resultHandlerMock.Object,
                JsonConvert.SerializeObject(ready));

            Assert.IsTrue(handler.CanHandle(ready.MessageType));

            resultHandlerMock.Verify(resultHandler => resultHandler.AddResult(It.IsAny<ReturnedObject>()));
        }

        [TestMethod]
        public void ReplyMessageHandler_HandleMessage_HandlesUpdated()
        {
            var connectionMock = new Mock<IDdpConnectionSender>();
            var collectionMock = new Mock<ICollectionManager>();
            var resultHandlerMock = new Mock<IResultHandler>();

            var updated = new Updated();

            var handler = new ReplyMessageHandler();

            handler.HandleMessage(connectionMock.Object, collectionMock.Object, resultHandlerMock.Object,
                JsonConvert.SerializeObject(updated));

            Assert.IsTrue(handler.CanHandle(updated.MessageType));

            resultHandlerMock.Verify(resultHandler => resultHandler.AddResult(It.IsAny<ReturnedObject>()));
        }
    }
}
