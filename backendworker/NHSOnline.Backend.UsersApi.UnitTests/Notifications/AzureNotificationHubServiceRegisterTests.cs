using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
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
    public class AzureNotificationHubServiceRegisterTests
    {
        private Fixture _fixture;
        private Mock<IAzureNotificationHubClient> _mockAzureNotificationsHubClient;
        private Mock<IInstallationFactory> _mockInstallationFactory;
        private AzureNotificationHubRegistrationService _systemUnderTest;
        private AccessToken _accessToken;
        private Installation _installation;
        private const string DevicePns = "DevicePns";
        private readonly DeviceType _deviceType = DeviceType.Android;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());

            _mockAzureNotificationsHubClient = _fixture.Freeze<Mock<IAzureNotificationHubClient>>();

            _installation = _fixture.Create<Installation>();

            _mockInstallationFactory = _fixture.Freeze<Mock<IInstallationFactory>>();

            var mockLogger = _fixture.Freeze<Mock<ILogger<AzureNotificationHubRegistrationService>>>();
            var accessTokenString = JwtToken.Generate(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _fixture.Create<string>()),
                new Claim("nhs_number", _fixture.Create<string>()),
            });

            _accessToken = AccessToken.Parse(mockLogger.Object, accessTokenString);

            _systemUnderTest = _fixture.Create<AzureNotificationHubRegistrationService>();
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

            _mockInstallationFactory.Setup(x => x.Create(DevicePns, _deviceType, _accessToken.Subject))
                .Returns(_installation);

            _mockAzureNotificationsHubClient.Setup(x => x.CreateOrUpdateInstallation(_installation))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _systemUnderTest.Register(DevicePns, _deviceType, _accessToken);

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
            var installationIds = _fixture.CreateMany<string>().ToList();

            var expectedResponse = new RegistrationResult.Success(
                new NotificationRegistrationResult
                {
                    Id = _installation.InstallationId
                });

            _mockInstallationFactory.Setup(x => x.Create(DevicePns, _deviceType, _accessToken.Subject))
                .Returns(_installation);

            _mockAzureNotificationsHubClient
                .Setup(x => x.FindInstallationIdentifiers(registerDeviceRequest.DevicePns))
                .ReturnsAsync(installationIds);

            foreach (var installationId in installationIds)
            {
                _mockAzureNotificationsHubClient.Setup(x => x.DeleteInstallation(installationId))
                    .Returns(Task.CompletedTask);
            }

            _mockAzureNotificationsHubClient
                .Setup(x => x.CreateOrUpdateInstallation(_installation))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _systemUnderTest.Register(DevicePns, _deviceType, _accessToken);

            // Assert
            _mockInstallationFactory.VerifyAll();
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public async Task Register_WhenInstallationFactoryThrowsException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            _mockInstallationFactory
                .Setup(x => x.Create(It.IsAny<string>(), It.IsAny<DeviceType>(), It.IsAny<string>()))
                .Throws<ArgumentOutOfRangeException>();

            // Act
            var result = await _systemUnderTest.Register(DevicePns, _deviceType, _accessToken);

            // Assert
            _mockInstallationFactory.VerifyAll();

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
            var installationIds = _fixture.CreateMany<string>().ToList();

            _mockAzureNotificationsHubClient
                .Setup(x => x.FindInstallationIdentifiers(registerDeviceRequest.DevicePns))
                .ReturnsAsync(installationIds);

            foreach (var installationId in installationIds)
            {
                _mockAzureNotificationsHubClient.Setup(x => x.DeleteInstallation(installationId))
                    .ThrowsAsync(new HttpRequestException());
            }

            // Act
            var result = await _systemUnderTest.Register(DevicePns, _deviceType, _accessToken);

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
            var installationIds = _fixture.CreateMany<string>().ToList();

            _mockAzureNotificationsHubClient
                .Setup(x => x.FindInstallationIdentifiers(registerDeviceRequest.DevicePns))
                .ReturnsAsync(installationIds);

            foreach (var installationId in installationIds)
            {
                _mockAzureNotificationsHubClient.Setup(x => x.DeleteInstallation(installationId))
                    .ThrowsAsync(new DivideByZeroException());
            }

            // Act
            var result = await _systemUnderTest.Register(DevicePns, _deviceType, _accessToken);

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
            var installationIds = _fixture.CreateMany<string>().ToList();

            _mockInstallationFactory.Setup(x => x.Create(DevicePns, _deviceType, _accessToken.Subject))
                .Returns(_installation);

            _mockAzureNotificationsHubClient
                .Setup(x => x.FindInstallationIdentifiers(registerDeviceRequest.DevicePns))
                .ReturnsAsync(installationIds);

            foreach (var installationId in installationIds)
            {
                _mockAzureNotificationsHubClient.Setup(x => x.DeleteInstallation(installationId))
                    .Returns(Task.CompletedTask);
            }

            _mockAzureNotificationsHubClient
                .Setup(x => x.CreateOrUpdateInstallation(_installation))
                .Throws<HttpRequestException>();

            // Act
            var result = await _systemUnderTest.Register(DevicePns, _deviceType, _accessToken);

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
            var installationIds = _fixture.CreateMany<string>().ToList();

            _mockInstallationFactory.Setup(x => x.Create(DevicePns, _deviceType, _accessToken.Subject))
                .Returns(_installation);

            _mockAzureNotificationsHubClient
                .Setup(x => x.FindInstallationIdentifiers(registerDeviceRequest.DevicePns))
                .ReturnsAsync(installationIds);

            foreach (var installationId in installationIds)
            {
                _mockAzureNotificationsHubClient.Setup(x => x.DeleteInstallation(installationId))
                    .Returns(Task.CompletedTask);
            }

            _mockAzureNotificationsHubClient
                .Setup(x => x.CreateOrUpdateInstallation(_installation))
                .Throws<Exception>();

            // Act
            var result = await _systemUnderTest.Register(DevicePns, _deviceType, _accessToken);

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
            var installationIds = _fixture.CreateMany<string>().ToList();

            _mockInstallationFactory.Setup(x => x.Create(DevicePns, _deviceType, _accessToken.Subject))
                .Returns(_installation);

            _mockAzureNotificationsHubClient
                .Setup(x => x.FindInstallationIdentifiers(registerDeviceRequest.DevicePns))
                .ReturnsAsync(installationIds);

            foreach (var installationId in installationIds)
            {
                _mockAzureNotificationsHubClient.Setup(x => x.DeleteInstallation(installationId))
                    .Returns(Task.CompletedTask);
            }

            _mockAzureNotificationsHubClient
                .Setup(x => x.CreateOrUpdateInstallation(_installation))
                .Throws(
                    new MessagingException("Message exception",
                        new WebException("Web exception",
                            null,
                            webExceptionStatus,
                            HttpWebResponseHelper.CreateFromStatusCode(statusCode))));

            // Act
            var result = await _systemUnderTest.Register(DevicePns, _deviceType, _accessToken);

            // Assert
            _mockInstallationFactory.VerifyAll();
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<RegistrationResult.BadGateway>();
        }
    }
}