using System.Collections.Generic;
using System.Linq;

namespace GeneticRoute
{
	public class VisitedClientCountEstimator : EstimatorBase
	{
		public override List<GeneticData> GetOrderedData(List<GeneticData> data, EnvironmentData envData)
		{
			return data.OrderByDescending(genetic => GetVisitedClientsCount(genetic, envData))
				.ToList();
		}

		private static int GetVisitedClientsCount(GeneticData data, EnvironmentData envData)
		{
			return data.ClipWrongEnds(envData).Data
				.Select(pair => pair.Value.Count)
				.Sum();
		}
	}
}