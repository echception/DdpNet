// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Unsubscribe.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the Unsubscribe message
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.Messages
{
    using Newtonsoft.Json;

    /// <summary>
    /// The Unsubscribe message, sent to the server to unsubscribe from a subscription
    /// </summary>
    internal class Unsubscribe : BaseMessage
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Unsubscribe"/> class.
        /// </summary>
        /// <param name="id">
        /// The id of the subscription to unsubscribe
        /// </param>
        public Unsubscribe(string id)
            : base("unsub")
        {
            this.Id = id;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the id of the subscription to unsubscribe.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        #endregion
    }
}