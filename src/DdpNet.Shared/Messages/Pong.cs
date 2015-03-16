// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Pong.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the Pong type
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.Messages
{
    using Newtonsoft.Json;

    /// <summary>
    /// The Pong message, sent in response to a Ping message
    /// </summary>
    internal class Pong : BaseMessage
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Pong"/> class.
        /// </summary>
        public Pong()
            : base("pong")
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the id that was sent in the Ping message.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        #endregion
    }
}