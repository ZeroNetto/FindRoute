using System;
using System.Collections.Generic;

namespace GeneticRoute
{
    public class TimeDictionary
    {
        private Dictionary<Tuple<Address, Address>, DateTime> timeBetweenAddresses;
        private Dictionary<Address, PriorityQueue<Address, DateTime>> addressesInRightRange;
        
        public TimeDictionary()
        {
            throw new NotImplementedException();
        }

        public DateTime GetTimeBetweenAddressesInSomeTime(
            Address start, Address end, DateTime currTime = new DateTime())
        {
            //Потом в зависимости от currTime будет разный разультат
            return timeBetweenAddresses[Tuple.Create(start, end)];
        }

        public PriorityQueue<Address, DateTime> GetAddressesInRightRangeInSomeTime(
            Address address, DateTime currTime = new DateTime())
        {
            //Потом в зависимости от currTime будет разный разультат
            return addressesInRightRange[address];
        }

        //public void AddAddress();
        //public void ChangeAddress();
        //public void RemoveAddress();
    }
}