// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateUserUserNameParameters.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the CreateUserUserNameParameters type
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.Packages.Accounts
{
    using DdpNet.ParameterObjects;

    using Newtonsoft.Json;

    /// <summary>
    /// Data object used as a parameter when creating a new user with a user name/password
    /// </summary>
    internal class CreateUserUserNameParameters
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateUserUserNameParameters"/> class.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        public CreateUserUserNameParameters(string username, Password password)
        {
            this.UserName = username;
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
        /// Gets or sets the user name.
        /// </summary>
        [JsonProperty(PropertyName = "username")]
        public string UserName { get; set; }

        #endregion
    }
}