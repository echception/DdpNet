namespace DdpNet
{
    using System;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    internal class WebSocketConnection : IWebSocketConnection
    {
        private readonly Uri serverUri;

        private readonly ClientWebSocket client;

        public WebSocketConnection(Uri serverUri)
        {
            this.serverUri = serverUri;
            this.client = new ClientWebSocket();
        }

        public Task ConnectAsync()
        {
            return this.client.ConnectAsync(this.serverUri, CancellationToken.None);
        }

        public Task CloseAsync()
        {
            return this.client.CloseAsync(WebSocketCloseStatus.Empty, string.Empty, CancellationToken.None);
        }

        public Task SendAsync(string text)
        {
            var bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(text));
            return this.client.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task<string> ReceiveAsync()
        {
            var buffer = new ArraySegment<byte>(new byte[1024]);
            var result = await this.client.ReceiveAsync(buffer, CancellationToken.None);
            var message = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);

            return message;
        }
    }
}
