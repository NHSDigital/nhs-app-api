using System;
using System.Collections.Generic;
using NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter.DataPoints;
using NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter.Metrics;

namespace NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter
{
    public class PerformanceCounterVisitor : IPerformanceCounterVisitor
    {
        private readonly Dictionary<DataPointType, PerformanceCounterMetric> _performanceCounterPerSecondMetricsDictionary =
            new Dictionary<DataPointType, PerformanceCounterMetric>();

        public PerformanceCounterVisitor()
        {
            // Initialize metrics registry with all PerformanceCounter types.
            foreach (DataPointType dataPointType in Enum.GetValues(typeof(DataPointType)))
            {
                _performanceCounterPerSecondMetricsDictionary.Add(dataPointType, new PerformanceCounterMetric());
            }
        }

        public void Visit(RequestCountDataPoint dataPoint) =>
            _performanceCounterPerSecondMetricsDictionary[dataPoint.DataPointType].Result++;

        public void Visit(ResponseTimeDataPoint dataPoint)
        {
            _performanceCounterPerSecondMetricsDictionary[dataPoint.DataPointType].Tally++;
            _performanceCounterPerSecondMetricsDictionary[dataPoint.DataPointType].Result += dataPoint.ResponseTimeInMs;
        }

        public Dictionary<DataPointType, PerformanceCounterMetric> Flush()
        {
            return _performanceCounterPerSecondMetricsDictionary;
        }
    }
}