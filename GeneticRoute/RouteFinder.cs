using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticRoute
{
    public class RouteFinder
    {
        private readonly EnvironmentData envData;
        private const int SelectedCount = 4;

        public RouteFinder(EnvironmentData envData)
        {
            this.envData = envData;
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
                    var currTime = !envData.AddressClient.ContainsKey(managerCurrAdd[manager]) ?
                        manager.StartOfWork : envData.AddressClient[managerCurrAdd[manager]].MeetingEndTime;
                    var nextAddress = envData
                        .TimeKeeper
                        .GetAddressesInRightRangeInSomeTime(managerCurrAdd[manager], currTime)
                        .OneOfPrioritiestValueNotVisites(notVisited).Item1;
                    notVisited.Remove(nextAddress);
                    managerCurrAdd[manager] = nextAddress;
                    managersWays[manager].Add(nextAddress);
                }
            }
            return new GeneticData(managersWays);
        }

        public GeneticData GeneticAlgorithm(
            EstimatorBase estimator,
            IMutator mutator,
            ICrosser crosser,
            IEndCondition endCondition,
            List<GeneticData> startPopulation
        )
        {
            GeneticData best = null;
            var currentCombinations = startPopulation;

            while (!endCondition.IsEnd(currentCombinations, envData))
            {
                var selected = estimator.SelectBests(currentCombinations, SelectedCount, envData);
                var crossed = crosser.Cross(selected, envData);
                currentCombinations = mutator.Mutate(crossed, envData);
                best = estimator.SelectBests(currentCombinations.Concat(new[] { best }).ToList(), 1, envData).First();
            }

            return best;
        }

        public double GetEstimate(GeneticData estimatedPartion, TimeDictionary timeDictionary)
        {
            // Меньше - лучше
            const int maxWorkTimeSeconds = 8 * 60 * 60;
            var managersWorkTimes = GetWorkTimes(estimatedPartion, timeDictionary);
            var estimate = 0.0;
            var totalWorkTime = managersWorkTimes
                .Sum(manager => manager.Value);
            var averageWorkTimeSeconds = totalWorkTime / managersWorkTimes.Count;
            // Счтитаем по отклонению от среднего времени и рабочего дня
            // Можно поэксперементировать с левой частью, т.к. она отвечает за равноправие
            foreach (var workTimeSeconds in managersWorkTimes.Values)
                estimate += Math.Abs(averageWorkTimeSeconds - workTimeSeconds) * workTimeSeconds / maxWorkTimeSeconds;
            return estimate;
        }

        private Dictionary<Manager, double> GetWorkTimes(
            GeneticData estimatedPartion,
            TimeDictionary timeDictionary
            )
        {
            var managersWorkTimes = new Dictionary<Manager, double>();
            foreach (var manager in this.envData.Managers)
            {
                var startAddress = manager.StartAddress;
                var endAddress = manager.StartAddress;
                var isFirst = true;
                var workTimeSeconds = 0.0;
                var currentManagerTime = manager.StartOfWork;
                var clientsWereVisited = 0;
                foreach (var address in estimatedPartion.Data[manager])
                {
                    if (isFirst)
                    {
                        isFirst = false;
                        continue;
                    }
                    var currentClient = envData.AddressClient[endAddress];
                    startAddress = endAddress;
                    endAddress = address;
                    workTimeSeconds = timeDictionary // В этом месте это просто время пути до клиента
                        .GetTimeBetweenAddressesInSomeTime(startAddress, endAddress, currentManagerTime)
                        .TotalSeconds;
                    var clearPathTime = currentClient.MeetingStartTime.Subtract(currentManagerTime).TotalSeconds;
                    if (clearPathTime >= 0 && clearPathTime <= workTimeSeconds) // Если успевает на встречу
                    {
                        workTimeSeconds += envData.AddressClient[endAddress].MeetingDuration.TotalSeconds;
                        clientsWereVisited++;
                    }

                    if (!managersWorkTimes.ContainsKey(manager))
                        managersWorkTimes.Add(manager, workTimeSeconds);
                    else
                        managersWorkTimes[manager] = workTimeSeconds;
                    currentManagerTime = currentClient.MeetingEndTime;
                }
                managersWorkTimes[manager] *= (estimatedPartion.Data[manager].Count + 1 - clientsWereVisited); // Т.е. умножаем время работы на кол-во непосещенных клиентов
            }
            return managersWorkTimes;
        }
    }
}