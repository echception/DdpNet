namespace DdpNet.UnitTest
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Connection;

    public class InMemoryConnection : IWebSocketConnection
    {
        private Queue<string> sendQueue;
        private Queue<string> receiveQueue;

        private object sendQueueSync = new object();
        private object receiveQueueSync = new object();

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
            lock (sendQueueSync)
            {
                this.sendQueue.Enqueue(text);
            }
            return Task.FromResult(true);
        }

        public Task<string> ReceiveAsync()
        {
            lock (receiveQueueSync)
            {
                if (this.receiveQueue.Any())
                {
                    var result = this.receiveQueue.Dequeue();
                    return Task.FromResult(result);
                }
            }

            return Task.FromResult(string.Empty);
        }

        public string GetSentMessage()
        {
            if (this.sendQueue.Any())
            {
                lock (sendQueueSync)
                {
                    return this.sendQueue.Dequeue();
                }
            }
            return string.Empty;
        }

        public void Reply(string message)
        {
            lock (receiveQueueSync)
            {
                this.receiveQueue.Enqueue(message);
            }
        }

        public void Dispose()
        {
           
        }
    }
}
