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


		/*
		private readonly List<List<int>> data;

		public GeneticData(List<List<int>> data)
		{
			this.data = data;
		}

		public GeneticData(params int[][] data)
		{
			this.data = data.Select(row => row.ToList())
				.ToList();
		}

		public List<int> this[int index] => data[index];
		*/
	}
}