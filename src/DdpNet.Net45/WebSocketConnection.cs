// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebSocketConnection.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the WebSocketConnection class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet
{
    using System;
    using System.Collections.Concurrent;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using DdpNet.Connection;

    /// <summary>
    /// Implementation of IWebSocketConnection that uses the .NET 4.5 ClientWebSocket class for communication
    /// </summary>
    internal class WebSocketConnection : IWebSocketConnection
    {
        #region Fields

        /// <summary>
        /// The client.
        /// </summary>
        private readonly ClientWebSocket client;

        /// <summary>
        /// The server uri.
        /// </summary>
        private readonly Uri serverUri;

        /// <summary>
        /// The read buffer.
        /// </summary>
        private ArraySegment<byte> buffer;

        /// <summary>
        /// The send queue.
        /// </summary>
        private BlockingCollection<string> sendQueue;

        /// <summary>
        /// The sending thread. The ClientWebSocket can only have one send, and one receive active at once.
        /// To allow Send to be called from multiple threads, all Sends are queued into a BlockingCollection.
        /// A background thread will take the items from the BlockingCollection, and perform the actual send operation.
        /// </summary>
        private Task sendingThread;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSocketConnection"/> class.
        /// </summary>
        /// <param name="serverUri">
        /// The server uri.
        /// </param>
        public WebSocketConnection(Uri serverUri)
        {
            this.serverUri = serverUri;
            this.client = new ClientWebSocket();

            this.sendQueue = new BlockingCollection<string>();
            this.buffer = new ArraySegment<byte>(new byte[1024]);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Closes the connection
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> which completes when the connection is closed.
        /// </returns>
        public Task CloseAsync()
        {
            this.sendQueue.CompleteAdding();
            return this.client.CloseAsync(WebSocketCloseStatus.Empty, string.Empty, CancellationToken.None);
        }

        /// <summary>
        ///  Opens the connection
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> which completes when the connection is open.
        /// </returns>
        public async Task ConnectAsync()
        {
            await this.client.ConnectAsync(this.serverUri, CancellationToken.None);
            this.sendingThread = new Task(this.BackgroundSend, TaskCreationOptions.LongRunning);
            this.sendingThread.Start();
        }

        /// <summary>
        /// Receive a message from the WebSocket connection
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> which completes when the message is ready.
        /// </returns>
        public async Task<string> ReceiveAsync()
        {
            string fullMessage = string.Empty;
            bool completed = false;
            do
            {
                var result = await this.client.ReceiveAsync(this.buffer, CancellationToken.None);
                var message = Encoding.UTF8.GetString(this.buffer.Array, 0, result.Count);

                fullMessage += message;
                completed = result.EndOfMessage;
            }
            while (!completed);

            return fullMessage;
        }

        /// <summary>
        /// Sends a message over the connection
        /// </summary>
        /// <param name="text">
        /// The text to send.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> which completes when the message is sent.
        /// </returns>
        public Task SendAsync(string text)
        {
            this.sendQueue.Add(text);
            return Task.FromResult(true);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The background send method. Will continually take items from the BlockingCollection and send them over the client connection
        /// </summary>
        private async void BackgroundSend()
        {
            while (!this.sendQueue.IsCompleted)
            {
                var stringToSend = this.sendQueue.Take();

                var bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(stringToSend));
                await this.client.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        #endregion
    }
}