// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginPasswordParameters.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the LoginPasswordParameters
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.ParameterObjects
{
    using Newtonsoft.Json;

    /// <summary>
    /// Data object that contains properties for logging in with a password
    /// </summary>
    internal class LoginPasswordParameters
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginPasswordParameters"/> class.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        public LoginPasswordParameters(UserLogin user, Password password)
        {
            this.User = user;
            this.Password = password;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [JsonProperty(PropertyName = "password")]
        public Password Password { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        [JsonProperty(PropertyName = "user")]
        public UserLogin User { get; set; }

        #endregion
    }
}