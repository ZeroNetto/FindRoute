using System;

namespace GeneticRoute
{
    public class Client
    {
        public DateTime StartOfMeeting { get; set; }
        public DateTime EndOfMeeting { get; set; }
        public Address AddressOfMeeting { get; set; }

        public Client(DateTime startOfMeeting, DateTime endOfMeeting, Address addressOfMeeting)
        {
            this.StartOfMeeting = startOfMeeting;
            this.EndOfMeeting = endOfMeeting;
            this.AddressOfMeeting = addressOfMeeting;
        }

        public override int GetHashCode()
        {
            return StartOfMeeting.GetHashCode() * 13 + EndOfMeeting.GetHashCode() * 17+
                   AddressOfMeeting.GetHashCode() * 19;
        }
    }
}