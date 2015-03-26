// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InsertParameter.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the InsertParameter class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet
{
    /// <summary>
    /// Contains parameters for a Collection.Insert call. Used by the ThreadSafeObservableCollection to pass the needed information through
    /// SynchronizationContext.Post
    /// </summary>
    /// <typeparam name="T">
    /// The type of the item being inserted
    /// </typeparam>
    internal class InsertParameter<T>
        where T : DdpObject
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InsertParameter{T}"/> class.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        public InsertParameter(T item, int index)
        {
            this.Item = item;
            this.Index = index;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the index.
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Gets the item.
        /// </summary>
        public T Item { get; private set; }

        #endregion
    }
}