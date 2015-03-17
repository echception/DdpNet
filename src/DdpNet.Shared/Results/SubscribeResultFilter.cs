// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubscribeResultFilter.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the SubscribeResultFilter class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.Results
{
    using System.Linq;

    using DdpNet.Messages;

    /// <summary>
    /// A ResultFilter that waits for either a 'ready' or a 'nosub' message
    /// </summary>
    internal class SubscribeResultFilter : ResultFilter
    {
        #region Fields

        /// <summary>
        /// The subscription id.
        /// </summary>
        private readonly string subscriptionId;

        /// <summary>
        /// Indicates if a 'nosub' message was received
        /// </summary>
        private bool nosubCalled;

        /// <summary>
        /// Indicates if a 'ready' message was received
        /// </summary>
        private bool readyCalled;

        /// <summary>
        /// The returned object.
        /// </summary>
        private ReturnedObject returnedObject;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscribeResultFilter"/> class.
        /// </summary>
        /// <param name="subscriptionId">
        /// The subscription id.
        /// </param>
        internal SubscribeResultFilter(string subscriptionId)
        {
            this.readyCalled = false;
            this.nosubCalled = false;

            this.subscriptionId = subscriptionId;
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
        /// Handles an object returned by the server
        /// </summary>
        /// <param name="messageReturned">
        /// The returned object.
        /// </param>
        internal override void HandleReturnObject(ReturnedObject messageReturned)
        {
            if (messageReturned.MessageType == "ready")
            {
                var readyObject = messageReturned.ParsedObject.ToObject<Ready>();

                if (readyObject.SubscriptionsReady.Contains(this.subscriptionId))
                {
                    this.readyCalled = true;
                    this.returnedObject = messageReturned;
                }
            }

            if (messageReturned.MessageType == "nosub")
            {
                var noSubObject = messageReturned.ParsedObject.ToObject<NoSubscribe>();

                if (noSubObject.ID == this.subscriptionId)
                {
                    this.nosubCalled = true;
                    this.returnedObject = messageReturned;
                }
            }
        }

        /// <summary>
        /// Determines if this ResultFilter has completed
        /// </summary>
        /// <returns>
        /// True if it has completed, otherwise false
        /// </returns>
        internal override bool IsCompleted()
        {
            return this.readyCalled || this.nosubCalled;
        }

        #endregion
    }
}