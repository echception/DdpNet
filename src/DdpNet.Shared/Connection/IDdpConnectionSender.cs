// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDdpConnectionSender.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the DdpConnectionSender interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.Connection
{
    using System.Threading.Tasks;

    /// <summary>
    /// The DdpConnectionSender interface. Defines an interface that can send objects
    /// </summary>
    internal interface IDdpConnectionSender
    {
        #region Public Methods and Operators

        /// <summary>
        /// Sends an object
        /// </summary>
        /// <param name="objectToSend">
        /// The object to send.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> which completes when the object is sent.
        /// </returns>
        Task SendObject(object objectToSend);

        #endregion
    }
}