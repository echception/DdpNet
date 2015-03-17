// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DdpServerException.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the DdpServerException class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet
{
    using System;
    using System.Globalization;

    using DdpNet.ReturnedObjects;

    /// <summary>
    /// Wraps errors returned from the DDP server
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable", Justification = "Serializable is not available in WinRT")]
    public class DdpServerException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DdpServerException"/> class.
        /// </summary>
        /// <param name="error">
        /// The error.
        /// </param>
        internal DdpServerException(Error error)
            : base(
                string.Format(
                    CultureInfo.CurrentCulture,
                    "Server returned an error {0}. Details: {1}. Reason: {2}",
                    error.ErrorMessage, 
                    error.Details, 
                    error.Reason))
        {
            this.Error = error.ErrorMessage;
            this.Details = error.Details;
            this.Reason = error.Reason;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the details.
        /// </summary>
        public string Details { get; private set; }

        /// <summary>
        /// Gets the error.
        /// </summary>
        public string Error { get; private set; }

        /// <summary>
        /// Gets the reason.
        /// </summary>
        public string Reason { get; private set; }

        #endregion
    }
}