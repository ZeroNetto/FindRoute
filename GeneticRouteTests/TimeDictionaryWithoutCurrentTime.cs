using System;
using System.Collections.Generic;
using GeneticRoute;

namespace GeneticRouteTests
{
	public class TimeDictionaryWithoutCurrentTime : ITimeDictionary
	{
		private readonly Dictionary<(Address from, Address to), TimeSpan> timeBetweenAddresses;
		private readonly Dictionary<Address, PriorityQueue<Address, TimeSpan>> addressesInRightRange;

		public TimeDictionaryWithoutCurrentTime()
		{
			timeBetweenAddresses = new Dictionary<(Address from, Address to), TimeSpan>();
			addressesInRightRange = new Dictionary<Address, PriorityQueue<Address, TimeSpan>>();
		}

		public TimeSpan GetTimeInterval(
			Address start, Address end, DateTime currentTime)
		{
			return timeBetweenAddresses[(start, end)];
		}

		public PriorityQueue<Address, TimeSpan> GetAddressesInRightRangeInSomeTime(
			Address address, DateTime currentTime)
		{
			return addressesInRightRange[address];
		}

		public void AddTimeInterval(
			Address startAddress, Address endAddress, DateTime currentTime, TimeSpan valueTime)
		{
			timeBetweenAddresses[(startAddress, endAddress)] = valueTime;
			if (!addressesInRightRange.ContainsKey(startAddress))
				addressesInRightRange[startAddress] = new PriorityQueue<Address, TimeSpan>();
			addressesInRightRange[startAddress].Add(valueTime, endAddress);
		}
	}
}