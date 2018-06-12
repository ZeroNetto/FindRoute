using System;

namespace GeneticRoute
{
	public interface ITimeDictionary
	{
		TimeSpan GetTimeBetweenAddressesInSomeTime(Address start, Address end, DateTime currTime);
		void AddTimeInterval(Address startAddress, Address endAddress, DateTime currTime, TimeSpan valueTime);
		PriorityQueue<Address, TimeSpan> GetAddressesInRightRangeInSomeTime(Address address, DateTime currTime);
	}
}