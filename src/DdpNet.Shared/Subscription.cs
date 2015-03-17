// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Subscription.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the Subscription class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet
{
    /// <summary>
    /// Information about a subscription a DdpClient is subscribed to.
    /// Created when Subscribe is called, and can be passed to Unsubscribe to unsubscribe from the subscription
    /// </summary>
    public class Subscription
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Subscription"/> class.
        /// </summary>
        /// <param name="id">
        /// The id of the subscription.
        /// </param>
        /// <param name="name">
        /// The name of the subscription.
        /// </param>
        internal Subscription(string id, string name)
        {
            this.Name = name;
            this.Id = id;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the name of the subscription.
        /// </summary>
        public string Name { get; private set; }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the id of the subscription.
        /// </summary>
        internal string Id { get; private set; }

        #endregion
    }
}