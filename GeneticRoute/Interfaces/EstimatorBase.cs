using System.Collections.Generic;
using System.Linq;

namespace GeneticRoute
{
	public abstract class EstimatorBase
	{
		public abstract List<GeneticData>  GetOrderedData(List<GeneticData> data, EnvironmentData envData);

		public List<GeneticData> SelectBests(List<GeneticData> data, int count, EnvironmentData envData)
		{
			return GetOrderedData(data, envData)
				.Take(count)
				.ToList();
		}

		public List<GeneticData> SelectWorsts(List<GeneticData> data, int count, EnvironmentData envData)
		{
			var ordered = GetOrderedData(data, envData);
			return GetOrderedData(data, envData)
				.Skip(ordered.Count - count)
				.ToList();
		}
	}
}