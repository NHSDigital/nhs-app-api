using System.Linq;
using NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter.DataPoints;

namespace NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter
{
    public class StatisticsStoreService : IStatisticsStoreService
    {
        private readonly FixedSizeConcurrentQueue<IDataPoint> _statisticStoreQueue;

        public StatisticsStoreService(PerformanceCounterConfiguration options)
        {
            _statisticStoreQueue = new FixedSizeConcurrentQueue<IDataPoint>(options.QueueLength);
        }

        public void Add(IDataPoint dataPoint)
        {
            _statisticStoreQueue.Enqueue(dataPoint);
        }

        public ILookup<long, IDataPoint> GetDataPointsInPeriod(long periodStartUnixTimeSeconds, long periodEndUnixTimeSeconds)
        {
            //  Query range: start at greater than last query time, up to and including specified end period.
            //  This gives us per-second Data Points.
            return _statisticStoreQueue
                .Where(x =>
                    x.UtcTimestampAsUnixTimeSeconds > periodStartUnixTimeSeconds &&
                    x.UtcTimestampAsUnixTimeSeconds <= periodEndUnixTimeSeconds)
                .ToLookup(x => x.UtcTimestampAsUnixTimeSeconds);
        }
    }
}
