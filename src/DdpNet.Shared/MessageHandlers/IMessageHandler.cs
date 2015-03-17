// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageHandler.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the MessageHandler interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.MessageHandlers
{
    using System.Threading.Tasks;

    using DdpNet.Collections;
    using DdpNet.Connection;
    using DdpNet.Results;

    /// <summary>
    /// The MessageHandler interface.  Exposes methods to handle messages received from the server
    /// </summary>
    internal interface IMessageHandler
    {
        #region Public Methods and Operators

        /// <summary>
        /// Determines if the handler can handle a specific message
        /// </summary>
        /// <param name="message">
        /// The message type.
        /// </param>
        /// <returns>
        /// True if it can handle this message type, false otherwise
        /// </returns>
        bool CanHandle(string message);

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
        /// The <see cref="Task"/>.
        /// </returns>
        Task HandleMessage(
            IDdpConnectionSender client, 
            ICollectionManager collectionManager, 
            IResultHandler resultHandler, 
            string message);

        #endregion
    }
}