// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WaitHandle.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the WaitHandle class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.Results
{
    /// <summary>
    /// When making a method call that needs to wait for a result, we first register a WaitHandle.
    /// The call to register the WaitHandle takes the ResultFilter to wait for
    /// After registering, the actual method call can be made, then the WaitHandle passed back to the ResultHandler
    /// to wait for the result.
    /// </summary>
    internal class WaitHandle
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitHandle"/> class.
        /// </summary>
        internal WaitHandle()
        {
            this.Triggered = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether it has been triggered.
        /// </summary>
        internal bool Triggered { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Sets that the WaitHandle has been triggered
        /// </summary>
        internal void SetTriggered()
        {
            this.Triggered = true;
        }

        #endregion
    }
}