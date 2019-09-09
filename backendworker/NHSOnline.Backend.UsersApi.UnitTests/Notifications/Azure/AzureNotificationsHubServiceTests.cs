using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.NotificationHubs.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.UsersApi.Notifications;
using NHSOnline.Backend.UsersApi.Notifications.Azure;
using NHSOnline.Backend.UsersApi.Notifications.Azure.Steps;
using RegistrationResult = NHSOnline.Backend.UsersApi.Notifications.RegistrationResult;

namespace NHSOnline.Backend.UsersApi.UnitTests.Notifications.Azure
{
    [TestClass]
    public class AzureNotificationsHubServiceTests
    {
        private Fixture _fixture;
        private Mock<IRegistrationDescriptionFactory> _mockRegistrationDescriptionFactory;
        private Mock<IAzureNotificationHubClient> _mockAzureNotificationsHubClient;
        private AzureNotificationHubService _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());

            _mockRegistrationDescriptionFactory = _fixture.Freeze<Mock<IRegistrationDescriptionFactory>>();
            _mockAzureNotificationsHubClient = _fixture.Freeze<Mock<IAzureNotificationHubClient>>();
            
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
            
            var expectedResponse = new RegistrationResult.Success(
                new NotificationRegistrationResult
                {
                    RegistrationExpiry = registrationExpiryTime,
                    RegistrationId = registrationId
                });
            
            var request = _fixture.Create<NotificationRegistrationRequest>();
            
            var registrationDescription = _fixture.Build<AppleRegistrationDescriptionTestWrapper>()
                .With(x => x.DeviceToken, request.DevicePns)
                .Without( x => x.ExtensionData)
                .Create();

            var registrationCreatedUpdatedResponse = _fixture.Build<AppleRegistrationDescriptionTestWrapper>()
                .With(x => x.DeviceToken, request.DevicePns)
                .With( x => x.Tags, registrationDescription.Tags)
                .With(x => x.RegistrationId, registrationId)
                .With( x => x.ExpirationTime, registrationExpiryTime)
                .Without( x => x.ExtensionData)
                .Create();

            _mockRegistrationDescriptionFactory
                .Setup(x => x.Create(request))
                .Returns(registrationDescription)
                .Verifiable();

            _mockAzureNotificationsHubClient
                .Setup(x => x.GetRegistrationsByChannelAsync(request.DevicePns))
                .ReturnsAsync(new List<RegistrationDescription>())
                .Verifiable();

            _mockAzureNotificationsHubClient
                .Setup(x => x.CreateRegistrationIdAsync())
                .ReturnsAsync(registrationId)
                .Verifiable();

            _mockAzureNotificationsHubClient
                .Setup(x => x.CreateOrUpdateRegistrationAsync(registrationDescription))
                .ReturnsAsync(registrationCreatedUpdatedResponse)
                .Verifiable();
            
            // Act
            var result = await _systemUnderTest.Register(request);
            
            // Assert
            _mockRegistrationDescriptionFactory.Verify();
            _mockAzureNotificationsHubClient.Verify();
            
            result.Should().BeEquivalentTo(expectedResponse);
        }
        
        [TestMethod]
        public async Task Register_WhenHasExistingRegistrations_ReturnsSuccess()
        {
            // Arrange
            var registrationId = _fixture.Create<string>();
            var registrationExpiryTime = _fixture.Create<DateTime>();
            
            var expectedResponse = new RegistrationResult.Success(
                new NotificationRegistrationResult
                {
                    RegistrationExpiry = registrationExpiryTime,
                    RegistrationId = registrationId
                });
            
            var request = _fixture.Create<NotificationRegistrationRequest>();
            
            var existingRegistrations = _fixture.Build<AppleRegistrationDescriptionTestWrapper>()
                .Without( x => x.ExtensionData)
                .CreateMany(3);
            
            var registrationDescription = _fixture.Build<AppleRegistrationDescriptionTestWrapper>()
                .With(x => x.DeviceToken, request.DevicePns)
                .Without( x => x.ExtensionData)
                .Create();

            var registrationCreatedUpdatedResponse = _fixture.Build<AppleRegistrationDescriptionTestWrapper>()
                .With(x => x.DeviceToken, request.DevicePns)
                .With( x => x.Tags, registrationDescription.Tags)
                .With(x => x.RegistrationId, registrationId)
                .With( x => x.ExpirationTime, registrationExpiryTime)
                .Without( x => x.ExtensionData)
                .Create();

            _mockRegistrationDescriptionFactory
                .Setup(x => x.Create(request))
                .Returns(registrationDescription)
                .Verifiable();

            _mockAzureNotificationsHubClient
                .Setup(x => x.GetRegistrationsByChannelAsync(request.DevicePns))
                .ReturnsAsync(existingRegistrations)
                .Verifiable();

            _mockAzureNotificationsHubClient
                .Setup(x => x.DeleteRegistrationAsync(It.IsAny<AppleRegistrationDescriptionTestWrapper>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _mockAzureNotificationsHubClient
                .Setup(x => x.CreateRegistrationIdAsync())
                .ReturnsAsync(registrationId)
                .Verifiable();

            _mockAzureNotificationsHubClient
                .Setup(x => x.CreateOrUpdateRegistrationAsync(registrationDescription))
                .ReturnsAsync(registrationCreatedUpdatedResponse)
                .Verifiable();
            
            // Act
            var result = await _systemUnderTest.Register(request);
            
            // Assert
            _mockRegistrationDescriptionFactory.Verify();
            _mockAzureNotificationsHubClient.Verify();
            
            result.Should().BeEquivalentTo(expectedResponse);
        }
        
        [TestMethod]
        public async Task Register_WhenCreateOrUpdateRegistrationAsyncReturnsNoData_ReturnsInternalServerError()
        {
            // Arrange
            var registrationId = _fixture.Create<string>();
            
            var request = _fixture.Create<NotificationRegistrationRequest>();
            
            var existingRegistrations = _fixture.Build<AppleRegistrationDescriptionTestWrapper>()
                .Without( x => x.ExtensionData)
                .CreateMany(3);
            
            var registrationDescription = _fixture.Build<AppleRegistrationDescriptionTestWrapper>()
                .With(x => x.DeviceToken, request.DevicePns)
                .Without( x => x.ExtensionData)
                .Create();

            var registrationCreatedUpdatedResponse = _fixture.Build<AppleRegistrationDescriptionTestWrapper>()
                .With(x => x.DeviceToken, request.DevicePns)
                .With( x => x.Tags, registrationDescription.Tags)
                .Without(x => x.RegistrationId)
                .Without( x => x.ExpirationTime)
                .Without( x => x.ExtensionData)
                .Create();

            _mockRegistrationDescriptionFactory
                .Setup(x => x.Create(request))
                .Returns(registrationDescription)
                .Verifiable();

            _mockAzureNotificationsHubClient
                .Setup(x => x.GetRegistrationsByChannelAsync(request.DevicePns))
                .ReturnsAsync(existingRegistrations)
                .Verifiable();

            _mockAzureNotificationsHubClient
                .Setup(x => x.DeleteRegistrationAsync(It.IsAny<AppleRegistrationDescriptionTestWrapper>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _mockAzureNotificationsHubClient
                .Setup(x => x.CreateRegistrationIdAsync())
                .ReturnsAsync(registrationId)
                .Verifiable();

            _mockAzureNotificationsHubClient
                .Setup(x => x.CreateOrUpdateRegistrationAsync(registrationDescription))
                .ReturnsAsync(registrationCreatedUpdatedResponse)
                .Verifiable();
            
            // Act
            var result = await _systemUnderTest.Register(request);
            
            // Assert
            _mockRegistrationDescriptionFactory.Verify();
            _mockAzureNotificationsHubClient.Verify();

            result.Should().BeOfType<RegistrationResult.InternalServerError>();
        }

        [TestMethod]
        public void Register_WhenRequestFactoryThrowsArgumentException_ThrowsArgumentException()
        {
            // Arrange
            var request = _fixture.Build<NotificationRegistrationRequest>()
                .Without(x => x.NhsLoginId)
                .Create();
            
            _mockRegistrationDescriptionFactory
                .Setup(x => x.Create(request))
                .Throws<ArgumentException>();
            
            // Act
            Func<Task> act = () => _systemUnderTest.Register(request);
            
            // Assert
            act.Should().Throw<ArgumentException>();
        }
        
        [TestMethod]
        public async Task Register_WhenGetRegistrationsByChannelAsyncThrowsAHttpException_ReturnsBadGatewayResult()
        {
            // Arrange
            var request = _fixture.Build<NotificationRegistrationRequest>()
                .Create();
            
            var registrationDescription = _fixture.Build<AppleRegistrationDescriptionTestWrapper>()
                .With(x => x.DeviceToken, request.DevicePns)
                .Without( x => x.ExtensionData)
                .Create();
            
            _mockRegistrationDescriptionFactory
                .Setup(x => x.Create(request))
                .Returns(registrationDescription)
                .Verifiable();
            
            _mockAzureNotificationsHubClient
                .Setup(x => x.GetRegistrationsByChannelAsync(request.DevicePns))
                .Throws<HttpRequestException>();
            
            // Act
            var result = await _systemUnderTest.Register(request);
            
            // Assert
            _mockRegistrationDescriptionFactory.Verify();
            result.Should().BeOfType<RegistrationResult.BadGateway>();
        }
        
        [TestMethod]
        public async Task Register_WhenGetRegistrationsByChannelAsyncThrowsAException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            var request = _fixture.Build<NotificationRegistrationRequest>()
                .Create();
            
            var registrationDescription = _fixture.Build<AppleRegistrationDescriptionTestWrapper>()
                .With(x => x.DeviceToken, request.DevicePns)
                .Without( x => x.ExtensionData)
                .Create();
            
            _mockRegistrationDescriptionFactory
                .Setup(x => x.Create(request))
                .Returns(registrationDescription)
                .Verifiable();
            
            _mockAzureNotificationsHubClient
                .Setup(x => x.GetRegistrationsByChannelAsync(request.DevicePns))
                .Throws<Exception>();
            
            // Act
            var result = await _systemUnderTest.Register(request);
            
            // Assert
            _mockRegistrationDescriptionFactory.Verify();
            result.Should().BeOfType<RegistrationResult.InternalServerError>();
        }
        
        [TestMethod]
        public async Task Register_WhenDeleteRegistrationAsyncThrowsAHttpException_ReturnsBadGatewayResult()
        {
            // Arrange
            var request = _fixture.Build<NotificationRegistrationRequest>().Create();
            
            var registrationDescription = _fixture.Build<AppleRegistrationDescriptionTestWrapper>()
                .With(x => x.DeviceToken, request.DevicePns)
                .Without( x => x.ExtensionData)
                .Create();
            
            var existingRegistrations = _fixture.Build<AppleRegistrationDescriptionTestWrapper>()
                .Without( x => x.ExtensionData)
                .CreateMany(3);
            
            _mockRegistrationDescriptionFactory
                .Setup(x => x.Create(request))
                .Returns(registrationDescription)
                .Verifiable();
            
            _mockAzureNotificationsHubClient
                .Setup(x => x.GetRegistrationsByChannelAsync(request.DevicePns))
                .ReturnsAsync(existingRegistrations)
                .Verifiable();

            _mockAzureNotificationsHubClient
                .Setup(x => x.DeleteRegistrationAsync(It.IsAny<AppleRegistrationDescriptionTestWrapper>()))
                .Throws<HttpRequestException>()
                .Verifiable();
            
            // Act
            var result = await _systemUnderTest.Register(request);
            
            // Assert
            _mockRegistrationDescriptionFactory.Verify();
            _mockAzureNotificationsHubClient.Verify();
            
            result.Should().BeOfType<RegistrationResult.BadGateway>();
        }
        
        [TestMethod]
        public async Task Register_WhenDeleteRegistrationAsyncThrowsAException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            var request = _fixture.Build<NotificationRegistrationRequest>().Create();
            
            var registrationDescription = _fixture.Build<AppleRegistrationDescriptionTestWrapper>()
                .With(x => x.DeviceToken, request.DevicePns)
                .Without( x => x.ExtensionData)
                .Create();
            
            var existingRegistrations = _fixture.Build<AppleRegistrationDescriptionTestWrapper>()
                .Without( x => x.ExtensionData)
                .CreateMany(3);
            
            _mockRegistrationDescriptionFactory
                .Setup(x => x.Create(request))
                .Returns(registrationDescription)
                .Verifiable();
            
            _mockAzureNotificationsHubClient
                .Setup(x => x.GetRegistrationsByChannelAsync(request.DevicePns))
                .ReturnsAsync(existingRegistrations)
                .Verifiable();

            _mockAzureNotificationsHubClient
                .Setup(x => x.DeleteRegistrationAsync(It.IsAny<AppleRegistrationDescriptionTestWrapper>()))
                .Throws<Exception>()
                .Verifiable();
            
            // Act
            var result = await _systemUnderTest.Register(request);
            
            // Assert
            _mockRegistrationDescriptionFactory.Verify();
            _mockAzureNotificationsHubClient.Verify();
            
            result.Should().BeOfType<RegistrationResult.InternalServerError>();
        }
        
        [TestMethod]
        public async Task Register_WhenCreateRegistrationIdThrowsAHttpException_ReturnsBadGatewayResult()
        {
            // Arrange
            var request = _fixture.Build<NotificationRegistrationRequest>().Create();
            
            var registrationDescription = _fixture.Build<AppleRegistrationDescriptionTestWrapper>()
                .With(x => x.DeviceToken, request.DevicePns)
                .Without( x => x.ExtensionData)
                .Create();
            
            _mockRegistrationDescriptionFactory
                .Setup(x => x.Create(request))
                .Returns(registrationDescription)
                .Verifiable();
            
            _mockAzureNotificationsHubClient
                .Setup(x => x.GetRegistrationsByChannelAsync(request.DevicePns))
                .ReturnsAsync(new List<RegistrationDescription>())
                .Verifiable();

            _mockAzureNotificationsHubClient
                .Setup(x => x.CreateRegistrationIdAsync())
                .Throws<HttpRequestException>();
            
            // Act
            var result = await _systemUnderTest.Register(request);
            
            // Assert
            _mockRegistrationDescriptionFactory.Verify();
            _mockAzureNotificationsHubClient.Verify();
            
            result.Should().BeOfType<RegistrationResult.BadGateway>();
        }
        
        [TestMethod]
        public async Task Register_WhenCreateRegistrationIdThrowsAException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            var request = _fixture.Build<NotificationRegistrationRequest>().Create();
            
            var registrationDescription = _fixture.Build<AppleRegistrationDescriptionTestWrapper>()
                .With(x => x.DeviceToken, request.DevicePns)
                .Without( x => x.ExtensionData)
                .Create();
            
            _mockRegistrationDescriptionFactory
                .Setup(x => x.Create(request))
                .Returns(registrationDescription)
                .Verifiable();
            
            _mockAzureNotificationsHubClient
                .Setup(x => x.GetRegistrationsByChannelAsync(request.DevicePns))
                .ReturnsAsync(new List<RegistrationDescription>())
                .Verifiable();

            _mockAzureNotificationsHubClient
                .Setup(x => x.CreateRegistrationIdAsync())
                .Throws<Exception>();
            
            // Act
            var result = await _systemUnderTest.Register(request);
            
            // Assert
            _mockRegistrationDescriptionFactory.Verify();
            _mockAzureNotificationsHubClient.Verify();
            
            result.Should().BeOfType<RegistrationResult.InternalServerError>();
        }
        
        [TestMethod]
        public async Task Register_WhenCreateOrUpdateRegistrationAsyncThrowsAHttpException_ReturnsBadGatewayResult()
        {
            // Arrange
            var registrationId = _fixture.Create<string>();
            
            var request = _fixture.Build<NotificationRegistrationRequest>().Create();
            
            var registrationDescription = _fixture.Build<AppleRegistrationDescriptionTestWrapper>()
                .With(x => x.DeviceToken, request.DevicePns)
                .Without( x => x.ExtensionData)
                .Create();
            
            _mockRegistrationDescriptionFactory
                .Setup(x => x.Create(request))
                .Returns(registrationDescription)
                .Verifiable();
            
            _mockAzureNotificationsHubClient
                .Setup(x => x.GetRegistrationsByChannelAsync(request.DevicePns))
                .ReturnsAsync(new List<RegistrationDescription>())
                .Verifiable();

            _mockAzureNotificationsHubClient
                .Setup(x => x.CreateRegistrationIdAsync())
                .ReturnsAsync(registrationId)
                .Verifiable();

            _mockAzureNotificationsHubClient
                .Setup(x => x.CreateOrUpdateRegistrationAsync(registrationDescription))
                .Throws<HttpRequestException>()
                .Verifiable();
            
            // Act
            var result = await _systemUnderTest.Register(request);
            
            // Assert
            _mockRegistrationDescriptionFactory.Verify();
            _mockAzureNotificationsHubClient.Verify();
            
            result.Should().BeOfType<RegistrationResult.BadGateway>();
        }
        
        [TestMethod]
        public async Task Register_WhenCreateOrUpdateRegistrationAsyncThrowsAnException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            var registrationId = _fixture.Create<string>();
            
            var request = _fixture.Build<NotificationRegistrationRequest>().Create();
            
            var registrationDescription = _fixture.Build<AppleRegistrationDescriptionTestWrapper>()
                .With(x => x.DeviceToken, request.DevicePns)
                .Without( x => x.ExtensionData)
                .Create();
            
            _mockRegistrationDescriptionFactory
                .Setup(x => x.Create(request))
                .Returns(registrationDescription)
                .Verifiable();
            
            _mockAzureNotificationsHubClient
                .Setup(x => x.GetRegistrationsByChannelAsync(request.DevicePns))
                .ReturnsAsync(new List<RegistrationDescription>())
                .Verifiable();

            _mockAzureNotificationsHubClient
                .Setup(x => x.CreateRegistrationIdAsync())
                .ReturnsAsync(registrationId)
                .Verifiable();

            _mockAzureNotificationsHubClient
                .Setup(x => x.CreateOrUpdateRegistrationAsync(registrationDescription))
                .Throws<Exception>()
                .Verifiable();
            
            // Act
            var result = await _systemUnderTest.Register(request);
            
            // Assert
            _mockRegistrationDescriptionFactory.Verify();
            _mockAzureNotificationsHubClient.Verify();
            
            result.Should().BeOfType<RegistrationResult.InternalServerError>();
        }
        
        [DataTestMethod]
        [DataRow(WebExceptionStatus.ProtocolError, HttpStatusCode.Gone)]
        [DataRow(WebExceptionStatus.ProtocolError, HttpStatusCode.Moved)]
        [DataRow(WebExceptionStatus.SendFailure, HttpStatusCode.Gone)]
        [DataRow(WebExceptionStatus.SendFailure, HttpStatusCode.Moved)]
        public async Task Register_WhenCreateOrUpdateRegistrationAsyncThrowsAMessageException_ReturnsBadGatewayResult(
            WebExceptionStatus webExceptionStatus, HttpStatusCode statusCode)
        {
            // Arrange
            var registrationId = _fixture.Create<string>();
            
            var request = _fixture.Build<NotificationRegistrationRequest>().Create();
            
            var registrationDescription = _fixture.Build<AppleRegistrationDescriptionTestWrapper>()
                .With(x => x.DeviceToken, request.DevicePns)
                .Without( x => x.ExtensionData)
                .Create();
            
            _mockRegistrationDescriptionFactory
                .Setup(x => x.Create(request))
                .Returns(registrationDescription)
                .Verifiable();
            
            _mockAzureNotificationsHubClient
                .Setup(x => x.GetRegistrationsByChannelAsync(request.DevicePns))
                .ReturnsAsync(new List<RegistrationDescription>())
                .Verifiable();

            _mockAzureNotificationsHubClient
                .Setup(x => x.CreateRegistrationIdAsync())
                .ReturnsAsync(registrationId)
                .Verifiable();

            _mockAzureNotificationsHubClient
                .Setup(x => x.CreateOrUpdateRegistrationAsync(registrationDescription))
                .Throws(
                    new MessagingException("Message exception", 
                        new WebException("Web exception",
                            null, 
                            webExceptionStatus, 
                            MockHttpWebResponseWithStatusCode(statusCode))))
                .Verifiable();
            
            // Act
            var result = await _systemUnderTest.Register(request);
            
            // Assert
            _mockRegistrationDescriptionFactory.Verify();
            _mockAzureNotificationsHubClient.Verify();
            
            result.Should().BeOfType<RegistrationResult.BadGateway>();
        }

        private static HttpWebResponse MockHttpWebResponseWithStatusCode(HttpStatusCode statusCode)
        {
            var response = new HttpWebResponse();
            var message = new HttpResponseMessage(statusCode);

            var fieldStatusCode = response.GetType().GetField("_httpResponseMessage", 
                BindingFlags.Public | 
                BindingFlags.NonPublic | 
                BindingFlags.Instance);
            fieldStatusCode.SetValue(response, message);

            return response;
        }
    }
}