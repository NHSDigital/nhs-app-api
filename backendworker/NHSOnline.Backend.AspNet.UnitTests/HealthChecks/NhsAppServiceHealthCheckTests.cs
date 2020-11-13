using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.AspNet.HealthChecks;
using RichardSzalay.MockHttp;
using UnitTestHelper;

namespace NHSOnline.Backend.AspNet.UnitTests.HealthChecks
{
    [TestClass]
    public class NhsAppServiceHealthCheckTests
    {
        [TestMethod]
        public async Task CheckHealthAsync_WhenServiceReturnsNoContent_HealthCheckResultIsHealthy()
        {
            // Arrange
            using var mockHttpHandler = new MockHttpMessageHandler();
            var healthCheckService = CreateHealthCheckService(mockHttpHandler);

            mockHttpHandler
                .Expect(HttpMethod.Get, "/health/ready")
                .Respond(HttpStatusCode.NoContent);

            // Act
            var status = await healthCheckService.CheckHealthAsync();

            // Assert
            status.Status.Should().Be(HealthStatus.Healthy);
            var reportEntry = status.Entries.Should().ContainSingle().Subject.Value;
            reportEntry.Status.Should().Be(HealthStatus.Healthy);
            reportEntry.Description.Should().Be("INhsAppHealthCheckClient Health check passed: NoContent");
        }

        [TestMethod]
        public async Task CheckHealthAsync_WhenServiceReturnsServiceUnavailable_HealthCheckResultIsDegraded()
        {
            // Arrange
            using var mockHttpHandler = new MockHttpMessageHandler();
            var healthCheckService = CreateHealthCheckService(mockHttpHandler);

            mockHttpHandler
                .Expect(HttpMethod.Get, "/health/ready")
                .Respond(HttpStatusCode.ServiceUnavailable);

            // Act
            var status = await healthCheckService.CheckHealthAsync();

            // Assert
            status.Status.Should().Be(HealthStatus.Degraded);
            var reportEntry = status.Entries.Should().ContainSingle().Subject.Value;
            reportEntry.Status.Should().Be(HealthStatus.Degraded);
            reportEntry.Description.Should().Be("INhsAppHealthCheckClient Health check degraded: ServiceUnavailable");
        }

        [TestMethod]
        [DataRow(HttpStatusCode.OK, "INhsAppHealthCheckClient Health check failed: OK")]
        [DataRow(HttpStatusCode.NotFound, "INhsAppHealthCheckClient Health check failed: NotFound")]
        [DataRow(HttpStatusCode.InternalServerError, "INhsAppHealthCheckClient Health check failed: InternalServerError")]
        public async Task CheckHealthAsync_WhenServiceReturnsOtherStatusCode_HealthCheckResultIsDegraded(
            HttpStatusCode statusCode,
            string expectedDescription)
        {
            // Arrange
            using var mockHttpHandler = new MockHttpMessageHandler();
            var healthCheckService = CreateHealthCheckService(mockHttpHandler);

            mockHttpHandler
                .Expect(HttpMethod.Get, "/health/ready")
                .Respond(statusCode);

            // Act
            var status = await healthCheckService.CheckHealthAsync();

            // Assert
            status.Status.Should().Be(HealthStatus.Degraded);
            var reportEntry = status.Entries.Should().ContainSingle().Subject.Value;
            reportEntry.Status.Should().Be(HealthStatus.Degraded);
            reportEntry.Description.Should().Be(expectedDescription);
        }

        [TestMethod]
        public async Task CheckHealthAsync_WhenServiceCallThrowsException_HealthCheckResultIsDegraded()
        {
            // Arrange
            using var mockHttpHandler = new MockHttpMessageHandler();
            var healthCheckService = CreateHealthCheckService(mockHttpHandler);

            mockHttpHandler
                .Expect(HttpMethod.Get, "/health/ready")
                .Throw(new InvalidOperationException());

            // Act
            var status = await healthCheckService.CheckHealthAsync();

            // Assert
            status.Status.Should().Be(HealthStatus.Degraded);
            var reportEntry = status.Entries.Should().ContainSingle().Subject.Value;
            reportEntry.Status.Should().Be(HealthStatus.Degraded);
            reportEntry.Description.Should().Be("INhsAppHealthCheckClient Health check failed");
        }

        [TestMethod]
        public async Task CheckHealthAsync_WhenServiceCallIsCancelled_HealthCheckResultIsDegraded()
        {
            // Arrange
            using var mockHttpHandler = new MockHttpMessageHandler();
            var healthCheckService = CreateHealthCheckService(mockHttpHandler);

            mockHttpHandler
                .Expect(HttpMethod.Get, "/health/ready")
                .Throw(new OperationCanceledException());

            // Act
            var status = await healthCheckService.CheckHealthAsync();

            // Assert
            status.Status.Should().Be(HealthStatus.Degraded);
            var reportEntry = status.Entries.Should().ContainSingle().Subject.Value;
            reportEntry.Status.Should().Be(HealthStatus.Degraded);
            reportEntry.Description.Should().Be("INhsAppHealthCheckClient Health check failed");
        }

        private static HealthCheckService CreateHealthCheckService(MockHttpMessageHandler mockHttpHandler)
        {
            var httpClient = new HttpClient(mockHttpHandler) { BaseAddress = new Uri("https://test.url/") };
            var mockNhsAppClient = new Mock<INhsAppHealthCheckClient>();
            mockNhsAppClient.Setup(x => x.Client).Returns(httpClient);

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddNhsAppHealthCheckService();
            serviceCollection.AddNhsAppHealthCheck<INhsAppHealthCheckClient>("Mock");
            serviceCollection.AddSingleton(mockNhsAppClient.Object);
            serviceCollection.AddMockLoggers();

            return serviceCollection.BuildServiceProvider().GetRequiredService<HealthCheckService>();
        }
    }
}
