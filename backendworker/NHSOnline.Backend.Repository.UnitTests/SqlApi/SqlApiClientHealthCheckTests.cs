using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Repository.SqlApi;

namespace NHSOnline.Backend.Repository.UnitTests.SqlApi
{
    [TestClass]
    public class SqlApiClientHealthCheckTests
    {
        private TestSqlApiRepositoryConfiguration _config;
        private Mock<ILogger<SqlApiClientHealthCheck<TestSqlApiRepositoryConfiguration>>> _logger;
        private Mock<ISqlApiClientService> _sqlApiClientService;
        private SqlApiClientHealthCheck<TestSqlApiRepositoryConfiguration> _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _config = new TestSqlApiRepositoryConfiguration
            {
                DatabaseName = "Database name",
                ContainerName = "Container name"
            };

            _logger = new Mock<ILogger<SqlApiClientHealthCheck<TestSqlApiRepositoryConfiguration>>>();
            _sqlApiClientService = new Mock<ISqlApiClientService>(MockBehavior.Strict);

            _systemUnderTest = new SqlApiClientHealthCheck<TestSqlApiRepositoryConfiguration>(
                _logger.Object, _sqlApiClientService.Object, _config);
        }

        [TestMethod]
        public async Task CheckHealthAsync_WhenClientReturns_ShouldReturnHealthy()
        {
            // Arrange
            var containerResponse = new Mock<ContainerResponse>(MockBehavior.Strict);
            _sqlApiClientService.Setup(s => s.CheckHealthAsync(_config))
                .ReturnsAsync(containerResponse.Object);

            // Act
            var result = await _systemUnderTest.CheckHealthAsync(new HealthCheckContext(), new CancellationToken());

            // Assert
            result.Should().BeAssignableTo<HealthCheckResult>();
            result.Status.Should().Be(HealthStatus.Healthy);
        }

        [TestMethod]
        public async Task CheckHealthAsync_WhenClientThrows_ShouldReturnUnhealthy()
        {
            // Arrange
            var containerResponse = new Mock<ContainerResponse>(MockBehavior.Strict);
            _sqlApiClientService.Setup(s => s.CheckHealthAsync(_config))
                .ThrowsAsync(new CosmosException("Testing a failure", HttpStatusCode.Forbidden, 1234, "activityId", 1.12));

            // Act
            var result = await _systemUnderTest.CheckHealthAsync(new HealthCheckContext(), new CancellationToken());

            // Assert
            result.Should().BeAssignableTo<HealthCheckResult>();
            result.Status.Should().Be(HealthStatus.Degraded);
        }
    }
}