using System.Collections.Generic;

namespace GeneticRoute
{
	class CountEndCondition : IEndCondition
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
			var result = countNow < count;
			countNow = (countNow + 1) % count;
			return result;
		}
	}
}