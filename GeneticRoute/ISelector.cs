using System.Collections.Generic;

namespace GeneticRoute
{
	public interface ISelector
	{
		List<GeneticData> SelectBests(List<GeneticData> data, int count, EnvironmentData envData);
	}
}