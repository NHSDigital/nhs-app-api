using System.Collections.Generic;
using System.Linq;

namespace NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter.Metrics
{
    public class ResponseTimeHealthCheckStatusHandler : IPerformanceCounterHealthCheckStatusFactory
    {
        public (bool healthStatus, string logMessage) CalculateHealthCheckStatus(PerformanceCounterConfiguration options, List<PerformanceCounterMetric> performanceCounterMetrics)
        {
            // Check if any average response time is above unhealthy threshold
            long result = 0;

            var averageResponseTimesInPeriod =
                performanceCounterMetrics.Sum(x => x.Result);

            var totalCounterTally = performanceCounterMetrics.Count;

            if (totalCounterTally > 0)
            {
                result = averageResponseTimesInPeriod / totalCounterTally;
            }

            if (result > options.ResponseTimeUnhealthyAverageThresholdInMs)
            {
                return (false,
                    $"ResponseTimeHealthCheck failed - Response time average of: {result} is above threshold: {options.ResponseTimeUnhealthyAverageThresholdInMs}");
            }

            return (true,
                $"ResponseTimeHealthCheck failed - Response time average of: {result} is below threshold: {options.ResponseTimeUnhealthyAverageThresholdInMs}");
        }
    }
}