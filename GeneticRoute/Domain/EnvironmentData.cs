using System.Collections.Generic;

namespace GeneticRoute
{
	public class EnvironmentData
	{
		public readonly HashSet<Manager> Managers;
		public readonly HashSet<Client> Clients;
		public readonly TimeDictionary TimeKeeper;
		public readonly Dictionary<Address, Client> AddressClient;

		public EnvironmentData(HashSet<Client> clients, HashSet<Manager> managers, TimeDictionary timeDictionary)
		{
			Clients = new HashSet<Client>();
			AddressClient = new Dictionary<Address, Client>();
			foreach (var client in clients)
				AddClient(client);
			Managers = managers;
			TimeKeeper = timeDictionary;
		}
		
		public EnvironmentData(TimeDictionary timeDictionary) 
			: this (new HashSet<Client>(), new HashSet<Manager>(), timeDictionary)
		{
		}

		public void AddClient(Client client)
		{
			Clients.Add(client);
			AddressClient.Add(client.Address, client);
		}
	}
}
