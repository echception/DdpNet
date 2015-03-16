// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseMessage.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the BaseMessage type
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.Messages
{
    using Newtonsoft.Json;

    /// <summary>
    /// The base type for messages received from the server
    /// </summary>
    internal class BaseMessage
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMessage"/> class.
        /// </summary>
        /// <param name="messageType">
        /// The message type.
        /// </param>
        protected BaseMessage(string messageType)
        {
            this.MessageType = messageType;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the message type.
        /// </summary>
        [JsonProperty(PropertyName = "msg")]
        public string MessageType { get; set; }

        #endregion
    }
}