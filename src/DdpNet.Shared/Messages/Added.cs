// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Added.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the Added type
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.Messages
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// The Added message from the server, to inform the client an object was added to a collection
    /// </summary>
    internal class Added : BaseMessage
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Added"/> class.
        /// </summary>
        public Added()
            : base("added")
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the collection that the object was added to.
        /// </summary>
        [JsonProperty(PropertyName = "collection")]
        public string Collection { get; set; }

        /// <summary>
        /// Gets or sets the fields of the object that were added.
        /// </summary>
        [JsonProperty(PropertyName = "fields")]
        public JObject Fields { get; set; }

        /// <summary>
        /// Gets or sets the id of the object added.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        #endregion
    }
}