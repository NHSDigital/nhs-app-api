namespace NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter.DataPoints
{
    public class RequestCountDataPoint : DataPoint
    {
        public override DataPointType DataPointType => DataPointType.RequestCount;

        public override void Accept(IPerformanceCounterVisitor visitor) => visitor.Visit(this);
    }
}