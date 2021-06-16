using System.Collections.Generic;
using NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter.DataPoints;
using NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter.Metrics;

namespace NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter
{
    public interface IPerformanceCounterVisitor
    {
        void Visit(RequestCountDataPoint dataPoint);

        void Visit(ResponseTimeDataPoint dataPoint);

        Dictionary<DataPointType, PerformanceCounterMetric> Flush();
    }
}