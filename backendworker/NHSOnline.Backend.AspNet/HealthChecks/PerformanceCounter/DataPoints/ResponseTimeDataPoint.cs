namespace NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter.DataPoints
{
    public class ResponseTimeDataPoint : DataPoint
    {
        public override DataPointType DataPointType => DataPointType.ResponseTime;

        public long ResponseTimeInMs { get; set; }

        public override void Accept(IPerformanceCounterVisitor visitor) => visitor.Visit(this);
    }
}