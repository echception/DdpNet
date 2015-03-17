// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeteorUser.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the MeteorUser class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Data object for a Meteor user
    /// </summary>
    public class MeteorUser : DdpObject
    {
        #region Public Properties

        /// <summary>
        /// Gets the user's emails.
        /// </summary>
        [JsonProperty(PropertyName = "emails")]
        public UserEmailAddress[] Emails { get; private set; }

        /// <summary>
        /// Gets the profile object.
        /// </summary>
        [JsonProperty(PropertyName = "profile")]
        public JObject Profile { get; private set; }

        /// <summary>
        /// Gets the user name.
        /// </summary>
        [JsonProperty(PropertyName = "username")]
        public string UserName { get; private set; }

        #endregion
    }
}