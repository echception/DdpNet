// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Exceptions.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the Exceptions class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet
{
    using System;

    /// <summary>
    /// Contains methods for validating parameters, which throw exceptions if the parameters are invalid
    /// </summary>
    public static class Exceptions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Throw an exception if the value is null
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="parameterName">
        /// The parameter name.
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown when value is null
        /// </exception>
        public static void ThrowIfNull(object value, string parameterName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        /// <summary>
        /// Throw an exception if the string value is null or whitespace
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="parameterName">
        /// The parameter name.
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown when the value is null or whitespace
        /// </exception>
        public static void ThrowIfNullOrWhiteSpace(string value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        #endregion
    }
}