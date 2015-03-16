// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Result.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the Result type
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.Messages
{
    using DdpNet.ReturnedObjects;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// The Result message, sent to the client in response to a method call
    /// </summary>
    internal class Result : BaseMessage
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Result"/> class.
        /// </summary>
        internal Result()
            : base("result")
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the error, if present.
        /// </summary>
        [JsonProperty(PropertyName = "error")]
        public Error Error { get; set; }

        /// <summary>
        /// Gets or sets the id of the method call.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the result object of the method call.
        /// </summary>
        [JsonProperty(PropertyName = "result")]
        public JToken ResultObject { get; set; }

        #endregion
    }
}