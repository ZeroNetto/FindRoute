using System;
using GeneticRoute.Util;

namespace GeneticRoute
{
    public class Manager : ValueType<Manager>
    {
	    public string Name { get; }
	    public readonly Address StartAddress;
	    public readonly DateTime StartOfWork;
        public readonly DateTime EndOfWork;
	    public Address CurrentAddress { get; set; }

        public Manager(Address startAddress, DateTime startOfWork, DateTime endOfWork, string name)
        {
	        Name = name;
            StartAddress = CurrentAddress = startAddress;
	        EndOfWork = endOfWork;
            StartOfWork = startOfWork;
        }

        public override int GetHashCode()
        {
	        return Name.GetHashCode();
        }

	    public override bool Equals(object obj)
	    {
		    return Name == ((Manager) obj)?.Name;
	    }
    }
}