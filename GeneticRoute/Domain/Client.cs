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
			if (meetingStartTime > meetingEndTime)
				throw new ArgumentException("Time of meeting end should be later than meeting start time");

		    Name = name;
            MeetingStartTime = meetingStartTime;
            MeetingEndTime = meetingEndTime;
            Address = address;
        }

	    public Client(Address address, DateTime meetingStartTime, TimeSpan duration, string name)
		    : this(address, meetingStartTime, meetingStartTime + duration, name)
	    {
	    }

        public override int GetHashCode()
        {
	        return (379 * ((379 * MeetingStartTime.GetHashCode()) ^ MeetingEndTime.GetHashCode())) ^
	               Address.GetHashCode();
        }
    }
}