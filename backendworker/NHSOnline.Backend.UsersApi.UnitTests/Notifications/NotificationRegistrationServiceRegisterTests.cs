using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Notifications;
using NHSOnline.Backend.UsersApi.Notifications.Models;

namespace NHSOnline.Backend.UsersApi.UnitTests.Notifications
{
    [TestClass]
    public class NotificationRegistrationServiceRegisterTests
    {
        private Mock<INotificationClient> _mockNotificationClient;
        private NotificationRegistrationService _systemUnderTest;
        private const string DevicePns = "DevicePns";
        private const string InstallationId = "InstallationId";
        private const string NhsLoginId = "Nhsloginid";
        private const DeviceType UserDeviceType = DeviceType.Android;

        private readonly InstallationRequest _installation = new InstallationRequest
        {
            DevicePns = DevicePns,
            DeviceType = UserDeviceType,
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
        public async Task Register_WhenNoExistingRegistrations_ReturnsSuccess()
        {
            // Arrange
            var expectedResponse = new RegistrationResult.Success(new NotificationRegistrationResult
            {
                Id = InstallationId
            });

            _mockNotificationClient
                .Setup(x => x.DeleteInstallationsByDevicePns(DevicePns))
                .Returns(Task.CompletedTask);

            _mockNotificationClient
                .Setup(x => x.CreateInstallation(_installation, null))
                .ReturnsAsync(InstallationId);

            // Act
            var result = await _systemUnderTest.Register(_installation);

            // Assert
            _mockNotificationClient.VerifyAll();

            result.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public async Task Register_WhenHasExistingRegistrations_ReturnsSuccess()
        {
            // Arrange
            var expectedResponse = new RegistrationResult.Success(new NotificationRegistrationResult
            {
                Id = InstallationId
            });

            _mockNotificationClient
                .Setup(x => x.DeleteInstallationsByDevicePns(DevicePns))
                .Returns(Task.CompletedTask);

            _mockNotificationClient
                .Setup(x => x.CreateInstallation(_installation, null))
                .ReturnsAsync(InstallationId);

            // Act
            var result = await _systemUnderTest.Register(_installation);

            // Assert
            _mockNotificationClient.VerifyAll();

            result.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public async Task Register_WhenInstallationFactoryThrowsException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            // Act
            var result = await _systemUnderTest.Register(_installation);

            // Assert
            _mockNotificationClient.VerifyAll();

            result.Should().BeOfType<RegistrationResult.InternalServerError>();
        }

        [TestMethod]
        public async Task Register_WhenDeletingAnyRegistrationThrowsAnHttpException_ReturnsBadGatewayResult()
        {
            // Arrange
            _mockNotificationClient
                .Setup(x => x.DeleteInstallationsByDevicePns(DevicePns))
                .ThrowsAsync(new HttpRequestException());

            // Act
            var result = await _systemUnderTest.Register(_installation);

            // Assert
            _mockNotificationClient.VerifyAll();

            result.Should().BeOfType<RegistrationResult.BadGateway>();
        }

        [TestMethod]
        public async Task Register_WhenDeletingAnyRegistrationThrowsAnException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            _mockNotificationClient
                .Setup(x => x.DeleteInstallationsByDevicePns(DevicePns))
                .ThrowsAsync(new DivideByZeroException());

            // Act
            var result = await _systemUnderTest.Register(_installation);

            // Assert
            _mockNotificationClient.VerifyAll();

            result.Should().BeOfType<RegistrationResult.InternalServerError>();
        }

        [TestMethod]
        public async Task Register_WhenCreateOrUpdateRegistrationThrowsAHttpException_ReturnsBadGatewayResult()
        {
            // Arrange
            _mockNotificationClient
                .Setup(x => x.DeleteInstallationsByDevicePns(DevicePns))
                .Returns(Task.CompletedTask);

            _mockNotificationClient
                .Setup(x => x.CreateInstallation(_installation, null))
                .Throws<HttpRequestException>();

            // Act
            var result = await _systemUnderTest.Register(_installation);

            // Assert
            _mockNotificationClient.VerifyAll();

            result.Should().BeOfType<RegistrationResult.BadGateway>();
        }

        [TestMethod]
        public async Task Register_WhenCreateOrUpdateRegistrationThrowsAnException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            _mockNotificationClient
                .Setup(x => x.DeleteInstallationsByDevicePns(DevicePns))
                .Returns(Task.CompletedTask);

            _mockNotificationClient
                .Setup(x => x.CreateInstallation(_installation, null))
                .Throws<Exception>();

            // Act
            var result = await _systemUnderTest.Register(_installation);

            // Assert
            _mockNotificationClient.VerifyAll();

            result.Should().BeOfType<RegistrationResult.InternalServerError>();
        }

        [TestMethod]
        public async Task Register_WhenCreateOrUpdateRegistrationThrowsAMessageException_ReturnsBadGatewayResult()
        {
            // Arrange
            _mockNotificationClient
                .Setup(x => x.DeleteInstallationsByDevicePns(DevicePns))
                .Returns(Task.CompletedTask);

            _mockNotificationClient
                .Setup(x => x.CreateInstallation(_installation, null))
                .Throws(MessagingExceptionFactory.Create());

            // Act
            var result = await _systemUnderTest.Register(_installation);

            // Assert
            _mockNotificationClient.VerifyAll();

            result.Should().BeOfType<RegistrationResult.BadGateway>();
        }
    }
}