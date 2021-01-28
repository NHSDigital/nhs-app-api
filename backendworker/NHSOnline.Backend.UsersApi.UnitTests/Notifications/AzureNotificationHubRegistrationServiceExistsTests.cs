using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.UsersApi.Notifications;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.UnitTests.Notifications
{
    [TestClass]
    public class AzureNotificationHubRegistrationServiceExistsTests
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
        public async Task Exists_WhenRegistrationExistsWithInstallationIdentifier_ReturnsFoundResult()
        {
            // Arrange
            var userDevice = new UserDevice
            {
                RegistrationId =  InstallationId
            };

            _mockAzureNotificationsHubClient
                .Setup(x => x.InstallationExists(userDevice.RegistrationId))
                .ReturnsAsync(true);

            // Act
            var result = await _systemUnderTest.Exists(userDevice);

            // Assert
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<RegistrationExistsResult.Found>();
        }

        [TestMethod]
        public async Task Exists_WhenRegistrationDoesNotExistWithInstallationIdentifier_ReturnsNotFoundResult()
        {
            // Arrange
            var userDevice = new UserDevice
            {
                RegistrationId =  InstallationId
            };

            _mockAzureNotificationsHubClient
                .Setup(x => x.InstallationExists(userDevice.RegistrationId))
                .ReturnsAsync(false);

            // Act
            var result = await _systemUnderTest.Exists(userDevice);

            // Assert
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<RegistrationExistsResult.NotFound>();
        }

        [TestMethod]
        public async Task Exists_WhenInstallationExistsThrowsException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            var userDevice = new UserDevice
            {
                RegistrationId =  InstallationId
            };

            _mockAzureNotificationsHubClient
                .Setup(x => x.InstallationExists(userDevice.RegistrationId))
                .Throws<ArgumentException>();

            // Act
            var result = await _systemUnderTest.Exists(userDevice);

            // Assert
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<RegistrationExistsResult.InternalServerError>();
        }

        [TestMethod]
        public async Task Exists_WhenInstallationExistsThrowsAMessageException_ReturnsBadGatewayResult()
        {
            // Arrange
            var userDevice = new UserDevice
            {
                RegistrationId =  InstallationId
            };

            _mockAzureNotificationsHubClient
                .Setup(x => x.InstallationExists(userDevice.RegistrationId))
                .Throws(MessagingExceptionFactory.Create());

            // Act
            var result = await _systemUnderTest.Exists(userDevice);

            // Assert
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<RegistrationExistsResult.BadGateway>();
        }

        [TestMethod]
        public async Task Exists_WhenInstallationExistsThrowsAHttpException_ReturnsBadGatewayResult()
        {
            // Arrange
            var userDevice = new UserDevice
            {
                RegistrationId =  InstallationId
            };

            _mockAzureNotificationsHubClient
                .Setup(x => x.InstallationExists(userDevice.RegistrationId))
                .Throws<HttpRequestException>();

            // Act
            var result = await _systemUnderTest.Exists(userDevice);

            // Assert
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<RegistrationExistsResult.BadGateway>();
        }
    }
}