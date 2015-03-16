// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Removed.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the Removed type
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.Messages
{
    using Newtonsoft.Json;

    /// <summary>
    /// The Removed message, sent to the client to inform it of an item removed from a collection
    /// </summary>
    internal class Removed : BaseMessage
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Removed"/> class.
        /// </summary>
        internal Removed()
            : base("removed")
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the collection an item was removed from.
        /// </summary>
        [JsonProperty(PropertyName = "collection")]
        public string Collection { get; set; }

        /// <summary>
        /// Gets or sets the id of the object to remove.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        #endregion
    }
}