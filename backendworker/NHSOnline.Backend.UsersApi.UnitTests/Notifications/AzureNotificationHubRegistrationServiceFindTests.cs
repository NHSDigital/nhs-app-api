using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.NotificationHubs.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.UsersApi.Notifications;

namespace NHSOnline.Backend.UsersApi.UnitTests.Notifications
{
    [TestClass]
    public class AzureNotificationHubRegistrationServiceFindTests
    {
        private Mock<IAzureNotificationHubClient> _mockAzureNotificationsHubClient;
        private AzureNotificationHubRegistrationService _systemUnderTest;
        private const string NhsLoginId = "NhsLoginId";

        [TestInitialize]
        public void TestInitialize()
        {
            _mockAzureNotificationsHubClient = new Mock<IAzureNotificationHubClient>(MockBehavior.Strict);

            _systemUnderTest = new AzureNotificationHubRegistrationService(_mockAzureNotificationsHubClient.Object,
                new Mock<IInstallationFactory>().Object,
                new Mock<ILogger<AzureNotificationHubRegistrationService>>().Object);
        }

        [TestMethod]
        public async Task Find_WhenRegistrationsExists_ReturnsFoundResult()
        {
            // Arrange
            var registrationIds = new[] { "Registration1", "Registration2" };

            _mockAzureNotificationsHubClient
                .Setup(x => x.FindInstallationIdentifiersByNhsLoginId(NhsLoginId))
                .ReturnsAsync(registrationIds);

            // Act
            var result = await _systemUnderTest.Find(NhsLoginId);

            // Assert
            _mockAzureNotificationsHubClient.VerifyAll();

            var subject = result.Should().BeOfType<FindRegistrationsResult.Found>().Subject;
            subject.RegistrationIds.Should().BeEquivalentTo(registrationIds);
        }

        [TestMethod]
        public async Task Find_WhenNoRegistrations_ReturnsNotFoundResult()
        {
            // Arrange
            _mockAzureNotificationsHubClient
                .Setup(x => x.FindInstallationIdentifiersByNhsLoginId(NhsLoginId))
                .ReturnsAsync(Array.Empty<string>());

            // Act
            var result = await _systemUnderTest.Find(NhsLoginId);

            // Assert
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<FindRegistrationsResult.NotFound>();
        }

        [TestMethod]
        public async Task Find_WhenAzureClientThrowsMessagingException_ReturnsBadGatewayResult()
        {
            // Arrange
            _mockAzureNotificationsHubClient
                .Setup(x => x.FindInstallationIdentifiersByNhsLoginId(NhsLoginId))
                .ThrowsAsync(new MessagingException("This is an exception"));

            // Act
            var result = await _systemUnderTest.Find(NhsLoginId);

            // Assert
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<FindRegistrationsResult.BadGateway>();
        }

        [TestMethod]
        public async Task Find_WhenAzureClientThrowsHttpRequestException_ReturnsBadGatewayResult()
        {
            // Arrange
            _mockAzureNotificationsHubClient
                .Setup(x => x.FindInstallationIdentifiersByNhsLoginId(NhsLoginId))
                .ThrowsAsync(new HttpRequestException("This is an exception"));

            // Act
            var result = await _systemUnderTest.Find(NhsLoginId);

            // Assert
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<FindRegistrationsResult.BadGateway>();
        }

        [TestMethod]
        public async Task Find_WhenAzureClientThrowsException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            _mockAzureNotificationsHubClient
                .Setup(x => x.FindInstallationIdentifiersByNhsLoginId(NhsLoginId))
                .ThrowsAsync(new AggregateException("This is an exception"));

            // Act
            var result = await _systemUnderTest.Find(NhsLoginId);

            // Assert
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<FindRegistrationsResult.InternalServerError>();
        }
    }
}