using System.Collections.Generic;

namespace GeneticRoute
{
	public class GeneticData
	{
		public readonly Dictionary<Manager, List<Address>> Data;

		public GeneticData(Dictionary<Manager, List<Address>> data)
		{
			Data = data;
		}
	}
}