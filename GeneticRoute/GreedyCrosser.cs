using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticRoute
{
	public class GreedyCrosser : ICrosser
	{
		private readonly EstimatorBase estimator;

		public GreedyCrosser(EstimatorBase estimator)
		{
			this.estimator = estimator;
		}

		public List<GeneticData> Cross(List<GeneticData> data, EnvironmentData envData)
		{
			var ordered = estimator.GetOrderedData(data, envData);

			if (ordered.Count < 3)
				throw new InvalidOperationException("Can't cross population with less than 3 values");

			var worst = ordered[ordered.Count - 1];
			ordered.Remove(worst);

			ordered.Add(CrossTwoMutations(ordered[0], ordered[1], envData));

			return ordered;
		}

		private GeneticData CrossTwoMutations(GeneticData first, GeneticData second, EnvironmentData envData)
		{
			var orderedRoutes = first.Data
				.Select(pair => (pair.Key, GetTimeCorrectWayFrom(pair.Value, pair.Key, envData)))
				.Concat(second.Data.Select(pair => (pair.Key, GetTimeCorrectWayFrom(pair.Value, pair.Key, envData))))
				.OrderBy(tuple => tuple.Item2.Count)
				.ToList();

			var result = new Dictionary<Manager, List<Address>>();
			var alreadyVisitedAddresses = new HashSet<Address>();

			foreach (var managerRoutePair in orderedRoutes)
			{
				if (result.ContainsKey(managerRoutePair.Item1))
					continue;

				var notMeetPartOfWay = GetNotMeetPartOfWay(managerRoutePair.Item2, alreadyVisitedAddresses);
				result[managerRoutePair.Item1] = notMeetPartOfWay;

				foreach (var notMeetAddress in notMeetPartOfWay)
					alreadyVisitedAddresses.Add(notMeetAddress);
			}

			return new GeneticData(result);
		}

		private static List<Address> GetNotMeetPartOfWay(List<Address> addresses, HashSet<Address> alreadyVisited)
		{
			return addresses.TakeWhile(address => !alreadyVisited.Contains(address))
				.ToList();
		}

		private static List<Address> GetTimeCorrectWayFrom(
			List<Address> addresses, 
			Manager manager,
			EnvironmentData envData
		)
		{
			var currentTime = manager.StartOfWork;
			var result = new List<Address> { addresses[0] };
			for (var i = 1; i < addresses.Count; i++)
			{
				if (!envData.AddressClient.ContainsKey(addresses[i]))
					throw new ArgumentException($"Can't find client with {addresses[i]} address");
				var client = envData.AddressClient[addresses[i]];
				var pathTime = envData.TimeKeeper.GetTimeBetweenAddressesInSomeTime(
					result[result.Count - 1],
					addresses[i],
					currentTime);

				if (currentTime + pathTime > client.MeetingStartTime)
					break;

				currentTime = client.MeetingEndTime;
				result.Add(addresses[i]);
			}

			return result;
		}
	}
}