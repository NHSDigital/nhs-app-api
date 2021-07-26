#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter;
using NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter.DataPoints;
using NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter.Metrics;

namespace NHSOnline.Backend.AspNet.UnitTests.HealthChecks.PerformanceCounter
{
    [TestClass]
    public sealed class PerformanceCounterRegistryTests
    {
        private readonly PerformanceCounterConfiguration _options;
        private readonly StatisticsStoreService _statisticsStoreService;
        private readonly PerformanceCounterStoreService _performanceCounterStoreService;

        private Mock<ILogger<PerformanceCounterRegistry>> _logger = new Mock<ILogger<PerformanceCounterRegistry>>();
        private readonly long _constantDateTimeUnixSeconds = DateTimeOffset.Parse("2021-01-01", CultureInfo.CurrentCulture).ToUnixTimeSeconds();

        public PerformanceCounterRegistryTests()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            Mock<ILogger<PerformanceCounterConfiguration>> configLogger = fixture.Freeze<Mock<ILogger<PerformanceCounterConfiguration>>>();
            Mock<IConfiguration> configuration = new Mock<IConfiguration>();
            var configurationSection = new Mock<IConfigurationSection>();
            _options = PerformanceCounterConfigurationHelper.SetupConfiguration(configuration, configurationSection, configLogger);

            // Setup default stores
            _statisticsStoreService = new StatisticsStoreService(_options);
            BuildStatisticsStore(_statisticsStoreService,_constantDateTimeUnixSeconds - _options.InitialMetricHealthCheckWindowSizeInSeconds - 1);

            _performanceCounterStoreService = new PerformanceCounterStoreService(_options);
        }

        [TestInitialize]
        public void Setup()
        {
            // Want a new logger for each test to verify.
            _logger = new Mock<ILogger<PerformanceCounterRegistry>>();
        }

        [TestMethod]
        public void BuildPerformanceCounters_CurrentSamplePeriodStartTimeIsZero_InitialMetricHealthCheckWindowSizeUsedInstead()
        {
            // Arrange
            var systemUnderTest = new PerformanceCounterRegistry(_logger.Object, _statisticsStoreService, _performanceCounterStoreService, _options);

            // Act
            systemUnderTest.BuildPerformanceCounters(_constantDateTimeUnixSeconds);

            // Assert - check logger was called the number of times specified by the default configuration setting for window size.
            _logger.Verify(
                 x => x.Log(
                     LogLevel.Information,
                     It.IsAny<EventId>(),
                     It.Is<It.IsAnyType>((v,t) => true),
                     It.IsAny<Exception>(),
                     It.Is<Func<It.IsAnyType, Exception, string>>((v,t) => true)),
                 Times.Exactly(_options.InitialMetricHealthCheckWindowSizeInSeconds));
        }

        [TestMethod]
        public void BuildPerformanceCounters_NoSamplesInStore_LogWrittenCorrectNumberOfTimes()
        {
            // Arrange - create empty store
            var localStatisticsStoreService = new StatisticsStoreService(_options);
            var systemUnderTest = new PerformanceCounterRegistry(_logger.Object, localStatisticsStoreService, _performanceCounterStoreService, _options);

            // Act
            systemUnderTest.BuildPerformanceCounters(_constantDateTimeUnixSeconds);

            // Assert - check logger was called the number of times specified by the default configuration setting for window size.
            _logger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v,t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v,t) => true)),
                Times.Exactly(_options.InitialMetricHealthCheckWindowSizeInSeconds));
        }

        [TestMethod]
        public void BuildPerformanceCounters_PerformanceCountersBuilt_CorrectNumberOfCountersWrittenToStore()
        {
            // Arrange
            var mockPerformanceCounterStoreService = new Mock<IPerformanceCounterStoreService>();

            var systemUnderTest = new PerformanceCounterRegistry(_logger.Object, _statisticsStoreService, mockPerformanceCounterStoreService.Object, _options);

            // Act
            systemUnderTest.BuildPerformanceCounters(_constantDateTimeUnixSeconds);

            // Assert - check store called correct number of times
            mockPerformanceCounterStoreService.Verify(
                x => x.Add(It.IsAny<long>(), It.IsAny<Dictionary<DataPointType,PerformanceCounterMetric>>()),
                Times.Exactly(_options.InitialMetricHealthCheckWindowSizeInSeconds));
        }

        [TestMethod]
        public void BuildPerformanceCounters_PerformanceCountersCalledMoreThanOnce_CorrectNumberOfOperationsPerformed()
        {
            // Arrange
            var systemUnderTest = new PerformanceCounterRegistry(_logger.Object, _statisticsStoreService, _performanceCounterStoreService, _options);

            // Build the counters for the initial time period
            systemUnderTest.BuildPerformanceCounters(_constantDateTimeUnixSeconds);

            // Act - build counters again for an additional 10 seconds period
             systemUnderTest.BuildPerformanceCounters(_constantDateTimeUnixSeconds + 10);

            // Assert - check logger was called the default number of times plus the number of additional times specified by the second window size.
            _logger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v,t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v,t) => true)),
                Times.Exactly(_options.InitialMetricHealthCheckWindowSizeInSeconds + 10));
        }

        /// <summary>
        /// Build a default set of Data Point stats for subsequent tests. Load in parallel to test out concurrency.
        /// </summary>
        private static void BuildStatisticsStore(StatisticsStoreService statisticsStoreService, long unixTimeInSeconds)
        {
            // Add 10 of each type of datapoint metrics for each unix second
            Parallel.For(0, 10, counter =>
            {
                for (var i = 0; i < 10; i++)
                {
                    statisticsStoreService.Add(new RequestCountDataPoint
                    {
                        UtcTimestampAsUnixTimeSeconds = counter + unixTimeInSeconds
                    });

                    statisticsStoreService.Add(new ResponseTimeDataPoint
                    {
                        UtcTimestampAsUnixTimeSeconds = unixTimeInSeconds,
                        ResponseTimeInMs = counter,
                    });
                }
            });
        }
    }
}