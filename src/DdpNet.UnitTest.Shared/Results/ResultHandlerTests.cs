namespace DdpNet.UnitTest.Results
{
    using System;
    using DdpNet.Results;
    using Messages;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    [TestClass]
    public class ResultHandlerTests
    {
        [TestMethod]
        public void ResultHandler_WaitForResult_WaitHandler()
        {
            var resultHandler = new ResultHandler();
            var waitHandle = resultHandler.RegisterWaitHandler(ResultFilterFactory.CreateConnectResultFilter());

            var waitTask = resultHandler.WaitForResult(waitHandle);

            var connected = new Connected() {Session = "TestSession"};
            var returnedObject = new ReturnedObject(connected.MessageType, JObject.FromObject(connected),
                JsonConvert.SerializeObject(connected));

            resultHandler.AddResult(returnedObject);

            waitTask.Wait();

            Assert.AreEqual(returnedObject, waitTask.Result);
        }

        [TestMethod]
        public void ResultHandler_WaitForResult_DuplicateWaitHandle()
        {
            var resultHandler = new ResultHandler();
            var waitHandle = resultHandler.RegisterWaitHandler(ResultFilterFactory.CreateConnectResultFilter());

            var waitTask = resultHandler.WaitForResult(waitHandle);
            var secondWaitTask = resultHandler.WaitForResult(waitHandle);

            var connected = new Connected() { Session = "TestSession" };
            var returnedObject = new ReturnedObject(connected.MessageType, JObject.FromObject(connected),
                JsonConvert.SerializeObject(connected));

            resultHandler.AddResult(returnedObject);

            waitTask.Wait();
            secondWaitTask.Wait();

            Assert.AreEqual(returnedObject, waitTask.Result);
            Assert.AreEqual(returnedObject, secondWaitTask.Result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ResultHandler_WaitForResult_ReactivateWaitHandle()
        {
            var resultHandler = new ResultHandler();
            var waitHandle = resultHandler.RegisterWaitHandler(ResultFilterFactory.CreateConnectResultFilter());

            var waitTask = resultHandler.WaitForResult(waitHandle);

            var connected = new Connected() { Session = "TestSession" };
            var returnedObject = new ReturnedObject(connected.MessageType, JObject.FromObject(connected),
                JsonConvert.SerializeObject(connected));

            resultHandler.AddResult(returnedObject);

            waitTask.Wait();

            Assert.AreEqual(returnedObject, waitTask.Result);

            resultHandler.WaitForResult(waitHandle);
        }

        [TestMethod]
        public void ResultHandler_RegisterResultCallback_CallbackCalled()
        {
            var resultHandler = new ResultHandler();

            bool callbackCalled = false;
            var connected = new Connected() { Session = "TestSession" };
            var returnedObject = new ReturnedObject(connected.MessageType, JObject.FromObject(connected),
                JsonConvert.SerializeObject(connected));

            resultHandler.RegisterResultCallback(ResultFilterFactory.CreateConnectResultFilter(), o =>
            {
                callbackCalled = true;
                Assert.AreEqual(returnedObject, o);
            });

            resultHandler.AddResult(returnedObject);

            Assert.IsTrue(callbackCalled);
        }

        [TestMethod]
        public void ResultHandler_RegisterResultCallback_DuplicateCallback()
        {
            var resultHandler = new ResultHandler();

            bool callbackCalled = false;
            bool secondCallbackCalled = false;
            
            var connected = new Connected() { Session = "TestSession" };
            var returnedObject = new ReturnedObject(connected.MessageType, JObject.FromObject(connected),
                JsonConvert.SerializeObject(connected));

            resultHandler.RegisterResultCallback(ResultFilterFactory.CreateConnectResultFilter(), o =>
            {
                callbackCalled = true;
                Assert.AreEqual(returnedObject, o);
            });

            resultHandler.RegisterResultCallback(ResultFilterFactory.CreateConnectResultFilter(), o =>
            {
                secondCallbackCalled = true;
                Assert.AreEqual(returnedObject, o);
            });

            resultHandler.AddResult(returnedObject);

            Assert.IsTrue(callbackCalled);
            Assert.IsTrue(secondCallbackCalled);
        }
    }
}
