// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegisteredResultWait.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the RegisteredResultWait
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.Results
{
    using System.Threading;

    /// <summary>
    /// Stores information about a ManualResetEvent that is registered to be signaled when a ResultFilter has completed
    /// </summary>
    internal class RegisteredResultWait
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisteredResultWait"/> class.
        /// </summary>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <param name="waitEvent">
        /// The wait event.
        /// </param>
        internal RegisteredResultWait(ResultFilter filter, ManualResetEvent waitEvent)
        {
            this.Filter = filter;
            this.WaitEvent = waitEvent;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the filter.
        /// </summary>
        internal ResultFilter Filter { get; private set; }

        /// <summary>
        /// Gets the wait event.
        /// </summary>
        internal ManualResetEvent WaitEvent { get; private set; }

        #endregion
    }
}