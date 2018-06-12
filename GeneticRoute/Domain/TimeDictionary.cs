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

        public TimeSpan GetTimeBetweenAddressesInSomeTime(
            Address start, Address end, DateTime currTime)
        {
	        currTime = currTime.RoundToNearestConstMinutes();
            if (!timeBetweenAddresses.ContainsKey(currTime))
                throw new FillingDictionaryException("Can't get time interval between addresses " +
                        $"[{start} - {end}] at {currTime} (can't find key in dictionary)");

            return timeBetweenAddresses[currTime][(start, end)];
        }

        public PriorityQueue<Address, TimeSpan> GetAddressesInRightRangeInSomeTime(
            Address address, DateTime currTime)
        {
            currTime = currTime.RoundToNearestConstMinutes();
            if (!addressesInRightRange.ContainsKey(currTime))
                throw new FillingDictionaryException("Что-то пошло не так");
            return addressesInRightRange[currTime][address];
        }

        public void AddTimeInterval(
	        Address startAddress, Address endAddress, DateTime currTime, TimeSpan valueTime)
        {
            if (!timeBetweenAddresses.ContainsKey(currTime))
                timeBetweenAddresses[currTime] = new Dictionary<(Address from, Address to), TimeSpan>();
            timeBetweenAddresses[currTime][(startAddress, endAddress)] = valueTime;
            if (!addressesInRightRange.ContainsKey(currTime))
                addressesInRightRange[currTime] = new Dictionary<Address, PriorityQueue<Address, TimeSpan>>();
            if (!addressesInRightRange[currTime].ContainsKey(startAddress))
                addressesInRightRange[currTime][startAddress] = new PriorityQueue<Address, TimeSpan>();
            addressesInRightRange[currTime][startAddress].Add(valueTime, endAddress);
        }
        //public void ChangeAddress();
        //public void RemoveAddress();
    }
}