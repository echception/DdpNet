namespace DdpNet.FunctionalTest
{
    using System.Threading.Tasks;
    using DataObjects;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DdpClientMethodCallTests
    {
        [TestMethod]
        [TestCategory("Functional")]
        public async Task DdpClient_Call_ValidMethodCall()
        {
            var meteorClient = TestEnvironment.GetClient();
            await meteorClient.ConnectAsync();

            int value = await meteorClient.Call<int>("returnTen");

            Assert.AreEqual(10, value);
        }

        [TestMethod]
        [TestCategory("Functional")]
        public async Task DdpClient_Call_InvalidMethodName()
        {
            var meteorClient = TestEnvironment.GetClient();
            await meteorClient.ConnectAsync();

            await ExceptionAssert.AssertDdpServerExceptionThrown(async () => await meteorClient.Call("INVALID_METHOD"), "404");
        }

        [TestMethod]
        [TestCategory("Functional")]
        public async Task DdpClient_Call_MethodNameCaseSensitive()
        {
            var meteorClient = TestEnvironment.GetClient();
            await meteorClient.ConnectAsync();

            await ExceptionAssert.AssertDdpServerExceptionThrown(async () => await meteorClient.Call("ReturnsTen"), "404");
        }

        [TestMethod]
        [TestCategory("Functional")]
        public async Task DdpClient_Call_MethodThrowsError()
        {
            var meteorClient = TestEnvironment.GetClient();
            await meteorClient.ConnectAsync();

            await ExceptionAssert.AssertDdpServerExceptionThrown(async () => await meteorClient.Call("throwException"), "500");
        }

        [TestMethod]
        [TestCategory("Functional")]
        public async Task DdpClient_CallWithParameters_PassedToServer()
        {
            var meteorClient = TestEnvironment.GetClient();
            await meteorClient.ConnectAsync();

            int result = await meteorClient.Call<int>("increment", 10);

            Assert.AreEqual(11, result);

            result = await meteorClient.Call<int>("increment", 2054);

            Assert.AreEqual(2055, result);
        }
    }
}
