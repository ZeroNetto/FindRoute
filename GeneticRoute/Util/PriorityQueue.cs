using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;

namespace GeneticRoute
{
    internal class Heap<T> where T : IComparable<T>
    {
        public readonly List<T> list = new List<T>();

        public void Add(T element)
        {
            list.Add(element);
            var c = list.Count - 1;
            while (c > 0 && list[c].CompareTo(list[c / 2]) == -1)
            {
                var tmp = list[c];
                list[c] = list[c / 2];
                list[c / 2] = tmp;
                c /= 2;
            }
        }

        public T RemoveMin()
        {
            var result = list[0];
            list[0] = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            var c = 0;
            while (c < list.Count)
            {
                var min = c;
                if (2 * c + 1 < list.Count && list[2 * c + 1].CompareTo(list[min]) == -1)
                    min = 2 * c + 1;
                if (2 * c + 2 < list.Count && list[2 * c + 2].CompareTo(list[min]) == -1)
                    min = 2 * c + 2;
                if (min == c)
                    break;
                else
                {
                    var tmp = list[c];
                    list[c] = list[min];
                    list[min] = tmp;
                    c = min;
                }
            }
            return result;
        }

        public T Peek() => list[0];

        public int Count => list.Count;
    }

    public class PriorityQueue<TValue, TPriority> : IEnumerable<Tuple<TValue, TPriority>> where TPriority : IComparable
    {
        internal class Node : IComparable<Node>
        {
            public TPriority Priority;
            public TValue Value;
            public int CompareTo(Node other)
            {
                return Priority.CompareTo(other.Priority);
            }

            public Tuple<TValue, TPriority> ToTuple()
            {
                return Tuple.Create(this.Value, this.Priority);
            }
        }

        public Tuple<TValue, TPriority> GetPriority(TValue value)
        {
            return this.heap.list.First(e => e.Value.Equals(value)).ToTuple();
        }
        
        private readonly Heap<Node> heap = new Heap<Node>();

        public Tuple<TValue, TPriority> OneOfPrioritiestValueNotVisites(HashSet<TValue> notVisitid)
        {
            var random = new Random();
            while (true)
            {
                foreach (var e in this.heap.list)
                {
                    if (notVisitid.Contains(e.Value) && random.Next(2) == 1)
                        return e.ToTuple();
                }
            }
        }

        public void Add(TPriority priority, TValue element)
        {
            heap.Add(new Node() { Priority = priority, Value = element });
        }

        public Tuple<TValue, TPriority> Peek() => heap.Peek().ToTuple();

        public int Count => heap.Count;

        public IEnumerator<Tuple<TValue, TPriority>> GetEnumerator()
        {
            return this.heap.list.Select(e => e.ToTuple()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}