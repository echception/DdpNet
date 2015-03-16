// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Method.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the Method type
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.Messages
{
    using Newtonsoft.Json;

    /// <summary>
    /// The Message, sent to the server to call a method
    /// </summary>
    internal class Method : BaseMessage
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Method"/> class.
        /// </summary>
        internal Method()
            : base("method")
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the id of the method call.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the method name to call.
        /// </summary>
        [JsonProperty(PropertyName = "method")]
        public string MethodName { get; set; }

        /// <summary>
        /// Gets or sets the parameters for the method.
        /// </summary>
        [JsonProperty(PropertyName = "params")]
        public object[] Parameters { get; set; }

        #endregion
    }
}