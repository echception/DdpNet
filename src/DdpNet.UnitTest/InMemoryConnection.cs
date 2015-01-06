using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DdpNet.UnitTest
{
    public class InMemoryConnection : IWebSocketConnection
    {
        private Queue<string> sendQueue;
        private Queue<string> receiveQueue;

        public InMemoryConnection()
        {
            this.sendQueue = new Queue<string>();
            this.receiveQueue = new Queue<string>();
        }

        public Task ConnectAsync()
        {
            return Task.FromResult(true);
        }

        public Task CloseAsync()
        {
            return Task.FromResult(true);
        }

        public Task SendAsync(string text)
        {
            this.sendQueue.Enqueue(text);
            return Task.FromResult(true);
        }

        public Task<string> ReceiveAsync()
        {
            var result = this.receiveQueue.Dequeue();
            return Task.FromResult(result);
        }

        public string GetSentMessage()
        {
            return this.sendQueue.Dequeue();
        }

        public void Reply(string message)
        {
            this.receiveQueue.Enqueue(message);
        }
    }
}
