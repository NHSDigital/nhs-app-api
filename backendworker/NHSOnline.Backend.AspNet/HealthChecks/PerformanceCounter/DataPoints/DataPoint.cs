namespace NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter.DataPoints
{
    public abstract class DataPoint : IDataPoint
    {
        public long UtcTimestampAsUnixTimeSeconds { get; set; }

        public abstract DataPointType DataPointType { get; }

        public abstract void Accept(IPerformanceCounterVisitor visitor);
    }
}