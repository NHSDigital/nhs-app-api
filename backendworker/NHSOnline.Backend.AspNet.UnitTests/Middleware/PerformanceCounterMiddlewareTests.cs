#nullable enable
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter;
using NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter.DataPoints;
using NHSOnline.Backend.AspNet.Middleware.PerformanceCounter;
using NHSOnline.Backend.AspNet.UnitTests.HealthChecks.PerformanceCounter;

namespace NHSOnline.Backend.AspNet.UnitTests.Middleware
{
    [TestClass]
    public sealed class PerformanceCounterMiddlewareTests
    {
        private PerformanceCounterMiddleware? _systemUnderTest;
        private readonly RequestDelegate _next;
        private readonly Mock<IStatisticsStoreService> _statisticsStoreServiceMock;
        private readonly Mock<ILogger<PerformanceCounterConfiguration>> _configLogger;
        private readonly PerformanceCounterConfiguration _options;

        public PerformanceCounterMiddlewareTests()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            _configLogger = fixture.Freeze<Mock<ILogger<PerformanceCounterConfiguration>>>();
            Mock<IConfiguration> configuration = new Mock<IConfiguration>();
            var configurationSection = new Mock<IConfigurationSection>();
            _options = PerformanceCounterConfigurationHelper.SetupConfiguration(configuration, configurationSection, _configLogger);

            _next = _ => Task.CompletedTask;
            _statisticsStoreServiceMock = new Mock<IStatisticsStoreService>();
        }

        [TestMethod]
        public void SetupConfig_WhenRequiredOptionIsMissing_UsesFallbackValue()
        {
            // Arrange
            var configuration = new Mock<IConfiguration>();
            var configurationSection = new Mock<IConfigurationSection>();

            configuration.Setup(a => a.GetSection(PerformanceCounterConfiguration.ConfigSectionName))
                .Returns(configurationSection.Object);

            // Act
            var builtConfiguration = new PerformanceCounterConfiguration(configuration.Object, _configLogger.Object);

            // Assert
            Assert.AreEqual(builtConfiguration.IsHealthCheckEnabled, false);
            Assert.AreEqual(builtConfiguration.IsMetricLoggingEnabled, false);
            Assert.AreEqual(builtConfiguration.QueueLength, PerformanceCounterConfiguration.DefaultQueueLength);
            Assert.AreEqual(builtConfiguration.InitialMetricHealthCheckWindowSizeInSeconds, PerformanceCounterConfiguration.DefaultInitialMetricHealthCheckWindowSizeInSeconds);
            Assert.AreEqual(builtConfiguration.ResponseTimeUnhealthyAverageThresholdInMs, PerformanceCounterConfiguration.DefaultResponseTimeUnhealthyAverageThresholdInMs);
            Assert.AreEqual(builtConfiguration.RequestCountUnhealthyNumberOfRequestsPerSecond, PerformanceCounterConfiguration.DefaultRequestCountUnhealthyNumberOfRequestsPerSecond);
        }

        [TestMethod]
        public async Task Invoke_WhenFeatureFlagIsNotEnabled_DoesNotInvokeMetricLogging()
        {
            // Arrange
            _options.IsMetricLoggingEnabled = false;

            var context = new DefaultHttpContext();
            _systemUnderTest = new PerformanceCounterMiddleware(_next, _statisticsStoreServiceMock.Object, _options);

            // Act
            await _systemUnderTest.Invoke(context);

            // Assert
            _statisticsStoreServiceMock.Verify(x => x.Add(It.IsAny<IDataPoint>()), Times.Never);
        }

        [TestMethod]
        public async Task Invoke_ValidHttpResponse_DoesInvokeMetricLogging()
        {
            // Arrange
            var feature = new DummyResponseFeature();
            var context = new DefaultHttpContext();

            context.Features.Set<IHttpResponseFeature>(feature);

            RequestDelegate next = async (ctx) => { await feature.InvokeCallBack(); };

            _systemUnderTest = new PerformanceCounterMiddleware(_next, _statisticsStoreServiceMock.Object, _options);

            // Act
            await _systemUnderTest.Invoke(context);

            // Assert
            _statisticsStoreServiceMock.Verify(x => x.Add(It.IsAny<IDataPoint>()), Times.Once);
        }
    }
}
