using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace GeneticRoute
{
	public class EnvironmentData
	{
		public readonly HashSet<Manager> Managers;
		public readonly HashSet<Client> Clients;
		public readonly TimeDictionary TimeKeeper;

		[CanBeNull]
		public Client FindClientWithAddress(Address address)
		{
			return Clients.FirstOrDefault(client => client.Address == address);
		}
	} 
}