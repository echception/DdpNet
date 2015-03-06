namespace DdpNet.FunctionalTest
{
    using DdpNet.Packages.Accounts;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AccountsTests
    {
        [TestMethod]
        [TestCategory("Functional")]
        public async Task Accounts_CreateUserWithUserName_ValidData()
        {
            var client = TestEnvironment.GetClient();
            await client.ConnectAsync();

            await client.CreateUserWithUserName("test", "test");

            Assert.IsNotNull(client.UserSession);

            Assert.AreEqual("test", client.User.UserName);
        }

        [TestMethod]
        [TestCategory("Functional")]
        public async Task Accounts_CreateUserWithUserName_AccountExists()
        {
            var client = TestEnvironment.GetClient();
            await client.ConnectAsync();

            await client.CreateUserWithUserName("test2", "test");

            Assert.IsNotNull(client.UserSession);

            await ExceptionAssert.AssertDdpServerExceptionThrown(async () => await client.CreateUserWithUserName("test2", "test"), "403");

            Assert.IsNotNull(client.UserSession);
        }

        [TestMethod]
        [TestCategory("Functional")]
        public async Task Accounts_CreateUserWithUserName_UserAlreadyLoggedIn()
        {
            var client = TestEnvironment.GetClient();
            await client.ConnectAsync();

            await client.CreateUserWithUserName("test3", "test");

            Assert.IsNotNull(client.UserSession);

            var oldSession = client.UserSession;

            await client.CreateUserWithUserName("test4", "test");

            Assert.IsNotNull(client.UserSession);

            var newSession = client.UserSession;
            Assert.AreNotEqual(oldSession.UserId, newSession.UserId);
            Assert.AreNotEqual(oldSession.Token, newSession.Token);
            Assert.AreNotEqual(oldSession.TokenExpiration, newSession.TokenExpiration);
        }
    }
}
