// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Ready.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the Ready type
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.Messages
{
    using Newtonsoft.Json;

    /// <summary>
    /// The Ready message, sent to the client to inform it of subscriptions which are ready with their initial data
    /// </summary>
    internal class Ready : BaseMessage
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Ready"/> class.
        /// </summary>
        internal Ready()
            : base("ready")
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the subscription IDs that are ready.
        /// </summary>
        [JsonProperty(PropertyName = "subs")]
        public string[] SubscriptionsReady { get; set; }

        #endregion
    }
}