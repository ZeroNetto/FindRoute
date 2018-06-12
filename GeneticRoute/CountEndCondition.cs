using System.Collections.Generic;

namespace GeneticRoute
{
	public class CountEndCondition : IEndCondition
	{
		private int countNow;
		private readonly int count;

		public CountEndCondition(int count)
		{
			countNow = 0;
			this.count = count;
		}

		public bool IsEnd(List<GeneticData> data, EnvironmentData envData)
		{
			if (countNow >= count)
				return true;

			countNow++;
			return false;
		}
	}
}