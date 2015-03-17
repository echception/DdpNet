// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountsExtensionMethods.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the accounts extension methods.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.Packages.Accounts
{
    using System.Threading.Tasks;

    /// <summary>
    /// Contains extension methods that are related to the Accounts package
    /// </summary>
    public static class AccountsExtensionMethods
    {
        #region Public Methods and Operators

        /// <summary>
        /// Creates a user with a user name and password
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <param name="userName">
        /// The user name.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> that completes when the user is created.
        /// </returns>
        public static Task CreateUserWithUserName(this MeteorClient client, string userName, string password)
        {
            Exceptions.ThrowIfNull(client, "client");
            Exceptions.ThrowIfNullOrWhitespace(userName, "userName");
            Exceptions.ThrowIfNullOrWhitespace(password, "password");

            var passwordParameters = Utilities.GetPassword(password);
            var parameters = new CreateUserUserNameParameters(userName, passwordParameters);

            return client.CallLoginMethod("createUser", parameters);
        }

        #endregion
    }
}