// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Connect.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the Connect type
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.Messages
{
    using Newtonsoft.Json;

    /// <summary>
    /// The Connect message sent to the server to initiate a connection
    /// </summary>
    internal class Connect : BaseMessage
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Connect"/> class.
        /// </summary>
        public Connect()
            : base("connect")
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the session ID.
        /// </summary>
        [JsonProperty(PropertyName = "session")]
        public string Session { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the versions supported.
        /// </summary>
        [JsonProperty(PropertyName = "support")]
        public string[] VersionsSupported { get; set; }

        #endregion
    }
}