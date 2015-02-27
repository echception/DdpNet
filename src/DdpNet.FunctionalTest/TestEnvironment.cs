namespace DdpNet.FunctionalTest
{
    using System;

    public class TestEnvironment
    {
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
    }
}
