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
using NHSOnline.Backend.UsersApi.Repository;
using UnitTestHelper;

namespace NHSOnline.Backend.UsersApi.UnitTests.Notifications
{
    [TestClass]
    public class AzureNotificationHubRegistrationServiceExistsTests
    {
        private Mock<IAzureNotificationHubClient> _mockAzureNotificationsHubClient;
        private AzureNotificationHubRegistrationService _systemUnderTest;
        private const string RegistrationId = "1111111111111111111-111111111111111111-1";
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
        public async Task Exists_WhenRegistrationExistsWithRegistrationIdentifier_ReturnsFoundResult()
        {
            // Arrange
            var userDevice = new UserDevice
            {
                RegistrationId =  RegistrationId
            };

            _mockAzureNotificationsHubClient
                .Setup(x => x.RegistrationExists(userDevice.RegistrationId))
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
        public async Task Exists_WhenRegistrationDoesNotExistWithRegistrationIdentifier_ReturnsNotFoundResult()
        {
            // Arrange
            var userDevice = new UserDevice
            {
                RegistrationId =  RegistrationId
            };

            _mockAzureNotificationsHubClient
                .Setup(x => x.RegistrationExists(userDevice.RegistrationId))
                .ReturnsAsync(false);

            // Act
            var result = await _systemUnderTest.Exists(userDevice);

            // Assert
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<RegistrationExistsResult.NotFound>();
        }

        [TestMethod]
        public async Task Exists_WhenRegistrationExistsThrowsException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            var userDevice = new UserDevice
            {
                RegistrationId =  RegistrationId
            };

            _mockAzureNotificationsHubClient
                .Setup(x => x.RegistrationExists(userDevice.RegistrationId))
                .Throws<ArgumentException>();

            // Act
            var result = await _systemUnderTest.Exists(userDevice);

            // Assert
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<RegistrationExistsResult.InternalServerError>();
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

        [DataTestMethod]
        [DataRow(WebExceptionStatus.ProtocolError, HttpStatusCode.Gone)]
        [DataRow(WebExceptionStatus.ProtocolError, HttpStatusCode.Moved)]
        [DataRow(WebExceptionStatus.SendFailure, HttpStatusCode.Gone)]
        [DataRow(WebExceptionStatus.SendFailure, HttpStatusCode.Moved)]
        public async Task Exists_WhenInstallationExistsThrowsAMessageException_ReturnsBadGatewayResult
        (
            WebExceptionStatus webExceptionStatus,
            HttpStatusCode statusCode
        )
        {
            // Arrange
            var userDevice = new UserDevice
            {
                RegistrationId =  InstallationId
            };

            _mockAzureNotificationsHubClient
                .Setup(x => x.InstallationExists(userDevice.RegistrationId))
                .Throws(
                    new MessagingException("Message exception",
                        new WebException("Web exception",
                            null,
                            webExceptionStatus,
                            HttpWebResponseHelper.CreateFromStatusCode(statusCode))));

            // Act
            var result = await _systemUnderTest.Exists(userDevice);

            // Assert
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<RegistrationExistsResult.BadGateway>();
        }

        [DataTestMethod]
        [DataRow(WebExceptionStatus.ProtocolError, HttpStatusCode.Gone)]
        [DataRow(WebExceptionStatus.ProtocolError, HttpStatusCode.Moved)]
        [DataRow(WebExceptionStatus.SendFailure, HttpStatusCode.Gone)]
        [DataRow(WebExceptionStatus.SendFailure, HttpStatusCode.Moved)]
        public async Task Exists_WhenRegistrationExistsThrowsAMessageException_ReturnsBadGatewayResult
        (
            WebExceptionStatus webExceptionStatus,
            HttpStatusCode statusCode
        )
        {
            // Arrange
            var userDevice = new UserDevice
            {
                RegistrationId =  RegistrationId
            };

            _mockAzureNotificationsHubClient
                .Setup(x => x.RegistrationExists(userDevice.RegistrationId))
                .Throws(
                    new MessagingException("Message exception",
                        new WebException("Web exception",
                            null,
                            webExceptionStatus,
                            HttpWebResponseHelper.CreateFromStatusCode(statusCode))));

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

        [TestMethod]
        public async Task Exists_WhenRegistrationExistsThrowsAHttpException_ReturnsBadGatewayResult()
        {
            // Arrange
            var userDevice = new UserDevice
            {
                RegistrationId =  RegistrationId
            };

            _mockAzureNotificationsHubClient
                .Setup(x => x.RegistrationExists(userDevice.RegistrationId))
                .Throws<HttpRequestException>();

            // Act
            var result = await _systemUnderTest.Exists(userDevice);

            // Assert
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<RegistrationExistsResult.BadGateway>();
        }
    }
}