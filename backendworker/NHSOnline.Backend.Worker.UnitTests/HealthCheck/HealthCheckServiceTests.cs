using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.HealthCheck;
using NHSOnline.Backend.Worker.HealthCheck.Redis;
using FluentAssertions;
using Moq;

namespace NHSOnline.Backend.Worker.UnitTests.HealthCheck
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
        public async Task RunAllHealthChecks_WhenCalledAndBothRedisChecksAreHealthy_ReturnsTwoHealthChecksWithIsHealthyInHealthCheckResponse()
        {
            // Arrange
            _redisHealthCheckFactory.SetupSequence(r => r
                    .Create(It.IsAny<ConnectionMultiplexerName>()).Execute())
                .Returns(Task.FromResult(
                    BaseHealthCheck.Result.Healthy(ConnectionMultiplexerName.OdsCodeLookup.ToString())))
                .Returns(Task.FromResult(
                    BaseHealthCheck.Result.Healthy(ConnectionMultiplexerName.Session.ToString())));
            
            // Act
            var healthCheckResponse = await _healthCheckService.RunHealthChecks();
            
            // Assert
            _redisHealthCheckFactory.Verify(x => x.Create(ConnectionMultiplexerName.OdsCodeLookup));
            _redisHealthCheckFactory.Verify(x => x.Create(ConnectionMultiplexerName.Session));
            healthCheckResponse.Should().NotBeNull();
            healthCheckResponse.HealthChecks.Where(x => x.IsHealthy).Should().HaveCount(2);
        }

        [TestMethod]
        public async Task RunAllHealthChecks_WhenCalledAndOdsRedisHealthCheckFails_ReturnsOdsRedisHealthCheckWithIsHealthyFalse()
        {
            // Arrange
            const string healthCheckNameFormat = "Redis {0}";
            var odsHealthCheckName =
                string.Format(CultureInfo.InvariantCulture, healthCheckNameFormat, ConnectionMultiplexerName.OdsCodeLookup.ToString());
            var sessionHealthCheckName =
                string.Format(CultureInfo.InvariantCulture, healthCheckNameFormat, ConnectionMultiplexerName.Session.ToString());
            
            _redisHealthCheckFactory.SetupSequence(r => r
                    .Create(It.IsAny<ConnectionMultiplexerName>()).Execute())
                .Returns(Task.FromResult(
                    BaseHealthCheck.Result.UnHealthy(odsHealthCheckName, "Error calling service")))
                .Returns(Task.FromResult(
                    BaseHealthCheck.Result.Healthy(sessionHealthCheckName)));
            
            // Act
            var healthCheckResponse = await _healthCheckService.RunHealthChecks();
           
            // Assert
            _redisHealthCheckFactory.Verify(x => x.Create(ConnectionMultiplexerName.OdsCodeLookup));
            _redisHealthCheckFactory.Verify(x => x.Create(ConnectionMultiplexerName.Session));
            healthCheckResponse.Should().NotBeNull();
            healthCheckResponse.HealthChecks.Where(
                    x => !x.IsHealthy && 
                    odsHealthCheckName.Equals(x.HealthCheckName, StringComparison.Ordinal))
                .Should().HaveCount(1);
            healthCheckResponse.HealthChecks.Where(
                    x => x.IsHealthy && sessionHealthCheckName.Equals(x.HealthCheckName, StringComparison.Ordinal))
                .Should().HaveCount(1);
        }
    }
}