using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticRoute
{
	public class RouteFinder
	{
		private readonly EnvironmentData envData;
		private const int SelectedCount = 4;

		public RouteFinder(EnvironmentData envData)
		{
			this.envData = envData;
		}

		public IEnumerable<GeneticData> GenerateStartPopulation()
		{
			return Enumerable.Range(0, SelectedCount).Select(_ => GeneratePartition());
		}

		private GeneticData GeneratePartition()
		{
			var notVisited = new HashSet<Address>(envData.Clients.Select(client => client.Address));
			var managersWays = new Dictionary<Manager, List<Address>>();
			var managerCurrAdd = new Dictionary<Manager, Address>();
			foreach (var manager in envData.Managers)
			{
				managerCurrAdd[manager] = manager.CurrentAddress;
				managersWays[manager] = new List<Address> {manager.StartAddress};
			}
			while (notVisited.Any())
			{
				foreach (var manager in envData.Managers)
				{
					var currTime = !envData.AddressClient.ContainsKey(managerCurrAdd[manager]) ?
						manager.StartOfWork : envData.AddressClient[managerCurrAdd[manager]].MeetingEndTime;
					var nextAddress = envData
						.TimeKeeper
						.GetAddressesInRightRangeInSomeTime(managerCurrAdd[manager], currTime)
						.OneOfPrioritiestValueNotVisites(notVisited).Item1;
					notVisited.Remove(nextAddress);
					managerCurrAdd[manager] = nextAddress;
					managersWays[manager].Add(nextAddress);
				}
			}
			return new GeneticData(managersWays);
		}

		public GeneticData GeneticAlgorithm(
			ISelector selector,
			IMutator mutator,
			ICrosser crosser,
			IEndCondition endCondition,
			List<GeneticData> startPopulation
		)
		{
			GeneticData best = null;
			var currentCombinations = startPopulation;

			while (!endCondition.IsEnd(currentCombinations, envData))
			{
				var selected = selector.SelectBests(currentCombinations, SelectedCount, envData);
				var crossed = crosser.Cross(selected, envData);
				currentCombinations = mutator.Mutate(crossed, envData);
				best = selector.SelectBests(currentCombinations.Concat(new[] { best }).ToList(), 1, envData).First();
			}

			return best;
		}
	}
}