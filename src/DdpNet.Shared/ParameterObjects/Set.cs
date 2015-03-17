// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Set.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the Set class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.ParameterObjects
{
    using Newtonsoft.Json;

    /// <summary>
    /// Data object for $set updates
    /// </summary>
    internal class Set
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Set"/> class.
        /// </summary>
        /// <param name="objectToSet">
        /// The object to set.
        /// </param>
        public Set(object objectToSet)
        {
            this.ObjectToSet = objectToSet;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the object to set.
        /// </summary>
        [JsonProperty(PropertyName = "$set")]
        public object ObjectToSet { get; set; }

        #endregion
    }
}