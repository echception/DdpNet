// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DdpFilteredCollection.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the DdpFilteredCollection clas
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Threading;

    /// <summary>
    /// Provides a filtered view of a DdpCollection, similar to a Meteor cursor. This will be kept synchronized with the DdpCollection
    /// </summary>
    /// <typeparam name="T">
    /// The type of item the collection stores
    /// </typeparam>
    public class DdpFilteredCollection<T> : ThreadSafeObservableCollection<T>
        where T : DdpObject
    {
        #region Fields

        /// <summary>
        /// The sort comparison.
        /// </summary>
        private readonly Comparison<T> sortComparison;

        /// <summary>
        /// The where filter.
        /// </summary>
        private readonly Func<T, bool> whereFilter;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DdpFilteredCollection{T}"/> class.
        /// </summary>
        /// <param name="collectionName">
        /// The collection name.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="whereFilter">
        /// The where filter.
        /// </param>
        /// <param name="sort">
        /// The sort.
        /// </param>
        internal DdpFilteredCollection(
            string collectionName, 
            SynchronizationContext context, 
            Func<T, bool> whereFilter, 
            Comparison<T> sort) : base(context)
        {
            this.CollectionName = collectionName;

            this.whereFilter = whereFilter;
            this.sortComparison = sort;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the collection name.
        /// </summary>
        public string CollectionName { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Called when an item is added to the DdpCollection. If the item matches the criteria of the 
        /// filter, it will be added
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        internal void OnAdded(T item)
        {
            if (this.whereFilter != null && !this.whereFilter(item))
            {
                return;
            }

            if (this.sortComparison == null)
            {
                this.Add(item);
            }
            else
            {
                int insertIndex = this.FindIndexForItem(item);
                this.Insert(insertIndex, item);
            }
        }

        /// <summary>
        /// Called when an item changes.  If there is a where filter, it will be reevaluated for the object.
        /// If there is a sorting method, the item will be moved if necessary
        /// </summary>
        /// <param name="item">
        /// The item added.
        /// </param>
        internal void OnChanged(T item)
        {
            if (this.Contains(item))
            {
                if (this.whereFilter != null && !this.whereFilter(item))
                {
                    this.OnRemoved(item);
                }
                else if (this.sortComparison != null)
                {
                    var newIndex = this.FindIndexForItem(item);
                    if (this.IndexOf(item) < newIndex)
                    {
                        newIndex--;
                    }

                    var currentIndex = this.IndexOf(item);

                    if (currentIndex != newIndex)
                    {
                        this.Move(currentIndex, newIndex);
                    }
                }
            }
            else
            {
                this.OnAdded(item);
            }
        }

        /// <summary>
        /// Called when an item is removed from the DdpCollection
        /// </summary>
        /// <param name="item">
        /// The item to be removed.
        /// </param>
        internal void OnRemoved(T item)
        {
            this.Remove(item);
        }

        /// <summary>
        /// Performs a binary search to determine where an item should be.
        /// Note that in cases where an item changes, the list may not be completely sorted,
        /// so this method contains logic to ignore the item in the list
        /// </summary>
        /// <param name="item">
        /// The item to find the position of.
        /// </param>
        /// <returns>
        /// The position where the item should be
        /// </returns>
        private int BinarySearch(T item)
        {
            int min = 0;
            int max = this.Count - 1;

            while (min <= max)
            {
                int mid = (min + max) / 2;
                T midItem = this[mid];

                if (midItem == item)
                {
                    if (mid + 1 <= max)
                    {
                        mid = mid + 1;
                        midItem = this[mid];
                    }
                    else if (mid - 1 >= min)
                    {
                        mid = mid - 1;
                        midItem = this[mid];
                    }
                    else
                    {
                        return mid;
                    }
                }

                int result = this.sortComparison(midItem, item);

                if (result == 0)
                {
                    return mid;
                }

                if (result < 0)
                {
                    min = mid + 1;
                }
                else
                {
                    max = mid - 1;
                }
            }

            return min;
        }

        /// <summary>
        /// Finds the index for where an item should be
        /// </summary>
        /// <param name="item">
        /// The item to find the index for
        /// </param>
        /// <returns>
        /// The position where the item should be
        /// </returns>
        private int FindIndexForItem(T item)
        {
            var index = this.BinarySearch(item);
            while (index < this.Count
                   && this.sortComparison(this[index], item) == 0)
            {
                index++;
            }

            return index;
        }

        #endregion
    }
}