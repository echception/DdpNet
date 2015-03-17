// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResultFilterFactory.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the ResultFilterFactory class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.Results
{
    /// <summary>
    /// Factory class for creating ResultFilters for different expected results
    /// </summary>
    internal static class ResultFilterFactory
    {
        #region Methods

        /// <summary>
        /// Creates a ResultFilter that completes when a method call is done
        /// </summary>
        /// <param name="methodId">
        /// The method id.
        /// </param>
        /// <returns>
        /// The <see cref="ResultFilter"/>.
        /// </returns>
        internal static ResultFilter CreateCallResultFilter(string methodId)
        {
            return new MethodCallResultFilter(methodId);
        }

        /// <summary>
        /// Creates a ResultFilter that completes when the 'connected' message is received
        /// </summary>
        /// <returns>
        /// The <see cref="ResultFilter"/>.
        /// </returns>
        internal static ResultFilter CreateConnectResultFilter()
        {
            return new ConnectedResultFilter();
        }

        /// <summary>
        /// Creates a ResultFilter that completes when a subscription is done syncing
        /// </summary>
        /// <param name="subscriptionId">
        /// The subscription id.
        /// </param>
        /// <returns>
        /// The <see cref="ResultFilter"/>.
        /// </returns>
        internal static ResultFilter CreateSubscribeResultFilter(string subscriptionId)
        {
            return new SubscribeResultFilter(subscriptionId);
        }

        /// <summary>
        /// Creates a ResultFilter that completes when an unsubscribe call has completed syncing data
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ResultFilter"/>.
        /// </returns>
        internal static ResultFilter CreateUnsubscribeResultFilter(string id)
        {
            return new UnsubscribeResultFilter(id);
        }

        #endregion
    }
}