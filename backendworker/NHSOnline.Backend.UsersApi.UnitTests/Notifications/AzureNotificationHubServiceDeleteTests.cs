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
using UnitTestHelper;

namespace NHSOnline.Backend.UsersApi.UnitTests.Notifications
{
    [TestClass]
    public class AzureNotificationHubServiceDeleteTests
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
        public async Task Delete_Success()
        {
            // Arrange
            var registrationId = _fixture.Create<string>();

            _mockAzureNotificationsHubClient.Setup(x => x.DeleteInstallation(registrationId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _systemUnderTest.Delete(registrationId);

            // Assert
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<DeleteRegistrationResult.Success>();
        }

        [TestMethod]
        public async Task Delete_WhenDeleteRegistrationThrowsException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            var registrationId = _fixture.Create<string>();

            _mockAzureNotificationsHubClient
                .Setup(x => x.DeleteInstallation(registrationId))
                .Throws<ArgumentException>();

            // Act
            var result = await _systemUnderTest.Delete(registrationId);

            // Assert
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<DeleteRegistrationResult.InternalServerError>();
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
            var registrationId = _fixture.Create<string>();

            _mockAzureNotificationsHubClient
                .Setup(x => x.DeleteInstallation(registrationId))
                .Throws(
                    new MessagingException("Message exception",
                        new WebException("Web exception",
                            null,
                            webExceptionStatus,
                            HttpWebResponseHelper.CreateFromStatusCode(statusCode))));

            // Act
            var result = await _systemUnderTest.Delete(registrationId);

            // Assert
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<DeleteRegistrationResult.BadGateway>();
        }

        [TestMethod]
        public async Task Delete_WhenDeleteRegistrationThrowsAHttpException_ReturnsBadGatewayResult()
        {
            // Arrange
            var registrationId = _fixture.Create<string>();

            _mockAzureNotificationsHubClient
                .Setup(x => x.DeleteInstallation(registrationId))
                .Throws<HttpRequestException>();

            // Act
            var result = await _systemUnderTest.Delete(registrationId);

            // Assert
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<DeleteRegistrationResult.BadGateway>();
        }
    }
}