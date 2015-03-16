// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Changed.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the Changed type
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.Messages
{
    using System.Collections.Generic;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// The Changed message from the server, to inform the client an object was changed
    /// </summary>
    internal class Changed : BaseMessage
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Changed"/> class.
        /// </summary>
        internal Changed()
            : base("changed")
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the fields that were cleared.
        /// </summary>
        [JsonProperty(PropertyName = "cleared")]
        public string[] Cleared { get; set; }

        /// <summary>
        /// Gets or sets the name of the collection that was changed.
        /// </summary>
        [JsonProperty(PropertyName = "collection")]
        public string Collection { get; set; }

        /// <summary>
        /// Gets or sets the fields that changed.
        /// </summary>
        [JsonProperty(PropertyName = "fields")]
        public Dictionary<string, JToken> Fields { get; set; }

        /// <summary>
        /// Gets or sets the id of the object that changed.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string ID { get; set; }

        #endregion
    }
}