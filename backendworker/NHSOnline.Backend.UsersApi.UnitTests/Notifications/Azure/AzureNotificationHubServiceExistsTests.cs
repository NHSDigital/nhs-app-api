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
using NHSOnline.Backend.UsersApi.Notifications.Azure;
using NHSOnline.Backend.UsersApi.Notifications.Azure.Steps;
using NHSOnline.Backend.UsersApi.Repository;
using UnitTestHelper;

namespace NHSOnline.Backend.UsersApi.UnitTests.Notifications.Azure
{
    [TestClass]
    public class AzureNotificationHubServiceExistsTests
    {
        private Fixture _fixture;
        private Mock<IAzureNotificationHubClient> _mockAzureNotificationsHubClient;
        private AzureNotificationHubService _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());

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
        public async Task Exists_WhenRegistrationExists_ReturnsFoundResult()
        {
            // Arrange
            var userDevice = _fixture.Create<UserDevice>();

            _mockAzureNotificationsHubClient
                .Setup(x => x.RegistrationExists(userDevice.RegistrationId, userDevice.PnsToken))
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
                .Setup(x => x.RegistrationExists(userDevice.RegistrationId, userDevice.PnsToken))
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
                .Setup(x => x.RegistrationExists(userDevice.RegistrationId, userDevice.PnsToken))
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
                .Setup(x => x.RegistrationExists(userDevice.RegistrationId, userDevice.PnsToken))
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
                .Setup(x => x.RegistrationExists(userDevice.RegistrationId, userDevice.PnsToken))
                .Throws<HttpRequestException>();

            // Act
            var result = await _systemUnderTest.Exists(userDevice);

            // Assert
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<RegistrationExistsResult.BadGateway>();
        }
    }
}