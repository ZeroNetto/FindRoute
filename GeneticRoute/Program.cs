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

			builder.RegisterType<IEndCondition>().As<CountEndCondition>();
			builder.RegisterType<EstimatorBase>().As<Estimator>();
			builder.RegisterType<ICrosser>().As<GreedyCrosser>();
			builder.RegisterType<IMutator>().As<EmptyMutator>();
			builder.RegisterType<EnvironmentData>();
			builder.RegisterType<RouteFinder>();

			return builder.Build();
		}
	}
}