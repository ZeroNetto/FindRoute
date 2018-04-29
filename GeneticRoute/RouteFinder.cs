using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticRoute
{
	public class RouteFinder
	{
		private readonly EnvironmentData envData;
		private const int selectedCount = 4;

		public RouteFinder(EnvironmentData envData)
		{
			this.envData = envData;
		}

		public IEnumerable<GeneticData> GenerateStartPopulation()
		{
			return Enumerable.Range(0, selectedCount).Select(_ => this.GeneratePartition());
		}

		public GeneticData GeneratePartition()
		{
			var notVisited = new HashSet<Address>(this.envData.Clients.Select(client => client.Address));
			var managersWays = new Dictionary<Manager, HashSet<Address>>();
			var managerCurrAdd = new Dictionary<Manager, Address>();
			foreach (var manager in this.envData.Managers)
			{
				managerCurrAdd[manager] = manager.CurrentAddress;
				managersWays[manager] = new HashSet<Address> {manager.StartAddress};
			}
			while (notVisited.Any())
			{
				foreach (var manager in this.envData.Managers)
				{
					var nextAddress = this.envData
						.TimeBetweenAddresses.GetAddressesInRightRangeInSomeTime(managerCurrAdd[manager])
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
			GeneticData startPopulation
		)
		{
			GeneticData best = null;
			var currentCombinations = new List<GeneticData> { startPopulation };

			while (!endCondition.IsEnd(currentCombinations, envData))
			{
				var selected = selector.SelectBests(currentCombinations, selectedCount, envData);
				var crossed = crosser.Cross(selected, envData);
				currentCombinations = mutator.Mutate(crossed, envData);
				best = selector.SelectBests(currentCombinations.Concat(new[] { best }).ToList(), 1, envData).First();
			}

			return best;
		}
	}
}