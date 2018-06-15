using System;

namespace GeneticRoute
{
	public interface ITimeDictionary
	{
		TimeSpan GetTimeInterval(Address start, Address end, DateTime currentTime);
		void AddTimeInterval(Address startAddress, Address endAddress, DateTime currentTime, TimeSpan valueTime);
		PriorityQueue<Address, TimeSpan> GetAddressesInRightRangeInSomeTime(Address address, DateTime currentTime);
	}
}