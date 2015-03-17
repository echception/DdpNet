// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageHandler.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the MessageHandler type
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.MessageHandlers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using DdpNet.Collections;
    using DdpNet.Connection;
    using DdpNet.Results;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Responsible for handling messages received from the server. This contains a list of IMessageHandlers, and will
    /// delegate the actual message handling to the proper IMessageHandler
    /// </summary>
    internal class MessageHandler
    {
        #region Fields

        /// <summary>
        /// The handlers.
        /// </summary>
        private List<IMessageHandler> handlers;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandler"/> class.
        /// </summary>
        public MessageHandler()
        {
            this.GetHandlers();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Handles a given message
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <param name="collectionManager">
        /// The collection manager.
        /// </param>
        /// <param name="resultHandler">
        /// The result handler.
        /// </param>
        /// <param name="messageText">
        /// The message text.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task HandleMessage(
            IDdpConnectionSender client, 
            ICollectionManager collectionManager, 
            IResultHandler resultHandler, 
            string messageText)
        {
            JObject message = JObject.Parse(messageText);

            if (message["msg"] != null)
            {
                var msg = message["msg"];

                if (msg != null)
                {
                    var messageType = (string)msg;
                    foreach (var handler in this.handlers)
                    {
                        if (handler.CanHandle(messageType))
                        {
                            await handler.HandleMessage(client, collectionManager, resultHandler, messageText);
                        }
                    }
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the IMessageHandlers
        /// </summary>
        private void GetHandlers()
        {
            this.handlers = new List<IMessageHandler>();

            this.handlers.Add(new PingHandler());
            this.handlers.Add(new ReplyMessageHandler());
            this.handlers.Add(new AddedHandler());
            this.handlers.Add(new ChangedHandler());
            this.handlers.Add(new RemovedHandler());
        }

        #endregion
    }
}