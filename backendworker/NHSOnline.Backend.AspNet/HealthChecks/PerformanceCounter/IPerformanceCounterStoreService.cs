using System.Collections.Generic;
using NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter.DataPoints;
using NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter.Metrics;

namespace NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter
{
    public interface IPerformanceCounterStoreService
    {
        void Add(long key, Dictionary<DataPointType, PerformanceCounterMetric> value);

        IEnumerable<KeyValuePair<long, Dictionary<DataPointType, PerformanceCounterMetric>>>
            GetPerformanceCountersInPeriod(
                long periodStartUnixTimeSeconds,
                long periodEndUnixTimeSeconds);
    }
}
