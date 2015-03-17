// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICollectionManager.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   The CollectionManager interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.Collections
{
    using DdpNet.Messages;

    /// <summary>
    /// The CollectionManager interface.
    /// </summary>
    internal interface ICollectionManager
    {
        #region Public Methods and Operators

        /// <summary>
        /// Adds an object to a collection
        /// If the collection does not yet exist, an UntypedCollection will be created to store the object in
        /// </summary>
        /// <param name="message">
        /// The Added message to process
        /// </param>
        void Added(Added message);

        /// <summary>
        /// Changes an object in a collection
        /// If the collection does not exist, will throw InvalidOperationException
        /// </summary>
        /// <param name="message">
        /// The Changed message to process
        /// </param>
        void Changed(Changed message);

        /// <summary>
        /// Gets a typed instance of a collection.
        /// Only one instance is kept of each collection, so repeated calls to this with the same parameters will return the same object
        /// If it is called again with the same collectionName, but a different type, an error will be thrown
        /// If the collection does not exist, a new one will be created
        /// If it exists, but is untyped, it will be converted to a typed collection
        /// If a typed collection exists, it will be returned directly
        /// </summary>
        /// <typeparam name="T">
        /// The type of the collection
        /// </typeparam>
        /// <param name="collectionName">
        /// The name of the collection
        /// </param>
        /// <returns>
        /// A DdpCollection
        /// </returns>
        DdpCollection<T> GetCollection<T>(string collectionName) where T : DdpObject;

        /// <summary>
        /// Removes an object from a collection
        /// If the collection does not exist, throws an InvalidOperationException
        /// </summary>
        /// <param name="message">
        /// The Removed message to process
        /// </param>
        void Removed(Removed message);

        #endregion
    }
}