using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.NotificationHubs.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.UsersApi.Notifications;
using UnitTestHelper;

namespace NHSOnline.Backend.UsersApi.UnitTests.Notifications
{
    [TestClass]
    public class AzureNotificationHubServiceDeleteTests
    {
        private Mock<IAzureNotificationHubClient> _mockAzureNotificationsHubClient;
        private AzureNotificationHubRegistrationService _systemUnderTest;
        private const string RegistrationId = "1111111111111111111-111111111111111111-1";
        private const string InstallationId = "fe7312a9-43dc-46f6-9727-03b3ddecab12";

        [TestInitialize]
        public void TestInitialize()
        {
            _mockAzureNotificationsHubClient = new Mock<IAzureNotificationHubClient>();

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
        public async Task Delete_WithRegistrationIdentifier_Success()
        {
            // Arrange
            _mockAzureNotificationsHubClient.Setup(x => x.DeleteRegistration(RegistrationId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _systemUnderTest.Delete(RegistrationId);

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
        public async Task Delete_WhenDeleteRegistrationThrowsException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            _mockAzureNotificationsHubClient
                .Setup(x => x.DeleteRegistration(RegistrationId))
                .Throws<ArgumentException>();

            // Act
            var result = await _systemUnderTest.Delete(RegistrationId);

            // Assert
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<DeleteRegistrationResult.InternalServerError>();
        }

        [DataTestMethod]
        [DataRow(WebExceptionStatus.ProtocolError, HttpStatusCode.Gone)]
        [DataRow(WebExceptionStatus.ProtocolError, HttpStatusCode.Moved)]
        [DataRow(WebExceptionStatus.SendFailure, HttpStatusCode.Gone)]
        [DataRow(WebExceptionStatus.SendFailure, HttpStatusCode.Moved)]
        public async Task Delete_WhenDeleteInstallationThrowsAMessageException_ReturnsBadGatewayResult
        (
            WebExceptionStatus webExceptionStatus,
            HttpStatusCode statusCode
        )
        {
            // Arrange
            _mockAzureNotificationsHubClient
                .Setup(x => x.DeleteInstallation(InstallationId))
                .Throws(
                    new MessagingException("Message exception",
                        new WebException("Web exception",
                            null,
                            webExceptionStatus,
                            HttpWebResponseHelper.CreateFromStatusCode(statusCode))));

            // Act
            var result = await _systemUnderTest.Delete(InstallationId);

            // Assert
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<DeleteRegistrationResult.BadGateway>();
        }

        [DataTestMethod]
        [DataRow(WebExceptionStatus.ProtocolError, HttpStatusCode.Gone)]
        [DataRow(WebExceptionStatus.ProtocolError, HttpStatusCode.Moved)]
        [DataRow(WebExceptionStatus.SendFailure, HttpStatusCode.Gone)]
        [DataRow(WebExceptionStatus.SendFailure, HttpStatusCode.Moved)]
        public async Task Delete_WhenDeleteRegistrationThrowsAMessageException_ReturnsBadGatewayResult
        (
            WebExceptionStatus webExceptionStatus,
            HttpStatusCode statusCode
        )
        {
            // Arrange
            _mockAzureNotificationsHubClient
                .Setup(x => x.DeleteRegistration(RegistrationId))
                .Throws(
                    new MessagingException("Message exception",
                        new WebException("Web exception",
                            null,
                            webExceptionStatus,
                            HttpWebResponseHelper.CreateFromStatusCode(statusCode))));

            // Act
            var result = await _systemUnderTest.Delete(RegistrationId);

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

        [TestMethod]
        public async Task Delete_WhenDeleteRegistrationThrowsAHttpException_ReturnsBadGatewayResult()
        {
            // Arrange
            _mockAzureNotificationsHubClient
                .Setup(x => x.DeleteRegistration(RegistrationId))
                .Throws<HttpRequestException>();

            // Act
            var result = await _systemUnderTest.Delete(RegistrationId);

            // Assert
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<DeleteRegistrationResult.BadGateway>();
        }
    }
}