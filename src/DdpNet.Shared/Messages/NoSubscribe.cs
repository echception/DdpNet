// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NoSubscribe.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the NoSubscribe class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.Messages
{
    using DdpNet.ReturnedObjects;

    using Newtonsoft.Json;

    /// <summary>
    /// The NoSubscribe message, sent to the client to inform it the subscription was either unsubscribed, or for a failed subscription
    /// </summary>
    internal class NoSubscribe : BaseMessage
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NoSubscribe"/> class.
        /// </summary>
        protected NoSubscribe()
            : base("nosub")
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the associated error, if present.
        /// </summary>
        [JsonProperty(PropertyName = "error")]
        public Error Error { get; set; }

        /// <summary>
        /// Gets or sets the id of the subscription.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string ID { get; set; }

        #endregion
    }
}