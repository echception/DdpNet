// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Password.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the Password class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.ParameterObjects
{
    using Newtonsoft.Json;

    /// <summary>
    /// Data object for a hashed password
    /// </summary>
    internal class Password
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Password"/> class.
        /// </summary>
        /// <param name="digest">
        /// The digest.
        /// </param>
        /// <param name="algorithm">
        /// The algorithm.
        /// </param>
        public Password(string digest, string algorithm)
        {
            this.Digest = digest;
            this.Algorithm = algorithm;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the algorithm.
        /// </summary>
        [JsonProperty(PropertyName = "algorithm")]
        public string Algorithm { get; set; }

        /// <summary>
        /// Gets or sets the digest.
        /// </summary>
        [JsonProperty(PropertyName = "digest")]
        public string Digest { get; set; }

        #endregion
    }
}