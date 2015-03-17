// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseMessageHandler.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the BaseMessageHandler type
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.MessageHandlers
{
    using System.Threading.Tasks;

    using DdpNet.Collections;
    using DdpNet.Connection;
    using DdpNet.Results;

    /// <summary>
    /// The base message handler. This provides a default implementation of IMessageHandler which can handle a specific message type
    /// </summary>
    internal abstract class BaseMessageHandler : IMessageHandler
    {
        #region Fields

        /// <summary>
        /// The type of message handled.
        /// </summary>
        private readonly string messageHandled;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMessageHandler"/> class.
        /// </summary>
        /// <param name="messageHandled">
        /// The message handled.
        /// </param>
        protected BaseMessageHandler(string messageHandled)
        {
            this.messageHandled = messageHandled;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Determines if this class can handle a given message
        /// </summary>
        /// <param name="message">
        /// The message type.
        /// </param>
        /// <returns>
        /// True if this can handle that message type, false otherwise
        /// </returns>
        public bool CanHandle(string message)
        {
            return string.Equals(this.messageHandled, message);
        }

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
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.  The task should be used to execute any long running operation (for instance, needing to send a message to the server as part of handling the message).
        /// </returns>
        public abstract Task HandleMessage(
            IDdpConnectionSender client, 
            ICollectionManager collectionManager, 
            IResultHandler resultHandler, 
            string message);

        #endregion
    }
}