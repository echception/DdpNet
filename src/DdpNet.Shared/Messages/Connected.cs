// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Connected.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the Connected type
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.Messages
{
    using Newtonsoft.Json;

    /// <summary>
    /// The Connected message from the server, to inform the client of a successful connection
    /// </summary>
    internal class Connected : BaseMessage
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Connected"/> class.
        /// </summary>
        internal Connected()
            : base("connected")
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the session.
        /// </summary>
        [JsonProperty(PropertyName = "session")]
        public string Session { get; set; }

        #endregion
    }
}