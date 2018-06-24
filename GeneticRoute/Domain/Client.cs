using System;
using GeneticRoute.Util;

namespace GeneticRoute
{
    public class Client : ValueType<Client>
    {
        public DateTime MeetingStartTime { get; }
        public DateTime MeetingEndTime { get; }
        public Address Address { get; }
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
	}
}