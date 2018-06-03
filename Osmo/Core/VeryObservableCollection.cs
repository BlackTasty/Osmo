using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osmo.Core
{
    /// <summary>
    /// This class is an extension to the <see cref="ObservableCollection{T}"/>. 
    /// (this removes the need to call the "OnNotifyPropertyChanged" event every time you add, edit or remove an entry.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    public class VeryObservableCollection<T> : ObservableCollection<T>, INotifyPropertyChanged
    {
        private bool autoSort;
        private string watchAlso;

        new event PropertyChangedEventHandler PropertyChanged;

        public string CollectionName { get; }

        /// <summary>
        /// Initializes the collection with the specified name.
        /// </summary>
        /// <param name="collectionName">The name of the collection (must match the property name!)</param>
        public VeryObservableCollection(string collectionName)
        {
            CollectionName = collectionName;
            CollectionChanged += Collection_CollectionChanged;
        }

        public VeryObservableCollection(string collectionName, T item) : this(collectionName)
        {
            Add(item);
        }

        /// <summary>
        /// Initializes the collection with the specified name and if auto-sorting is enabled.
        /// </summary>
        /// <param name="collectionName">The name of the collection (must match the property name!)</param>
        /// <param name="autoSort">If true the list is sorted after every change</param>
        public VeryObservableCollection(string collectionName, bool autoSort) : this(collectionName)
        {
            this.autoSort = autoSort;
        }

        /// <summary>
        /// Tell this <see cref="VeryObservableCollection{T}"/> to trigger the PropertyChanged event on another property.
        /// </summary>
        /// <param name="propertyName">The name of the property (the property which is exposed to XAML)</param>
        public void WatchAlso(string propertyName)
        {
            watchAlso = propertyName;
        }

        private void Collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(this, new PropertyChangedEventArgs(CollectionName));
            if (watchAlso != null)
                OnPropertyChanged(this, new PropertyChangedEventArgs(watchAlso));
        }

        /// <summary>
        /// NOT IMPLEMENTED! Initializes the collection with the specified name and copies all given items into into it.
        /// </summary>
        /// <param name="collectionName">The name of the collection (must match the property name!)</param>
        /// <param name="items">The <see cref="List{T}"/> to copy the items from</param>
        public VeryObservableCollection(string collectionName, List<T> items) : this(collectionName)
        {

        }
        
        /// <summary>
        /// NOT IMPLEMENTED! Initializes the collection with the specified name and copies all given items into into it.
        /// </summary>
        /// <param name="collectionName">The name of the collection (must match the property name!)</param>
        /// <param name="items">The <see cref="IEnumerable{T}"/> to copy the items from</param>
        public VeryObservableCollection(string collectionName, IEnumerable<T> items) : this(collectionName)
        {

        }

        /// <summary>
        /// Adds multiple objects to the end of the <see cref="Collection{T}"/>.
        /// </summary>
        /// <param name="items">The objects to be added to the end of the <see cref="Collection{T}"/>.</param>
        public void Add(IEnumerable<T> items)
        {
            foreach (T item in items)
                Add(item);
        }

        /// <summary>
        /// Adds multiple objects to the end of the <see cref="Collection{T}"/>.
        /// </summary>
        /// <param name="items">The objects to be added to the end of the <see cref="Collection{T}"/>.</param>
        /// <param name="sort">If true, the collection is sorted after adding all items</param>
        public void Add(List<T> items, bool sort = true)
        {
            foreach (T item in items)
            {
                if (sort)
                    Add(item);
                else
                    base.Add(item);
            }
        }

        public new void Add(T item)
        {
            base.Add(item);
            List<T> lookupList = Items.OrderBy(x => x.ToString(), StringComparer.CurrentCultureIgnoreCase)
                .ToList();
            foreach (T obj in lookupList)
            {
                if (obj.Equals(item))
                {
                    Remove(item);
                    Insert(lookupList.IndexOf(obj), obj);
                }
            }

        }

        public new void Clear()
        {
            try
            {
                base.Clear();
            }
            catch
            {
                for (int i = 0; i < Count; i++)
                    RemoveAt(0);
            }
        }

        public void Refresh()
        {
            OnPropertyChanged(this, new PropertyChangedEventArgs(CollectionName));
            if (watchAlso != null)
                OnPropertyChanged(this, new PropertyChangedEventArgs(watchAlso));
        }

        protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }
    }
}