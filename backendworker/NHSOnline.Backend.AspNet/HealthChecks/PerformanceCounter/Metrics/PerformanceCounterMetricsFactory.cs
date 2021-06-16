using System;
using System.Collections.Generic;
using NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter.DataPoints;

namespace NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter.Metrics
{
    public static class PerformanceCounterMetricsFactory
    {
        public static readonly Dictionary<DataPointType, Func<IPerformanceCounterMetricsFactory>> GetMetricsHandler =
            new Dictionary<DataPointType, Func<IPerformanceCounterMetricsFactory>>
            {
                { DataPointType.ResponseTime, () => new ResponseTimeMetricsHandler() },
            };
    }
}