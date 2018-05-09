using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticRoute
{
    public class Estimator : EstimatorBase
    {
        private const int maxWorkTimeSeconds = 8 * 60 * 60;

        public override List<GeneticData> GetOrderedData(List<GeneticData> data, EnvironmentData envData)
        {
            return data
                .OrderBy(geneticData => GetEstimate(geneticData, envData))
                .ToList();
        }

        private double GetEstimate(GeneticData geneticData, EnvironmentData envData)
        {
            // Меньше - лучше
            var managersWorkTimes = GetWorkTimes(geneticData, envData);
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
            GeneticData geneticData,
            EnvironmentData envData
            )
        {
            var managersWorkTimes = new Dictionary<Manager, double>();
            foreach (var manager in geneticData.Data.Keys)
            {
                var startAddress = manager.StartAddress;
                var endAddress = manager.StartAddress;
                var isFirst = true;
                var workTimeSeconds = 0.0;
                var currentManagerTime = manager.StartOfWork;
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
                            startAddress, endAddress, currentManagerTime, out workTimeSeconds,
                            currentClient, manager, managersWorkTimes, envData))
                        clientsWereVisited++;
                    currentManagerTime = currentClient.MeetingEndTime;
                }
                managersWorkTimes[manager] *= (geneticData.Data[manager].Count - clientsWereVisited); // Т.е. умножаем время работы на кол-во непосещенных клиентов
            }
            return managersWorkTimes;
        }

        private bool TryAddWorkTime(
            Address startAddress,
            Address endAddress,
            DateTime currentManagerTime,
            out double workTimeSeconds,
            Client currentClient,
            Manager manager,
            Dictionary<Manager, double> managersWorkTimes,
            EnvironmentData envData
            )
        {
            var wasVisited = false;
            workTimeSeconds = envData.TimeKeeper // В этом месте это просто время пути до клиента
                .GetTimeBetweenAddressesInSomeTime(startAddress, endAddress, currentManagerTime)
                .TotalSeconds;
            var clearPathTime = currentClient.MeetingStartTime.Subtract(currentManagerTime).TotalSeconds;
            if (clearPathTime >= 0 && clearPathTime <= workTimeSeconds) // Если успевает на встречу
            {
                workTimeSeconds += envData.AddressClient[endAddress].MeetingDuration.TotalSeconds;
                wasVisited = true;
            }
            if (!managersWorkTimes.ContainsKey(manager))
                managersWorkTimes.Add(manager, workTimeSeconds);
            else
                managersWorkTimes[manager] = workTimeSeconds;
            return wasVisited;
        }
    }
}
