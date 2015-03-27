// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangedParameter.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the ChangedParameter class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DdpNet
{
    using System.Collections.Generic;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Stores information about changing an object
    /// </summary>
    internal class ChangedParameter : IModificationParameter
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangedParameter"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="fields">
        /// The fields that changed.
        /// </param>
        /// <param name="cleared">
        /// The cleared fields.
        /// </param>
        public ChangedParameter(string id, Dictionary<string, JToken> fields, string[] cleared)
        {
            this.Id = id;
            this.Fields = fields;
            this.Cleared = cleared;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the cleared fields.
        /// </summary>
        public string[] Cleared { get; private set; }

        /// <summary>
        /// Gets the fields that changed.
        /// </summary>
        public Dictionary<string, JToken> Fields { get; private set; }

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
                return ModificationType.Changed;
            }
        }

        #endregion
    }
}