using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Azure.NotificationHubs.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Notifications;
using NHSOnline.Backend.UsersApi.Notifications.Azure;
using NHSOnline.Backend.UsersApi.Notifications.Azure.Steps;
using UnitTestHelper;
using RegistrationResult = NHSOnline.Backend.UsersApi.Notifications.RegistrationResult;

namespace NHSOnline.Backend.UsersApi.UnitTests.Notifications.Azure
{
    [TestClass]
    public class AzureNotificationHubServiceRegisterTests
    {
        private Fixture _fixture;
        private Mock<IRegistrationDescriptionFactory> _mockRegistrationDescriptionFactory;
        private Mock<IAzureNotificationHubClient> _mockAzureNotificationsHubClient;
        private AzureNotificationHubService _systemUnderTest;
        private AccessToken _accessToken;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());

            _mockRegistrationDescriptionFactory = _fixture.Freeze<Mock<IRegistrationDescriptionFactory>>();
            _mockAzureNotificationsHubClient = _fixture.Freeze<Mock<IAzureNotificationHubClient>>();

            var mockLogger = _fixture.Freeze<Mock<ILogger<AzureNotificationHubService>>>();
            var accessTokenString = JwtToken.Generate(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _fixture.Create<string>()),
                new Claim("nhs_number", _fixture.Create<string>()),
            });

            _accessToken = AccessToken.Parse(mockLogger.Object, accessTokenString);

            var mockServiceProvider = _fixture.Freeze<Mock<IServiceProvider>>();
            mockServiceProvider.Setup(x => x.GetService(It.Is<Type>(t => t == typeof(RemoveRegistrationsStep))))
                .Returns(_fixture.Create<RemoveRegistrationsStep>());

            mockServiceProvider.Setup(x => x.GetService(It.Is<Type>(t => t == typeof(CreateRegistrationIdStep))))
                .Returns(_fixture.Create<CreateRegistrationIdStep>());

            mockServiceProvider.Setup(x => x.GetService(It.Is<Type>(t => t == typeof(CreateOrUpdateRegistrationStep))))
                .Returns(_fixture.Create<CreateOrUpdateRegistrationStep>());

            var operationStepBuilder = _fixture.Create<OperationStepBuilder>();
            _fixture.Inject<IOperationStepBuilder>(operationStepBuilder);

            _systemUnderTest = _fixture.Create<AzureNotificationHubService>();
        }

        [TestMethod]
        public async Task Register_WhenNoExistingRegistrations_ReturnsSuccess()
        {
            // Arrange
            var registrationId = _fixture.Create<string>();
            var registrationExpiryTime = _fixture.Create<DateTime>();
            var registerDeviceRequest = _fixture.Create<RegisterDeviceRequest>();

            var expectedResponse = new RegistrationResult.Success(
                new NotificationRegistrationResult
                {
                    RegistrationExpiry = registrationExpiryTime,
                    RegistrationId = registrationId
                });

            var registrationDescription = _fixture.Build<AppleRegistrationDescriptionTestWrapper>()
                .With(x => x.DeviceToken, registerDeviceRequest.DevicePns)
                .Without(x => x.ExtensionData)
                .Create();

            var registrationCreatedUpdatedResponse = _fixture.Build<AppleRegistrationDescriptionTestWrapper>()
                .With(x => x.DeviceToken, registerDeviceRequest.DevicePns)
                .With(x => x.Tags, registrationDescription.Tags)
                .With(x => x.RegistrationId, registrationId)
                .With(x => x.ExpirationTime, registrationExpiryTime)
                .Without(x => x.ExtensionData)
                .Create();

            _mockRegistrationDescriptionFactory
                .Setup(x => x.Create(registerDeviceRequest, _accessToken.Subject))
                .Returns(registrationDescription);

            _mockAzureNotificationsHubClient.Setup(x => x.CreateRegistrationId())
                .ReturnsAsync(registrationId);

            _mockAzureNotificationsHubClient.Setup(x => x.CreateOrUpdateRegistration(registrationDescription))
                .ReturnsAsync(registrationCreatedUpdatedResponse);

            // Act
            var result = await _systemUnderTest.Register(registerDeviceRequest, _accessToken);

            // Assert
            _mockRegistrationDescriptionFactory.VerifyAll();
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public async Task Register_WhenHasExistingRegistrations_ReturnsSuccess()
        {
            // Arrange
            var registrationId = _fixture.Create<string>();
            var registrationExpiryTime = _fixture.Create<DateTime>();
            var registerDeviceRequest = _fixture.Create<RegisterDeviceRequest>();

            var expectedResponse = new RegistrationResult.Success(
                new NotificationRegistrationResult
                {
                    RegistrationExpiry = registrationExpiryTime,
                    RegistrationId = registrationId
                });

            var registrationDescription = _fixture.Build<AppleRegistrationDescriptionTestWrapper>()
                .With(x => x.DeviceToken, registerDeviceRequest.DevicePns)
                .Without(x => x.ExtensionData)
                .Create();

            var registrationCreatedUpdatedResponse = _fixture.Build<AppleRegistrationDescriptionTestWrapper>()
                .With(x => x.DeviceToken, registerDeviceRequest.DevicePns)
                .With(x => x.Tags, registrationDescription.Tags)
                .With(x => x.RegistrationId, registrationId)
                .With(x => x.ExpirationTime, registrationExpiryTime)
                .Without(x => x.ExtensionData)
                .Create();

            _mockRegistrationDescriptionFactory
                .Setup(x => x.Create(registerDeviceRequest, _accessToken.Subject))
                .Returns(registrationDescription);

            _mockAzureNotificationsHubClient
                .Setup(x => x.DeleteAllRegistrations(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _mockAzureNotificationsHubClient
                .Setup(x => x.CreateRegistrationId())
                .ReturnsAsync(registrationId);

            _mockAzureNotificationsHubClient
                .Setup(x => x.CreateOrUpdateRegistration(registrationDescription))
                .ReturnsAsync(registrationCreatedUpdatedResponse);

            // Act
            var result = await _systemUnderTest.Register(registerDeviceRequest, _accessToken);

            // Assert
            _mockRegistrationDescriptionFactory.VerifyAll();
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public async Task Register_WhenCreateOrUpdateRegistrationReturnsNoData_ReturnsInternalServerError()
        {
            // Arrange
            var registrationId = _fixture.Create<string>();
            var registerDeviceRequest = _fixture.Create<RegisterDeviceRequest>();

            var registrationDescription = _fixture.Build<AppleRegistrationDescriptionTestWrapper>()
                .With(x => x.DeviceToken, registerDeviceRequest.DevicePns)
                .Without(x => x.ExtensionData)
                .Create();

            var registrationCreatedUpdatedResponse = _fixture.Build<AppleRegistrationDescriptionTestWrapper>()
                .With(x => x.DeviceToken, registerDeviceRequest.DevicePns)
                .With(x => x.Tags, registrationDescription.Tags)
                .Without(x => x.RegistrationId)
                .Without(x => x.ExpirationTime)
                .Without(x => x.ExtensionData)
                .Create();

            _mockRegistrationDescriptionFactory
                .Setup(x => x.Create(registerDeviceRequest, _accessToken.Subject))
                .Returns(registrationDescription);

            _mockAzureNotificationsHubClient
                .Setup(x => x.DeleteAllRegistrations(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _mockAzureNotificationsHubClient
                .Setup(x => x.CreateRegistrationId())
                .ReturnsAsync(registrationId);

            _mockAzureNotificationsHubClient
                .Setup(x => x.CreateOrUpdateRegistration(registrationDescription))
                .ReturnsAsync(registrationCreatedUpdatedResponse);

            // Act
            var result = await _systemUnderTest.Register(registerDeviceRequest, _accessToken);

            // Assert
            _mockRegistrationDescriptionFactory.VerifyAll();
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<RegistrationResult.InternalServerError>();
        }

        [TestMethod]
        public void Register_WhenRequestFactoryThrowsArgumentException_ThrowsArgumentException()
        {
            // Arrange
            _mockRegistrationDescriptionFactory
                .Setup(x => x.Create(It.IsAny<RegisterDeviceRequest>(), It.IsAny<string>()))
                .Throws<ArgumentException>();

            // Act
            Func<Task> act = () => _systemUnderTest.Register(_fixture.Create<RegisterDeviceRequest>(), _accessToken);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public async Task Register_WhenDeleteAllRegistrationsThrowsAHttpException_ReturnsBadGatewayResult()
        {
            // Arrange
            var registerDeviceRequest = _fixture.Create<RegisterDeviceRequest>();

            var registrationDescription = _fixture.Build<AppleRegistrationDescriptionTestWrapper>()
                .With(x => x.DeviceToken, registerDeviceRequest.DevicePns)
                .Without(x => x.ExtensionData)
                .Create();

            _mockRegistrationDescriptionFactory
                .Setup(x => x.Create(registerDeviceRequest, _accessToken.Subject))
                .Returns(registrationDescription);

            _mockAzureNotificationsHubClient
                .Setup(x => x.DeleteAllRegistrations(It.IsAny<string>()))
                .Throws<HttpRequestException>();

            // Act
            var result = await _systemUnderTest.Register(registerDeviceRequest, _accessToken);

            // Assert
            _mockRegistrationDescriptionFactory.VerifyAll();
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<RegistrationResult.BadGateway>();
        }

        [TestMethod]
        public async Task Register_WhenDeleteAllRegistrationsThrowsAException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            var registerDeviceRequest = _fixture.Create<RegisterDeviceRequest>();

            var registrationDescription = _fixture.Build<AppleRegistrationDescriptionTestWrapper>()
                .With(x => x.DeviceToken, registerDeviceRequest.DevicePns)
                .Without(x => x.ExtensionData)
                .Create();

            _mockRegistrationDescriptionFactory
                .Setup(x => x.Create(registerDeviceRequest, _accessToken.Subject))
                .Returns(registrationDescription);

            _mockAzureNotificationsHubClient
                .Setup(x => x.DeleteAllRegistrations(It.IsAny<string>()))
                .Throws<Exception>();

            // Act
            var result = await _systemUnderTest.Register(registerDeviceRequest, _accessToken);

            // Assert
            _mockRegistrationDescriptionFactory.VerifyAll();
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<RegistrationResult.InternalServerError>();
        }

        [TestMethod]
        public async Task Register_WhenCreateRegistrationIdThrowsAHttpException_ReturnsBadGatewayResult()
        {
            // Arrange
            var registerDeviceRequest = _fixture.Create<RegisterDeviceRequest>();

            var registrationDescription = _fixture.Build<AppleRegistrationDescriptionTestWrapper>()
                .With(x => x.DeviceToken, registerDeviceRequest.DevicePns)
                .Without(x => x.ExtensionData)
                .Create();

            _mockRegistrationDescriptionFactory
                .Setup(x => x.Create(registerDeviceRequest, _accessToken.Subject))
                .Returns(registrationDescription);

            _mockAzureNotificationsHubClient
                .Setup(x => x.CreateRegistrationId())
                .Throws<HttpRequestException>();

            // Act
            var result = await _systemUnderTest.Register(registerDeviceRequest, _accessToken);

            // Assert
            _mockRegistrationDescriptionFactory.VerifyAll();
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<RegistrationResult.BadGateway>();
        }

        [TestMethod]
        public async Task Register_WhenCreateRegistrationIdThrowsAException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            var registerDeviceRequest = _fixture.Create<RegisterDeviceRequest>();

            var registrationDescription = _fixture.Build<AppleRegistrationDescriptionTestWrapper>()
                .With(x => x.DeviceToken, registerDeviceRequest.DevicePns)
                .Without(x => x.ExtensionData)
                .Create();

            _mockRegistrationDescriptionFactory
                .Setup(x => x.Create(registerDeviceRequest, _accessToken.Subject))
                .Returns(registrationDescription);

            _mockAzureNotificationsHubClient
                .Setup(x => x.CreateRegistrationId())
                .Throws<Exception>();

            // Act
            var result = await _systemUnderTest.Register(registerDeviceRequest, _accessToken);

            // Assert
            _mockRegistrationDescriptionFactory.VerifyAll();
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<RegistrationResult.InternalServerError>();
        }

        [TestMethod]
        public async Task Register_WhenCreateOrUpdateRegistrationThrowsAHttpException_ReturnsBadGatewayResult()
        {
            // Arrange
            var registrationId = _fixture.Create<string>();
            var registerDeviceRequest = _fixture.Create<RegisterDeviceRequest>();

            var registrationDescription = _fixture.Build<AppleRegistrationDescriptionTestWrapper>()
                .With(x => x.DeviceToken, registerDeviceRequest.DevicePns)
                .Without(x => x.ExtensionData)
                .Create();

            _mockRegistrationDescriptionFactory
                .Setup(x => x.Create(registerDeviceRequest, _accessToken.Subject))
                .Returns(registrationDescription);

            _mockAzureNotificationsHubClient
                .Setup(x => x.CreateRegistrationId())
                .ReturnsAsync(registrationId);

            _mockAzureNotificationsHubClient
                .Setup(x => x.CreateOrUpdateRegistration(registrationDescription))
                .Throws<HttpRequestException>();

            // Act
            var result = await _systemUnderTest.Register(registerDeviceRequest, _accessToken);

            // Assert
            _mockRegistrationDescriptionFactory.VerifyAll();
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<RegistrationResult.BadGateway>();
        }

        [TestMethod]
        public async Task
            Register_WhenCreateOrUpdateRegistrationThrowsAnException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            var registrationId = _fixture.Create<string>();
            var registerDeviceRequest = _fixture.Create<RegisterDeviceRequest>();

            var registrationDescription = _fixture.Build<AppleRegistrationDescriptionTestWrapper>()
                .With(x => x.DeviceToken, registerDeviceRequest.DevicePns)
                .Without(x => x.ExtensionData)
                .Create();

            _mockRegistrationDescriptionFactory
                .Setup(x => x.Create(registerDeviceRequest, _accessToken.Subject))
                .Returns(registrationDescription);

            _mockAzureNotificationsHubClient
                .Setup(x => x.CreateRegistrationId())
                .ReturnsAsync(registrationId);

            _mockAzureNotificationsHubClient
                .Setup(x => x.CreateOrUpdateRegistration(registrationDescription))
                .Throws<Exception>();

            // Act
            var result = await _systemUnderTest.Register(registerDeviceRequest, _accessToken);

            // Assert
            _mockRegistrationDescriptionFactory.VerifyAll();
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
            var registrationId = _fixture.Create<string>();
            var registerDeviceRequest = _fixture.Create<RegisterDeviceRequest>();

            var registrationDescription = _fixture.Build<AppleRegistrationDescriptionTestWrapper>()
                .With(x => x.DeviceToken, registerDeviceRequest.DevicePns)
                .Without(x => x.ExtensionData)
                .Create();

            _mockRegistrationDescriptionFactory
                .Setup(x => x.Create(registerDeviceRequest, _accessToken.Subject))
                .Returns(registrationDescription);

            _mockAzureNotificationsHubClient
                .Setup(x => x.CreateRegistrationId())
                .ReturnsAsync(registrationId);

            _mockAzureNotificationsHubClient
                .Setup(x => x.CreateOrUpdateRegistration(registrationDescription))
                .Throws(
                    new MessagingException("Message exception",
                        new WebException("Web exception",
                            null,
                            webExceptionStatus,
                            HttpWebResponseHelper.CreateFromStatusCode(statusCode))));

            // Act
            var result = await _systemUnderTest.Register(registerDeviceRequest, _accessToken);

            // Assert
            _mockRegistrationDescriptionFactory.VerifyAll();
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<RegistrationResult.BadGateway>();
        }
    }
}