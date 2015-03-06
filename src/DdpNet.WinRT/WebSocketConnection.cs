namespace DdpNet
{
    using System;
    using System.Collections.Concurrent;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Windows.Networking.Sockets;
    using Windows.Storage.Streams;
    using Connection;
    using UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding;

    internal class WebSocketConnection : IWebSocketConnection
    {
        private MessageWebSocket messageWebSocket;
        private readonly Uri serverUri;

        private BlockingCollection<string> sendQueue;
        private BlockingCollection<string> receiveQueue; 
        private Task sendingThread;

        public WebSocketConnection(Uri serverUri)
        {
            this.serverUri = serverUri;
            this.messageWebSocket = new MessageWebSocket();

            this.sendQueue = new BlockingCollection<string>();
            this.receiveQueue = new BlockingCollection<string>();
        }

        public async Task ConnectAsync()
        {
            await this.messageWebSocket.ConnectAsync(this.serverUri);
            this.messageWebSocket.Control.MessageType = SocketMessageType.Utf8;
            this.messageWebSocket.MessageReceived += MessageWebSocketOnMessageReceived;
            this.sendingThread = new Task(this.BackgroundSend, TaskCreationOptions.LongRunning);
            this.sendingThread.Start();
        }

        private void MessageWebSocketOnMessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
        {
            using (var reader = args.GetDataReader())
            {
                reader.UnicodeEncoding = UnicodeEncoding.Utf8;
                var stringRead = reader.ReadString(reader.UnconsumedBufferLength);
                this.receiveQueue.Add(stringRead);
            }
        }

        public Task CloseAsync()
        {
            this.sendQueue.CompleteAdding();
            this.messageWebSocket.Close(0, "Close requested");

            return Task.FromResult(true);
        }

        public Task SendAsync(string text)
        {
            this.sendQueue.Add(text);
            return Task.FromResult(true);
        }

        public Task<string> ReceiveAsync()
        {
            return Task.Run(() => this.receiveQueue.Take());
        }

        private async void BackgroundSend()
        {
            DataWriter messageWriter = new DataWriter(this.messageWebSocket.OutputStream);
            while (!this.sendQueue.IsCompleted)
            {
                string stringToSend = this.sendQueue.Take();

                messageWriter.WriteString(stringToSend);
                await messageWriter.StoreAsync();
            }
        }
    }
}
