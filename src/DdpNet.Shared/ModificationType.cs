// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModificationType.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the ModificationType enum
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet
{
    /// <summary>
    /// Different types of object modification
    /// </summary>
    internal enum ModificationType
    {
        /// <summary>
        /// Object was added
        /// </summary>
        Added, 

        /// <summary>
        /// Object was changed
        /// </summary>
        Changed, 

        /// <summary>
        /// Object was removed
        /// </summary>
        Removed
    }
}