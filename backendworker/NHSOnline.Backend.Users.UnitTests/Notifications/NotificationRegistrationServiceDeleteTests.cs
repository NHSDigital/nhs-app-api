using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Users.Notifications;

namespace NHSOnline.Backend.Users.UnitTests.Notifications
{
    [TestClass]
    public class NotificationRegistrationServiceDeleteTests
    {
        private Mock<INotificationClient> _mockNotificationClient;
        private NotificationRegistrationService _systemUnderTest;
        private const string InstallationId = "fe7312a9-43dc-46f6-9727-03b3ddecab12";
        private const string NhsLoginId = "NhsLoginId";

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
        public async Task Delete_WithInstallationIdentifier_Success()
        {
            // Arrange
            _mockNotificationClient
                .Setup(x => x.DeleteInstallation(InstallationId, NhsLoginId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _systemUnderTest.Delete(InstallationId, NhsLoginId);

            // Assert
            _mockNotificationClient.VerifyAll();

            result.Should().BeOfType<DeleteRegistrationResult.Success>();
        }

        [TestMethod]
        public async Task Delete_WhenDeleteInstallationThrowsException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            _mockNotificationClient
                .Setup(x => x.DeleteInstallation(InstallationId, NhsLoginId))
                .Throws<ArgumentException>();

            // Act
            var result = await _systemUnderTest.Delete(InstallationId, NhsLoginId);

            // Assert
            _mockNotificationClient.VerifyAll();

            result.Should().BeOfType<DeleteRegistrationResult.InternalServerError>();
        }

        [TestMethod]
        public async Task Delete_WhenDeleteInstallationThrowsAMessageException_ReturnsBadGatewayResult()
        {
            // Arrange
            _mockNotificationClient
                .Setup(x => x.DeleteInstallation(InstallationId, NhsLoginId))
                .Throws(MessagingExceptionFactory.CreateMessagingException());

            // Act
            var result = await _systemUnderTest.Delete(InstallationId, NhsLoginId);

            // Assert
            _mockNotificationClient.VerifyAll();

            result.Should().BeOfType<DeleteRegistrationResult.BadGateway>();
        }

        [TestMethod]
        public async Task Delete_WhenDeleteInstallationThrowsAHttpException_ReturnsBadGatewayResult()
        {
            // Arrange
            _mockNotificationClient
                .Setup(x => x.DeleteInstallation(InstallationId, NhsLoginId))
                .Throws<HttpRequestException>();

            // Act
            var result = await _systemUnderTest.Delete(InstallationId, NhsLoginId);

            // Assert
            _mockNotificationClient.VerifyAll();

            result.Should().BeOfType<DeleteRegistrationResult.BadGateway>();
        }
    }
}