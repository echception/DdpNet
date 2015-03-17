// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserLogin.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the UserLogin class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.ParameterObjects
{
    using Newtonsoft.Json;

    /// <summary>
    /// Data object for a username
    /// </summary>
    internal class UserLogin
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UserLogin"/> class.
        /// </summary>
        /// <param name="userName">
        /// The user name.
        /// </param>
        public UserLogin(string userName)
        {
            this.UserName = userName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        [JsonProperty(PropertyName = "username")]
        public string UserName { get; set; }

        #endregion
    }
}