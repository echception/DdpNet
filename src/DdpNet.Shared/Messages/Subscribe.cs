// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Subscribe.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the Subscribe type
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.Messages
{
    using Newtonsoft.Json;

    /// <summary>
    /// The Subscribe message, sent to the server to subscribe to a subscription
    /// </summary>
    internal class Subscribe : BaseMessage
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Subscribe"/> class.
        /// </summary>
        /// <param name="id">
        /// The id of the subscription
        /// </param>
        /// <param name="name">
        /// The name of the subscription.
        /// </param>
        /// <param name="parameters">
        /// The subscription parameters.
        /// </param>
        public Subscribe(string id, string name, object[] parameters = null)
            : base("sub")
        {
            this.Id = id;
            this.Name = name;
            this.Parameters = parameters;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the id of the subscription.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the subscription.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the parameters to the subscription.
        /// </summary>
        [JsonProperty(PropertyName = "params")]
        public object[] Parameters { get; set; }

        #endregion
    }
}