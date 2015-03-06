namespace DdpNet.UnitTest.Results
{
    using DdpNet.Results;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RegisteredResultCallbackTests
    {
        [TestMethod]
        public void RegisteredResultCallback_Constructor_ValidData()
        {
            var filter = new SubscribeResultFilter("subID");
            ResultCallback callback = returnedObject => { };

            var registeredCallback = new RegisteredResultCallback(filter, callback);

            Assert.AreEqual(filter, registeredCallback.Filter);
            Assert.AreEqual(callback, registeredCallback.Callback);
        }
    }
}
