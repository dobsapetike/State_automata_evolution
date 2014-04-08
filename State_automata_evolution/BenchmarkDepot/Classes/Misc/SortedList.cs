using System;
using System.Collections;
using System.Collections.Generic;

namespace BenchmarkDepot.Classes.Misc
{

    /// <summary>
    /// A simple generic list which keeps its comparable elements sorted
    /// - because .NET just doesn't provide one
    /// </summary>
    public class SortedList<T> : IList<T>, IEnumerable
        where T : IComparable
    {

        #region Private region

        /// <summary>
        /// Elements are saved in a generic list
        /// </summary>
        private List<T> _elements;
        
        /// <summary>
        /// Used for finding an element's index - or it's desired index.
        /// Binary search ensures lg(n) complexity
        /// </summary>
        private int BinarySearchIndex(T elem)
        {
            if (Count == 0) return 0;
            int begin = 0, end = _elements.Count - 1, middle = 0;
            while (begin <= end)
            {
                middle = (begin + end) / 2;
                switch (elem.CompareTo(_elements[middle]))
                {
                    case -1:
                        end = middle - 1;
                        break;
                    case 0:
                        begin = end + 1;
                        break;
                    case 1:
                        begin = middle + 1;
                        break;
                }
            }
            // if no such element exists return where it should be
            return elem.CompareTo(_elements[middle]) == 1 ? middle + 1 : middle;
        }

        #endregion

        #region Constructor

        public SortedList()
        {
            _elements = new List<T>();
        }

        #endregion

        #region Interface implementation

        /// <summary>
        /// Adds a new element to the list
        /// </summary>
        public void Add(T value)
        {
            if (IsReadOnly) return;
            var index = BinarySearchIndex(value);
            _elements.Insert(index, value);
        }

        /// <summary>
        /// Adds a collection of new elements
        /// </summary>
        public void AddRange(IEnumerable<T> values)
        {
            foreach (var value in values)
            {
                Add(value);
            }
        }

        /// <summary>
        /// Removes all elements from the list
        /// </summary>
        public void Clear()
        {
            if (IsReadOnly) return;
            _elements.Clear();
        }

        /// <summary>
        /// Determines whether an element is in the list 
        /// </summary>
        public bool Contains(T value)
        {
            if (Count == 0) return false;
            return _elements.Contains(value);
        }

        /// <summary>
        /// Determines the index of a given element
        /// </summary>
        /// <returns>Index of the element - if no such element exists returns -1</returns>
        public int IndexOf(T value)
        {
            if (Count == 0) return -1;
            return _elements.IndexOf(value);
        }

        /// <summary>
        /// Interface defines method for adding element at a specified index
        /// It could mess up the order so instead it throws an exception
        /// </summary>
        public void Insert(int index, T value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// No need for setting a fixed size
        /// </summary>
        public bool IsFixedSize
        {
            get { return false; }
        }

        /// <summary>
        /// Property for setting the list as read-only
        /// </summary>
        public bool IsReadOnly
        {
            get;
            set;
        }

        /// <summary>
        /// Removes the given element if it exists in the list
        /// In case of multiple instances of the same value removes only one of them
        /// </summary>
        /// <returns>Whether the element could be removed</returns>
        public bool Remove(T value)
        {
            if (IsReadOnly || Count == 0) return false;
            return _elements.Remove(value);
        }

        /// <summary>
        /// Removes the n-th element from the list
        /// </summary>
        public void RemoveAt(int index)
        {
            if (IsReadOnly) return;
            if (index >= Count)
            {
                throw new IndexOutOfRangeException();
            }
            _elements.RemoveAt(index);
        }

        /// <summary>
        /// Indexing operator which returns the element at the specified index
        /// </summary>
        public T this[int index]
        {
            get
            {
                if (index >= Count)
                {
                    throw new IndexOutOfRangeException();
                }
                return _elements[index];
            }
            set
            {
                if (IsReadOnly) return;
                if (index >= Count)
                {
                    throw new IndexOutOfRangeException();
                }
                _elements[index] = (T) value;
            }
        }

        /// <summary>
        /// Returns the first (lowest) element of the collection
        /// </summary>
        public T First()
        {
            if (Count == 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            return _elements[0];
        }

        /// <summary>
        /// Returns the last (greatest) element of the collection
        /// </summary>
        public T Last()
        {
            if (Count == 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            return _elements[Count - 1];
        }

        /// <summary>
        /// Copies the entire list into an array
        /// </summary>
        /// <param name="array">Destination</param>
        /// <param name="index">Starts at this index</param>
        public void CopyTo(T[] array, int index)
        {
            if (index >= Count)
            {
                throw new IndexOutOfRangeException();
            }
            _elements.CopyTo(array, index);
        }

        /// <summary>
        /// Returns the number of elements in the list
        /// </summary>
        public int Count
        {
            get { return _elements.Count; }
        }

        /// <summary>
        /// SortedList is not synchronized
        /// </summary>
        public bool IsSynchronized
        {
            get { return false; }
        }

        /// <summary>
        /// Returns the current instance
        /// </summary>
        public object SyncRoot
        {
            get { return this; }
        }

        /// <summary>
        /// Returns the underlying collection's enumerator
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        /// <summary>
        /// Same as the previous but not generic - needed for the IEnumerator interface
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        #endregion
        
    }

}
