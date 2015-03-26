// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThreadSafeObservableCollection.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   The thread safe observable collection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DdpNet
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// An ObservableCollection that allows modification functions to be called from any thread.
    /// This is still used under the constraint that a single thread is updating the collection (does not guarantee multiple changes at once),
    /// just that the modifying thread does not have to be the main/UI thread.
    /// </summary>
    /// <typeparam name="T">
    /// The type of item to store
    /// </typeparam>
    public class ThreadSafeObservableCollection<T> : ReadOnlyObservableCollection<T>
        where T : DdpObject
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadSafeObservableCollection{T}"/> class.
        /// </summary>
        internal ThreadSafeObservableCollection()
            : this(new ObservableCollection<T>(), SynchronizationContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadSafeObservableCollection{T}"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        internal ThreadSafeObservableCollection(SynchronizationContext context)
            : this(new ObservableCollection<T>(), context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadSafeObservableCollection{T}"/> class.
        /// </summary>
        /// <param name="internalCollection">
        /// The internal collection.
        /// </param>
        /// <param name="synchronizationContext">
        /// The synchronization Context.
        /// </param>
        private ThreadSafeObservableCollection(
            ObservableCollection<T> internalCollection, 
            SynchronizationContext synchronizationContext)
            : base(internalCollection)
        {
            this.InternalCollection = internalCollection;
            this.SynchronizationContext = synchronizationContext;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the synchronization context.
        /// </summary>
        internal SynchronizationContext SynchronizationContext { get; private set; }

        /// <summary>
        /// Gets or sets the internal collection.
        /// </summary>
        private ObservableCollection<T> InternalCollection { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get enumerator. Overridden to return a snapshot, as the collection can be modified on multiple threads.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        public new IEnumerator<T> GetEnumerator()
        {
            var snapshot = this.InternalCollection.ToList();

            return snapshot.GetEnumerator();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds an item to the collection
        /// </summary>
        /// <param name="item">
        /// The item to add.
        /// </param>
        internal void Add(T item)
        {
            this.InvokeOnSynchrnoizationContext(this.AddInternal, item);
        }

        /// <summary>
        /// Inserts an item at a given index
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="item">
        /// The item.
        /// </param>
        internal void Insert(int index, T item)
        {
            var insertInfo = new InsertParameter<T>(item, index);

            this.InvokeOnSynchrnoizationContext(this.InsertInternal, insertInfo);
        }

        /// <summary>
        /// Moves an item at one index to another index
        /// </summary>
        /// <param name="oldIndex">
        /// The old index.
        /// </param>
        /// <param name="newIndex">
        /// The new index.
        /// </param>
        internal void Move(int oldIndex, int newIndex)
        {
            var moveInfo = new MoveParameter(oldIndex, newIndex);

            this.InvokeOnSynchrnoizationContext(this.MoveInternal, moveInfo);
        }

        /// <summary>
        /// Removes an item from the collection
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        internal void Remove(T item)
        {
            this.InvokeOnSynchrnoizationContext(this.RemoveInternal, item);
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
            if (SynchronizationContext.Current == this.SynchronizationContext)
            {
                // Execute the PropertyChanged event on the current thread
                this.RaisePropertyChanged(args);
            }
            else
            {
                // Raises the PropertyChanged event on the creator thread
                this.SynchronizationContext.Post(this.RaisePropertyChanged, args);
            }
        }

        /// <summary>
        /// Internal Add function. Takes a generic object so it can be called from SynchronizationContext.Post
        /// </summary>
        /// <param name="param">
        /// The parameter. Must be of type T
        /// </param>
        private void AddInternal(object param)
        {
            this.InternalCollection.Add((T)param);
        }

        /// <summary>
        /// Internal Insert function. Takes an object so it can be called with SynchronizationContext.Post
        /// </summary>
        /// <param name="param">
        /// The parameter. Must be an InsertParameter(T)
        /// </param>
        private void InsertInternal(object param)
        {
            var insertInfo = (InsertParameter<T>)param;

            this.InternalCollection.Insert(insertInfo.Index, insertInfo.Item);
        }

        /// <summary>
        /// If the current SynchronizationContext is not equal to the one this was created on, calls Post on the SynchronizationContext
        /// with the callback. Otherwise it calls the callback directly.
        /// A little context as to why this is necessary. There is a background receive thread that processes messages from the server.
        /// Many of these messages make modifications to the a collection (adding an item, changing an item, removing an item, etc).
        /// When these changes are made to the collection, they often result in the CollectionChanged event being fired. Since this happens
        /// in the background thread, the event is also fired on the background thread.
        /// WPF/XAML does not like it when the events are fired on the background thread, they require the event be fired from the UI thread.
        /// In the original implementation, OnCollectionChanged was overridden to fire the event on the UI thread using
        /// SynchronizationContext.Send. This worked great, however Send is not available in Windows 8.1 / WP 8.1 apps, they only support
        /// Post. Post is very similar except it executes asynchronously. This appeared to work for a while, however an issue was discovered
        /// when many changes were coming in at once. An item would be added, then moved, possibly multiple times before the async Post was
        /// executed. This resulted in events being fired that no longer had correct parameters, resulting in unhandled exceptions.
        /// Some workarounds were attempted, however it came down to the events need to be fired synchronously to the collection being changed.
        /// As a result, instead of just firing the event on another thread, the actual collection modification functions are executed on the main thread.
        /// This ensures the event is fired on the same thread, and that it executed synchronously with the modification.
        /// </summary>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        private void InvokeOnSynchrnoizationContext(SendOrPostCallback callback, object parameter)
        {
            if (this.SynchronizationContext == SynchronizationContext.Current)
            {
                callback(parameter);
            }
            else
            {
                this.SynchronizationContext.Post(callback, parameter);
            }
        }

        /// <summary>
        /// Internal Move function. Takes an object so it can be called with SynchronizationContext.Post
        /// </summary>
        /// <param name="param">
        /// The parameter. Must be a MoveParameter
        /// </param>
        private void MoveInternal(object param)
        {
            var moveInfo = (MoveParameter)param;

            this.InternalCollection.Move(moveInfo.OldIndex, moveInfo.NewIndex);
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

        /// <summary>
        /// Internal Remove function. Takes an object so it can be called with SynchronizationContext.Post
        /// </summary>
        /// <param name="param">
        /// The parameter. Must be of type T
        /// </param>
        private void RemoveInternal(object param)
        {
            this.InternalCollection.Remove((T)param);
        }

        #endregion
    }
}