using System.Collections.Generic;
using System.Linq;

namespace NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter.Metrics
{
    public class RequestCountHealthCheckStatusHandler : IPerformanceCounterHealthCheckStatusFactory
    {
        public (bool healthStatus, string logMessage) CalculateHealthCheckStatus(PerformanceCounterConfiguration options, List<PerformanceCounterMetric> performanceCounterMetrics)
        {
            // Check if request count rises above unhealthy threshold
            var maxRequestsPerSecondInPeriod = performanceCounterMetrics
                .Select(x => x.Result)
                .DefaultIfEmpty()
                .Max();

            if (maxRequestsPerSecondInPeriod > options.RequestCountUnhealthyNumberOfRequestsPerSecond)
            {
                return (false,
                    $"RequestCountHealthCheck failed - Request count maximum of: {maxRequestsPerSecondInPeriod} is above threshold: {options.RequestCountUnhealthyNumberOfRequestsPerSecond}");
            }

            return (true,
                $"RequestCountHealthCheck passed - Request count maximum of: {maxRequestsPerSecondInPeriod} is below threshold: {options.RequestCountUnhealthyNumberOfRequestsPerSecond}");
        }
    }
}