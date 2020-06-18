using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Azure.NotificationHubs.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.UsersApi.Notifications;
using NHSOnline.Backend.UsersApi.Repository;
using UnitTestHelper;

namespace NHSOnline.Backend.UsersApi.UnitTests.Notifications
{
    [TestClass]
    public class AzureNotificationHubServiceExistsTests
    {
        private Fixture _fixture;
        private Mock<IAzureNotificationHubClient> _mockAzureNotificationsHubClient;
        private AzureNotificationHubRegistrationService _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());

            _mockAzureNotificationsHubClient = _fixture.Freeze<Mock<IAzureNotificationHubClient>>();

            _systemUnderTest = _fixture.Create<AzureNotificationHubRegistrationService>();
        }

        [TestMethod]
        public async Task Exists_WhenRegistrationExists_ReturnsFoundResult()
        {
            // Arrange
            var userDevice = _fixture.Create<UserDevice>();

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
        public async Task Exists_WhenRegistrationDoesNotExist_ReturnsNotFoundResult()
        {
            // Arrange
            var userDevice = _fixture.Create<UserDevice>();

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
        public async Task Exists_WhenRegistrationExistsThrowsException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            var userDevice = _fixture.Create<UserDevice>();

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
        public async Task Exists_WhenRegistrationExistsThrowsAMessageException_ReturnsBadGatewayResult
        (
            WebExceptionStatus webExceptionStatus,
            HttpStatusCode statusCode
        )
        {
            // Arrange
            var userDevice = _fixture.Create<UserDevice>();

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

        [TestMethod]
        public async Task Exists_WhenRegistrationExistsThrowsAHttpException_ReturnsBadGatewayResult()
        {
            // Arrange
            var userDevice = _fixture.Create<UserDevice>();

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