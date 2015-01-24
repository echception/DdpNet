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
    public class AddedHandlerTests
    {
        [TestMethod]
        public void AddedHandler_HandleMessage_DeserializsAndCallsCollectionManager()
        {
            var connectionMock = new Mock<IDdpConnectionSender>();
            var collectionMock = new Mock<ICollectionManager>();
            var resultHandlerMock = new Mock<IResultHandler>();

            var testObject = new SimpleDdpObject {IsTrue = false};
            var added = new Added {Collection = "Tests", Fields = JObject.FromObject(testObject),ID = "1"};

            var handler = new AddedHandler();

            handler.HandleMessage(connectionMock.Object, collectionMock.Object, resultHandlerMock.Object,
                JsonConvert.SerializeObject(added));

            collectionMock.Verify(collection => collection.Added(It.IsAny<Added>()), Times.Once);
        }

        [TestMethod]
        public void AddedHandler_CanHandle_ValidMessageType()
        {
            var handler = new AddedHandler();

            Assert.IsTrue(handler.CanHandle("added"));
        }

        [TestMethod]
        public void AddedHandler_CanHandle_InvalidMessageType()
        {
            var handler = new AddedHandler();

            Assert.IsFalse(handler.CanHandle("invalid"));
        }
    }
}
