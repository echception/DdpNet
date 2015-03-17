// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddedHandler.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the AddedHandler type
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
    /// Processes 'added' messages received from the server
    /// </summary>
    internal class AddedHandler : BaseMessageHandler
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AddedHandler"/> class. 
        /// Creates a new AddedHandler
        /// </summary>
        public AddedHandler()
            : base("added")
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Processes the added message
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
            var parsedObject = JsonConvert.DeserializeObject<Added>(message);
            collectionManager.Added(parsedObject);

            return Task.FromResult(true);
        }

        #endregion
    }
}