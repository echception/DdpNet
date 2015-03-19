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
    using System.Threading.Tasks;

    using DdpNet.Connection;

    using Windows.Networking.Sockets;
    using Windows.Storage.Streams;

    /// <summary>
    /// Implementation of IWebSocketConnection for WinRT
    /// </summary>
    internal class WebSocketConnection : IWebSocketConnection
    {
        #region Fields

        /// <summary>
        /// The server uri.
        /// </summary>
        private readonly Uri serverUri;

        /// <summary>
        /// Indicates if this has been disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// The message web socket.
        /// </summary>
        private MessageWebSocket messageWebSocket;

        /// <summary>
        /// The receive queue.
        /// </summary>
        private BlockingCollection<string> receiveQueue;

        /// <summary>
        /// The send queue.
        /// </summary>
        private BlockingCollection<string> sendQueue;

        /// <summary>
        /// The sending thread.
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
            this.messageWebSocket = new MessageWebSocket();

            this.sendQueue = new BlockingCollection<string>();
            this.receiveQueue = new BlockingCollection<string>();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="WebSocketConnection"/> class. 
        /// Finalizer to cleanup resources
        /// </summary>
        ~WebSocketConnection()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Closes the connection
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task CloseAsync()
        {
            this.sendQueue.CompleteAdding();
            this.messageWebSocket.Close(0, "Close requested");

            return Task.FromResult(true);
        }

        /// <summary>
        /// Starts the connection
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task ConnectAsync()
        {
            this.messageWebSocket.Control.MessageType = SocketMessageType.Utf8;
            this.messageWebSocket.MessageReceived += this.MessageWebSocketOnMessageReceived;

            await this.messageWebSocket.ConnectAsync(this.serverUri);

            this.sendingThread = new Task(this.BackgroundSend, TaskCreationOptions.LongRunning);
            this.sendingThread.Start();
        }

        /// <summary>
        /// Cleanup resources
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The background receive thread entry point
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task<string> ReceiveAsync()
        {
            return Task.Run(() => this.receiveQueue.Take());
        }

        /// <summary>
        /// Sends an item to the server
        /// </summary>
        /// <param name="text">
        /// The text to send.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task SendAsync(string text)
        {
            this.sendQueue.Add(text);
            return Task.FromResult(true);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Cleanup resources
        /// </summary>
        /// <param name="disposing">
        /// True if called from user code, false if called from the finalizer
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.messageWebSocket.Dispose();
                    this.receiveQueue.Dispose();
                    this.sendQueue.Dispose();
                }

                this.disposed = true;
            }
        }

        /// <summary>
        /// The background send method.
        /// </summary>
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

        /// <summary>
        /// The message web socket on message received.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        private void MessageWebSocketOnMessageReceived(
            MessageWebSocket sender, 
            MessageWebSocketMessageReceivedEventArgs args)
        {
            using (var reader = args.GetDataReader())
            {
                reader.UnicodeEncoding = UnicodeEncoding.Utf8;
                var stringRead = reader.ReadString(reader.UnconsumedBufferLength);
                this.receiveQueue.Add(stringRead);
            }
        }

        #endregion
    }
}