using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter.DataPoints;
using NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter.Metrics;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter
{
    public class PerformanceCounterRegistry : IPerformanceCounterRegistry
    {
        private long _performanceCounterLastBuiltUnixTimeInSeconds = 0;

        private readonly ILogger<PerformanceCounterRegistry> _logger;
        private readonly IStatisticsStoreService _statisticsStoreService;
        private readonly IPerformanceCounterStoreService _performanceCounterStoreService;
        private readonly PerformanceCounterConfiguration _options;

        public PerformanceCounterRegistry(
            ILogger<PerformanceCounterRegistry> logger,
            IStatisticsStoreService statisticsStoreService,
            IPerformanceCounterStoreService performanceCounterStoreService,
            PerformanceCounterConfiguration options)
        {
            _logger = logger;
            _statisticsStoreService = statisticsStoreService;
            _performanceCounterStoreService = performanceCounterStoreService;
            _options = options;
        }

        public void BuildPerformanceCounters(long currentSamplePeriodEndUnixTimeSeconds)
        {
            // Get current EpochTimeInSeconds, take away 1 so will be working with a complete second for the end of period.
            var currentSamplePeriodStartUnixTimeSeconds = GetPerformanceCounterLastBuiltUnixTimeInSeconds();

            if (currentSamplePeriodStartUnixTimeSeconds == 0)
            {
                // Set the performance counter window start point on first iteration as we do not wish to start at 0
                currentSamplePeriodStartUnixTimeSeconds =
                    currentSamplePeriodEndUnixTimeSeconds - _options.InitialMetricHealthCheckWindowSizeInSeconds;
            }

            var dataPointsForPeriod =
                _statisticsStoreService.GetDataPointsInPeriod(
                    currentSamplePeriodStartUnixTimeSeconds,
                    currentSamplePeriodEndUnixTimeSeconds);

            for (var i = currentSamplePeriodStartUnixTimeSeconds + 1; i <= currentSamplePeriodEndUnixTimeSeconds; i++)
            {
                BuildPerformanceCounterForSecond(dataPointsForPeriod, i);
            }
        }

        private void BuildPerformanceCounterForSecond(ILookup<long, IDataPoint> dataPointsForPeriod, long currentSecond)
        {
            var performanceCounterBuilderVisitor = new PerformanceCounterVisitor();

            foreach (var dataPoint in dataPointsForPeriod[currentSecond])
            {
                dataPoint.Accept(performanceCounterBuilderVisitor);
            }

            // Get organised metrics for this second.
            var performanceCountersForSecond = performanceCounterBuilderVisitor.Flush();

            // Perform any calculations for each type if required
            foreach (DataPointType dataPointType in Enum.GetValues(typeof(DataPointType)))
            {
                if (PerformanceCounterMetricsFactory.GetMetricsHandler.ContainsKey(dataPointType))
                {
                    var handler = PerformanceCounterMetricsFactory.GetMetricsHandler[dataPointType].Invoke();
                    handler.CalculateMetrics(performanceCountersForSecond[dataPointType]);
                }
            }

            // Write log entry. Concatenates all Performance Counter names with their respective 'Result' value into a single log entry
            WritePerformanceCounterLogMessage(currentSecond, performanceCountersForSecond);

            // Add refined metrics to PerformanceCounter store
            _performanceCounterStoreService.Add(currentSecond, performanceCountersForSecond);

            // Update the value of when the PerformanceCounters were last calculated as the basis for the subsequent run
            SetPerformanceCounterLastBuiltUnixTimeInSeconds(currentSecond);
        }

        public long GetPerformanceCounterLastBuiltUnixTimeInSeconds()
        {
            return _performanceCounterLastBuiltUnixTimeInSeconds;
        }

        private void SetPerformanceCounterLastBuiltUnixTimeInSeconds(long lastBuiltUnixTimeInSeconds) =>
            _performanceCounterLastBuiltUnixTimeInSeconds = lastBuiltUnixTimeInSeconds;

        private void WritePerformanceCounterLogMessage(long currentEpochSecond, Dictionary<DataPointType, PerformanceCounterMetric> performanceCounterPerSecondMetricsDictionary)
        {
            var logMessage = $"PerformanceCounter Metric: epochSeconds={currentEpochSecond}";

            foreach (var performanceCounterPerSecondMetric in performanceCounterPerSecondMetricsDictionary)
            {
                logMessage +=
                    $", {EnumHelper.GetDescriptionOrThrowException(performanceCounterPerSecondMetric.Key)}={performanceCounterPerSecondMetric.Value.Result}";
            }

            _logger.LogInformation(logMessage);
        }
    }
}