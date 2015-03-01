namespace DdpNet.Connection
{
    using System;
    using System.Collections.Concurrent;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using NLog;

    internal class WebSocketConnection : IWebSocketConnection
    {
        private readonly Uri serverUri;

        private readonly ClientWebSocket client;
        private BlockingCollection<string> sendQueue;
        private Thread sendingThread;

        private static Logger logger = LogManager.GetCurrentClassLogger();


        public WebSocketConnection(Uri serverUri)
        {
            this.serverUri = serverUri;
            this.client = new ClientWebSocket();

            this.sendQueue = new BlockingCollection<string>();
        }

        public async Task ConnectAsync()
        {
            await this.client.ConnectAsync(this.serverUri, CancellationToken.None);
            this.sendingThread = new Thread(BackgroundSend);
            this.sendingThread.IsBackground = true;
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
            var buffer = new ArraySegment<byte>(new byte[2048]);
            var receiveTask = this.client.ReceiveAsync(buffer, CancellationToken.None);
            receiveTask.Wait();
            if (receiveTask.Exception != null)
            {
                System.Diagnostics.Debug.WriteLine(receiveTask.Exception.ToString());
            }
            var result = receiveTask.Result;
            var message = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);

            logger.Trace(string.Format("Recv: {0}", message));

            return message;
        }

        private async void BackgroundSend()
        {
            while (!this.sendQueue.IsCompleted)
            {
                var stringToSend = this.sendQueue.Take();

                logger.Trace("Send: {0}", stringToSend);

                var bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(stringToSend));
                var sendTask = this.client.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
                sendTask.Wait();

                if (sendTask.Exception != null)
                {
                    System.Diagnostics.Debug.WriteLine(sendTask.Exception.ToString());
                }
            }
        }
    }
}
