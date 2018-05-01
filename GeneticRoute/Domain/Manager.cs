using System;

namespace GeneticRoute
{
    public class Manager
    {
	    private static int idNow;

	    private readonly int Id;
	    public readonly string Name;
        public Address StartAddress { get; set; }
        public DateTime StartOfWork { get; set; }
        public DateTime EndOfWork { get; set; }
        public Address CurrentAddress { get; set; }

        public Manager(Address startAddress, DateTime startOfWork, DateTime endOfWork, string name)
        {
	        Name = name;
	        Id = idNow++;
            StartAddress = CurrentAddress = startAddress;
	        EndOfWork = endOfWork;
            StartOfWork = startOfWork;
        }

        public override int GetHashCode()
        {
	        return Id.GetHashCode();
        }
    }
}