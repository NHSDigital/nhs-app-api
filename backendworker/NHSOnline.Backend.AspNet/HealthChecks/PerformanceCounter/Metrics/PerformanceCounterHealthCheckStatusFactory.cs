using System;
using System.Collections.Generic;
using NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter.DataPoints;

namespace NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter.Metrics
{
    public static class PerformanceCounterHealthCheckStatusFactory
    {
        public static readonly Dictionary<DataPointType, Func<IPerformanceCounterHealthCheckStatusFactory>> GetHealthCheckStatus =
            new Dictionary<DataPointType, Func<IPerformanceCounterHealthCheckStatusFactory>>
            {
                { DataPointType.RequestCount, () => new RequestCountHealthCheckStatusHandler() },
                { DataPointType.ResponseTime, () => new ResponseTimeHealthCheckStatusHandler() },
            };
    }
}