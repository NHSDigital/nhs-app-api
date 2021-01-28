using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.UsersApi.Notifications;

namespace NHSOnline.Backend.UsersApi.UnitTests.Notifications
{
    [TestClass]
    public class AzureNotificationHubRegistrationServiceDeleteTests
    {
        private Mock<IAzureNotificationHubClient> _mockAzureNotificationsHubClient;
        private AzureNotificationHubRegistrationService _systemUnderTest;
        private const string InstallationId = "fe7312a9-43dc-46f6-9727-03b3ddecab12";

        [TestInitialize]
        public void TestInitialize()
        {
            _mockAzureNotificationsHubClient = new Mock<IAzureNotificationHubClient>(MockBehavior.Strict);

            _systemUnderTest = new AzureNotificationHubRegistrationService(_mockAzureNotificationsHubClient.Object,
                new Mock<IInstallationFactory>().Object,
                new Mock<ILogger<AzureNotificationHubRegistrationService>>().Object);
        }

        [TestMethod]
        public async Task Delete_WithInstallationIdentifier_Success()
        {
            // Arrange
            _mockAzureNotificationsHubClient.Setup(x => x.DeleteInstallation(InstallationId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _systemUnderTest.Delete(InstallationId);

            // Assert
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<DeleteRegistrationResult.Success>();
        }

        [TestMethod]
        public async Task Delete_WhenDeleteInstallationThrowsException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            _mockAzureNotificationsHubClient
                .Setup(x => x.DeleteInstallation(InstallationId))
                .Throws<ArgumentException>();

            // Act
            var result = await _systemUnderTest.Delete(InstallationId);

            // Assert
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<DeleteRegistrationResult.InternalServerError>();
        }

        [TestMethod]
        public async Task Delete_WhenDeleteInstallationThrowsAMessageException_ReturnsBadGatewayResult()
        {
            // Arrange
            _mockAzureNotificationsHubClient
                .Setup(x => x.DeleteInstallation(InstallationId))
                .Throws(MessagingExceptionFactory.Create());

            // Act
            var result = await _systemUnderTest.Delete(InstallationId);

            // Assert
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<DeleteRegistrationResult.BadGateway>();
        }

        [TestMethod]
        public async Task Delete_WhenDeleteInstallationThrowsAHttpException_ReturnsBadGatewayResult()
        {
            // Arrange
            _mockAzureNotificationsHubClient
                .Setup(x => x.DeleteInstallation(InstallationId))
                .Throws<HttpRequestException>();

            // Act
            var result = await _systemUnderTest.Delete(InstallationId);

            // Assert
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<DeleteRegistrationResult.BadGateway>();
        }
    }
}