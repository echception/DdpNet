// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemovedHandler.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the RemovedHandler class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.MessageHandlers
{
    using System.Threading.Tasks;

    using DdpNet.Collections;
    using DdpNet.Connection;
    using DdpNet.Messages;
    using DdpNet.Results;

    using Newtonsoft.Json;

    /// <summary>
    /// Handles 'removed' messages from the server
    /// </summary>
    internal class RemovedHandler : BaseMessageHandler
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RemovedHandler"/> class. 
        /// Creates a new RemovedHandler
        /// </summary>
        public RemovedHandler()
            : base("removed")
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Processes the removed message
        /// </summary>
        /// <param name="client">
        /// The connection to send replies to the server
        /// </param>
        /// <param name="collectionManager">
        /// The CollectionManager
        /// </param>
        /// <param name="resultHandler">
        /// The ResultHandler
        /// </param>
        /// <param name="message">
        /// The message to process
        /// </param>
        /// <returns>
        /// Task to process the message
        /// </returns>
        public override Task HandleMessage(
            IDdpConnectionSender client, 
            ICollectionManager collectionManager, 
            IResultHandler resultHandler, 
            string message)
        {
            var removedMessage = JsonConvert.DeserializeObject<Removed>(message);
            collectionManager.Removed(removedMessage);

            return Task.FromResult(true);
        }

        #endregion
    }
}