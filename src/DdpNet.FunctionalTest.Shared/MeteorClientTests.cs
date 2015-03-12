namespace DdpNet.FunctionalTest
{
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MeteorClientTests
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            TestEnvironment.Cleanup();
            TestEnvironment.InitializeTestUser().Wait();
        }

        [TestMethod]
        [TestCategory("Functional")]
        public async Task MeteorClient_LoginPassword_ValidInformation()
        {
            var client = TestEnvironment.GetClient();
            await client.ConnectAsync();

            await client.LoginPassword(TestEnvironment.TestUserName, TestEnvironment.TestUserPassword);

            Assert.AreEqual(TestEnvironment.TestUserName, client.User.UserName);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(client.User.ID));
        }

        [TestMethod]
        [TestCategory("Functional")]
        public async Task MeteorClient_LoginPassword_InvalidUserName()
        {
            var client = TestEnvironment.GetClient();
            await client.ConnectAsync();

            await ExceptionAssert.AssertDdpServerExceptionThrown(async () => await client.LoginPassword("foobar", TestEnvironment.TestUserPassword), "403");
        }

        [TestMethod]
        [TestCategory("Functional")]
        public async Task MeteorClient_LoginPassword_InvalidPassword()
        {
            var client = TestEnvironment.GetClient();
            await client.ConnectAsync();

            await ExceptionAssert.AssertDdpServerExceptionThrown(async () => await client.LoginPassword(TestEnvironment.TestUserName, "foobar"), "403");
        }

        [TestMethod]
        [TestCategory("Functional")]
        public async Task MeteorClient_LoginPassword_LoginTwice()
        {
            var client = TestEnvironment.GetClient();
            await client.ConnectAsync();

            await client.LoginPassword(TestEnvironment.TestUserName, TestEnvironment.TestUserPassword);
            await client.LoginPassword(TestEnvironment.TestUserName, TestEnvironment.TestUserPassword);

            Assert.AreEqual(TestEnvironment.TestUserName, client.User.UserName);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(client.User.ID));
        }

        [TestMethod]
        [TestCategory("Functional")]
        public async Task MeteorClient_Logout_LoggedIn()
        {
            var client = TestEnvironment.GetClient();
            await client.ConnectAsync();

            await client.LoginPassword(TestEnvironment.TestUserName, TestEnvironment.TestUserPassword);

            Assert.IsNotNull(client.User);
            Assert.IsNotNull(client.UserSession);

            await client.Logout();

            Assert.IsNull(client.User);
            Assert.IsNull(client.UserSession);
        }

        [TestMethod]
        [TestCategory("Functional")]
        public async Task MeteorClient_Logout_NotLoggedIn()
        {
            var client = TestEnvironment.GetClient();
            await client.ConnectAsync();

            await client.Logout();

            Assert.IsNull(client.User);
            Assert.IsNull(client.UserSession);
        }

        [TestMethod]
        [TestCategory("Functional")]
        public async Task MeteorClient_LoginResumeSession_StartingAnotherSession()
        {
            var client = TestEnvironment.GetClient();
            await client.ConnectAsync();

            await client.LoginPassword(TestEnvironment.TestUserName, TestEnvironment.TestUserPassword);

            var token = client.UserSession.Token;

            var client2 = TestEnvironment.GetClient();
            await client2.ConnectAsync();

            await client2.LoginResumeSession(token);

            Assert.AreEqual(client.User.ID, client2.User.ID);
        }

        [TestMethod]
        [TestCategory("Functional")]
        public async Task MeteorClient_LoginResumeSession_SessionLoggedOut()
        {
            var client = TestEnvironment.GetClient();
            await client.ConnectAsync();

            await client.LoginPassword(TestEnvironment.TestUserName, TestEnvironment.TestUserPassword);

            var token = client.UserSession.Token;

            await client.Logout();

            await ExceptionAssert.AssertDdpServerExceptionThrown(async () => await client.LoginResumeSession(token), "403");
        }
    }
}
