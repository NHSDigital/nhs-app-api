using System.Linq;
using NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter.DataPoints;

namespace NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter
{
    public interface IStatisticsStoreService
    {
        void Add(IDataPoint dataPoint);

        ILookup<long, IDataPoint> GetDataPointsInPeriod(long periodStartUnixTimeSeconds,
            long periodEndUnixTimeSeconds);
    }
}
