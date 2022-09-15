/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.oM.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.oM.RDF
{
    public class SortedHashSet<T> : ICollection<T>
    {
        private readonly IEqualityComparer<T> m_comparer;
        private IDictionary<T, LinkedListNode<T>> m_Dictionary;
        private LinkedList<T> m_LinkedList;

        public SortedHashSet()
            : this(EqualityComparer<T>.Default)
        {
        }

        public SortedHashSet(IEqualityComparer<T> comparer)
        {
            m_comparer = comparer;
            m_Dictionary = new Dictionary<T, LinkedListNode<T>>(comparer);
            m_LinkedList = new LinkedList<T>();
        }

        public int Count => m_Dictionary.Count;

        public virtual bool IsReadOnly => m_Dictionary.IsReadOnly;

        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        public bool Add(T item)
        {
            if (m_Dictionary.ContainsKey(item)) return false;
            var node = m_LinkedList.AddLast(item);
            m_Dictionary.Add(item, node);
            return true;
        }


        public bool Set(int index, T toSet)
        {
            Dictionary<T, LinkedListNode<T>> newDict = new Dictionary<T, LinkedListNode<T>>(m_comparer);
            LinkedList<T> newLinkedList = new LinkedList<T>();

            int i = 0;
            foreach (var item in m_LinkedList)
            {
                if (i != index) 
                {
                    var nod = newLinkedList.AddLast(item);
                    newDict.Add(item, nod);
                    i++;
                    continue;
                }

                var node = newLinkedList.AddLast(toSet);
                newDict.Add(item, node);
                i++;
            }

            m_LinkedList = newLinkedList;
            m_Dictionary = newDict;
            return true;
        }

        public void Clear()
        {
            m_LinkedList.Clear();
            m_Dictionary.Clear();
        }

        public bool Remove(T item)
        {
            if (item == null) return false;
            var found = m_Dictionary.TryGetValue(item, out var node);
            if (!found) return false;
            m_Dictionary.Remove(item);
            m_LinkedList.Remove(node);
            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return m_LinkedList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Contains(T item)
        {
            return item != null && m_Dictionary.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            m_LinkedList.CopyTo(array, arrayIndex);
        }

        public T this[int i]
        {
            get => m_LinkedList.ElementAtOrDefault(i);
            set => Set(i, value);
        }
    }
}
