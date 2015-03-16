// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Updated.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the Updated type
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.Messages
{
    using Newtonsoft.Json;

    /// <summary>
    /// The Updated message, sent to the client to inform it that a method call has finished modifying the date it is subscribed to
    /// </summary>
    internal class Updated : BaseMessage
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Updated"/> class.
        /// </summary>
        internal Updated()
            : base("updated")
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the methods whose writes are completed
        /// </summary>
        [JsonProperty(PropertyName = "methods")]
        public string[] Methods { get; set; }

        #endregion
    }
}