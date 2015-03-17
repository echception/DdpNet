// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnsubscribeResultFilter.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the UnsubscribeResultFilter class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.Results
{
    using DdpNet.Messages;

    /// <summary>
    /// ResultFilter that waits for the result of a 'unsubscribe' message sent to the server
    /// </summary>
    internal class UnsubscribeResultFilter : ResultFilter
    {
        #region Fields

        /// <summary>
        /// The subscription id.
        /// </summary>
        private readonly string subscriptionId;

        /// <summary>
        /// Indicates if a 'nosub' message has been received
        /// </summary>
        private bool nosubCalled;

        /// <summary>
        /// The returned object.
        /// </summary>
        private ReturnedObject returnedObject;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsubscribeResultFilter"/> class.
        /// </summary>
        /// <param name="subscriptionId">
        /// The subscription id.
        /// </param>
        internal UnsubscribeResultFilter(string subscriptionId)
        {
            this.subscriptionId = subscriptionId;
            this.nosubCalled = false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the object returned by the server
        /// </summary>
        /// <returns>
        /// The <see cref="ReturnedObject"/>.
        /// </returns>
        internal override ReturnedObject GetReturnedObject()
        {
            return this.returnedObject;
        }

        /// <summary>
        /// The handle return object.
        /// </summary>
        /// <param name="returnedMessage">
        /// The returned object.
        /// </param>
        internal override void HandleReturnObject(ReturnedObject returnedMessage)
        {
            if (returnedMessage.MessageType == "nosub")
            {
                var noSubObject = returnedMessage.ParsedObject.ToObject<NoSubscribe>();

                if (noSubObject.ID == this.subscriptionId)
                {
                    this.nosubCalled = true;
                    this.returnedObject = returnedMessage;
                }
            }
        }

        /// <summary>
        /// Determines if the ResultFilter has completed
        /// </summary>
        /// <returns>
        /// True if the ResultFilter has completed, otherwise false
        /// </returns>
        internal override bool IsCompleted()
        {
            return this.nosubCalled;
        }

        #endregion
    }
}