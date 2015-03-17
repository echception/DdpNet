// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdParameter.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   TContains the IdParameter class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.ParameterObjects
{
    using Newtonsoft.Json;

    /// <summary>
    /// Data object used in message, that contains a single _id field
    /// </summary>
    internal class IdParameter
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IdParameter"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public IdParameter(string id)
        {
            this.Id = id;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [JsonProperty(PropertyName = "_id")]
        public string Id { get; set; }

        #endregion
    }
}