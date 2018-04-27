using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticRoute
{
	public class RouteFinder
	{
		private readonly EnvironmentData envData;

		public RouteFinder(EnvironmentData envData)
		{
			this.envData = envData;
		}

		public GeneticData GenerateStartPopulation()
		{
			throw new NotImplementedException();
		}

		public GeneticData GeneticAlgorithm(
			ISelector selector,
			IMutator mutator,
			ICrosser crosser,
			IEndCondition endCondition,
			GeneticData startPopulation
		)
		{
			const int selectedCount = 4;

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