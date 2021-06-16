using System.Collections.Generic;

namespace NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter.Metrics
{
    public interface IPerformanceCounterHealthCheckStatusFactory
    {
        (bool healthStatus, string logMessage) CalculateHealthCheckStatus(PerformanceCounterConfiguration options, List<PerformanceCounterMetric> performanceCounterMetrics);
    }
}