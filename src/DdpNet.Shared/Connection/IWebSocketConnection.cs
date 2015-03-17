// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWebSocketConnection.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the WebSocketConnection interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.Connection
{
    using System.Threading.Tasks;

    /// <summary>
    /// The WebSocketConnection interface, which supports WebSocket operations 
    /// </summary>
    public interface IWebSocketConnection
    {
        #region Public Methods and Operators

        /// <summary>
        /// Closes the socket connection
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task CloseAsync();

        /// <summary>
        /// Opens the WebSocket connection
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task ConnectAsync();

        /// <summary>
        /// Receives a message from the connection
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>, where the return object is the message received.
        /// </returns>
        Task<string> ReceiveAsync();

        /// <summary>
        /// Sends an object over the connection
        /// </summary>
        /// <param name="text">
        /// The text to send.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>, which completes when the object has been sent.
        /// </returns>
        Task SendAsync(string text);

        #endregion
    }
}