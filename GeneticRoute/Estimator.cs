using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticRoute
{
    public class Estimator : EstimatorBase
    {
        private const int MaxWorkTimeSeconds = 8 * 60 * 60;

        public override List<GeneticData> GetOrderedData(List<GeneticData> data, EnvironmentData envData)
        {
            var estimatesWithClientsNum = new List<(GeneticData data, double value, int visitedCount)>();
            foreach (var geneticData in data)
            {
	            var estimates = GetEstimateWithClientsNum(geneticData, envData);
	            estimatesWithClientsNum.Add((geneticData, estimates.value, estimates.visitedCount));
            }

            var temp = estimatesWithClientsNum
                .OrderBy(estimate => estimate.value)
                .ThenByDescending(estimate => estimate.visitedCount)
                .Select(estimate => estimate.data)
                .ToList();
            return temp;
        }

        private (double value, int visitedCount) GetEstimateWithClientsNum(
	        GeneticData geneticData, EnvironmentData envData)
        {
            // Меньше - лучше
            var managersWorkTimes = GetWorkTimes(geneticData, envData);
            var estimate = 0.0;
            var totalWorkTime = managersWorkTimes
                .Sum(manager => manager.Value.Item1);
            var clientsWereVisited = 0;
            var averageWorkTimeSeconds = totalWorkTime / managersWorkTimes.Count;
            // Считаем по отклонению от среднего времени и рабочего дня.
            // Можно поэксперементировать с левой частью, т.к. она отвечает за равноправие.
            foreach (var workTimeWithClientsNum in managersWorkTimes.Values)
            {
                var workTimeSeconds = workTimeWithClientsNum.Item1;
                estimate += workTimeSeconds + Math.Abs(averageWorkTimeSeconds - workTimeSeconds) *
                            workTimeSeconds / MaxWorkTimeSeconds;
                clientsWereVisited += workTimeWithClientsNum.Item2;
            }
            return (estimate, clientsWereVisited);
        }

        private Dictionary<Manager, Tuple<double, int>> GetWorkTimes(
            GeneticData geneticData,
            EnvironmentData envData
            )
        {
            var managersWorksTimeWithClientsNum = new Dictionary<Manager, Tuple<double, int>>();
            foreach (var manager in geneticData.Data.Keys)
            {
                var startAddress = manager.StartAddress;
                var endAddress = manager.StartAddress;
                var isFirst = true;
                var currentManagerTime = manager.StartOfWork;
                var managerWorkTimeSeconds = 0.0;
                var clientsWereVisited = 0;
                foreach (var address in geneticData.Data[manager])
                {
                    if (isFirst)
                    {
                        isFirst = false;
                        continue;
                    }
                    startAddress = endAddress;
                    endAddress = address;
                    var currentClient = envData.AddressClient[endAddress];
                    if (TryAddWorkTime(
                            startAddress, endAddress, currentManagerTime,
                            currentClient, manager, ref managerWorkTimeSeconds, envData))
                        clientsWereVisited++;
                    currentManagerTime = currentClient.MeetingEndTime;
                }
                // Умножаем время работы на кол-во непосещенных клиентов
                if (!managersWorksTimeWithClientsNum.ContainsKey(manager))
                    managersWorksTimeWithClientsNum.Add(manager,
                        Tuple.Create(
                            managerWorkTimeSeconds * (geneticData.Data[manager].Count - clientsWereVisited),
                            clientsWereVisited));
                else
                    managersWorksTimeWithClientsNum[manager] = Tuple.Create(
                        managerWorkTimeSeconds * (geneticData.Data[manager].Count - clientsWereVisited),
                        clientsWereVisited);
            }
            return managersWorksTimeWithClientsNum;
        }

        private bool TryAddWorkTime(
            Address startAddress,
            Address endAddress,
            DateTime currentManagerTime,
            Client currentClient,
            Manager manager,
            ref double managerWorkTimeSeconds,
            EnvironmentData envData
            )
        {
            var wasVisited = false;
            // В этом месте это просто время пути до клиента
            var serveClientTimeSeconds = envData.TimeKeeper
                .GetTimeInterval(startAddress, endAddress, currentManagerTime)
                .TotalSeconds;
            var hasFreeTimeSeconds = currentClient.MeetingStartTime.Subtract(currentManagerTime).TotalSeconds;
            // Если успевает на встречу
            if (hasFreeTimeSeconds >= 0 && serveClientTimeSeconds <= hasFreeTimeSeconds)
            {
                serveClientTimeSeconds += envData.AddressClient[endAddress].MeetingDuration.TotalSeconds;
                wasVisited = true;
            }
            managerWorkTimeSeconds += serveClientTimeSeconds;
            return wasVisited;
        }
    }
}
