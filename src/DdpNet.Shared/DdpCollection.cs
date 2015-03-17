// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DdpCollection.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Defines the DdpCollection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DdpNet
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using DdpNet.Collections;
    using DdpNet.ParameterObjects;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Typed collection that inherits from ReadOnlyObservableCollection. Managed by a DdpClient.
    /// The DdpClient will automatically sync changes to this collection
    /// </summary>
    /// <typeparam name="T">The type to parse the objects into</typeparam>
    public class DdpCollection<T> : ReadOnlyObservableCollection<T>, IDdpCollection
        where T : DdpObject
    {
        #region Fields

        /// <summary>
        /// ObjectChanger used to apply diffs to objects
        /// </summary>
        private readonly ObjectChanger changer = new ObjectChanger();

        /// <summary>
        /// The client used to call remote methods
        /// </summary>
        private readonly IDdpRemoteMethodCall client;

        /// <summary>
        /// The filter lock. Used to lock the collection when a filter is being created.
        /// Necessary since Filter can be called from a different thread than the modification functions.
        /// Filter can be called from a user thread, while the modification functions are called from the receive thread.
        /// Locking the collection before filtering prevents any race conditions when an object is added/removed while the filter is created
        /// </summary>
        private readonly object filterLock = new object();

        /// <summary>
        /// The filtered collection for this collection. All changes are synced to these collections
        /// </summary>
        private readonly List<DdpFilteredCollection<T>> filteredCollections;

        /// <summary>
        /// The internal ObservableCollection. This is where changes, and is exposed through the ReadOnlyObservableCollection
        /// this class inherits from
        /// </summary>
        private readonly ObservableCollection<T> internalList;

        /// <summary>
        /// The synchronization context. Ensures the Collection/Property changed events are always raised on the correct thread.
        /// </summary>
        private readonly SynchronizationContext synchronizationContext;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DdpCollection{T}"/> class.
        /// </summary>
        /// <param name="client">
        /// The remote method call client
        /// </param>
        /// <param name="collectionName">
        /// The name of the collection
        /// </param>
        internal DdpCollection(IDdpRemoteMethodCall client, string collectionName)
            : this(new ObservableCollection<T>())
        {
            Exceptions.ThrowIfNull(client, "client");
            Exceptions.ThrowIfNullOrWhitespace(collectionName, "collectionName");

            this.CollectionName = collectionName;
            this.client = client;
            this.synchronizationContext = SynchronizationContext.Current;
            this.filteredCollections = new List<DdpFilteredCollection<T>>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DdpCollection{T}"/> class.
        /// </summary>
        /// <param name="internalList">
        /// The internal list.
        /// </param>
        private DdpCollection(ObservableCollection<T> internalList)
            : base(internalList)
        {
            this.internalList = internalList;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the collection name.
        /// </summary>
        public string CollectionName { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Adds an item to the collection. This will result in a call to the server.
        /// </summary>
        /// <param name="item">
        /// The item to add
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> will complete the add is completed on the server, and if necessary, changes are synced to this client.
        /// </returns>
        public Task AddAsync(T item)
        {
            Exceptions.ThrowIfNull(item, "item");

            if (string.IsNullOrWhiteSpace(item.Id))
            {
                item.Id = Utilities.GenerateId();
            }

            var methodName = string.Format(CultureInfo.InvariantCulture, @"/{0}/insert", this.CollectionName);
            return this.client.Call(methodName, item);
        }

        /// <summary>
        /// Creates a filtered view of the collection. The view will be kept in sync with this collection. Similar to the Meteor cursor
        /// </summary>
        /// <param name="whereFilter">
        /// The Where filter. Decides which objects will be in the Filtered collection.
        /// </param>
        /// <param name="sortFilter">
        /// Specifies how the Filtered view will be sorted.
        /// </param>
        /// <returns>
        /// The <see cref="DdpFilteredCollection"/>.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown when both whereFilter and sortFilter are null
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Easier than creating overrides for each combination")]
        public DdpFilteredCollection<T> Filter(Func<T, bool> whereFilter = null, Comparison<T> sortFilter = null)
        {
            if (whereFilter == null && sortFilter == null)
            {
                throw new ArgumentException("whereFilter and sortFilter cannot both be null");
            }

            lock (this.filterLock)
            {
                var filteredCollection = new DdpFilteredCollection<T>(
                    this.CollectionName, 
                    this.synchronizationContext, 
                    whereFilter, 
                    sortFilter);

                this.filteredCollections.Add(filteredCollection);

                foreach (var item in this.internalList)
                {
                    filteredCollection.OnAdded(item);
                }

                return filteredCollection;
            }
        }

        /// <summary>
        /// The get enumerator. Overridden to return a snapshot, as the collection can be modified on multiple threads.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        public new IEnumerator<T> GetEnumerator()
        {
            var snapshot = this.internalList.ToList();

            return snapshot.GetEnumerator();
        }

        /// <summary>
        /// Removes an item from the collection. This will result in a server call.
        /// </summary>
        /// <param name="id">
        /// The id of the item to remove
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> will complete once the item is removed.
        /// </returns>
        public Task<bool> RemoveAsync(string id)
        {
            Exceptions.ThrowIfNullOrWhitespace(id, "id");

            var methodName = string.Format(CultureInfo.InvariantCulture, @"/{0}/remove", this.CollectionName);
            return this.CallConvertNumberToBool(methodName, new IdParameter(id));
        }

        /// <summary>
        /// Updates an item
        /// </summary>
        /// <param name="id">
        /// The id of the item to update
        /// </param>
        /// <param name="fieldsToUpdate">
        /// The fields to update. This object will be serialized, and all the resulting object passed to the Meteor update with a $set operator.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> will complete once the item is updated
        /// </returns>
        public Task<bool> UpdateAsync(string id, object fieldsToUpdate)
        {
            Exceptions.ThrowIfNull(fieldsToUpdate, "fieldsToUpdate");
            Exceptions.ThrowIfNullOrWhitespace(id, "id");

            var methodName = string.Format(CultureInfo.InvariantCulture, @"/{0}/update", this.CollectionName);
            var selector = new IdParameter(id);
            var set = new Set(fieldsToUpdate);

            return this.CallConvertNumberToBool(methodName, selector, set);
        }

        #endregion

        #region Explicit Interface Methods

        /// <summary>
        /// Called internally when an object has been added.
        /// </summary>
        /// <param name="id">
        /// The id of the object added
        /// </param>
        /// <param name="deserializedObject">
        /// The deserialized object
        /// </param>
        void IDdpCollection.Added(string id, JObject deserializedObject)
        {
            lock (this.filterLock)
            {
                var typedObject = deserializedObject.ToObject<T>();
                typedObject.OnAdded(id, this.synchronizationContext);

                this.internalList.Add(typedObject);

                foreach (var filteredCollection in this.filteredCollections)
                {
                    filteredCollection.OnAdded(typedObject);
                }
            }
        }

        /// <summary>
        /// Called internally when an object has changed
        /// </summary>
        /// <param name="id">
        /// The id of the object that changed
        /// </param>
        /// <param name="fields">
        /// The fields that changed
        /// </param>
        /// <param name="cleared">
        /// The fields that were cleared
        /// </param>
        void IDdpCollection.Changed(string id, Dictionary<string, JToken> fields, string[] cleared)
        {
            lock (this.filterLock)
            {
                var objectToChange = this.internalList.SingleOrDefault(x => x.Id == id);

                if (objectToChange == null)
                {
                    return;
                }

                this.changer.ChangeObject(objectToChange, fields, cleared);

                foreach (var filteredCollection in this.filteredCollections)
                {
                    filteredCollection.OnChanged(objectToChange);
                }
            }
        }

        /// <summary>
        /// Called internally when an item has been removed from the collection
        /// </summary>
        /// <param name="id">
        /// The id of the object to remove
        /// </param>
        void IDdpCollection.Removed(string id)
        {
            lock (this.filterLock)
            {
                var objectToRemove = this.SingleOrDefault(x => x.Id == id);

                if (objectToRemove != null)
                {
                    this.internalList.Remove(objectToRemove);
                }

                foreach (var filteredCollection in this.filteredCollections)
                {
                    filteredCollection.OnRemoved(objectToRemove);
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called when the collection has changed. Overridden to ensure the event is raised on the correct thread.
        /// This is necessary because the collection is modified on the internal Receive thread,
        /// but the events need to be raised on the user/UI thread.
        /// </summary>
        /// <param name="args">
        /// The args for the event
        /// </param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (SynchronizationContext.Current == this.synchronizationContext)
            {
                // Execute the CollectionChanged event on the current thread
                this.RaiseCollectionChanged(args);
            }
            else
            {
                // Raises the CollectionChanged event on the creator thread
                this.synchronizationContext.Post(this.RaiseCollectionChanged, args);
            }
        }

        /// <summary>
        /// Called when a property has changed. Ensures the event is raised on the correct thread.
        /// This is necessary because properties can be changed on the internal Receive thread,
        /// but the events need to be raised on the user/UI thread
        /// </summary>
        /// <param name="args">
        /// The args for the event
        /// </param>
        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            if (SynchronizationContext.Current == this.synchronizationContext)
            {
                // Execute the PropertyChanged event on the current thread
                this.RaisePropertyChanged(args);
            }
            else
            {
                // Raises the PropertyChanged event on the creator thread
                this.synchronizationContext.Post(this.RaisePropertyChanged, args);
            }
        }

        /// <summary>
        /// Calls a method, and converts the integer result to an integer.
        /// This is useful because most of the collection modification functions return the number of items added/removed/updated.
        /// The untrusted callers can only operate on a single item at a time, so the number returned is only 0 or 1.
        /// This will convert those numbers to true/false to indicate if the operation was successful
        /// I.E., if a user adds an item and 0 is returned, the call was unsuccessful and this will return false
        /// </summary>
        /// <param name="methodName">
        /// The method name to call
        /// </param>
        /// <param name="parameters">
        /// The parameters to invoke the method with
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>. This will return when the method call completes, and the result is returned
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown when an internal error occurs</exception>
        private async Task<bool> CallConvertNumberToBool(string methodName, params object[] parameters)
        {
            int numberUpdated = await this.client.Call<int>(methodName, parameters);

            if (numberUpdated == 0)
            {
                return false;
            }
            else if (numberUpdated == 1)
            {
                return true;
            }

            throw new InvalidOperationException("Unexpected number of documents were updated");
        }

        /// <summary>
        /// Raises the OnCollectionChanged event with the given arguments
        /// </summary>
        /// <param name="param">
        /// The parameters to raise the event with. This must be a NotifyCollectionChangedEventArgs, however
        /// the parameter is generic so it can be invoked with SynchronizationContext.Post 
        /// </param>
        private void RaiseCollectionChanged(object param)
        {
            base.OnCollectionChanged((NotifyCollectionChangedEventArgs)param);
        }

        /// <summary>
        /// Raises the OnPropertyChanged event with the given arguments
        /// </summary>
        /// <param name="param">
        /// The parameters to raise the event with. This must be a PropertyChangedEventArgs, however
        /// the parameter is generic so it can be invoked with SynchronizationContext.Post 
        /// </param>
        private void RaisePropertyChanged(object param)
        {
            base.OnPropertyChanged((PropertyChangedEventArgs)param);
        }
        #endregion
    }
}