namespace DdpNet
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;

    public class DdpFilteredCollection<T> : ReadOnlyObservableCollection<T> where T: DdpObject
    {
        public string CollectionName { get; private set; }

        private readonly ObservableCollection<T> internalCollection;

        private readonly SynchronizationContext synchronizationContext;

        private readonly Func<T, bool> whereFilter;
        private readonly Comparison<T> sortComparison; 

        internal DdpFilteredCollection(string collectionName, SynchronizationContext context, Func<T, bool> whereFilter, Comparison<T> sort) : this(new ObservableCollection<T>())
        {
            this.CollectionName = collectionName;
            this.synchronizationContext = context;

            this.whereFilter = whereFilter;
            this.sortComparison = sort;
        }

        private DdpFilteredCollection(ObservableCollection<T> internalCollection) : base(internalCollection)
        {
            this.internalCollection = internalCollection;
        }

        internal void OnAdded(T item)
        {
            if (this.whereFilter != null && !this.whereFilter(item))
            {
                return;
            }

            if (this.sortComparison == null)
            {
                this.internalCollection.Add(item);
            }
            else
            {
                int insertIndex = this.FindIndexForItem(item);
                this.internalCollection.Insert(insertIndex, item);
            }
        }

        internal void OnChanged(T item)
        {
            if (this.internalCollection.Contains(item))
            {
                if (this.whereFilter != null && !this.whereFilter(item))
                {
                    this.OnRemoved(item);
                }
                else if (this.sortComparison != null)
                {
                    var newIndex = this.FindIndexForItem(item);
                    if (this.internalCollection.IndexOf(item) < newIndex)
                    {
                        newIndex--;
                    }

                    this.internalCollection.Move(this.internalCollection.IndexOf(item), newIndex);
                }
            }
            else
            {
                this.OnAdded(item);
            }
        }

        internal void OnRemoved(T item)
        {
            this.internalCollection.Remove(item);
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (SynchronizationContext.Current == this.synchronizationContext)
            {
                // Execute the CollectionChanged event on the current thread
                RaiseCollectionChanged(e);
            }
            else
            {
                // Raises the CollectionChanged event on the creator thread
                this.synchronizationContext.Post(RaiseCollectionChanged, e);
            }
        }

        private void RaiseCollectionChanged(object param)
        {
            base.OnCollectionChanged((NotifyCollectionChangedEventArgs)param);
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (SynchronizationContext.Current == this.synchronizationContext)
            {
                // Execute the PropertyChanged event on the current thread
                RaisePropertyChanged(e);
            }
            else
            {
                // Raises the PropertyChanged event on the creator thread
                this.synchronizationContext.Post(RaisePropertyChanged, e);
            }
        }
        private void RaisePropertyChanged(object param)
        {
            base.OnPropertyChanged((PropertyChangedEventArgs)param);
        }

        private int FindIndexForItem(T item)
        {
            var index = BinarySearch(item);
            while (index < this.internalCollection.Count && this.sortComparison(this.internalCollection[index], item) == 0)
            {
                index++;
            }

            return index;
        }

        private int BinarySearch(T item)
        {
            int min = 0;
            int max = this.internalCollection.Count - 1;

            while (min <= max)
            {
                int mid = (min + max) / 2;
                T midItem = this.internalCollection[mid];

                if (midItem == item)
                {
                    if (mid + 1 <= max)
                    {
                        mid = mid + 1;
                        midItem = this.internalCollection[mid];
                    }
                    else if (mid - 1 >= min)
                    {
                        mid = mid - 1;
                        midItem = this.internalCollection[mid];
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
    }
}
