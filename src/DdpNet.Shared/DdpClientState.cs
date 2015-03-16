// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DdpClientState.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Defines the DdpClientState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet
{
    /// <summary>
    /// The connection state for the DdpClient
    /// </summary>
    public enum DdpClientState
    {
        /// <summary>
        /// The client has not connected  yet
        /// </summary>
        NotConnected,

        /// <summary>
        /// The client has connected
        /// </summary>
        Connected
    }
}
