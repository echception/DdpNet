namespace DdpNet.FunctionalTest
{
    using System;
    using System.Threading.Tasks;
    using DataObjects;
    using Packages.Accounts;

    public class TestEnvironment
    {
        public const string TestUserName = "TestUserName";
        public const string TestUserPassword = "TestPassword";

        public static MeteorClient GetClient()
        {
            return new MeteorClient(new Uri("ws://localhost:3000/websocket"));
        }

        public static void Cleanup()
        {
            var client = GetClient();
            client.ConnectAsync().Wait();
            client.Call("cleanup");
        }

        public static async Task AddDenyAllEntry(MeteorClient client, Entry entry)
        {
            await client.Call("addDenyAll", entry);
        }

        public static async Task InitializeTestUser()
        {
            var client = GetClient();
            await client.ConnectAsync();

            await client.CreateUserWithUserName(TestUserName, TestUserPassword);

            await client.Logout();
        }
    }
}
