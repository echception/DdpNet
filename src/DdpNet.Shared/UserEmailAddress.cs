// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserEmailAddress.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the UserEmailAddress class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet
{
    using Newtonsoft.Json;

    /// <summary>
    /// Data object for storing a user's email addresses
    /// </summary>
    public class UserEmailAddress
    {
        #region Public Properties

        /// <summary>
        /// Gets the email address.
        /// </summary>
        [JsonProperty(PropertyName = "address")]
        public string Address { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the email account is verified.
        /// </summary>
        [JsonProperty(PropertyName = "verified")]
        public bool Verified { get; private set; }

        #endregion
    }
}