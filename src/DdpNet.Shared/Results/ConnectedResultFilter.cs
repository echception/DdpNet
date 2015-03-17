// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectedResultFilter.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the ConnectedResultFilter class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.Results
{
    /// <summary>
    /// ResultFilter that looks for 'connected' messages and 'failed' messages
    /// </summary>
    internal class ConnectedResultFilter : ResultFilter
    {
        #region Fields

        /// <summary>
        /// Indicates if a 'connected' message was received
        /// </summary>
        private bool connectedCalled;

        /// <summary>
        /// Indicates if a 'failed' message was received
        /// </summary>
        private bool failedCalled;

        /// <summary>
        /// The returned object.
        /// </summary>
        private ReturnedObject returnedObject;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectedResultFilter"/> class.
        /// </summary>
        public ConnectedResultFilter()
        {
            this.connectedCalled = false;
            this.failedCalled = false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the returned object
        /// </summary>
        /// <returns>
        /// The <see cref="ReturnedObject"/>.
        /// </returns>
        internal override ReturnedObject GetReturnedObject()
        {
            return this.returnedObject;
        }

        /// <summary>
        /// Handles a message received from the server
        /// </summary>
        /// <param name="returnObject">
        /// The returned object.
        /// </param>
        internal override void HandleReturnObject(ReturnedObject returnObject)
        {
            if (returnObject.MessageType == "connected")
            {
                this.connectedCalled = true;
                this.returnedObject = returnObject;
            }
            else if (returnObject.MessageType == "failed")
            {
                this.failedCalled = true;
                this.returnedObject = returnObject;
            }
        }

        /// <summary>
        /// Determines if the ResultFilter has completed
        /// </summary>
        /// <returns>
        /// True if this has received the expected messages, false if it has not received the expected messages
        /// </returns>
        internal override bool IsCompleted()
        {
            return this.connectedCalled || this.failedCalled;
        }

        #endregion
    }
}