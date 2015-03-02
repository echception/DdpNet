namespace DdpNet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Collections;
    using Connection;
    using MessageHandlers;
    using Messages;
    using Newtonsoft.Json;
    using Results;

    public class DdpClient : IDdpConnectionSender, IDdpRemoteMethodCall
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

        private readonly Dictionary<string, string> subscriptions; 

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

        public async Task ConnectAsync()
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

            this.receiveThread = new Thread(this.BackgroundReceive);
            this.receiveThread.IsBackground = true;
            this.receiveThread.Start();

            var resultMessage = await this.ResultHandler.WaitForResult(waitHandle);

            if (resultMessage.MessageType == "failed")
            {
                throw new InvalidOperationException("Server version is incompatible with the version of this client");
            }

            var connected = JsonConvert.DeserializeObject<Connected>(resultMessage.Message);
            this.SetSession(connected.Session);

            this.state = DdpClientState.Connected;
        }

        private Task SendObject(object objectToSend)
        {
            return ((IDdpConnectionSender) this).SendObject(objectToSend);
        }

        Task IDdpConnectionSender.SendObject(object objectToSend)
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
                await this.handler.HandleMessage(this, this.CollectionManager, this.ResultHandler, result);
            }
           
        }

        public Task Call(string methodName, params object[] parameters)
        {
            this.VerifyConnected();

            return this.CallGetResult(methodName, parameters);
        }

        public Task<T> Call<T>(string methodName, params object[] parameters)
        {
            this.VerifyConnected();

            return this.CallParseResult<T>(methodName, parameters);
        }

        public Task Subscribe(string subscriptionName, params object[] parameters)
        {
            this.VerifyConnected();

            return this.SubscribeWithParameters(subscriptionName, parameters);
        }

        public Task Unsubscribe(string subscriptionName)
        {
            return this.UnsubscribeInternal(subscriptionName);
        }

        public DdpCollection<T> GetCollection<T>(string collectionName) where T: DdpObject
        {
            return this.CollectionManager.GetCollection<T>(collectionName);
        }

        private void VerifyConnected()
        {
            if (this.state != DdpClientState.Connected)
            {
                throw new InvalidOperationException("DdpClient.ConnectAsync must be called before any client methods can be called");
            }
        }

        private async Task SubscribeWithParameters(string subscriptionName, object[] parameters)
        {
            if (!this.subscriptions.ContainsKey(subscriptionName))
            {
                var id = Utilities.GenerateID();
                var sub = new Subscribe(id, subscriptionName, parameters);

                var readyWaitHandler =
                    this.ResultHandler.RegisterWaitHandler(ResultFilterFactory.CreateSubscribeResultFilter(id));

                await this.SendObject(sub);

                var returnedObject = await this.ResultHandler.WaitForResult(readyWaitHandler);

                if (returnedObject.MessageType == "nosub")
                {
                    var noSub = returnedObject.ParsedObject.ToObject<NoSubscribe>();

                    throw new DdpServerException(noSub.Error);
                }

                this.subscriptions.Add(subscriptionName, id);
            }
        }

        private async Task UnsubscribeInternal(string subscriptionName)
        {
            string subscriptionId;

            if (this.subscriptions.TryGetValue(subscriptionName, out subscriptionId))
            {
                var unsubscribe = new Unsubscribe(subscriptionId);

                var unsubscribeWaitHandler =
                    this.ResultHandler.RegisterWaitHandler(
                        ResultFilterFactory.CreateUnsubscribeResultFilter(subscriptionId));

                await this.SendObject(unsubscribe);

                var returnedObject = await this.ResultHandler.WaitForResult(unsubscribeWaitHandler);

                var nosub = returnedObject.ParsedObject.ToObject<NoSubscribe>();

                if (nosub.Error != null)
                {
                    throw new DdpServerException(nosub.Error);
                }
            }
        }

        private async Task<T> CallParseResult<T>(string methodName, object[] parameters)
        {
            var resultObject = await this.CallGetResult(methodName, parameters);

            if (resultObject.ResultObject == null)
            {
                throw new InvalidOperationException("Server did not return an object when a return value was expected");
            }

            return resultObject.ResultObject.ToObject<T>();
        }

        private async Task<Result> CallGetResult(string methodName, object[] parameters)
        {
            var id = Utilities.GenerateID();

            var resultWaitHandler =
                this.ResultHandler.RegisterWaitHandler(ResultFilterFactory.CreateCallResultFilter(id));

            var method = new Method() {MethodName = methodName, Parameters = parameters, ID = id};
            await this.SendObject(method);

            var result = await this.ResultHandler.WaitForResult(resultWaitHandler);

            var resultObject = JsonConvert.DeserializeObject<Result>(result.Message);

            if (resultObject.Error != null)
            {
                throw new DdpServerException(resultObject.Error);
            }

            return resultObject;
        }
    }
}
