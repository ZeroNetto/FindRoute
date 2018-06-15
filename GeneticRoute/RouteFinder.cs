using System.Collections.Generic;
using System.Linq;

namespace GeneticRoute
{
    public class RouteFinder
    {
	    private const int SelectedCount = 4;

		private readonly EnvironmentData envData;
	    private readonly EstimatorBase estimator;
	    private readonly IMutator mutator;
	    private readonly ICrosser crosser;
	    private readonly IEndCondition endCondition;

        public RouteFinder(
			EnvironmentData envData, 
			EstimatorBase estimator, 
			IMutator mutator, 
			ICrosser crosser, 
			IEndCondition endCondition
	    )
        {
	        this.envData = envData;
	        this.estimator = estimator;
	        this.mutator = mutator;
	        this.crosser = crosser;
	        this.endCondition = endCondition;
        }

        public IEnumerable<GeneticData> GenerateStartPopulation()
        {
            return Enumerable.Range(0, SelectedCount).Select(_ => GeneratePartition());
        }

        private GeneticData GeneratePartition()
        {
            var notVisited = new HashSet<Address>(envData.Clients.Select(client => client.Address));
            var managersWays = new Dictionary<Manager, List<Address>>();
            var managerCurrAdd = new Dictionary<Manager, Address>();
            foreach (var manager in envData.Managers)
            {
                managerCurrAdd[manager] = manager.CurrentAddress;
                managersWays[manager] = new List<Address> { manager.StartAddress };
            }
            while (notVisited.Any())
            {
                foreach (var manager in envData.Managers)
                {
	                if (!notVisited.Any())
		                break;

                    var currTime = !envData.AddressClient.ContainsKey(managerCurrAdd[manager]) ?
                        manager.StartOfWork : envData.AddressClient[managerCurrAdd[manager]].MeetingEndTime;
                    var nextAddress = envData
                        .TimeKeeper
                        .GetAddressesInRightRangeInSomeTime(managerCurrAdd[manager], currTime)
                        .GetMostPrioritiestValueExcept(notVisited).Item1;
                    notVisited.Remove(nextAddress);
                    managerCurrAdd[manager] = nextAddress;
                    managersWays[manager].Add(nextAddress);
                }
            }
            return new GeneticData(managersWays);
        }

        public GeneticData GeneticAlgorithm(List<GeneticData> startPopulation)
        {
            GeneticData best = null;
            var currentCombinations = startPopulation;

            while (!endCondition.IsEnd(currentCombinations, envData))
            {
                var selected = estimator.SelectBests(currentCombinations, SelectedCount, envData);
				System.Console.WriteLine();
                var crossed = crosser.Cross(selected, envData);
                currentCombinations = mutator.Mutate(crossed, envData);
                best = estimator.SelectBests(best == null ? currentCombinations : currentCombinations.Concat(new[] { best }).ToList(), 1, envData).First();
            }

            return best;
        }
    }
}