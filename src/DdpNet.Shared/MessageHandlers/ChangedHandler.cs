// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangedHandler.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the ChangedHandler class
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
    /// Processes the 'changed' messages received from the server
    /// </summary>
    internal class ChangedHandler : BaseMessageHandler
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangedHandler"/> class. 
        /// Creates a new ChangedHandler
        /// </summary>
        public ChangedHandler()
            : base("changed")
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Processes the changed message
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
            var changedMessage = JsonConvert.DeserializeObject<Changed>(message);
            collectionManager.Changed(changedMessage);

            return Task.FromResult(true);
        }

        #endregion
    }
}