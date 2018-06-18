using System;
using System.Linq;
using NUnit.Framework;
using FluentAssertions;
using GeneticRoute;

namespace GeneticRouteTests
{
	[TestFixture]
	public class GeneticAlgorithm_Should
	{
		private const int CountOfCycles = 20000;
		private const int SelectedCount = 20;

		private static RouteFinder GetRouteFinder(EnvironmentData environmentData)
		{
			var estimator = new VisitedClientCountEstimator();
			return new RouteFinder(
				environmentData, 
				estimator, 
				new EmptyMutator(), 
				new GreedyCrosser(estimator), 
				new CountEndCondition(CountOfCycles)
			);
		}

		private static GeneticData GetResult(EnvironmentData envData)
		{
			var routeFinder = GetRouteFinder(envData);
			var startPopulation = routeFinder.GenerateStartPopulation(SelectedCount).ToList();
			return routeFinder.GeneticAlgorithm(startPopulation, SelectedCount - 2);
		}

        [Test]
        public void FindRoute_UrFU_to_Parahod()
        {
            var envData = new GoogleDataParser().ParseFromFile(
                "test_managers",
                "test_clients"
                );
            var result = GetResult(envData);
            result.Data[envData.Managers.First()].ShouldBeEquivalentTo(
                envData.Clients.Select(c => c.Address).Union(envData.Managers.Select(c => c.StartAddress)));
        }

        [Test]
		public void FindRoute_OneManagerThreePointsWithEmptyMeetingTime_ShouldFindBest()
		{
			var adresses = new[]
			{
				new Address("0"),
				new Address("1"),
				new Address("2"),
				new Address("3")
			};

			var timeDict = new TimeDictionary();
			for (var t = 0; t < 24; t++)
			{
				for (var i = 0; i < adresses.Length; i++)
				{
					for (var j = 0; j < adresses.Length; j++)
					{
						if (i == j)
							continue;
						timeDict.AddTimeInterval(
							adresses[i], 
							adresses[j], 
							DateTime.Today + TimeSpan.FromHours(t), 
							TimeSpan.FromHours(2)
						);
					}
				}
			}
			var envData = new EnvironmentData(timeDict);

			var manager = new Manager(adresses[0], DateTime.Today, DateTime.Now, "Igor");
			envData.Managers.Add(manager);

			var timeNow = DateTime.Today + TimeSpan.FromHours(12);
			envData.AddClient(new Client(adresses[1], timeNow + TimeSpan.FromHours(2), TimeSpan.Zero, "1"));
			envData.AddClient(new Client(adresses[2], timeNow + TimeSpan.FromHours(4), TimeSpan.Zero, "2"));
			envData.AddClient(new Client(adresses[3], timeNow + TimeSpan.FromHours(6), TimeSpan.Zero, "3"));

			var result = GetResult(envData);

			result.Data[manager].ShouldBeEquivalentTo(adresses);
		}

		[Test]
		public void FindRoute_OneManagerFivePointsWithNotEmptyMeetingTime_ShouldFindBest()
		{
			var addresses = new[]
			{
				new Address("1"),
				new Address("2"),
				new Address("3"),
				new Address("4")
			};

			var timeDict = new TimeDictionary();
			for (var t = 0; t < 24; t++)
			{
				for (var i = 0; i < addresses.Length; i++)
				{
					for (var j = 0; j < addresses.Length; j++)
					{
						if (i == j)
							continue;
						timeDict.AddTimeInterval(
							addresses[i],
							addresses[j],
							DateTime.Today + TimeSpan.FromHours(t),
							TimeSpan.FromHours(2)
						);
					}
				}
			}
			var envData = new EnvironmentData(timeDict);

			var manager = new Manager(addresses[0], DateTime.Today, DateTime.Now, "Igor");
			envData.Managers.Add(manager);

			var timeNow = DateTime.Today + TimeSpan.FromHours(12);
			envData.AddClient(new Client(addresses[2], timeNow + TimeSpan.FromHours(6), TimeSpan.FromHours(1), "2"));
			envData.AddClient(new Client(addresses[1], timeNow + TimeSpan.FromHours(2), TimeSpan.FromHours(1), "1"));
			envData.AddClient(new Client(addresses[3], timeNow + TimeSpan.FromHours(9), TimeSpan.FromHours(2), "3"));

			var result = GetResult(envData);

			result.Data[manager].Select(address => address.Name)
				.ShouldBeEquivalentTo(addresses.Select(address => address.Name).OrderBy(name => name));
		}

		[Test]
		public void FindRoute_OneManagerThreePointWithOneUnreacheble_ShouldFindBestReachebleRoute()
		{
			var adresses = new[]
			{
				new Address("0"),
				new Address("11"),
				new Address("12"),
				new Address("2")
			};

			var timeDict = new TimeDictionary();
			for (var t = 0; t < 24; t++)
			{
				for (var i = 0; i < adresses.Length; i++)
				{
					for (var j = 0; j < adresses.Length; j++)
					{
						if (i == j)
							continue;
						timeDict.AddTimeInterval(
							adresses[i],
							adresses[j],
							DateTime.Today + TimeSpan.FromHours(t),
							TimeSpan.FromHours(2)
						);
					}
				}
			}
			var envData = new EnvironmentData(timeDict);

			var manager = new Manager(adresses[0], DateTime.Today, DateTime.Now, "Igor");
			envData.Managers.Add(manager);

			var timeNow = DateTime.Today + TimeSpan.FromHours(12);
			envData.AddClient(new Client(adresses[1], timeNow + TimeSpan.FromHours(2), TimeSpan.FromHours(1), "1"));
			envData.AddClient(new Client(adresses[2], timeNow + TimeSpan.FromHours(2), TimeSpan.FromHours(1), "2"));
			envData.AddClient(new Client(adresses[3], timeNow + TimeSpan.FromHours(6), TimeSpan.FromHours(2), "3"));

			var result = GetResult(envData);

			result.Data[manager].Should().HaveCount(2 + 1);
		}

		[Test]
		public void FindRoute_TwoManagersFivePoints_OneShouldVisitThreePointsAndSecondOtherTwoPoints()
		{
			// ReSharper disable once UnusedVariable
			const string urlToMap = "https://yandex.ru/maps/-/CBuEB0f4lC";
			const int managersCount = 2;

			var distances = new[]
			{
				new [] {0, 0, 16, 15, 19, 14, 28},
				new [] {0, 0, 16, 15, 19, 14, 28},
				new [] {16, 16, 0, 15, 30, 31, 36},
				new [] {15, 15, 15, 0, 16, 25, 21},
				new [] {19, 19, 30, 16, 0, 15, 7},
				new [] {14, 14, 31, 25, 15, 0, 24},
				new [] {28, 28, 36, 21, 7, 24, 0}
			};

			var timeDict = TimeDictionaryTestHelper.FillByDistances(managersCount, distances);
			var envData = new EnvironmentData(timeDict);

			var managersAddress = new Address("0");
			for (var i = 0; i < managersCount; i++)
				envData.Managers.Add(new Manager(
					managersAddress,
					DateTime.Today, 
					DateTime.Today + TimeSpan.FromDays(1),
					i.ToString()
				));

			envData.AddClient(new Client(new Address("1"), DateTime.Today + TimeSpan.FromHours(4), TimeSpan.FromMinutes(80), "1"));
			envData.AddClient(new Client(new Address("2"), DateTime.Today + TimeSpan.FromMinutes(2 * 60 + 15), TimeSpan.FromMinutes(60), "2"));
			envData.AddClient(new Client(new Address("3"), DateTime.Today + TimeSpan.FromHours(6), TimeSpan.FromHours(2), "3"));
			envData.AddClient(new Client(new Address("4"), DateTime.Today + TimeSpan.FromHours(1), TimeSpan.FromMinutes(35), "4"));
			envData.AddClient(new Client(new Address("5"), DateTime.Today + TimeSpan.FromHours(2), TimeSpan.FromMinutes(25), "5"));

			var result = GetResult(envData);

			// exclude start point (address of manager)
			var visitedCounts = result.Data.Select(pair => pair.Value.Count - 1); 
			visitedCounts.Should().Contain(2).And.Contain(3);
		}
	}
}
