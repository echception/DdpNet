// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PingHandler.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the PingHandler type
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
    /// Handles 'ping' messages from the server
    /// </summary>
    internal class PingHandler : BaseMessageHandler
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PingHandler"/> class. 
        /// Creates a new PingHandler
        /// </summary>
        public PingHandler()
            : base("ping")
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Processes the ping message
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
            var pingMessage = JsonConvert.DeserializeObject<Ping>(message);
            var pongReply = new Pong() { Id = pingMessage.Id };

            return client.SendObject(pongReply);
        }

        #endregion
    }
}