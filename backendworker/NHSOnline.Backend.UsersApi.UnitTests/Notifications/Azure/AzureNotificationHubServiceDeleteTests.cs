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
using UnitTestHelper;

namespace NHSOnline.Backend.UsersApi.UnitTests.Notifications.Azure
{
    [TestClass]
    public class AzureNotificationHubServiceDeleteTests
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
        public async Task Delete_Success()
        {
            // Arrange
            var registrationId = _fixture.Create<string>();

            _mockAzureNotificationsHubClient.Setup(x => x.DeleteRegistration(registrationId))
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
                .Setup(x => x.DeleteRegistration(registrationId))
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
                .Setup(x => x.DeleteRegistration(registrationId))
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
                .Setup(x => x.DeleteRegistration(registrationId))
                .Throws<HttpRequestException>();

            // Act
            var result = await _systemUnderTest.Delete(registrationId);

            // Assert
            _mockAzureNotificationsHubClient.VerifyAll();

            result.Should().BeOfType<DeleteRegistrationResult.BadGateway>();
        }
    }
}