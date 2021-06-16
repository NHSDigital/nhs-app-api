namespace NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter.Metrics
{
    public class ResponseTimeMetricsHandler : IPerformanceCounterMetricsFactory
    {
        public void CalculateMetrics(PerformanceCounterMetric performanceCounterMetric)
        {
            // Calculate the average ResponseTime.
            if (performanceCounterMetric.Tally > 0)
            {
                var averageResponseTime =
                    performanceCounterMetric.Result
                    /
                    performanceCounterMetric.Tally;

                performanceCounterMetric.Result = averageResponseTime;
            }
        }
    }
}