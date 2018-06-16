using System.Collections.Generic;
using System.Linq;

namespace GeneticRoute
{
	public class GeneticData
	{
		public readonly Dictionary<Manager, List<Address>> Data;

		public GeneticData(Dictionary<Manager, List<Address>> data)
		{
			Data = data;
		}

		public GeneticData ClipWrongEnds(EnvironmentData envData)
		{
			var result = new Dictionary<Manager, List<Address>>();

			foreach (var manager in Data.Keys)
			{
				var currentAddress = manager.CurrentAddress;
				var currentTime = manager.StartOfWork;
				if (Data[manager].Count == 0)
					continue;

				result[manager] = new List<Address> { Data[manager][0] };

				foreach (var address in Data[manager].Skip(1))
				{
					var client = envData.AddressClient[address];
					if (currentTime + envData.TimeKeeper.GetTimeInterval(currentAddress, address, currentTime) >
					    client.MeetingStartTime)
						break;

					result[manager].Add(address);

					currentTime = client.MeetingEndTime;
					currentAddress = address;
				}
			}

			return new GeneticData(result);
		}
	}
}