using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DdpNet.Test.Console
{
    using Console = System.Console;

    class Program
    {
        static void Main(string[] args)
        {
            var client = new MeteorClient(new ConsoleLoggingConnection(new WebSocketConnection(new Uri("ws://localhost:3000/websocket"))));
            client.ConnectAsync().Wait();

            client.LoginPassword("chris", "password");

            Console.ReadKey();
        }
    }
}
