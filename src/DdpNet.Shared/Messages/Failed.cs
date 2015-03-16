// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Failed.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the Failed type
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.Messages
{
    using Newtonsoft.Json;

    /// <summary>
    /// The Failed message, sent from the server to inform the client of a failed connection
    /// </summary>
    internal class Failed : BaseMessage
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Failed"/> class.
        /// </summary>
        internal Failed()
            : base("failed")
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the version the server suggests.
        /// </summary>
        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }

        #endregion
    }
}