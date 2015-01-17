namespace DdpNet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Collections;
    using MessageHandlers;
    using Messages;
    using Newtonsoft.Json;
    using Results;

    public class DdpClient
    {
        private readonly IWebSocketConnection webSocketConnection;

        private readonly string[] supportedVersions = new[] {"1"};

        private readonly string preferredVersion = "1";

        private Thread receiveThread;

        internal string SessionId { get; private set; }

        private DdpClientState state;

        private MessageHandler handler;

        internal ResultHandler ResultHandler { get; private set; }

        internal CollectionManager CollectionManager { get; private set; }

        private Dictionary<string, string> subscriptions; 

        public DdpClient(Uri serverUri) : this(new WebSocketConnection(serverUri))
        {
            
        }

        internal DdpClient(IWebSocketConnection webSocketConnection)
        {
            this.webSocketConnection = webSocketConnection;
            this.state = DdpClientState.NotConnected;
            this.handler = new MessageHandler();
            this.ResultHandler = new ResultHandler();
            this.CollectionManager = new CollectionManager(this);
            this.subscriptions = new Dictionary<string, string>();
        }

        internal async Task ConnectAsync(bool startBackgroundThread)
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

            var waitHandle = this.ResultHandler.RegisterWaitHandler(ResultFilterFactory.CreateConnectResultFilter());

            await this.SendObject(connectMessage);

            if (startBackgroundThread)
            {
                this.receiveThread = new Thread(this.BackgroundReceive);
                this.receiveThread.IsBackground = true;
                this.receiveThread.Start();
            }

            var resultMessage = await this.ResultHandler.WaitForResult(waitHandle);

            if (resultMessage.MessageType == "failed")
            {
                throw new InvalidOperationException("Server version is incompatible with the version of this client");
            }

            var connected = JsonConvert.DeserializeObject<Connected>(resultMessage.Message);
            this.SetSession(connected.Session);

            this.state = DdpClientState.Connected;
        }

        public Task ConnectAsync()
        {
            return this.ConnectAsync(true);
        }

        internal Task SendObject(object objectToSend)
        {
            return this.webSocketConnection.SendAsync(JsonConvert.SerializeObject(objectToSend));
        }

        private void SetSession(string session)
        {
            if (!string.IsNullOrWhiteSpace(this.SessionId))
            {
                throw new InvalidOperationException("Session has already been set.");
            }

            this.SessionId = session;
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

            if (!String.IsNullOrWhiteSpace(result))
            {
                await this.handler.HandleMessage(this, result);
            }
        }

        public Task Call(string methodName)
        {
            return this.Call(methodName, new List<object>());
        }

        public Task<T> Call<T>(string methodName)
        {
            return this.Call<T>(methodName, new List<object>());
        }

        public Task Call(string methodName, List<object> parameters)
        {
            return this.CallGetResult(methodName, parameters);
        }

        public async Task<T> Call<T>(string methodName, List<object> parameters)
        {
            var resultObject = await this.CallGetResult(methodName, parameters);

            if (resultObject.ResultObject == null)
            {
                throw new InvalidOperationException("Server did not return an object when a return value was expected");
            }

            return resultObject.ResultObject.ToObject<T>();
        }

        public Task Subscribe(string subscriptionName)
        {
            return this.Subscribe(subscriptionName, new List<object>());
        }

        public async Task Subscribe(string subscriptionName, List<object> parameters)
        {
            var id = Utilities.GenerateID();
            this.subscriptions.Add(subscriptionName, id);
            var sub = new Subscribe(id, subscriptionName, parameters.ToArray());

            var readyWaitHandler = this.ResultHandler.RegisterWaitHandler(ResultFilterFactory.CreateSubscribeResultFilter(id));

            await this.SendObject(sub);

            await this.ResultHandler.WaitForResult(readyWaitHandler);
        }

        public DdpCollection<T> GetCollection<T>(string collectionName) where T: DdpObject
        {
            return this.CollectionManager.GetCollection<T>(collectionName);
        }

        private async Task<Result> CallGetResult(string methodName, List<object> parameters)
        {
            var id = Utilities.GenerateID();

            var resultWaitHandler =
                this.ResultHandler.RegisterWaitHandler(ResultFilterFactory.CreateCallResultFilter(id));

            var method = new Method(methodName, parameters, id);
            await this.SendObject(method);

            var result = await this.ResultHandler.WaitForResult(resultWaitHandler);

            var resultObject = JsonConvert.DeserializeObject<Result>(result.Message);

            if (resultObject.Error != null)
            {
                throw new InvalidOperationException(string.Format("Server returned an error {0}. Details: {1}. Reason: {2}", resultObject.Error.ErrorMessage, resultObject.Error.Details, resultObject.Error.Reason));
            }

            return resultObject;
        }
    }
}
