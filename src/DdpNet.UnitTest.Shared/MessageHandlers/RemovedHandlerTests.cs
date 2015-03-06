namespace DdpNet.UnitTest.MessageHandlers
{
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
    public class RemovedHandlerTests
    {
        [TestMethod]
        public void RemovedHandler_HandleMessage_DeserializeAndCallsCollectionManager()
        {
            var connectionMock = new Mock<IDdpConnectionSender>();
            var collectionMock = new Mock<ICollectionManager>();
            var resultHandlerMock = new Mock<IResultHandler>();

            var testObject = new SimpleDdpObject { IsTrue = false };
            var added = new Removed() {Collection = "Test", ID = "ID"};

            var handler = new RemovedHandler();

            handler.HandleMessage(connectionMock.Object, collectionMock.Object, resultHandlerMock.Object,
                JsonConvert.SerializeObject(added));

            collectionMock.Verify(collection => collection.Removed(It.IsAny<Removed>()), Times.Once);
        }

        [TestMethod]
        public void RemovedHandler_CanHandle_ValidMessageType()
        {
            var handler = new RemovedHandler();

            Assert.IsTrue(handler.CanHandle("removed"));
        }

        [TestMethod]
        public void RemovedHandler_CanHandle_InvalidMessageType()
        {
            var handler = new RemovedHandler();

            Assert.IsFalse(handler.CanHandle("invalid"));
        }
    }
}
