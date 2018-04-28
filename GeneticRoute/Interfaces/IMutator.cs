using System.Collections.Generic;

namespace GeneticRoute
{
	public interface IMutator
	{
		List<GeneticData> Mutate(List<GeneticData> data, EnvironmentData envData);
	}
}