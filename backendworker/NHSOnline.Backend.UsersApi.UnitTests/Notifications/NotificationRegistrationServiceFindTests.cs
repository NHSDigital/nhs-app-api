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
    public class NotificationRegistrationServiceFindTests
    {
        private Mock<INotificationClient> _mockNotificationClient;
        private NotificationRegistrationService _systemUnderTest;
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
        public async Task Find_WhenRegistrationsExists_ReturnsFoundResult()
        {
            // Arrange
            var registrationIds = new[] { "Registration1", "Registration2" };

            _mockNotificationClient
                .Setup(x => x.FindInstallationIdsByNhsLoginId(NhsLoginId))
                .ReturnsAsync(registrationIds);

            // Act
            var result = await _systemUnderTest.Find(NhsLoginId);

            // Assert
            _mockNotificationClient.VerifyAll();

            var subject = result.Should().BeOfType<FindRegistrationsResult.Found>().Subject;
            subject.RegistrationIds.Should().BeEquivalentTo(registrationIds);
        }

        [TestMethod]
        public async Task Find_WhenNoRegistrations_ReturnsNotFoundResult()
        {
            // Arrange
            _mockNotificationClient
                .Setup(x => x.FindInstallationIdsByNhsLoginId(NhsLoginId))
                .ReturnsAsync(Array.Empty<string>());

            // Act
            var result = await _systemUnderTest.Find(NhsLoginId);

            // Assert
            _mockNotificationClient.VerifyAll();

            result.Should().BeOfType<FindRegistrationsResult.NotFound>();
        }

        [TestMethod]
        public async Task Find_WhenAzureClientThrowsMessagingException_ReturnsBadGatewayResult()
        {
            // Arrange
            _mockNotificationClient
                .Setup(x => x.FindInstallationIdsByNhsLoginId(NhsLoginId))
                .ThrowsAsync(MessagingExceptionFactory.Create());

            // Act
            var result = await _systemUnderTest.Find(NhsLoginId);

            // Assert
            _mockNotificationClient.VerifyAll();

            result.Should().BeOfType<FindRegistrationsResult.BadGateway>();
        }

        [TestMethod]
        public async Task Find_WhenAzureClientThrowsHttpRequestException_ReturnsBadGatewayResult()
        {
            // Arrange
            _mockNotificationClient
                .Setup(x => x.FindInstallationIdsByNhsLoginId(NhsLoginId))
                .ThrowsAsync(new HttpRequestException("This is an exception"));

            // Act
            var result = await _systemUnderTest.Find(NhsLoginId);

            // Assert
            _mockNotificationClient.VerifyAll();

            result.Should().BeOfType<FindRegistrationsResult.BadGateway>();
        }

        [TestMethod]
        public async Task Find_WhenAzureClientThrowsException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            _mockNotificationClient
                .Setup(x => x.FindInstallationIdsByNhsLoginId(NhsLoginId))
                .ThrowsAsync(new AggregateException("This is an exception"));

            // Act
            var result = await _systemUnderTest.Find(NhsLoginId);

            // Assert
            _mockNotificationClient.VerifyAll();

            result.Should().BeOfType<FindRegistrationsResult.InternalServerError>();
        }
    }
}