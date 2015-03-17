// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginResume.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the LoginResume class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet
{
    using Newtonsoft.Json;

    /// <summary>
    /// Data object sent to the server to resume a previous session
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login", Justification = "Using the same terms Meteor does")]
    public class LoginResume
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginResume"/> class.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        public LoginResume(string token)
        {
            this.Token = token;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        [JsonProperty(PropertyName = "resume")]
        public string Token { get; set; }

        #endregion
    }
}