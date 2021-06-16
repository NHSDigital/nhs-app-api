namespace NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter.Metrics
{
    public interface IPerformanceCounterMetricsFactory
    {
        void CalculateMetrics(PerformanceCounterMetric performanceCounterMetric);
    }
}