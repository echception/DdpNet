// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddedParameter.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the AddedParameter class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet
{
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Stores information about a collection Add
    /// </summary>
    internal class AddedParameter : IModificationParameter
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AddedParameter"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="deserialized">
        /// The deserialized object.
        /// </param>
        public AddedParameter(string id, JObject deserialized)
        {
            this.Id = id;
            this.DeserializedObject = deserialized;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the deserialized object.
        /// </summary>
        public JObject DeserializedObject { get; private set; }

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
                return ModificationType.Added;
            }
        }

        #endregion
    }
}