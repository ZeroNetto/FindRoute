using System;

namespace GeneticRoute
{
    public class Manager
    {
        public Address StartAddress { get; set; }
        public DateTime StartOfWork { get; set; }
        public Address CurrentAddress { get; set; }

        public Manager(Address startAddress, DateTime startOfWork)
        {
            this.StartAddress = this.CurrentAddress = startAddress;
            this.StartOfWork = startOfWork;
        }

        public override int GetHashCode()
        {
            return StartAddress.GetHashCode() * 13 +
                   StartOfWork.GetHashCode() * 19;
        }
    }
}