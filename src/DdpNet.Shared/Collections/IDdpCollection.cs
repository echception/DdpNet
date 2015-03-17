// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDdpCollection.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   The DdpCollection interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.Collections
{
    using System.Collections.Generic;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// The DdpCollection interface.
    /// </summary>
    internal interface IDdpCollection
    {
        #region Public Methods and Operators

        /// <summary>
        /// Adds an object to the DdpCollection
        /// </summary>
        /// <param name="id">
        /// The id of the item.
        /// </param>
        /// <param name="deserializedObject">
        /// The deserialized, untyped object.
        /// </param>
        void Added(string id, JObject deserializedObject);

        /// <summary>
        /// Changes and object
        /// </summary>
        /// <param name="id">
        /// The id of the changed object.
        /// </param>
        /// <param name="fields">
        /// The fields that changed.
        /// </param>
        /// <param name="cleared">
        /// The fields that were cleared.
        /// </param>
        void Changed(string id, Dictionary<string, JToken> fields, string[] cleared);

        /// <summary>
        /// Removes an object from the collection
        /// </summary>
        /// <param name="id">
        /// The id of the item to remove.
        /// </param>
        void Removed(string id);

        #endregion
    }
}