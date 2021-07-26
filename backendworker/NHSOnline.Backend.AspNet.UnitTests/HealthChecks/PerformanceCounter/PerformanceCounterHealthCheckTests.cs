#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using NHSOnline.Backend.AspNet.HealthChecks;
using NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter;
using NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter.DataPoints;
using NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter.Metrics;

namespace NHSOnline.Backend.AspNet.UnitTests.HealthChecks.PerformanceCounter
{
    [TestClass]
    public sealed class PerformanceCounterHealthCheckTests
    {
        private readonly PerformanceCounterConfiguration _options;
        private Mock<ILogger<PerformanceCounterHealthCheck>> _logger = new Mock<ILogger<PerformanceCounterHealthCheck>>();
        private readonly Mock<IDateTimeHelperService> _mockDateTimeHelperService;
        private readonly Mock<IPerformanceCounterRegistry> _mockPerformanceCounterRegistry;
        private readonly PerformanceCounterStoreService _performanceCounterStoreService;
        private readonly long _constantDateTimeUnixSeconds = DateTimeOffset.Parse("2021-01-01", CultureInfo.CurrentCulture).ToUnixTimeSeconds();

        public PerformanceCounterHealthCheckTests()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            Mock<ILogger<PerformanceCounterConfiguration>> configLogger = fixture.Freeze<Mock<ILogger<PerformanceCounterConfiguration>>>();
            Mock<IConfiguration> configuration = new Mock<IConfiguration>();
            var configurationSection = new Mock<IConfigurationSection>();
            _options = PerformanceCounterConfigurationHelper.SetupConfiguration(configuration, configurationSection, configLogger);


            _mockDateTimeHelperService = new Mock<IDateTimeHelperService>();
            _mockDateTimeHelperService.Setup(x => x.GetUtcNowTimestampAsUnixTimeSeconds())
                .Returns(_constantDateTimeUnixSeconds);

            _mockPerformanceCounterRegistry = new Mock<IPerformanceCounterRegistry>();
            _performanceCounterStoreService = new PerformanceCounterStoreService(_options);
            BuildPerformanceCounterStore(_performanceCounterStoreService, _constantDateTimeUnixSeconds - _options.InitialMetricHealthCheckWindowSizeInSeconds);
        }

        [TestInitialize]
        public void Setup()
        {
            // Want a new logger for each test to verify.
            _logger = new Mock<ILogger<PerformanceCounterHealthCheck>>();
        }

        [TestMethod]
        public async Task CheckHealthAsync_ExceptionThrown_ErrorLoggedAndDegradedResultReturned()
        {
            // Arrange - set this service up so it errors
            var localMockDateTimeHelperService = new Mock<IDateTimeHelperService>();
            localMockDateTimeHelperService
                .Setup(x => x.GetUtcNowTimestampAsUnixTimeSeconds())
                .Throws<Exception>();

            var systemUnderTest = new PerformanceCounterHealthCheck(
                localMockDateTimeHelperService.Object,
                _mockPerformanceCounterRegistry.Object,
                _performanceCounterStoreService,
                _logger.Object,
                _options);

            // Act
            var result = await systemUnderTest.CheckHealthAsync(new HealthCheckContext());

            // Assert
            _logger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v,t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v,t) => true)),
                Times.Once());

            Assert.AreEqual(result.Status, HealthStatus.Degraded);
        }

        [TestMethod]
        public async Task CheckHealthAsync_HealthCheckIsEnabledFalse_HealthCheckPasses()
        {
            // Arrange
            var localOptions = JsonConvert.DeserializeObject<PerformanceCounterConfiguration>(JsonConvert.SerializeObject(_options));
            localOptions!.IsHealthCheckEnabled = false;

            var systemUnderTest = new PerformanceCounterHealthCheck(
                _mockDateTimeHelperService.Object,
                _mockPerformanceCounterRegistry.Object,
                _performanceCounterStoreService,
                _logger.Object,
                localOptions);

            // Act
            var result = await systemUnderTest.CheckHealthAsync(new HealthCheckContext());

            // Assert
            Assert.AreEqual(result.Status, HealthStatus.Healthy);
        }

        [TestMethod]
        public async Task CheckHealthAsync_AllPerformanceCountersReturnSuccess_HealthCheckPasses()
        {
            // Arrange
            var systemUnderTest = new PerformanceCounterHealthCheck(
                _mockDateTimeHelperService.Object,
                _mockPerformanceCounterRegistry.Object,
                _performanceCounterStoreService,
                _logger.Object,
                _options);

            // Act
            var result = await systemUnderTest.CheckHealthAsync(new HealthCheckContext());

            // Assert
            Assert.AreEqual(result.Status, HealthStatus.Healthy);
        }

        [TestMethod]
        public async Task CheckHealthAsync_RequestCountPerformanceCounterReturnFailure_HealthCheckFails()
        {
            // Arrange
            var systemUnderTest = new PerformanceCounterHealthCheck(
                _mockDateTimeHelperService.Object,
                _mockPerformanceCounterRegistry.Object,
                _performanceCounterStoreService,
                _logger.Object,
                _options);

            // Add failing performance counter
            _performanceCounterStoreService.Add(100,
                new Dictionary<DataPointType, PerformanceCounterMetric>
                {
                    {
                        DataPointType.RequestCount,
                        new PerformanceCounterMetric
                        {
                            Result = _options.RequestCountUnhealthyNumberOfRequestsPerSecond + 1
                        }
                    },
                    {
                        DataPointType.ResponseTime,
                        new PerformanceCounterMetric
                        {
                            Tally = 1,
                            Result = 1
                        }
                    },
                });

            // Act
            var result = await systemUnderTest.CheckHealthAsync(new HealthCheckContext());

            // Assert
            Assert.AreEqual(HealthStatus.Unhealthy, result.Status);
            StringAssert.Contains(result.Description, "RequestCountHealthCheck", StringComparison.CurrentCulture);
        }

        [TestMethod]
        public async Task CheckHealthAsync_ResponseTimePerformanceCounterReturnFailure_HealthCheckFails()
        {
            // Arrange
            // Add new store with failing performance counter
            var localPerformanceCounterStoreService = new PerformanceCounterStoreService(_options);

            localPerformanceCounterStoreService.Add(_constantDateTimeUnixSeconds - _options.InitialMetricHealthCheckWindowSizeInSeconds,
                new Dictionary<DataPointType, PerformanceCounterMetric>
                {
                    {
                        DataPointType.RequestCount,
                        new PerformanceCounterMetric
                        {
                            Result = 1
                        }
                    },
                    {
                        DataPointType.ResponseTime,
                        new PerformanceCounterMetric
                        {
                            Tally = 1,
                            Result = _options.ResponseTimeUnhealthyAverageThresholdInMs + 1
                        }
                    },
                });

            var systemUnderTest = new PerformanceCounterHealthCheck(
                _mockDateTimeHelperService.Object,
                _mockPerformanceCounterRegistry.Object,
                localPerformanceCounterStoreService,
                _logger.Object,
                _options);

            // Act
            var result = await systemUnderTest.CheckHealthAsync(new HealthCheckContext());

            // Assert
            Assert.AreEqual(HealthStatus.Unhealthy, result.Status);
            StringAssert.Contains(result.Description, "ResponseTimeHealthCheck", StringComparison.CurrentCulture);
        }

        [TestMethod]
        public async Task CheckHealthAsync_BothRequestCountAndResponseTimePerformanceCounterReturnFailure_HealthCheckFailsAndLogsBoth()
        {
            // Arrange
            // Add new store with x2 failing performance counters
            var localPerformanceCounterStoreService = new PerformanceCounterStoreService(_options);

            localPerformanceCounterStoreService.Add(_constantDateTimeUnixSeconds - _options.InitialMetricHealthCheckWindowSizeInSeconds,
                new Dictionary<DataPointType, PerformanceCounterMetric>
                {
                    {
                        DataPointType.RequestCount,
                        new PerformanceCounterMetric
                        {
                            Result = _options.RequestCountUnhealthyNumberOfRequestsPerSecond + 1
                        }
                    },
                    {
                        DataPointType.ResponseTime,
                        new PerformanceCounterMetric
                        {
                            Tally = 1,
                            Result = _options.ResponseTimeUnhealthyAverageThresholdInMs + 1
                        }
                    },
                });

            var systemUnderTest = new PerformanceCounterHealthCheck(
                _mockDateTimeHelperService.Object,
                _mockPerformanceCounterRegistry.Object,
                localPerformanceCounterStoreService,
                _logger.Object,
                _options);

            // Act
            var result = await systemUnderTest.CheckHealthAsync(new HealthCheckContext());

            // Assert
            Assert.AreEqual(HealthStatus.Unhealthy, result.Status);
            Assert.AreEqual(result.Description.Split("failed").Length, 4);
        }

        [TestMethod]
        public async Task CheckHealthAsync_FailingPerformanceCounterIsOutsideTimeWindow_HealthCheckPasses()
        {
            // Arrange
            var performanceCounterLastBuiltUnixTimeInSeconds = 1;

            _mockPerformanceCounterRegistry
                .Setup(x => x.GetPerformanceCounterLastBuiltUnixTimeInSeconds())
                .Returns(performanceCounterLastBuiltUnixTimeInSeconds);

            // Add new store with failing performance counter which is not in time window
            var localPerformanceCounterStoreService = new PerformanceCounterStoreService(_options);

            localPerformanceCounterStoreService.Add(performanceCounterLastBuiltUnixTimeInSeconds,
                new Dictionary<DataPointType, PerformanceCounterMetric>
                {
                    {
                        DataPointType.RequestCount,
                        new PerformanceCounterMetric
                        {
                            Result = 1
                        }
                    },
                    {
                        DataPointType.ResponseTime,
                        new PerformanceCounterMetric
                        {
                            Tally = 1,
                            Result = _options.ResponseTimeUnhealthyAverageThresholdInMs + 1
                        }
                    },
                });

            var systemUnderTest = new PerformanceCounterHealthCheck(
                _mockDateTimeHelperService.Object,
                _mockPerformanceCounterRegistry.Object,
                localPerformanceCounterStoreService,
                _logger.Object,
                _options);

            // Act
            var result = await systemUnderTest.CheckHealthAsync(new HealthCheckContext());

            // Assert
            Assert.AreEqual(HealthStatus.Healthy, result.Status);
            StringAssert.Contains(result.Description, "ResponseTimeHealthCheck", StringComparison.CurrentCulture);
        }

        /// <summary>
        /// Build a default set of performance counters for subsequent tests. Load in parallel to test out concurrency.
        /// </summary>
        private static void BuildPerformanceCounterStore(PerformanceCounterStoreService performanceCounterStoreService, long unixTimeInSeconds)
        {
            var defaultNumberOfDataPointsToLoad = 100;
            Parallel.For(0, defaultNumberOfDataPointsToLoad, _ =>
            {
                performanceCounterStoreService.Add(unixTimeInSeconds,
                    new Dictionary<DataPointType, PerformanceCounterMetric>
                    {
                        {
                            DataPointType.RequestCount,
                            new PerformanceCounterMetric
                            {
                                Result = 1
                            }
                        },
                        {
                            DataPointType.ResponseTime,
                            new PerformanceCounterMetric
                            {
                                Tally = 1,
                                Result = 1
                            }
                        },
                    });

                unixTimeInSeconds++;
            });
        }
    }
}