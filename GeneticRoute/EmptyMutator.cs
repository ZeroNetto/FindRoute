using System.Collections.Generic;

namespace GeneticRoute
{
	class EmptyMutator : IMutator
	{
		public List<GeneticData> Mutate(List<GeneticData> data, EnvironmentData envData)
		{
			return data;
		}
	}
}