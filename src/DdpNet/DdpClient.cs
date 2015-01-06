namespace DdpNet
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Messages;
    using Newtonsoft.Json;

    public class DdpClient
    {
        private readonly IWebSocketConnection webSocketConnection;

        private readonly string[] supportedVersions = new[] {"1"};

        private readonly string preferredVersion = "1";

        private Thread receiveThread;

        private string sessionId;

        private DdpClientState state;

        private MessageHandler handler;

        public DdpClient(Uri serverUri) : this(new WebSocketConnection(serverUri))
        {
            
        }

        internal DdpClient(IWebSocketConnection webSocketConnection)
        {
            this.webSocketConnection = webSocketConnection;
            this.state = DdpClientState.NotConnected;
            this.handler = new MessageHandler();
        }

        public async Task ConnectAsync()
        {
            this.ConnectAsync(true);
        }

        internal async Task ConnectAsync(bool startReceiveThread)
        {
            if (this.state != DdpClientState.NotConnected)
            {
                throw new InvalidOperationException("Client is already connected");
            }

            await this.webSocketConnection.ConnectAsync();

            var connectMessage = new Connect
            {
                Version = this.preferredVersion,
                VersionsSupported = this.supportedVersions
            };

            await this.SendObject(connectMessage);

            if (startReceiveThread)
            {
                this.receiveThread = new Thread(this.BackgroundReceive);
                this.receiveThread.IsBackground = true;
                this.receiveThread.Start();
            }

            this.state = DdpClientState.Connected;
        }

        internal Task SendObject(object objectToSend)
        {
            return this.webSocketConnection.SendAsync(JsonConvert.SerializeObject(objectToSend));
        }

        internal void SetSession(string session)
        {
            if (!string.IsNullOrWhiteSpace(this.sessionId))
            {
                throw new InvalidOperationException("Session has already been set.");
            }

            this.sessionId = session;
        }

        private async void BackgroundReceive()
        {
            while (true)
            {
                await this.ReceiveAsync();
            }
        }

        internal async Task ReceiveAsync()
        {
            var result = await this.webSocketConnection.ReceiveAsync();
            this.handler.HandleMessage(this, result);
        }
    }
}
