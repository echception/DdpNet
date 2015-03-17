// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DdpClient.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Defines the DdpClient type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet
{
    using System;
    using System.Threading.Tasks;
    using Collections;
    using Connection;
    using MessageHandlers;
    using Messages;
    using Newtonsoft.Json;
    using Results;

    /// <summary>
    /// A client for communicating with a DDP server.  This implements only the DDP protocol, it does not include account methods.
    /// For the account method, use the MeteorClient
    /// </summary>
    public class DdpClient : IDdpConnectionSender, IDdpRemoteMethodCall
    {
        /// <summary>
        /// The versions this client supports
        /// </summary>
        private readonly string[] supportedVersions = new[] { "1" };

        /// <summary>
        /// The preferred version for the client
        /// </summary>
        private readonly string preferredVersion = "1";

        /// <summary>
        /// The web socket connection.
        /// </summary>
        private readonly IWebSocketConnection webSocketConnection;

        /// <summary>
        /// The background task that processes incoming messages
        /// </summary>
        private Task receiveThread;

        /// <summary>
        /// The current connection state of the client
        /// </summary>
        private DdpClientState state;

        /// <summary>
        /// The MessageHandler for handling incoming messages
        /// </summary>
        private MessageHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="DdpClient"/> class.
        /// </summary>
        /// <param name="serverUri">
        /// The server uri.
        /// </param>
        public DdpClient(Uri serverUri) : this(new WebSocketConnection(serverUri))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DdpClient"/> class.
        /// </summary>
        /// <param name="webSocketConnection">
        /// The web socket connection.
        /// </param>
        internal DdpClient(IWebSocketConnection webSocketConnection)
        {
            this.webSocketConnection = webSocketConnection;
            this.state = DdpClientState.NotConnected;
            this.handler = new MessageHandler();
            this.ResultHandler = new ResultHandler();
            this.CollectionManager = new CollectionManager(this);
        }

        /// <summary>
        /// Gets the current DDP session ID. This will be set after a successful connection
        /// </summary>
        internal string SessionId { get; private set; }

        /// <summary>
        /// Gets or sets the ResultHandler for handling results
        /// Results are messages the server sends in response to a message the client sent,
        /// like a function call return value
        /// </summary>
        private ResultHandler ResultHandler { get; set; }

        /// <summary>
        /// Gets or sets the CollectionManager for this client
        /// </summary>
        private CollectionManager CollectionManager { get; set; }

        /// <summary>
        /// Connect the client to the server. This must be called prior to any other DdpClient methods.
        /// </summary>
        /// <returns>Task that completes when connection is established, or when the connection fails</returns>
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

            this.receiveThread = new Task(this.BackgroundReceive, TaskCreationOptions.LongRunning);
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

        /// <summary>
        /// Calls a method on the server with the given parameters
        /// </summary>
        /// <param name="methodName">The name of the server method to call</param>
        /// <param name="parameters">The parameters to pass to the method</param>
        /// <returns>Task that completes when the method has been fully processed</returns>
        public Task Call(string methodName, params object[] parameters)
        {
            this.VerifyConnected();

            return this.CallGetResult(methodName, parameters);
        }

        /// <summary>
        /// Calls a method on the server with the given parameters, and returns the result
        /// </summary>
        /// <typeparam name="T">The type of the return value</typeparam>
        /// <param name="methodName">The name of the server method to call</param>
        /// <param name="parameters">The parameters to pass to the method</param>
        /// <returns>Task that completes when the method has been fully processed, and the result is available</returns>
        public Task<T> Call<T>(string methodName, params object[] parameters)
        {
            this.VerifyConnected();

            return this.CallParseResult<T>(methodName, parameters);
        }

        /// <summary>
        /// Subscribes to a subscription with the given parameters
        /// </summary>
        /// <param name="subscriptionName">The name of the subscription</param>
        /// <param name="parameters">The parameters to subscript with</param>
        /// <returns>Task that completes when the subscription is complete (client has all the objects in the subscription).
        /// Return object is a handle to the subscription that can be passed to Unsubscribe </returns>
        public Task<Subscription> Subscribe(string subscriptionName, params object[] parameters)
        {
            this.VerifyConnected();

            return this.SubscribeWithParameters(subscriptionName, parameters);
        }

        /// <summary>
        /// Unsubscribes from a subscription.
        /// </summary>
        /// <param name="subscription">The subscription to unsubscribe</param>
        /// <returns>Task that completes when the unsubscribe is complete (client has removed all objects in the subscription)</returns>
        public Task Unsubscribe(Subscription subscription)
        {
            return this.UnsubscribeInternal(subscription);
        }

        /// <summary>
        /// Gets a typed collection. A collection name can have only one type.
        /// </summary>
        /// <typeparam name="T">The type of the collection</typeparam>
        /// <param name="collectionName">The name of the collection</param>
        /// <returns>The typed collection. Note that the client will store objects in an un-typed state if it doesn't know the type.
        /// Once this has been called with a type, it will convert the un-typed objects to a typed collection.
        /// This means it is slightly faster to call GetCollection prior to a collection having objects added (i.e. prior to subscribing)</returns>
        public DdpCollection<T> GetCollection<T>(string collectionName) where T : DdpObject
        {
            return this.CollectionManager.GetCollection<T>(collectionName);
        }

        /// <summary>
        /// Serializes an object and sends the serialized form to the server.
        /// </summary>
        /// <param name="objectToSend">The object to serialize and send</param>
        /// <returns>Task that completes when the object has been serialized and queued for sending</returns>
        Task IDdpConnectionSender.SendObject(object objectToSend)
        {
            return this.webSocketConnection.SendAsync(JsonConvert.SerializeObject(objectToSend));
        }

        /// <summary>
        /// Waits for a single message to be received, and processes that message.
        /// </summary>
        /// <returns>Task that completes when a message has been processed</returns>
        internal async Task ReceiveAsync()
        {
            var result = await this.webSocketConnection.ReceiveAsync();

            if (!string.IsNullOrWhiteSpace(result))
            {
                await this.handler.HandleMessage(this, this.CollectionManager, this.ResultHandler, result);
            }
        }

        /// <summary>
        /// Serializes an object and sends the serialized form to the server.
        /// </summary>
        /// <param name="objectToSend">The object to serialize and send</param>
        /// <returns>Task that completes when the object has been serialized and queued for sending</returns>
        private Task SendObject(object objectToSend)
        {
            return ((IDdpConnectionSender)this).SendObject(objectToSend);
        }

        /// <summary>
        /// Sets the client's current Session ID
        /// </summary>
        /// <param name="session">The current Session ID</param>
        private void SetSession(string session)
        {
            if (!string.IsNullOrWhiteSpace(this.SessionId))
            {
                throw new InvalidOperationException("Session has already been set.");
            }

            this.SessionId = session;
        }

        /// <summary>
        /// Entry point for the background receive thread. Continually gets new messages and processes them
        /// </summary>
        private async void BackgroundReceive()
        {
            while (true)
            {
                await this.ReceiveAsync();
            }
        }

        /// <summary>
        /// Verifies the client is connected. Throws an exception if the client is not connected
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ConnectAsync", Justification = "Async is a valid word in this context")]
        private void VerifyConnected()
        {
            if (this.state != DdpClientState.Connected)
            {
                throw new InvalidOperationException("ConnectAsync() must be called before any client methods can be called");
            }
        }

        /// <summary>
        /// Subscribes to a subscription with the given parameters
        /// </summary>
        /// <param name="subscriptionName">The name of the subscription</param>
        /// <param name="parameters">The parameters to subscribe with</param>
        /// <returns>Task that completes when the subscription is complete (all objects are synced to the client).
        /// Return object is a handle to the subscription</returns>
        private async Task<Subscription> SubscribeWithParameters(string subscriptionName, object[] parameters)
        {
            var id = Utilities.GenerateId();
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

            var subscription = new Subscription(id, subscriptionName);

            return subscription;
        }

        /// <summary>
        /// Unsubscribes from a subscription
        /// </summary>
        /// <param name="subscription">The subscription to unsubscribe from</param>
        /// <returns>Task that completes when the unsubscribe is complete (all objects in the subscription are removed from the client store)</returns>
        private async Task UnsubscribeInternal(Subscription subscription)
        {
            var unsubscribe = new Unsubscribe(subscription.Id);

            var unsubscribeWaitHandler =
                this.ResultHandler.RegisterWaitHandler(
                    ResultFilterFactory.CreateUnsubscribeResultFilter(subscription.Id));

            await this.SendObject(unsubscribe);

            var returnedObject = await this.ResultHandler.WaitForResult(unsubscribeWaitHandler);

            var nosub = returnedObject.ParsedObject.ToObject<NoSubscribe>();

            if (nosub.Error != null)
            {
                throw new DdpServerException(nosub.Error);
            }
        }

        /// <summary>
        /// Calls a method on the server, and parses the return value to the specified type
        /// </summary>
        /// <typeparam name="T">The type to parse the return object to</typeparam>
        /// <param name="methodName">The name of the method to call</param>
        /// <param name="parameters">The parameters to call the method with</param>
        /// <returns>Task that completes when the method call is complete (server returns a value for the call).
        /// Return object is the object returned from the server, parsed to the given type.</returns>
        private async Task<T> CallParseResult<T>(string methodName, object[] parameters)
        {
            var resultObject = await this.CallGetResult(methodName, parameters);

            if (resultObject.ResultObject == null)
            {
                throw new InvalidOperationException("Server did not return an object when a return value was expected");
            }

            return resultObject.ResultObject.ToObject<T>();
        }

        /// <summary>
        /// Calls a method on the server, with the given parameters
        /// </summary>
        /// <param name="methodName">Name of the method to call</param>
        /// <param name="parameters">The parameters to call the method with</param>
        /// <returns>Task that completes when the method call is complete (server returns a value for the call).
        /// Return object is the object returned from the server, in an unparsed form.</returns>
        private async Task<Result> CallGetResult(string methodName, object[] parameters)
        {
            var id = Utilities.GenerateId();

            var resultWaitHandler =
                this.ResultHandler.RegisterWaitHandler(ResultFilterFactory.CreateCallResultFilter(id));

            var method = new Method() { MethodName = methodName, Parameters = parameters, Id = id };
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
