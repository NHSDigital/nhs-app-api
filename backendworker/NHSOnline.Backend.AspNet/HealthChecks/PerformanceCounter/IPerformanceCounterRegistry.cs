namespace NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter
{
    public interface IPerformanceCounterRegistry
    {
        void BuildPerformanceCounters(long currentSamplePeriodEndUnixTimeSeconds);

        long GetPerformanceCounterLastBuiltUnixTimeInSeconds();
    }
}