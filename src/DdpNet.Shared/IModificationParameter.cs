// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModificationParameter.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   The ModificationParameter interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet
{
    /// <summary>
    /// The ModificationParameter interface.
    /// </summary>
    internal interface IModificationParameter
    {
        #region Public Properties

        /// <summary>
        /// Gets the modification type.
        /// </summary>
        ModificationType ModificationType { get; }

        #endregion
    }
}