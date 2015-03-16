// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Ping.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the Ping class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.Messages
{
    using Newtonsoft.Json;

    /// <summary>
    /// The Ping message, usually sent to the client to ensure its still alive
    /// </summary>
    internal class Ping : BaseMessage
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Ping"/> class.
        /// </summary>
        public Ping()
            : base("ping")
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the id, which should be used in the response Pong.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        #endregion
    }
}