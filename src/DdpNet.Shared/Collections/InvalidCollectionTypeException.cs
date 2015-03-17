// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidCollectionTypeException.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   The invalid collection type exception.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.Collections
{
    using System;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// The invalid collection type exception.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable", Justification = "Serializable is not available in WinRT")]
    public class InvalidCollectionTypeException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCollectionTypeException"/> class.
        /// </summary>
        internal InvalidCollectionTypeException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCollectionTypeException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        internal InvalidCollectionTypeException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCollectionTypeException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="innerException">
        /// The inner exception.
        /// </param>
        internal InvalidCollectionTypeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCollectionTypeException"/> class.
        /// </summary>
        /// <param name="collectionName">
        /// The collection name.
        /// </param>
        /// <param name="existingType">
        /// The existing type.
        /// </param>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        internal InvalidCollectionTypeException(string collectionName, Type existingType, Type targetType)
            : base(
                string.Format(
                    CultureInfo.CurrentCulture,
                    "Collection {0} was already initialized with type {1}. An attempt was made to retrieve a collection of the same name, but with type {2}. The type of a collection cannot change after it is initialized", 
                    collectionName, 
                    existingType.GenericTypeArguments.First().Name, 
                    targetType.GenericTypeArguments.First().Name))
        {
        }

        #endregion
    }
}