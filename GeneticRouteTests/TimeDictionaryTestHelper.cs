using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneticRoute;

namespace GeneticRouteTests
{
	public static class TimeDictionaryTestHelper
	{
		public static TimeDictionary FillUsingCoordinates((int x, int y)[] points, DateTime currentTime)
		{
			var addresses = PointsToAddresses(points);
			var timeDictionary = new TimeDictionary();

			for (var i = 0; i < points.Length; i++)
			{
				for (var j = 0; j < points.Length; j++)
				{
					if (i == j)
						continue;

					var distanse = Math.Abs(points[i].x - points[j].x) + Math.Abs(points[i].y - points[j].y);
					timeDictionary.AddTimeInterval(
						addresses[i], addresses[j], currentTime, TimeSpan.FromMinutes(distanse));
				}
			}

			return timeDictionary;
		}

		public static Address[] PointsToAddresses((int x, int y)[] points)
		{
			return points.Select(point => new Address($"{point.x},{point.y}"))
				.ToArray();
		}

		public static TimeDictionary FillByDistances(int managersCount, int[][] distances, DateTime currentTime)
		{
			var dictTime = new TimeDictionary();

			for (var i = 0; i < distances.Length; i++)
			{
				for (var j = 0; j < distances[0].Length; j++)
				{
					if (distances[i][j] == 0 || i == j)
						continue;

					dictTime.AddTimeInterval(new Address(i.ToString()), new Address(j.ToString()), 
						currentTime, TimeSpan.FromMinutes(distances[i][j]));
				}
			}

			return dictTime;
		}

		public static TimeDictionaryWithoutCurrentTime FillByDistances(int managersCount, int[][] distances)
		{
			var dictTime = new TimeDictionaryWithoutCurrentTime();

			for (var i = 0; i < distances.Length; i++)
			{
				for (var j = 0; j < distances[0].Length; j++)
				{
					if (distances[i][j] == 0 || i == j)
						continue;

					dictTime.AddTimeInterval(new Address(i.ToString()), new Address(j.ToString()),
						DateTime.Today, TimeSpan.FromMinutes(distances[i][j]));
				}
			}

			return dictTime;
		}
	}
}
