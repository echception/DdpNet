// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Utilities.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the Utilities class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet
{
    using System;
    using System.Text;

    using DdpNet.ParameterObjects;

    using PCLCrypto;

    /// <summary>
    /// Contains miscellaneous utility functions
    /// </summary>
    internal static class Utilities
    {
        #region Methods

        /// <summary>
        /// Generates a unique ID string
        /// </summary>
        /// <returns>
        /// The ID string
        /// </returns>
        internal static string GenerateId()
        {
            return Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// Takes a password string, hashes it, and returns a Password object
        /// </summary>
        /// <param name="password">
        /// The password to hash.
        /// </param>
        /// <returns>
        /// The <see cref="Password"/>.
        /// </returns>
        internal static Password GetPassword(string password)
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hasher = WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Sha256);
            var hashedPassword = hasher.HashData(bytes);
            var hashedString = string.Empty;

            foreach (var x in hashedPassword)
            {
                hashedString += string.Format("{0:x2}", x);
            }

            return new Password(hashedString, "sha-256");
        }

        #endregion
    }
}