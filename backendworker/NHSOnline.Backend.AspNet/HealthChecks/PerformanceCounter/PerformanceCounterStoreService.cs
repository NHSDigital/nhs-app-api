using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter.DataPoints;
using NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter.Metrics;

namespace NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter
{
    public class PerformanceCounterStoreService : IPerformanceCounterStoreService
    {
        private readonly FixedSizeDictionary<long, Dictionary<DataPointType, PerformanceCounterMetric>> _performanceCounterQueue;

        public PerformanceCounterStoreService(PerformanceCounterConfiguration options) =>
            _performanceCounterQueue = new FixedSizeDictionary<long, Dictionary<DataPointType, PerformanceCounterMetric>>(options.InitialMetricHealthCheckWindowSizeInSeconds);

        public void Add(long key, Dictionary<DataPointType, PerformanceCounterMetric> value) =>
            _performanceCounterQueue.Add(key, value);

        public IEnumerable<KeyValuePair<long, Dictionary<DataPointType, PerformanceCounterMetric>>> GetPerformanceCountersInPeriod(
            long periodStartUnixTimeSeconds,
            long periodEndUnixTimeSeconds)
        {
            return _performanceCounterQueue
                .Where(x =>
                    x.Key > periodStartUnixTimeSeconds &&
                    x.Key <= periodEndUnixTimeSeconds);
        }
    }
}
