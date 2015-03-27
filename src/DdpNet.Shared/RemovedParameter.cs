// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemovedParameter.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the RemovedParameter class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet
{
    /// <summary>
    /// Stores information about removing an item
    /// </summary>
    internal class RemovedParameter : IModificationParameter
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RemovedParameter"/> class.
        /// </summary>
        /// <param name="id">
        /// The id of the item to remove.
        /// </param>
        public RemovedParameter(string id)
        {
            this.Id = id;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the id.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the modification type.
        /// </summary>
        public ModificationType ModificationType
        {
            get
            {
                return ModificationType.Removed;
            }
        }

        #endregion
    }
}