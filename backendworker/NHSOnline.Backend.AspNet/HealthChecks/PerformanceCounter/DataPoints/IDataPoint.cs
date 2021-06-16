namespace NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter.DataPoints
{
    public interface IDataPoint
    {
        long UtcTimestampAsUnixTimeSeconds { get; set; }

        DataPointType DataPointType { get; }

        void Accept(IPerformanceCounterVisitor visitor);
    }
}