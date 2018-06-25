using System.Collections.Generic;
using System.Linq;

namespace GeneticRoute.Util
{
	public static class EnumerableExtensions
	{
		public static IEnumerable<List<T>> Partition<T>(this IEnumerable<T> sequence, int size)
		{
			var buffer = new List<T>(size);
			foreach (var e in sequence)
			{
				if (buffer.Count != size)
					buffer.Add(e);
				else
				{
					yield return buffer;
					buffer.Clear();
				}
			}

			if (buffer.Any())
				yield return buffer;
		}
	}
}
