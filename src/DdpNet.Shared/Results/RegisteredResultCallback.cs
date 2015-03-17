// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegisteredResultCallback.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the RegisteredResultCallback class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.Results
{
    /// <summary>
    /// A callback that is registered to be invoked when a ResultFilter has completed
    /// </summary>
    internal class RegisteredResultCallback
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisteredResultCallback"/> class.
        /// </summary>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        internal RegisteredResultCallback(ResultFilter filter, ResultCallback callback)
        {
            this.Filter = filter;
            this.Callback = callback;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the callback.
        /// </summary>
        internal ResultCallback Callback { get; private set; }

        /// <summary>
        /// Gets the filter.
        /// </summary>
        internal ResultFilter Filter { get; private set; }

        #endregion
    }
}