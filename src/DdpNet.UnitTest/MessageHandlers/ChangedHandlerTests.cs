namespace DdpNet.UnitTest.MessageHandlers
{
    using Collections.TestObjects;
    using Connection;
    using DdpNet.Collections;
    using DdpNet.MessageHandlers;
    using Messages;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Results;

    [TestClass]
    public class ChangedHandlerTests
    {
        [TestMethod]
        public void ChangedHandler_HandleMessage_DeserializsAndCallsCollectionManager()
        {
            var connectionMock = new Mock<IDdpConnectionSender>();
            var collectionMock = new Mock<ICollectionManager>();
            var resultHandlerMock = new Mock<IResultHandler>();

            var testObject = new SimpleDdpObject { IsTrue = false };
            var added = new Changed { Collection = "Tests", Fields = null, ID = "1" };

            var handler = new ChangedHandler();

            handler.HandleMessage(connectionMock.Object, collectionMock.Object, resultHandlerMock.Object,
                JsonConvert.SerializeObject(added));

            collectionMock.Verify(collection => collection.Changed(It.IsAny<Changed>()), Times.Once);
        }

        [TestMethod]
        public void ChangedHandler_CanHandle_ValidMessageType()
        {
            var handler = new ChangedHandler();

            Assert.IsTrue(handler.CanHandle("changed"));
        }

        [TestMethod]
        public void ChangedHandler_CanHandle_InvalidMessageType()
        {
            var handler = new ChangedHandler();

            Assert.IsFalse(handler.CanHandle("invalid"));
        }
    }
}
