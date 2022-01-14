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
    public class NotificationRegistrationServiceExistsTests
    {
        private Mock<INotificationClient> _mockNotificationClient;
        private NotificationRegistrationService _systemUnderTest;
        private const string InstallationId = "fe7312a9-43dc-46f6-9727-03b3ddecab12";
        private const string NhsLoginId = "a3cb5305-a12d-47ea-a704-a85c18b52485";

        private readonly UserDevice _userDevice = new UserDevice
        {
            DeviceId = InstallationId,
            NhsLoginId = NhsLoginId
        };

        [TestInitialize]
        public void TestInitialize()
        {
            _mockNotificationClient = new Mock<INotificationClient>(MockBehavior.Strict);

            _systemUnderTest = new NotificationRegistrationService(
                _mockNotificationClient.Object,
                new Mock<ILogger<NotificationRegistrationService>>().Object
            );
        }

        [TestMethod]
        public async Task Exists_WhenRegistrationExistsWithInstallationIdentifier_ReturnsFoundResult()
        {
            // Arrange
            _mockNotificationClient
                .Setup(x => x.InstallationExists(_userDevice.RegistrationId, _userDevice.NhsLoginId))
                .ReturnsAsync(true);

            // Act
            var result = await _systemUnderTest.Exists(_userDevice);

            // Assert
            _mockNotificationClient.VerifyAll();

            result.Should().BeOfType<RegistrationExistsResult.Found>();
        }

        [TestMethod]
        public async Task Exists_WhenRegistrationDoesNotExistWithInstallationIdentifier_ReturnsNotFoundResult()
        {
            // Arrange
            _mockNotificationClient
                .Setup(x => x.InstallationExists(_userDevice.RegistrationId, _userDevice.NhsLoginId))
                .ReturnsAsync(false);

            // Act
            var result = await _systemUnderTest.Exists(_userDevice);

            // Assert
            _mockNotificationClient.VerifyAll();

            result.Should().BeOfType<RegistrationExistsResult.NotFound>();
        }

        [TestMethod]
        public async Task Exists_WhenInstallationExistsThrowsException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            _mockNotificationClient
                .Setup(x => x.InstallationExists(_userDevice.RegistrationId, _userDevice.NhsLoginId))
                .Throws<ArgumentException>();

            // Act
            var result = await _systemUnderTest.Exists(_userDevice);

            // Assert
            _mockNotificationClient.VerifyAll();

            result.Should().BeOfType<RegistrationExistsResult.InternalServerError>();
        }

        [TestMethod]
        public async Task Exists_WhenInstallationExistsThrowsAMessageException_ReturnsBadGatewayResult()
        {
            // Arrange
            _mockNotificationClient
                .Setup(x => x.InstallationExists(_userDevice.RegistrationId, _userDevice.NhsLoginId))
                .Throws(MessagingExceptionFactory.Create());

            // Act
            var result = await _systemUnderTest.Exists(_userDevice);

            // Assert
            _mockNotificationClient.VerifyAll();

            result.Should().BeOfType<RegistrationExistsResult.BadGateway>();
        }

        [TestMethod]
        public async Task Exists_WhenInstallationExistsThrowsAHttpException_ReturnsBadGatewayResult()
        {
            // Arrange
            _mockNotificationClient
                .Setup(x => x.InstallationExists(_userDevice.RegistrationId, _userDevice.NhsLoginId))
                .Throws<HttpRequestException>();

            // Act
            var result = await _systemUnderTest.Exists(_userDevice);

            // Assert
            _mockNotificationClient.VerifyAll();

            result.Should().BeOfType<RegistrationExistsResult.BadGateway>();
        }
    }
}