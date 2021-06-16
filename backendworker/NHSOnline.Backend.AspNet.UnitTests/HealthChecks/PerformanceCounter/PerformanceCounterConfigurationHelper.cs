#nullable enable
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter;

namespace NHSOnline.Backend.AspNet.UnitTests.HealthChecks.PerformanceCounter
{
    public static class PerformanceCounterConfigurationHelper
    {
        public static PerformanceCounterConfiguration SetupConfiguration(
            Mock<IConfiguration> configuration,
            Mock<IConfigurationSection> configurationSection,
            Mock<ILogger<PerformanceCounterConfiguration>> configLogger,
            string? isHealthCheckEnabled = "true",
            string? isMetricLoggingEnabled = "true",
            string? queueLength = "100000",
            string? initialMetricHealthCheckWindowSizeInSeconds = "300",
            string? responseTimeUnhealthyAverageThresholdInMs = "5000",
            string? unhealthyNumberOfRequests = "100")
        {
#pragma warning disable 8604
            configurationSection
                .Setup(x => x[nameof(PerformanceCounterConfiguration.IsHealthCheckEnabled)])
                .Returns(isHealthCheckEnabled);

            configurationSection
                .Setup(x => x[nameof(PerformanceCounterConfiguration.IsMetricLoggingEnabled)])
                .Returns(isMetricLoggingEnabled);

            configurationSection
                .Setup(x => x[nameof(PerformanceCounterConfiguration.QueueLength)])
                .Returns(queueLength);

            configurationSection
                .Setup(x => x[nameof(PerformanceCounterConfiguration.InitialMetricHealthCheckWindowSizeInSeconds)])
                .Returns(initialMetricHealthCheckWindowSizeInSeconds);

            configurationSection
                .Setup(x => x[nameof(PerformanceCounterConfiguration.ResponseTimeUnhealthyAverageThresholdInMs)])
                .Returns(responseTimeUnhealthyAverageThresholdInMs);

            configurationSection
                .Setup(x => x[nameof(PerformanceCounterConfiguration.RequestCountUnhealthyNumberOfRequestsPerSecond)])
                .Returns(unhealthyNumberOfRequests);
#pragma warning restore 8604

            configuration.Setup(a => a.GetSection(PerformanceCounterConfiguration.ConfigSectionName))
                .Returns(configurationSection.Object);

            return new PerformanceCounterConfiguration(configuration.Object, configLogger.Object);
        }
    }
}