using System.Collections.Generic;

namespace GeneticRoute
{
	public interface ICrosser
	{
		List<GeneticData> Cross(List<GeneticData> data, EnvironmentData envData);
	}
}