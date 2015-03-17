// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Error.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the Error class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.ReturnedObjects
{
    using Newtonsoft.Json;

    /// <summary>
    /// Error data object returned from the server
    /// </summary>
    internal class Error
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the details.
        /// </summary>
        [JsonProperty(PropertyName = "details")]
        public string Details { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        [JsonProperty(PropertyName = "error")]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the reason.
        /// </summary>
        [JsonProperty(PropertyName = "reason")]
        public string Reason { get; set; }

        #endregion
    }
}