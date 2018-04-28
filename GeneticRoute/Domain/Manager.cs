using System;

namespace GeneticRoute
{
    public class Manager
    {
	    private static int idNow;

		public readonly int id;
        public Address StartAddress { get; set; }
        public DateTime StartOfWork { get; set; }
        public Address CurrentAddress { get; set; }

        public Manager(Address startAddress, DateTime startOfWork)
        {
	        id = idNow++;
            StartAddress = CurrentAddress = startAddress;
            StartOfWork = startOfWork;
        }

        public override int GetHashCode()
        {
	        return id.GetHashCode();
        }
    }
}