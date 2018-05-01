using System;

namespace GeneticRoute
{
    public class Client
    {
        public DateTime MeetingStartTime { get; set; }
        public DateTime MeetingEndTime { get; set; }
        public Address Address { get; set; }
	    public readonly string Name;

	    public TimeSpan MeetingDuration => MeetingEndTime - MeetingStartTime;

		public Client(Address address, DateTime meetingStartTime, DateTime meetingEndTime, string name)
		{
		    Name = name;
            MeetingStartTime = meetingStartTime;
            MeetingEndTime = meetingEndTime;
            Address = address;
        }

        public override int GetHashCode()
        {
	        return (379 * ((379 * MeetingStartTime.GetHashCode()) ^ MeetingEndTime.GetHashCode())) ^
	               Address.GetHashCode();
        }
    }
}