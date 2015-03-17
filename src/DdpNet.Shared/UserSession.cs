// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserSession.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the UserSession class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet
{
    using System;

    using Newtonsoft.Json;

    /// <summary>
    /// Data object for storing a user's session information
    /// </summary>
    public class UserSession
    {
        #region Public Properties

        /// <summary>
        /// Gets the token.
        /// </summary>
        [JsonProperty(PropertyName = "token")]
        public string Token { get; private set; }

        /// <summary>
        /// Gets the token expiration.
        /// </summary>
        [JsonIgnore]
        public DateTime TokenExpiration
        {
            get
            {
                return this.TokenExpires.DateTime;
            }
        }

        /// <summary>
        /// Gets the user id.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string UserId { get; private set; }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets when the token expires.
        /// </summary>
        [JsonProperty(PropertyName = "tokenExpires")]
        private DdpDate TokenExpires { get; set; }

        #endregion
    }
}