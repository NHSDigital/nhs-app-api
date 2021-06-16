using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter
{
    public class PerformanceCounterConfiguration
    {
        public const int DefaultQueueLength = 100000;

        public const int DefaultInitialMetricHealthCheckWindowSizeInSeconds = 300;

        public const int DefaultResponseTimeUnhealthyAverageThresholdInMs = 5000;

        public const int DefaultRequestCountUnhealthyNumberOfRequestsPerSecond = 150;

        public const string ConfigSectionName = "PerformanceCounter";

        public bool IsHealthCheckEnabled { get; set; }

        public bool IsMetricLoggingEnabled { get; set; }

        public int QueueLength { get; set; }

        public int InitialMetricHealthCheckWindowSizeInSeconds { get; set; }

        public int ResponseTimeUnhealthyAverageThresholdInMs { get; set; }

        public int RequestCountUnhealthyNumberOfRequestsPerSecond { get; set; }

        public PerformanceCounterConfiguration()
        {}

        public PerformanceCounterConfiguration(IConfiguration configuration, ILogger<PerformanceCounterConfiguration> logger)
        {
            var configSection =
                configuration.GetSection(ConfigSectionName);

            IsHealthCheckEnabled = configSection.GetBoolOrDefault(nameof(IsHealthCheckEnabled), logger);
            IsMetricLoggingEnabled = configSection.GetBoolOrDefault(nameof(IsMetricLoggingEnabled), logger);
            QueueLength = configSection.GetIntOrFallback(nameof(QueueLength), DefaultQueueLength, logger);
            InitialMetricHealthCheckWindowSizeInSeconds = configSection.GetIntOrFallback(nameof(InitialMetricHealthCheckWindowSizeInSeconds), DefaultInitialMetricHealthCheckWindowSizeInSeconds, logger);
            ResponseTimeUnhealthyAverageThresholdInMs = configSection.GetIntOrFallback(nameof(ResponseTimeUnhealthyAverageThresholdInMs), DefaultResponseTimeUnhealthyAverageThresholdInMs, logger);
            RequestCountUnhealthyNumberOfRequestsPerSecond = configSection.GetIntOrFallback(nameof(RequestCountUnhealthyNumberOfRequestsPerSecond), DefaultRequestCountUnhealthyNumberOfRequestsPerSecond, logger);
        }
    }
}