using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DdpNet.Test.Console
{
    using System.Collections.Specialized;
    using System.Threading;
    using Connection;
    using Console = System.Console;

    class Program
    {
        static void Main(string[] args)
        {
            Run().Wait();
        }

        static async Task Run()
        {
            var meteorClient = new MeteorClient(new ConsoleLoggingConnection(new WebSocketConnection(new Uri("ws://localhost:3000/websocket"))));
            await meteorClient.ConnectAsync();

            var entryCollection = meteorClient.GetCollection<Entry>("entries");
            await meteorClient.Subscribe("entries");

            var inserts = 100;
            var currentCount = entryCollection.Count;
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < inserts; i++)
            {
                var entry = new Entry()
                {
                    Count = i,
                    IsActive = true,
                    Name = "Item " + i.ToString()
                };

                tasks.Add(entryCollection.AddAsync(entry));
            }

            Task.WaitAll(tasks.ToArray());

            var newCount = entryCollection.Count;
        }
    }
}
