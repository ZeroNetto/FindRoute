using System;
using System.Collections.Generic;

namespace GeneticRoute
{
	public class TimeDictionary : ITimeDictionary
    {
        private readonly Dictionary<DateTime, Dictionary<(Address from, Address to), TimeSpan>> timeBetweenAddresses;
        private readonly Dictionary<DateTime, Dictionary<Address, PriorityQueue<Address, TimeSpan>>> addressesInRightRange;

        public TimeDictionary()
        {
            timeBetweenAddresses = new Dictionary<DateTime, Dictionary<(Address from, Address to), TimeSpan>>();
            addressesInRightRange = new Dictionary<DateTime, Dictionary<Address, PriorityQueue<Address, TimeSpan>>>();
        }

        public TimeSpan GetTimeInterval(
            Address start, Address end, DateTime currentTime)
        {
			if (Equals(start, end))
				return TimeSpan.Zero;

	        currentTime = currentTime.RoundToNearestConstMinutes();
            if (!timeBetweenAddresses.ContainsKey(currentTime))
                throw new FillingDictionaryException("Can't get time interval between addresses " +
                        $"[{start} - {end}] at {currentTime} (can't find key in dictionary)");

            return timeBetweenAddresses[currentTime][(start, end)];
        }

        public PriorityQueue<Address, TimeSpan> GetAddressesInRightRangeInSomeTime(
            Address address, DateTime currentTime)
        {
            currentTime = currentTime.RoundToNearestConstMinutes();
            if (!addressesInRightRange.ContainsKey(currentTime))
                throw new FillingDictionaryException("Can't get addresses in right range " +
                        $"[{address}] at {currentTime} (can't find key in dictionary)");
            return addressesInRightRange[currentTime][address];
        }

        public void AddTimeInterval(
	        Address startAddress, Address endAddress, DateTime currentTime, TimeSpan valueTime)
        {
            if (!timeBetweenAddresses.ContainsKey(currentTime))
                timeBetweenAddresses[currentTime] = new Dictionary<(Address from, Address to), TimeSpan>();
            timeBetweenAddresses[currentTime][(startAddress, endAddress)] = valueTime;
            if (!addressesInRightRange.ContainsKey(currentTime))
                addressesInRightRange[currentTime] = new Dictionary<Address, PriorityQueue<Address, TimeSpan>>();
            if (!addressesInRightRange[currentTime].ContainsKey(startAddress))
                addressesInRightRange[currentTime][startAddress] = new PriorityQueue<Address, TimeSpan>();
            addressesInRightRange[currentTime][startAddress].Add(valueTime, endAddress);
        }
        //public void ChangeAddress();
        //public void RemoveAddress();
    }
}