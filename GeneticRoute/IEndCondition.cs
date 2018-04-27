using System.Collections.Generic;

namespace GeneticRoute
{
	public interface IEndCondition
	{
		bool IsEnd(List<GeneticData> data, EnvironmentData envData);
	}
}