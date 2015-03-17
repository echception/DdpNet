// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReplyMessageHandler.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the ReplyMessageHandler class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.MessageHandlers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using DdpNet.Collections;
    using DdpNet.Connection;
    using DdpNet.Results;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Handles all messages from the server that are sent in response to a client action
    /// </summary>
    internal class ReplyMessageHandler : IMessageHandler
    {
        #region Static Fields

        /// <summary>
        /// All the message types to handle
        /// </summary>
        private static readonly string[] ResultMessageTypes = new[]
                                                                  {
                                                                      "connected", "failed", "result", "ready", "updated", 
                                                                      "nosub"
                                                                  };

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Determines if this class can handle a given message
        /// </summary>
        /// <param name="message">
        /// The message to see if we can handle
        /// </param>
        /// <returns>
        /// True if it can handle it, false otherwise
        /// </returns>
        public bool CanHandle(string message)
        {
            return ResultMessageTypes.Contains(message, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Processes the reply message
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
        public Task HandleMessage(
            IDdpConnectionSender client, 
            ICollectionManager collectionManager, 
            IResultHandler resultHandler, 
            string message)
        {
            JObject parsedObject = JObject.Parse(message);
            var result = new ReturnedObject((string)parsedObject["msg"], parsedObject, message);

            resultHandler.AddResult(result);

            return Task.FromResult(true);
        }

        #endregion
    }
}