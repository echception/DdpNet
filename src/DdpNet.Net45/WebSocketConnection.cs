namespace DdpNet
{
    using System;
    using System.Collections.Concurrent;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Connection;

    internal class WebSocketConnection : IWebSocketConnection
    {
        private readonly Uri serverUri;

        private readonly ClientWebSocket client;
        private BlockingCollection<string> sendQueue;
        private Task sendingThread;

        private ArraySegment<byte> buffer;

        public WebSocketConnection(Uri serverUri)
        {
            this.serverUri = serverUri;
            this.client = new ClientWebSocket();

            this.sendQueue = new BlockingCollection<string>();
            this.buffer = new ArraySegment<byte>(new byte[1024]);
        }

        public async Task ConnectAsync()
        {
            await this.client.ConnectAsync(this.serverUri, CancellationToken.None);
            this.sendingThread = new Task(this.BackgroundSend, TaskCreationOptions.LongRunning);
            this.sendingThread.Start();
        }

        public Task CloseAsync()
        {
            this.sendQueue.CompleteAdding();
            return this.client.CloseAsync(WebSocketCloseStatus.Empty, string.Empty, CancellationToken.None);
        }

        public Task SendAsync(string text)
        {
            this.sendQueue.Add(text);
            return Task.FromResult(true);
        }

        public async Task<string> ReceiveAsync()
        {
            String fullMessage = string.Empty;
            bool completed = false;
            do
            {
                var result = await this.client.ReceiveAsync(this.buffer, CancellationToken.None);
                var message = Encoding.UTF8.GetString(this.buffer.Array, 0, result.Count);

                fullMessage += message;
                completed = result.EndOfMessage;
            } while (!completed);

            return fullMessage;
        }

        private async void BackgroundSend()
        {
            while (!this.sendQueue.IsCompleted)
            {
                var stringToSend = this.sendQueue.Take();

                var bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(stringToSend));
                await this.client.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}
