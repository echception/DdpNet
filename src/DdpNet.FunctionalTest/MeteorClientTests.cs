namespace DdpNet.FunctionalTest
{
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MeteorClientTests
    {
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
    }
}
