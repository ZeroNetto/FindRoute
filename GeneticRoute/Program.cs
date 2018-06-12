using System.Linq;
using Autofac;

namespace GeneticRoute
{
	public class Program
	{ 
		public static void Main(string[] args)
		{
			var container = GetDiContainer();
			var routeFinder = container.Resolve<RouteFinder>();
			var startPopulation = routeFinder.GenerateStartPopulation().ToList();
			var result = routeFinder.GeneticAlgorithm(startPopulation);
		}

		private static IContainer GetDiContainer()
		{
			var builder = new ContainerBuilder();

			builder.RegisterType<CountEndCondition>()
				.As<IEndCondition>()
				.WithParameter("count", 100);
			builder.RegisterType<Estimator>().As<EstimatorBase>();
			builder.RegisterType<GreedyCrosser>().As<ICrosser>();
			builder.RegisterType<EmptyMutator>().As<IMutator>();
			builder.RegisterType<EnvironmentData>()
				.WithParameter("timeDictionary", new TimeDictionary());
			builder.RegisterType<RouteFinder>();

			return builder.Build();
		}
	}
}