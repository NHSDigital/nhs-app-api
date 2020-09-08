using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.NotificationHubs.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Notifications;
using UnitTestHelper;
using RegistrationResult = NHSOnline.Backend.UsersApi.Notifications.RegistrationResult;

namespace NHSOnline.Backend.UsersApi.UnitTests.Notifications
{
    [TestClass]
    public class AzureNotificationHubRegistrationServiceRegisterTests
    {
        private Mock<IAzureNotificationHubClient> _mockAzureNotificationsHubClient;
        private Mock<IInstallationFactory> _mockInstallationFactory;
        private AzureNotificationHubRegistrationService _systemUnderTest;
        private AccessToken _accessToken;
        private Installation _installation;
        private const string DevicePns = "DevicePns";
        private const string InstallationId = "InstallId";
        private const string RegistrationId = "RegistrationId";
        private const string NhsLoginId = "Nhsloginid";
        private readonly DeviceType _deviceType = DeviceType.Android;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockAzureNotificationsHubClient = new Mock<IAzureNotificationHubClient>(MockBehavior.Strict);

            _installation = new Installation();
            _mockInstallationFactory = new Mock<IInstallationFactory>(MockBehavior.Strict);

            var mockLogger = new Mock<ILogger<AzureNotificationHubRegistrationService>>();
            var accessTokenString = JwtToken.Generate(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, NhsLoginId),
                new Claim("nhs_number", "NhsNumber"),
            });

            _accessToken = AccessToken.Parse(mockLogger.Object, accessTokenString);

            _systemUnderTest = new AzureNotificationHubRegistrationService(
                _mockAzureNotificationsHubClient.Object,
                _mockInstallationFactory.Object,
                mockLogger.Object);
        }

        [TestMethod]
        public async Task Register_WhenNoExistingRegistrations_ReturnsSuccess()
        {
            // Arrange
            var expectedResponse = new RegistrationResult.Success(
                new NotificationRegistrationResult
                {
                    Id = _installation.InstallationId
                });

            _mockAzureNotificationsHubClient
                .Setup(x => x.FindInstallationIdentifiers(DevicePns))
                .ReturnsAsync(new List<NotificationRegistrationItem>());

            _mockInstallationFactory.Setup(x => x.Create(DevicePns, _deviceType, _accessToken.Subject))
                .Returns(_installation);

            _mockAzureNotificationsHubClient.Setup(x => x.CreateOrUpdateInstallation(_installation))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _systemUnderTest.Register(DevicePns, _deviceType, NhsLoginId);

            // Assert
            _mockInstallationFactory.VerifyAll();
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public async Task Register_WhenHasExistingRegistrations_ReturnsSuccess()
        {
            // Arrange
            var registerDeviceRequest = new RegisterDeviceRequest
            {
                DevicePns = DevicePns,
                DeviceType = _deviceType
            };
            var notificationRegistrationItems = GenerateNotificationRegistrationItems();

            var expectedResponse = new RegistrationResult.Success(
                new NotificationRegistrationResult
                {
                    Id = _installation.InstallationId
                });

            _mockInstallationFactory.Setup(x => x.Create(DevicePns, _deviceType, _accessToken.Subject))
                .Returns(_installation);

            _mockAzureNotificationsHubClient
                .Setup(x => x.FindInstallationIdentifiers(registerDeviceRequest.DevicePns))
                .ReturnsAsync(notificationRegistrationItems);

            _mockAzureNotificationsHubClient.Setup(x => x.DeleteInstallation(InstallationId))
                .Returns(Task.CompletedTask);
            _mockAzureNotificationsHubClient.Setup(x => x.DeleteRegistration(RegistrationId))
                .Returns(Task.CompletedTask);

            _mockAzureNotificationsHubClient
                .Setup(x => x.CreateOrUpdateInstallation(_installation))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _systemUnderTest.Register(DevicePns, _deviceType, NhsLoginId);

            // Assert
            _mockInstallationFactory.VerifyAll();
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public async Task Register_WhenInstallationFactoryThrowsException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            _mockAzureNotificationsHubClient
                .Setup(x => x.FindInstallationIdentifiers(DevicePns))
                .ReturnsAsync(new List<NotificationRegistrationItem>());

            _mockInstallationFactory
                .Setup(x => x.Create(DevicePns, _deviceType, NhsLoginId))
                .Throws<ArgumentOutOfRangeException>();

            // Act
            var result = await _systemUnderTest.Register(DevicePns, _deviceType, NhsLoginId);

            // Assert
            _mockInstallationFactory.VerifyAll();
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<RegistrationResult.InternalServerError>();
        }

        [TestMethod]
        public async Task Register_WhenDeletingAnyRegistrationThrowsAnHttpException_ReturnsBadGatewayResult()
        {
            // Arrange
            var registerDeviceRequest = new RegisterDeviceRequest
            {
                DevicePns = DevicePns,
                DeviceType = _deviceType
            };

            var notificationRegistrationItems = GenerateNotificationRegistrationItems();

            _mockAzureNotificationsHubClient
                .Setup(x => x.FindInstallationIdentifiers(registerDeviceRequest.DevicePns))
                .ReturnsAsync(notificationRegistrationItems);

            _mockAzureNotificationsHubClient.Setup(x => x.DeleteInstallation(InstallationId))
                .ThrowsAsync(new HttpRequestException());
            _mockAzureNotificationsHubClient.Setup(x => x.DeleteRegistration(RegistrationId))
                .ThrowsAsync(new HttpRequestException());

            // Act
            var result = await _systemUnderTest.Register(DevicePns, _deviceType, NhsLoginId);

            // Assert
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<RegistrationResult.BadGateway>();
        }

        [TestMethod]
        public async Task Register_WhenDeletingAnyRegistrationThrowsAnException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            var registerDeviceRequest = new RegisterDeviceRequest
            {
                DevicePns = DevicePns,
                DeviceType = _deviceType
            };
            var notificationRegistrationItems = GenerateNotificationRegistrationItems();

            _mockAzureNotificationsHubClient
                .Setup(x => x.FindInstallationIdentifiers(registerDeviceRequest.DevicePns))
                .ReturnsAsync(notificationRegistrationItems);

            _mockAzureNotificationsHubClient.Setup(x => x.DeleteInstallation(InstallationId))
                .ThrowsAsync(new DivideByZeroException());
            _mockAzureNotificationsHubClient.Setup(x => x.DeleteRegistration(RegistrationId))
                .ThrowsAsync(new DivideByZeroException());

            // Act
            var result = await _systemUnderTest.Register(DevicePns, _deviceType, NhsLoginId);

            // Assert
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<RegistrationResult.InternalServerError>();
        }

        [TestMethod]
        public async Task Register_WhenCreateOrUpdateRegistrationThrowsAHttpException_ReturnsBadGatewayResult()
        {
            // Arrange
            var registerDeviceRequest = new RegisterDeviceRequest
            {
                DevicePns = DevicePns,
                DeviceType = _deviceType
            };
            var notificationRegistrationItems = GenerateNotificationRegistrationItems();

            _mockInstallationFactory.Setup(x => x.Create(DevicePns, _deviceType, _accessToken.Subject))
                .Returns(_installation);

            _mockAzureNotificationsHubClient
                .Setup(x => x.FindInstallationIdentifiers(registerDeviceRequest.DevicePns))
                .ReturnsAsync(notificationRegistrationItems);

            _mockAzureNotificationsHubClient.Setup(x => x.DeleteInstallation(InstallationId))
                .Returns(Task.CompletedTask);
            _mockAzureNotificationsHubClient.Setup(x => x.DeleteRegistration(RegistrationId))
                .Returns(Task.CompletedTask);

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
            var registerDeviceRequest = new RegisterDeviceRequest
            {
                DevicePns = DevicePns,
                DeviceType = _deviceType
            };
            var notificationRegistrationItems = GenerateNotificationRegistrationItems();

            _mockInstallationFactory.Setup(x => x.Create(DevicePns, _deviceType, _accessToken.Subject))
                .Returns(_installation);

            _mockAzureNotificationsHubClient
                .Setup(x => x.FindInstallationIdentifiers(registerDeviceRequest.DevicePns))
                .ReturnsAsync(notificationRegistrationItems);

            _mockAzureNotificationsHubClient.Setup(x => x.DeleteInstallation(InstallationId))
                .Returns(Task.CompletedTask);
            _mockAzureNotificationsHubClient.Setup(x => x.DeleteRegistration(RegistrationId))
                .Returns(Task.CompletedTask);

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
            var registerDeviceRequest = new RegisterDeviceRequest
            {
                DevicePns = DevicePns,
                DeviceType = _deviceType
            };
            var notificationRegistrationItems = GenerateNotificationRegistrationItems();

            _mockInstallationFactory.Setup(x => x.Create(DevicePns, _deviceType, _accessToken.Subject))
                .Returns(_installation);

            _mockAzureNotificationsHubClient
                .Setup(x => x.FindInstallationIdentifiers(registerDeviceRequest.DevicePns))
                .ReturnsAsync(notificationRegistrationItems);

            _mockAzureNotificationsHubClient.Setup(x => x.DeleteInstallation(InstallationId))
                .Returns(Task.CompletedTask);
            _mockAzureNotificationsHubClient.Setup(x => x.DeleteRegistration(RegistrationId))
                .Returns(Task.CompletedTask);

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

        private List<NotificationRegistrationItem> GenerateNotificationRegistrationItems()
        {
            return  new List<NotificationRegistrationItem>
            {
                { new NotificationRegistrationItem
                {
                    Id = InstallationId,
                    Type = NotificationRegistrationItem.RegistrationType.Installation
                }},
                { new NotificationRegistrationItem
                {
                    Id = RegistrationId,
                    Type = NotificationRegistrationItem.RegistrationType.Registration
                }}

            };
        }
    }
}