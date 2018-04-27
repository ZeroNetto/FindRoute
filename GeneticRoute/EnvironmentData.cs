using System;
using System.Collections.Generic;

namespace GeneticRoute
{
	public class EnvironmentData
	{
		public readonly HashSet<Manager> Managers;
		public readonly HashSet<Client> Clients;
		public readonly Dictionary<Address, Dictionary<Address, DateTime>> TimeBetweenAddresses;
	} 
}