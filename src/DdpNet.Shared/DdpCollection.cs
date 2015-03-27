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
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
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
    /// <typeparam name="T">
    /// The type to parse the objects into
    /// </typeparam>
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
        /// The internal collection.
        /// </summary>
        private readonly ObservableCollection<T> internalCollection;

        /// <summary>
        /// The modification lock. Ensures only one modification occurs at a time
        /// </summary>
        private readonly object modificationLock = new object();

        /// <summary>
        /// The queued modifications.
        /// </summary>
        private readonly ConcurrentQueue<IModificationParameter> modifications;

        /// <summary>
        /// The synchronization context.
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
            Exceptions.ThrowIfNullOrWhiteSpace(collectionName, "collectionName");

            this.CollectionName = collectionName;
            this.client = client;
            this.filteredCollections = new List<DdpFilteredCollection<T>>();
            this.modifications = new ConcurrentQueue<IModificationParameter>();
            this.synchronizationContext = SynchronizationContext.Current;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DdpCollection{T}"/> class.
        /// </summary>
        /// <param name="internalCollection">
        /// The internal collection.
        /// </param>
        private DdpCollection(ObservableCollection<T> internalCollection)
            : base(internalCollection)
        {
            this.internalCollection = internalCollection;
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
        public async Task<string> AddAsync(T item)
        {
            Exceptions.ThrowIfNull(item, "item");

            if (string.IsNullOrWhiteSpace(item.Id))
            {
                item.Id = Utilities.GenerateId();
            }

            var methodName = string.Format(CultureInfo.InvariantCulture, @"/{0}/insert", this.CollectionName);
            await this.client.Call(methodName, item);

            return item.Id;
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
        /// <exception cref="ArgumentException">
        /// Thrown when both whereFilter and sortFilter are null
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", 
            Justification = "Easier than creating overrides for each combination")]
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
                    whereFilter, 
                    sortFilter);

                this.filteredCollections.Add(filteredCollection);

                foreach (var item in this)
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
            var snapshot = this.internalCollection.ToList();

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
            Exceptions.ThrowIfNullOrWhiteSpace(id, "id");

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
            Exceptions.ThrowIfNullOrWhiteSpace(id, "id");

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
            var addedParameter = new AddedParameter(id, deserializedObject);

            this.modifications.Enqueue(addedParameter);

            this.RaiseModification();
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
            var changed = new ChangedParameter(id, fields, cleared);

            this.modifications.Enqueue(changed);

            this.RaiseModification();
        }

        /// <summary>
        /// Called internally when an item has been removed from the collection
        /// </summary>
        /// <param name="id">
        /// The id of the object to remove
        /// </param>
        void IDdpCollection.Removed(string id)
        {
            var removed = new RemovedParameter(id);

            this.modifications.Enqueue(removed);

            this.RaiseModification();
        }

        #endregion

        #region Methods

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
        /// <exception cref="InvalidOperationException">
        /// Thrown when an internal error occurs
        /// </exception>
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
        /// Adds an object to the collection. This must be called from the user thread
        /// </summary>
        /// <param name="added">
        /// Information about the addition
        /// </param>
        private void InternalAdded(AddedParameter added)
        {
            lock (this.filterLock)
            {
                var typedObject = added.DeserializedObject.ToObject<T>();
                typedObject.OnAdded(added.Id, this.synchronizationContext);

                this.internalCollection.Add(typedObject);

                foreach (var filteredCollection in this.filteredCollections)
                {
                    filteredCollection.OnAdded(typedObject);
                }
            }
        }

        /// <summary>
        /// Changes an object in the collection. Must be called from the user thread
        /// </summary>
        /// <param name="changed">
        /// Information about the change
        /// </param>
        private void InternalChanged(ChangedParameter changed)
        {
            lock (this.filterLock)
            {
                var objectToChange = this.SingleOrDefault(x => x.Id == changed.Id);

                if (objectToChange == null)
                {
                    return;
                }

                this.changer.ChangeObject(objectToChange, changed.Fields, changed.Cleared);

                foreach (var filteredCollection in this.filteredCollections)
                {
                    filteredCollection.OnChanged(objectToChange);
                }
            }
        }

        /// <summary>
        /// Removes an item from the collection. Must be called from the user thread
        /// </summary>
        /// <param name="removed">
        /// Information about the removal
        /// </param>
        private void InternalRemoved(RemovedParameter removed)
        {
            lock (this.filterLock)
            {
                var objectToRemove = this.SingleOrDefault(x => x.Id == removed.Id);

                if (objectToRemove != null)
                {
                    this.internalCollection.Remove(objectToRemove);
                }

                foreach (var filteredCollection in this.filteredCollections)
                {
                    filteredCollection.OnRemoved(objectToRemove);
                }
            }
        }

        /// <summary>
        /// Processes a modification. Must be called on the user thread.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when an invalid modification type is passed
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "Argument is on one of the inner proeprties")]
        private void ProcessModification(object state)
        {
            lock (this.modificationLock)
            {
                IModificationParameter parameter;
                if (this.modifications.TryDequeue(out parameter))
                {
                    switch (parameter.ModificationType)
                    {
                        case ModificationType.Added:
                            this.InternalAdded((AddedParameter)parameter);
                            break;
                        case ModificationType.Changed:
                            this.InternalChanged((ChangedParameter)parameter);
                            break;
                        case ModificationType.Removed:
                            this.InternalRemoved((RemovedParameter)parameter);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("ModificationType");
                    }
                }
            }
        }

        /// <summary>
        /// Raises the ProcessModification method on the correct Synchronization context.
        /// Some context: there is a background receive thread that receives messages from the server.
        /// It then needs to apply those changes to the collection. Since this is an ObservableCollection,
        /// changes trigger the CollectionChanged and PropertyChanged events. However, WPF/XAML applications
        /// expect those events to come from the UI thread.
        /// One option would be to raise just the event on the UI thread, but process the change on the receive thread.
        /// This would be doable if SynchronizationContext.Send was available on all platforms. It is not available on 
        /// WinRT however, only Post is available. Post executes the event asynchronously, and because of this, 
        /// additional changes can happen after the Post, before the Post method actually executes to raise the event,
        /// resulting in an invalid event (for an example, if a item changes position, it would trigger a move event. Another item also
        /// moving before the event fires would result in an exception in the event).
        /// This necessitates that the event occur synchronously after the modification. Thus, the entire add/change/remove is done on the
        /// UI thread using the SynchronizationContext.Post method.
        /// That would be the end of that if we could make a couple assumptions:
        ///     Post delegates execute in queue order
        ///     Post delegates execute one at a time
        /// However, according to https://msdn.microsoft.com/en-us/magazine/gg598924.aspx,
        /// neither of these can be assumed depending on the platform.
        /// We need them to execute in order so the changes are processed in the correct order,
        /// and we don't want different changes to be applied at the same time.
        /// Thus we maintain our own internal queue of modifications to process, and wrap everything in a lock so only one executes at a time.
        /// When the collection receives a change from the receive thread, it puts it in the queue, and then calls Post to process the notification.
        /// It doesn't matter what order they execute in, since they will just take the next item from the queue.
        /// This is a round about way to achieve our goal, however, I haven't found another way to accomplish it in a cross-platform way.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private void RaiseModification()
        {
            if (this.synchronizationContext == SynchronizationContext.Current)
            {
                this.ProcessModification(null);
            }
            else
            {
                this.synchronizationContext.Post(this.ProcessModification, null);
            }
        }

        #endregion
    }
}