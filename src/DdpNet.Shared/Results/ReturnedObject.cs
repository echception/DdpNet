// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReturnedObject.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the ReturnedObject class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.Results
{
    using System;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Stores information about an object returned by the server in response to a client action
    /// </summary>
    internal class ReturnedObject
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ReturnedObject"/> class.
        /// </summary>
        /// <param name="messageType">
        /// The message type.
        /// </param>
        /// <param name="parsedObject">
        /// The parsed object.
        /// </param>
        /// <param name="message">
        /// The raw message.
        /// </param>
        internal ReturnedObject(string messageType, JObject parsedObject, string message)
        {
            this.Message = message;
            this.MessageType = messageType;
            this.ParsedObject = parsedObject;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the raw message.
        /// </summary>
        internal string Message { get; private set; }

        /// <summary>
        /// Gets the message type.
        /// </summary>
        internal string MessageType { get; private set; }

        /// <summary>
        /// Gets the parsed object.
        /// </summary>
        internal JObject ParsedObject { get; private set; }

        #endregion
    }
}