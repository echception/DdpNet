// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DdpObject.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the DdpObject class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading;

    using Newtonsoft.Json;

    /// <summary>
    /// Base object for all items that can be contained in a DdpCollection.
    /// </summary>
    public abstract class DdpObject : INotifyPropertyChanged
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DdpObject"/> class.
        /// </summary>
        protected DdpObject()
        {
            this.SerializeId = true;
        }

        #endregion

        #region Public Events

        /// <summary>
        /// The property changed event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the id.
        /// </summary>
        [JsonProperty(PropertyName = "_id")]
        public string Id { get; internal set; }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the id should be serialized
        /// </summary>
        [JsonIgnore]
        internal bool SerializeId { get; set; }

        /// <summary>
        /// Gets or sets the synchronization context.
        /// </summary>
        private SynchronizationContext SynchronizationContext { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Determines if the Id should be serialized
        /// </summary>
        /// <returns>
        /// True if it should be serialized, false otherwise
        /// </returns>
        public bool ShouldSerializeId()
        {
            if (this.SerializeId == false)
            {
                return false;
            }

            return !string.IsNullOrWhiteSpace(this.Id);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called when the object is added to a collection
        /// </summary>
        /// <param name="id">
        /// The id of the object.
        /// </param>
        /// <param name="synchronizationContext">
        /// The synchronization context.
        /// </param>
        internal void OnAdded(string id, SynchronizationContext synchronizationContext)
        {
            Exceptions.ThrowIfNullOrWhiteSpace(id, "id");

            this.Id = id;
            this.SynchronizationContext = synchronizationContext;
        }

        /// <summary>
        /// Called when the object is added to a collection. Override to implement object initialization logic
        /// </summary>
        protected virtual void Initialized()
        {
        }

        /// <summary>
        /// Raises the PropertyChanged event on the correct thread.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "CallerMemberName attribute requires a default value")]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var args = new PropertyChangedEventArgs(propertyName);
            if (this.SynchronizationContext == null || SynchronizationContext.Current == this.SynchronizationContext)
            {
                this.RaisePropertyChanged(args);
            }
            else
            {
                this.SynchronizationContext.Post(this.RaisePropertyChanged, args);
            }
        }

        /// <summary>
        /// Raises the PropertyChanged event
        /// </summary>
        /// <param name="param">
        /// The parameter, which must be a PropertyChangedEventArgs
        /// </param>
        private void RaisePropertyChanged(object param)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, (PropertyChangedEventArgs)param);
            }
        }

        #endregion
    }
}