﻿using System.Collections.Generic;
using System.Linq;

namespace GeneticRoute
{
	public class GeneticData
	{
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
	}
}