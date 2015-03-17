// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResultFilter.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the ResultFilter abstract class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.Results
{
    /// <summary>
    /// Base class for all ResultFilter. ResultFilters are a way for a method call to wait for a specific set of responses.
    /// For example, a method call needs to wait for the 'result' and 'update' messages, while a 'sub' message needs to wait
    /// for the 'ready' message.
    /// The ResultFilter will have HandleResultObject for all results received. The filter will look at that, and modify its internal state.
    /// When it has received the necessary messages, IsCompleted should return true.
    /// If a method call returns a value, it should be returned in the GetReturnedObject method
    /// </summary>
    internal abstract class ResultFilter
    {
        #region Methods

        /// <summary>
        /// Gets the return value for the method call
        /// </summary>
        /// <returns>
        /// The <see cref="ReturnedObject"/>.
        /// </returns>
        internal abstract ReturnedObject GetReturnedObject();

        /// <summary>
        /// Handles a message returned from the server. This is called for every message, so the 
        /// derived classes are responsible for determining if it cares about a specific message
        /// </summary>
        /// <param name="returnedObject">
        /// The returned object.
        /// </param>
        internal abstract void HandleReturnObject(ReturnedObject returnedObject);

        /// <summary>
        /// Gets whether the ResultFilter has completed
        /// </summary>
        /// <returns>
        /// TTrue if its completed, false otherwise
        /// </returns>
        internal abstract bool IsCompleted();

        #endregion
    }
}