using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.PfsApi.HealthCheck;
using FluentAssertions;
using Moq;
using NHSOnline.Backend.GpSystems;

namespace NHSOnline.Backend.PfsApi.UnitTests.HealthCheck
{
    [TestClass]
    public class HealthCheckServiceTests
    {
        private HealthCheckService _healthCheckService;
        private IFixture _fixture;
        private Mock<IRedisHealthCheckFactory> _redisHealthCheckFactory;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());           
            _redisHealthCheckFactory = _fixture.Freeze<Mock<IRedisHealthCheckFactory>>();
            _healthCheckService = _fixture.Create<HealthCheckService>();
        }
        
        [TestMethod]
        public async Task RunAllHealthChecks_WhenCalledAndOdsRedisChecksIsHealthy_ReturnsSingleHealthCheckWithIsHealthyInHealthCheckResponse()
        {
            // Arrange
            _redisHealthCheckFactory.SetupSequence(r => r
                    .Create(It.IsAny<ConnectionMultiplexerName>()).Execute())
                .Returns(Task.FromResult(
                    BaseHealthCheck.Result.Healthy(ConnectionMultiplexerName.OdsCodeLookup.ToString())));
            
            // Act
            var healthCheckResponse = await _healthCheckService.RunHealthChecks();
            
            // Assert
            _redisHealthCheckFactory.Verify(x => x.Create(ConnectionMultiplexerName.OdsCodeLookup));
            healthCheckResponse.Should().NotBeNull();
            healthCheckResponse.HealthChecks.Where(x => x.IsHealthy).Should().HaveCount(1);
        }

        [TestMethod]
        public async Task RunAllHealthChecks_WhenCalledAndOdsRedisHealthCheckFails_ReturnsOdsRedisHealthCheckWithIsHealthyFalse()
        {
            // Arrange
            const string healthCheckNameFormat = "Redis {0}";
            var odsHealthCheckName =
                string.Format(CultureInfo.InvariantCulture, healthCheckNameFormat, ConnectionMultiplexerName.OdsCodeLookup.ToString());

            _redisHealthCheckFactory.SetupSequence(r => r
                    .Create(It.IsAny<ConnectionMultiplexerName>()).Execute())
                .Returns(Task.FromResult(
                    BaseHealthCheck.Result.UnHealthy(odsHealthCheckName, "Error calling service")));
            
            // Act
            var healthCheckResponse = await _healthCheckService.RunHealthChecks();
           
            // Assert
            _redisHealthCheckFactory.Verify(x => x.Create(ConnectionMultiplexerName.OdsCodeLookup));
            healthCheckResponse.Should().NotBeNull();
            healthCheckResponse.HealthChecks.Where(
                    x => !x.IsHealthy && 
                    odsHealthCheckName.Equals(x.HealthCheckName, StringComparison.Ordinal))
                .Should().HaveCount(1);
        }
    }
}