using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.NotificationHubs.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Notifications;
using UnitTestHelper;
using RegistrationResult = NHSOnline.Backend.UsersApi.Notifications.RegistrationResult;

namespace NHSOnline.Backend.UsersApi.UnitTests.Notifications
{
    [TestClass]
    public class MigrationServiceTests
    {
        private Mock<IAzureNotificationHubClient> _mockAzureNotificationsHubClient;
        private Mock<IInstallationFactory> _mockInstallationFactory;
        private Mock<INotificationRegistrationService> _mockNotificationRegistrationService;
        private MigrationService _systemUnderTest;
        private Installation _installation;
        private const string DevicePns = "DevicePns";
        private const string NhsLoginId = "Nhsloginid";
        private readonly DeviceType _deviceType = DeviceType.Android;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockNotificationRegistrationService = new Mock<INotificationRegistrationService>();
            _mockAzureNotificationsHubClient = new Mock<IAzureNotificationHubClient>();

            _installation = new Installation
            {
                InstallationId = "InstallId"
            };
            _mockInstallationFactory = new Mock<IInstallationFactory>();

            _systemUnderTest = new MigrationService(
                _mockNotificationRegistrationService.Object,
                _mockAzureNotificationsHubClient.Object,
                _mockInstallationFactory.Object,
                new Mock<ILogger<MigrationService>>().Object);
        }

        [TestMethod]
        public async Task Register_WhenNoExistingRegistrations_ReturnsSuccess()
        {
            // Arrange
            var expectedResult = new RegistrationResult.Success(new NotificationRegistrationResult
            {
                Id = _installation.InstallationId
            });

            _mockInstallationFactory.Setup(x => x.Create(DevicePns, _deviceType, NhsLoginId))
                .Returns(_installation);

            _mockAzureNotificationsHubClient
                .Setup(x => x.CreateOrUpdateInstallation(_installation))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _systemUnderTest.Register(DevicePns, _deviceType, NhsLoginId);

            // Assert
            _mockInstallationFactory.VerifyAll();
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<RegistrationResult.Success>();
            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public async Task Register_WhenInstallationFactoryThrowsException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            _mockInstallationFactory
                .Setup(x => x.Create(DevicePns, _deviceType, NhsLoginId))
                .Throws<ArgumentOutOfRangeException>();

            // Act
            var result = await _systemUnderTest.Register(DevicePns, _deviceType, NhsLoginId);

            // Assert
            _mockInstallationFactory.VerifyAll();

            result.Should().BeOfType<RegistrationResult.InternalServerError>();
        }

        [TestMethod]
        public async Task Register_WhenCreateOrUpdateRegistrationThrowsAHttpException_ReturnsBadGatewayResult()
        {
            // Arrange
            _mockInstallationFactory.Setup(x => x.Create(DevicePns, _deviceType, NhsLoginId))
                .Returns(_installation);

            _mockAzureNotificationsHubClient
                .Setup(x => x.CreateOrUpdateInstallation(_installation))
                .Throws<HttpRequestException>();

            // Act
            var result = await _systemUnderTest.Register(DevicePns, _deviceType, NhsLoginId);

            // Assert
            _mockInstallationFactory.VerifyAll();
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<RegistrationResult.BadGateway>();
        }
        
        [TestMethod]
        public async Task
            Register_WhenCreateOrUpdateRegistrationThrowsAnException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            _mockInstallationFactory.Setup(x => x.Create(DevicePns, _deviceType, NhsLoginId))
                .Returns(_installation);

            _mockAzureNotificationsHubClient
                .Setup(x => x.CreateOrUpdateInstallation(_installation))
                .Throws<Exception>();

            // Act
            var result = await _systemUnderTest.Register(DevicePns, _deviceType, NhsLoginId);

            // Assert
            _mockInstallationFactory.VerifyAll();
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<RegistrationResult.InternalServerError>();
        }

        [DataTestMethod]
        [DataRow(WebExceptionStatus.ProtocolError, HttpStatusCode.Gone)]
        [DataRow(WebExceptionStatus.ProtocolError, HttpStatusCode.Moved)]
        [DataRow(WebExceptionStatus.SendFailure, HttpStatusCode.Gone)]
        [DataRow(WebExceptionStatus.SendFailure, HttpStatusCode.Moved)]
        public async Task Register_WhenCreateOrUpdateRegistrationThrowsAMessageException_ReturnsBadGatewayResult
        (
            WebExceptionStatus webExceptionStatus,
            HttpStatusCode statusCode
        )
        {
            // Arrange
            _mockInstallationFactory.Setup(x => x.Create(DevicePns, _deviceType, NhsLoginId))
                .Returns(_installation);

            _mockAzureNotificationsHubClient
                .Setup(x => x.CreateOrUpdateInstallation(_installation))
                .Throws(
                    new MessagingException("Message exception",
                        new WebException("Web exception",
                            null,
                            webExceptionStatus,
                            HttpWebResponseHelper.CreateFromStatusCode(statusCode))));

            // Act
            var result = await _systemUnderTest.Register(DevicePns, _deviceType, NhsLoginId);

            // Assert
            _mockInstallationFactory.VerifyAll();
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<RegistrationResult.BadGateway>();
        }
    }
}