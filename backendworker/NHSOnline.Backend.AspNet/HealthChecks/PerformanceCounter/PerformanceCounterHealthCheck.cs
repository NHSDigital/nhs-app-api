using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter.DataPoints;
using NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter.Metrics;

namespace NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter
{
    public class PerformanceCounterHealthCheck : IHealthCheck
    {
        private readonly IDateTimeHelperService _dateTimeHelperService;
        private readonly IPerformanceCounterRegistry _performanceCounterRegistry;
        private readonly IPerformanceCounterStoreService _performanceCounterStoreService;
        private readonly ILogger _logger;
        private readonly PerformanceCounterConfiguration _options;

        public PerformanceCounterHealthCheck(
            IDateTimeHelperService dateTimeHelperService,
            IPerformanceCounterRegistry performanceCounterRegistry,
            IPerformanceCounterStoreService performanceCounterStoreService,
            ILogger<PerformanceCounterHealthCheck> logger,
            PerformanceCounterConfiguration options)
        {
            _dateTimeHelperService = dateTimeHelperService;
            _performanceCounterRegistry = performanceCounterRegistry;
            _performanceCounterStoreService = performanceCounterStoreService;
            _logger = logger;
            _options = options;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
            CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                return Task.FromResult(BuildPerformanceCountersAndExecuteHealthCheck());
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "PerformanceCounter health check failed");

                return Task.FromResult(
                    HealthCheckResult.Degraded("PerformanceCounter health check failed")
                );
            }
        }

        private HealthCheckResult BuildPerformanceCountersAndExecuteHealthCheck()
        {
            var currentSamplePeriodStartUnixTimeSeconds = _performanceCounterRegistry.GetPerformanceCounterLastBuiltUnixTimeInSeconds();
            var currentSamplePeriodEndUnixTimeSeconds = _dateTimeHelperService.GetUtcNowTimestampAsUnixTimeSeconds() - 1;

            BuildPerformanceCounters(currentSamplePeriodEndUnixTimeSeconds);
            return ExecuteHealthCheck(currentSamplePeriodStartUnixTimeSeconds, currentSamplePeriodEndUnixTimeSeconds);
        }

        private void BuildPerformanceCounters(long currentSamplePeriodEndUnixTimeSeconds) =>
            _performanceCounterRegistry.BuildPerformanceCounters(currentSamplePeriodEndUnixTimeSeconds);

        private HealthCheckResult ExecuteHealthCheck(long currentSamplePeriodStartUnixTimeSeconds, long currentSamplePeriodEndUnixTimeSeconds)
        {
            if (!_options.IsHealthCheckEnabled)
            {
                return HealthCheckResult.Healthy("PerformanceCounter health check passed as Health Checks are disabled.");
            }

            var healthCheckLogMessageFragments = new List<string>();

            var performanceCountersInPeriod = _performanceCounterStoreService.GetPerformanceCountersInPeriod(
                    currentSamplePeriodStartUnixTimeSeconds,
                    currentSamplePeriodEndUnixTimeSeconds)
                .ToList();

            var overallHealthCheckStatusIsHealthy = true;
            foreach (DataPointType dataPointType in Enum.GetValues(typeof(DataPointType)))
            {
                if (PerformanceCounterHealthCheckStatusFactory.GetHealthCheckStatus.ContainsKey(dataPointType))
                {
                    overallHealthCheckStatusIsHealthy = GetHealthCheckStatusForDataPointType(
                        dataPointType,
                        performanceCountersInPeriod,
                        overallHealthCheckStatusIsHealthy,
                        healthCheckLogMessageFragments);
                }
            }

            var healthCheckMergedResultMessage = string.Join(", ", healthCheckLogMessageFragments);

            if (overallHealthCheckStatusIsHealthy)
            {
                return HealthCheckResult.Healthy(
                    $"PerformanceCounter health check passed. {healthCheckMergedResultMessage}");
            }

            return HealthCheckResult.Unhealthy($"PerformanceCounter health check failed. {healthCheckMergedResultMessage}");
        }

        private bool GetHealthCheckStatusForDataPointType(
            DataPointType dataPointType,
            List<KeyValuePair<long, Dictionary<DataPointType, PerformanceCounterMetric>>> performanceCountersInPeriod,
            bool healthCheckStatusIsHealthy,
            List<string> healthCheckLogMessageFragments)
        {
            var performanceCountersForDataPointTypeInPeriod =
                performanceCountersInPeriod
                    .SelectMany(x => x.Value)
                    .Where(x => x.Key == dataPointType)
                    .Select(x => x.Value)
                    .ToList();

            var handler = PerformanceCounterHealthCheckStatusFactory.GetHealthCheckStatus[dataPointType].Invoke();
            var result = handler.CalculateHealthCheckStatus(_options, performanceCountersForDataPointTypeInPeriod);

            if (!result.healthStatus)
            {
                healthCheckStatusIsHealthy = false;
            }

            healthCheckLogMessageFragments.Add(result.logMessage);

            return healthCheckStatusIsHealthy;
        }
    }
}