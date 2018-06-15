using System;
using System.Linq;
using NUnit.Framework;
using FluentAssertions;
using GeneticRoute;
using Autofac;

namespace GeneticRouteTests
{
	[TestFixture]
	public class GeneticAlgorithm_Should
	{
		private RouteFinder routeFinder;
		private EnvironmentData envData;

		private static IContainer GetDiContainer()
		{
			var builder = new ContainerBuilder();

			builder.RegisterType<CountEndCondition>()
				.As<IEndCondition>()
				.WithParameter("count", 100);
			builder.RegisterType<Estimator>().As<EstimatorBase>();
			builder.RegisterType<GreedyCrosser>().As<ICrosser>();
			builder.RegisterType<EmptyMutator>().As<IMutator>();
			builder.RegisterType<TimeDictionary>();
			builder.RegisterType<RouteFinder>();

			return builder.Build();
		}

		[Test]
		public void FindRoute_OneManagerThreePointsWithEmptyMeetingTime_ShouldFindBest()
		{
			var adresses = new Address[]
			{
				new Address("123"),
				new Address("1234"),
				new Address("1235"),
				new Address("1236")
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
			envData = new EnvironmentData(timeDict);

			var manager = new Manager(adresses[0], DateTime.Today, DateTime.Now, "Igor");
			envData.Managers.Add(manager);

			var timeNow = DateTime.Today + TimeSpan.FromHours(12);
			envData.AddClient(new Client(adresses[1], timeNow + TimeSpan.FromHours(2), TimeSpan.Zero, "1"));
			envData.AddClient(new Client(adresses[2], timeNow + TimeSpan.FromHours(4), TimeSpan.Zero, "2"));
			envData.AddClient(new Client(adresses[3], timeNow + TimeSpan.FromHours(6), TimeSpan.Zero, "3"));

			var estimator = new Estimator();
			routeFinder = new RouteFinder(envData, estimator, new EmptyMutator(), new GreedyCrosser(estimator), new CountEndCondition(100));
			var startPopulation = routeFinder.GenerateStartPopulation().ToList();
			var result = routeFinder.GeneticAlgorithm(startPopulation);

			result.Data[manager].ShouldBeEquivalentTo(adresses);
		}

		[Test]
		public void FindRoute_OneManagerFivePointsWithNotEmptyMeetingTime_ShouldFindBest()
		{
			var addresses = new Address[]
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
			envData = new EnvironmentData(timeDict);

			var manager = new Manager(addresses[0], DateTime.Today, DateTime.Now, "Igor");
			envData.Managers.Add(manager);

			var timeNow = DateTime.Today + TimeSpan.FromHours(12);
			envData.AddClient(new Client(addresses[2], timeNow + TimeSpan.FromHours(6), TimeSpan.FromHours(1), "2"));
			envData.AddClient(new Client(addresses[1], timeNow + TimeSpan.FromHours(2), TimeSpan.FromHours(1), "1"));
			envData.AddClient(new Client(addresses[3], timeNow + TimeSpan.FromHours(11), TimeSpan.FromHours(2), "3"));

			var estimator = new Estimator();
			routeFinder = new RouteFinder(envData, estimator, new EmptyMutator(), new GreedyCrosser(estimator), new CountEndCondition(100));
			var startPopulation = routeFinder.GenerateStartPopulation().ToList();
			var result = routeFinder.GeneticAlgorithm(startPopulation);

			result.Data[manager].Select(address => address.Name)
				.ShouldBeEquivalentTo(addresses.Select(address => address.Name).OrderBy(name => name));
		}

		[Test]
		public void FindRoute_OneManagerThreePointWithOneUnreacheble_ShouldFindBestReachebleRoute()
		{
			var adresses = new Address[]
			{
				new Address("123"),
				new Address("1234"),
				new Address("1235"),
				new Address("1236")
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
			envData = new EnvironmentData(timeDict);

			var manager = new Manager(adresses[0], DateTime.Today, DateTime.Now, "Igor");
			envData.Managers.Add(manager);

			var timeNow = DateTime.Today + TimeSpan.FromHours(12);
			envData.AddClient(new Client(adresses[1], timeNow + TimeSpan.FromHours(2), TimeSpan.FromHours(1), "1"));
			envData.AddClient(new Client(adresses[2], timeNow + TimeSpan.FromHours(2), TimeSpan.FromHours(1), "2"));
			envData.AddClient(new Client(adresses[3], timeNow + TimeSpan.FromHours(6), TimeSpan.FromHours(2), "3"));

			var estimator = new Estimator();
			routeFinder = new RouteFinder(envData, estimator, new EmptyMutator(), new GreedyCrosser(estimator), new CountEndCondition(100));
			var startPopulation = routeFinder.GenerateStartPopulation().ToList();
			var result = routeFinder.GeneticAlgorithm(startPopulation);

			result.Data[manager].Should().HaveCount(3);
		}

		[Test]
		public void FindRoute_TwoManagersFivePoints_OneShouldVisitThreePointsAndSecondOtherTwoPoints()
		{
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
			envData.AddClient(new Client(new Address("2"), DateTime.Today + TimeSpan.FromMinutes(60 + 15), TimeSpan.FromMinutes(60), "2"));
			envData.AddClient(new Client(new Address("3"), DateTime.Today + TimeSpan.FromHours(6), TimeSpan.FromHours(2), "3"));
			envData.AddClient(new Client(new Address("4"), DateTime.Today + TimeSpan.FromHours(1), TimeSpan.FromMinutes(35), "4"));
			envData.AddClient(new Client(new Address("5"), DateTime.Today + TimeSpan.FromHours(2), TimeSpan.FromMinutes(25), "5"));

			var estimator = new Estimator();
			routeFinder = new RouteFinder(envData, estimator, new EmptyMutator(), new GreedyCrosser(estimator), new CountEndCondition(100));
			var startPopulation = routeFinder.GenerateStartPopulation().ToList();
			var result = routeFinder.GeneticAlgorithm(startPopulation);
		}
	}
}
