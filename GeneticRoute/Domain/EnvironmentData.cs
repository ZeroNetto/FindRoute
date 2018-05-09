using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;

namespace GeneticRoute
{
	public class EnvironmentData
	{
		public readonly HashSet<Manager> Managers;
		public readonly HashSet<Client> Clients;
		public readonly TimeDictionary TimeKeeper;
		public readonly Dictionary<Address, Client> AddressClient;
		private DateTime firstTime;
		private DateTime lastTime;
		
		public EnvironmentData()
		{
			Clients = new HashSet<Client>();
			Managers = new HashSet<Manager>();
			TimeKeeper = new TimeDictionary();
			AddressClient = new Dictionary<Address, Client>();
			firstTime = DateTime.MaxValue;
			lastTime = DateTime.MinValue;
		}
		
		public void ParseFromFile(string pathToManagersFile, string pathToClientsFile)
		{
			using (var managersFile = new StreamReader(pathToManagersFile))
			{
				while (true)
				{
					var managerString = managersFile.ReadLine();
					if (managerString == null)
						break;
					var managerData = managerString.Split('/');
					var startTime = DateTime.ParseExact(managerData[2], "yyyy-MM-dd HH-mm", null);
					var endTime = DateTime.ParseExact(managerData[3], "yyyy-MM-dd HH-mm", null);
					if (startTime < firstTime)
						firstTime = startTime;
					if (endTime > lastTime)
						lastTime = endTime;
					Managers.Add(new Manager(
						new Address(managerData[1]),
						startTime,
						endTime,
						managerData[0])
					);
				}
			}
			using (var clientsFile = new StreamReader(pathToClientsFile))
			{
				while (true)
				{
					var clientString = clientsFile.ReadLine();
					if (clientString == null)
						break;
					var clientData = clientString.Split('/');
					var startTime = DateTime.ParseExact(clientData[2], "yyyy-MM-dd HH-mm", null);
					var endTime = DateTime.ParseExact(clientData[3], "yyyy-MM-dd HH-mm", null);
					if (startTime < firstTime)
						firstTime = startTime;
					if (endTime > lastTime)
						lastTime = endTime;
					Clients.Add(new Client(
						new Address(clientData[1]),
						startTime,
						endTime,
						clientData[0])
					);
				}
			}
			ToFillTime();
		}

		private void ToFillTime()
		{			
			const int sizeOfRequest = 5;
			const string trafficModel = "pessimistic";
			var keys = new List<string>();
			using (var keysFile = new StreamReader("keys"))
				while (true)
				{
					var key = keysFile.ReadLine();
					if (key == null)
						break;
					keys.Add(key);
				}
			var keysEnum = keys.GetEnumerator();
			var count = 0;
			keysEnum.MoveNext();
			var listAddresses = this.Managers
				.Select(manager => manager.CurrentAddress)
				.ToList()
				.Union(this.Clients.Select(client => client.Address).ToList())
				.Partition(sizeOfRequest);
			var date = firstTime.RoundToNearestConstMinutes().ToUniversalTime().ToDropMinutes();
			var endData = lastTime.RoundToNearestConstMinutes().ToUniversalTime();
			do
			{
				foreach (var addresses in listAddresses)
				{
					while (true)
					{
						var timeSeconds = date.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
						var origins = string.Join("+ON|", addresses.Select(address => address.Name)).Replace(' ', '+');
						var url = string.Format(
							"https://maps.googleapis.com/maps/api/distancematrix/json?origins={0}+ON&destinations={0}+ON&mode=driving&language=ru-RU&key={1}&traffic_model={3}&departure_time={2}",
							origins,
							keysEnum.Current,
							timeSeconds,
							trafficModel
						);
						var request = (HttpWebRequest) WebRequest.Create(url);
						request.UserAgent = "Mozilla/5.0 (compatible; ABrowse 0.4; Syllable)";
						using (var stream = request.GetResponse().GetResponseStream())
						{
							using (var reader = new StreamReader(stream))
							{
								var result = JsonConvert.DeserializeObject<GoogleMapAnswer>(reader.ReadToEnd());
								if (result.error_message != null)
								{
									if (!result.error_message.StartsWith("You have exceeded your daily request quota"))
										throw new AcquisitionApiException(result.error_message);
									keysEnum.MoveNext();
									Console.WriteLine(keysEnum.Current);
									if (keysEnum.Current == null)
										throw new AcquisitionApiException("Закончились ключи");
									continue;
								}
								for (var i = 0; i < result.origin_addresses.Length; i++)
								{
									for (var j = 0; j < result.destination_addresses.Length; j++)
									{
										count += 1;
										Console.WriteLine(count);
										if (result.rows[i].elements[j].status != "OK")
											throw new AcquisitionApiException(result.rows[i].elements[j].status);
										var timeAnswer = result.rows[i].elements[j].duration_in_traffic.value;
										this.TimeKeeper.AddAddress(
											addresses[i], addresses[j], date,
											TimeSpan.FromSeconds(double.Parse(timeAnswer)));
									}
								}
							}
						}
						break;
					}
				}
				date = date.AddMinutes(15);
			} while (date != endData);
		}
	}

	class GoogleMapAnswer
	{
		public class DistanceInfo
		{
			public string text { get; set; }
			public string value { get; set; }
		}

		public class Distance
		{
			public DistanceInfo distance { get; set; }
			public DistanceInfo duration { get; set; }
			public DistanceInfo duration_in_traffic { get; set; }
			public string status { get; set; }
	}
		
		public class DistanceMatrix
		{
			public Distance[] elements { get; set; }
		}
		
		public string[] destination_addresses { get; set; }
		public string[] origin_addresses { get; set; }
		public DistanceMatrix[] rows { get; set; }
		public string status { get; set; }
		public string error_message { get; set; }
	}
}
