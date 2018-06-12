using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GeneticRoute
{
    internal class Heap<T> where T : IComparable<T>
    {
        public readonly List<T> List = new List<T>();
	    public T Peek => List[0];
	    public int Count => List.Count;

		public void Add(T element)
        {
            List.Add(element);
            var c = List.Count - 1;
            while (c > 0 && List[c].CompareTo(List[c / 2]) == -1)
            {
                var tmp = List[c];
                List[c] = List[c / 2];
                List[c / 2] = tmp;
                c /= 2;
            }
        }

        public T RemoveMin()
        {
            var result = List[0];
            List[0] = List[List.Count - 1];
            List.RemoveAt(List.Count - 1);
            var c = 0;
            while (c < List.Count)
            {
                var min = c;
                if (2 * c + 1 < List.Count && List[2 * c + 1].CompareTo(List[min]) == -1)
                    min = 2 * c + 1;
                if (2 * c + 2 < List.Count && List[2 * c + 2].CompareTo(List[min]) == -1)
                    min = 2 * c + 2;
                if (min == c)
                    break;

	            var tmp = List[c];
	            List[c] = List[min];
	            List[min] = tmp;
	            c = min;
            }

            return result;
        }
    }

    public class PriorityQueue<TValue, TPriority> : IEnumerable<(TValue, TPriority)> 
	    where TPriority : IComparable
    {
        internal class Node : IComparable<Node>
        {
            public TPriority Priority;
            public TValue Value;

            public int CompareTo(Node other)
            {
                return Priority.CompareTo(other.Priority);
            }

            public (TValue, TPriority) ToTuple()
            {
                return (Value, Priority);
            }
        }

	    private readonly Heap<Node> heap = new Heap<Node>();

	    public (TValue, TPriority) Peek => heap.Peek.ToTuple();
	    public int Count => heap.Count;

	    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	    public IEnumerator<(TValue, TPriority)> GetEnumerator()
	    {
		    return heap.List.Select(e => e.ToTuple()).GetEnumerator();
	    }

		public (TValue, TPriority) GetPriority(TValue value)
        {
            return heap.List.First(e => e.Value.Equals(value)).ToTuple();
        }
        
        public (TValue, TPriority) GetMostPrioritiestValueExcept(HashSet<TValue> notNeeded)
        {
            var random = new Random();
            while (true)
            {
                foreach (var e in heap.List)
                {
                    if (notNeeded.Contains(e.Value) && random.Next(2) == 1)
                        return e.ToTuple();
                }
            }
        }

        public void Add(TPriority priority, TValue element)
        {
            heap.Add(new Node { Priority = priority, Value = element });
        }
    }
}