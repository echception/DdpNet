// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UntypedCollection.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the UntypedCollection type
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.Collections
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// The untyped collection. Stores objects received from the server that we don't yet know the type of. 
    /// This will be converted to a typed collection with the user calls GetCollection
    /// </summary>
    internal class UntypedCollection : IDdpCollection
    {
        #region Fields

        /// <summary>
        /// Responsible for applying changes to an object
        /// </summary>
        private UntypedObjectChanger changer;

        /// <summary>
        /// The currently stored objects
        /// </summary>
        private Dictionary<string, JObject> objects;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UntypedCollection"/> class. 
        /// Creates an instance of UntypedCollection
        /// </summary>
        /// <param name="collectionName">
        /// The name of the collection
        /// </param>
        public UntypedCollection(string collectionName)
        {
            this.CollectionName = collectionName;
            this.objects = new Dictionary<string, JObject>();
            this.changer = new UntypedObjectChanger();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the name of the collection
        /// </summary>
        public string CollectionName { get; private set; }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a read only view of the contents. Should not lock on syncObject, as this is called when converting to the typed collection,
        /// which already has a lock on the object.
        /// </summary>
        internal ReadOnlyDictionary<string, JObject> Objects
        {
            get
            {
                return new ReadOnlyDictionary<string, JObject>(this.objects);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Called when an object was added to the collection
        /// </summary>
        /// <param name="id">
        /// The id of the object added
        /// </param>
        /// <param name="value">
        /// The object added
        /// </param>
        public void Added(string id, JObject value)
        {
            this.objects.Add(id, value);
        }

        /// <summary>
        /// Called when an object was changed.
        /// </summary>
        /// <param name="id">
        /// The ID of the object changed
        /// </param>
        /// <param name="fields">
        /// The fields that changed
        /// </param>
        /// <param name="cleared">
        /// The fields that were cleared
        /// </param>
        public void Changed(string id, Dictionary<string, JToken> fields, string[] cleared)
        {
            JObject objectToChange;

            if (this.objects.TryGetValue(id, out objectToChange))
            {
                this.changer.ChangeObject(objectToChange, fields, cleared);
            }
        }

        /// <summary>
        /// Called when an object was removed from the collection
        /// </summary>
        /// <param name="id">
        /// The ID of the object removed
        /// </param>
        public void Removed(string id)
        {
            this.objects.Remove(id);
        }

        #endregion
    }
}