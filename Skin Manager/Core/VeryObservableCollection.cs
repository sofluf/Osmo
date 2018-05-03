﻿using System;
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
    class VeryObservableCollection<T> : ObservableCollection<T>, INotifyPropertyChanged
    {
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

        private void Collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(CollectionName));
        }

        public VeryObservableCollection(string collectionName, List<T> items) : this(collectionName)
        {

        }

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
        public void Add(List<T> items)
        {
            foreach (T item in items)
                Add(item);
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
    }
}