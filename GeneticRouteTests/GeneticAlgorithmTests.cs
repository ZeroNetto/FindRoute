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
		private TimeDictionary timeDictionary;
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
		public void FindRoute_OneManagerThreePoints_ShouldFindBest()
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
			envData.AddClient(new Client(adresses[1], timeNow + TimeSpan.FromHours(2), timeNow + TimeSpan.FromHours(2), "1"));
			envData.AddClient(new Client(adresses[2], timeNow + TimeSpan.FromHours(4), timeNow + TimeSpan.FromHours(4), "2"));
			envData.AddClient(new Client(adresses[3], timeNow + TimeSpan.FromHours(6), timeNow + TimeSpan.FromHours(6), "3"));

			var estimator = new Estimator();
			routeFinder = new RouteFinder(envData, estimator, new EmptyMutator(), new GreedyCrosser(estimator), new CountEndCondition(100));
			var startPopulation = routeFinder.GenerateStartPopulation().ToList();
			var result = routeFinder.GeneticAlgorithm(startPopulation);

			result.Data[manager].ShouldBeEquivalentTo(adresses);
		}
	}
}
