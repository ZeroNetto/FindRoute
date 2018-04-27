namespace GeneticRoute
{
	public class Program
	{ 
		public static void Main(string[] args)
		{
			var envData = new EnvironmentData();
			var routeFinder = new RouteFinder(envData);
			var startPopulation = routeFinder.GenerateStartPopulation();
			// var result = routeFinder.GeneticAlgorithm(...);
		}
	}
}
