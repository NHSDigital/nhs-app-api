using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Notifications;
using NotificationRequest = NHSOnline.Backend.UsersApi.Notifications.Models.NotificationRequest;

namespace NHSOnline.Backend.UsersApi.UnitTests.Notifications
{
    [TestClass]
    public class NotificationServiceTests
    {
        private INotificationService _systemUnderTest;
        private Mock<INotificationClient> _mockNotificationClient;
        private const string NhsLoginId = "NhsLoginId";

        [TestInitialize]
        public void TestInitialize()
        {
            _mockNotificationClient = new Mock<INotificationClient>(MockBehavior.Strict);

            _systemUnderTest = new NotificationService(
                new Mock<ILogger<NotificationService>>().Object,
                _mockNotificationClient.Object);
        }

        [TestMethod]
        public async Task Send_ReturnsSuccessResult()
        {
            // Arrange
            const string title = "title";
            const string subtitle = "subtitle";
            const string body = "body";
            const string url = "http://www.example.com";

            var request = new NotificationSendRequest
            {
                Title = title,
                Subtitle = subtitle,
                Body = body,
                Url = url,
            };

            _mockNotificationClient
                .Setup(x => x.SendNotification(
                    It.Is<NotificationRequest>(y =>
                        y.Title == title &&
                        y.Subtitle == subtitle &&
                        y.Body == body &&
                        y.Url == new Uri(url) &&
                        y.NhsLoginId == NhsLoginId)))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _systemUnderTest.Send(NhsLoginId, request);

            // Assert
            _mockNotificationClient.VerifyAll();

            result.Should().BeOfType<NotificationSendResult.Success>();
        }

        [TestMethod]
        public async Task Send_RequestTitleIsNull_ReturnsSuccessResult()
        {
            // Arrange
            const string subtitle = "subtitle";
            const string body = "body";
            const string url = "http://www.example.com";

            var request = new NotificationSendRequest
            {
                Title = null,
                Subtitle = subtitle,
                Body = body,
                Url = url
            };

            _mockNotificationClient
                .Setup(x => x.SendNotification(
                    It.Is<NotificationRequest>(y =>
                        y.Title == null &&
                        y.Subtitle == subtitle &&
                        y.Body == body &&
                        y.Url == new Uri(url) &&
                        y.NhsLoginId == NhsLoginId)))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _systemUnderTest.Send(NhsLoginId, request);

            // Assert
            _mockNotificationClient.VerifyAll();

            result.Should().BeOfType<NotificationSendResult.Success>();
        }

        [TestMethod]
        public async Task Send_RequestSubtitleIsNull_ReturnsSuccessResult()
        {
            // Arrange
            const string title = "title";
            const string body = "body";
            const string url = "http://www.example.com";

            var request = new NotificationSendRequest
            {
                Title = title,
                Subtitle = null,
                Body = body,
                Url = url
            };

            _mockNotificationClient
                .Setup(x => x.SendNotification(
                    It.Is<NotificationRequest>(y =>
                        y.Title == title &&
                        y.Subtitle == null &&
                        y.Body == body &&
                        y.Url == new Uri(url) &&
                        y.NhsLoginId == NhsLoginId)))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _systemUnderTest.Send(NhsLoginId, request);

            // Assert
            _mockNotificationClient.VerifyAll();

            result.Should().BeOfType<NotificationSendResult.Success>();
        }

        [TestMethod]
        [DataRow(null, DisplayName = "Send_WhenUrlIsNull_MapsToNull")]
        [DataRow("", DisplayName = "Send_WhenUrlIsEmpty_MapsToNull")]
        [DataRow("     ", DisplayName = "Send_WhenUrlIsWhiteSpace_MapsToNull")]
        public async Task Send_WhenUrlIsNotSupplied_MapsToNull(string url)
        {
            // Arrange
            const string title = "title";
            const string subtitle = "subtitle";
            const string body = "body";

            var request = new NotificationSendRequest
            {
                Title = title,
                Subtitle = subtitle,
                Body = body,
                Url = url
            };

            _mockNotificationClient
                .Setup(x => x.SendNotification(
                    It.Is<NotificationRequest>(y =>
                        y.Title == title &&
                        y.Subtitle == subtitle &&
                        y.Body == body &&
                        y.Url == null &&
                        y.NhsLoginId == NhsLoginId)))
                .Returns(Task.CompletedTask);

            // Act
            await _systemUnderTest.Send(NhsLoginId, request);

            // Assert
            _mockNotificationClient.VerifyAll();
        }

        [TestMethod]
        public async Task Send_WhenRequestPropertiesContainWhiteSpace_WhiteSpaceIsTrimmed()
        {
            // Arrange
            var request = new NotificationSendRequest
            {
                Title = "   title  ",
                Subtitle = "   subtitle  ",
                Body = "   body  ",
                Url = "   http://www.example.com  "
            };

            _mockNotificationClient
                .Setup(x => x.SendNotification(
                    It.Is<NotificationRequest>(y =>
                        y.Title == "title" &&
                        y.Subtitle == "subtitle" &&
                        y.Body == "body" &&
                        y.Url == new Uri("http://www.example.com") &&
                        y.NhsLoginId == NhsLoginId)))
                .Returns(Task.CompletedTask);

            // Act
            await _systemUnderTest.Send(NhsLoginId, request);

            // Assert
            _mockNotificationClient.VerifyAll();
        }

        [TestMethod]
        public async Task Send_NotificationServiceThrowsMessagingException_ReturnsBadGatewayResult()
        {
            // Arrange
            const string title = "title";
            const string subtitle = "subtitle";
            const string body = "body";
            const string url = "http://www.example.com";

            var request = new NotificationSendRequest
            {
                Title = title,
                Subtitle = subtitle,
                Body = body,
                Url = url
            };

            _mockNotificationClient
                .Setup(x => x.SendNotification(
                    It.Is<NotificationRequest>(y =>
                        y.Title == title &&
                        y.Subtitle == subtitle &&
                        y.Body == body &&
                        y.Url == new Uri(url) &&
                        y.NhsLoginId == NhsLoginId)))
                .ThrowsAsync(MessagingExceptionFactory.Create());

            // Act
            var result = await _systemUnderTest.Send(NhsLoginId, request);

            // Assert
            _mockNotificationClient.VerifyAll();

            result.Should().BeOfType<NotificationSendResult.BadGateway>();
        }

        [TestMethod]
        public async Task Send_NotificationServiceThrowsHttpRequestException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            const string title = "title";
            const string subtitle = "subtitle";
            const string body = "body";
            const string url = "http://www.example.com";

            var request = new NotificationSendRequest
            {
                Title = title,
                Subtitle = subtitle,
                Body = body,
                Url = url
            };

            _mockNotificationClient
                .Setup(x => x.SendNotification(
                    It.Is<NotificationRequest>(y =>
                        y.Title == title &&
                        y.Subtitle == subtitle &&
                        y.Body == body &&
                        y.Url == new Uri(url) &&
                        y.NhsLoginId == NhsLoginId)))
                .ThrowsAsync(new HttpRequestException("This is an exception"));

            // Act
            var result = await _systemUnderTest.Send(NhsLoginId, request);

            // Assert
            _mockNotificationClient.VerifyAll();

            result.Should().BeOfType<NotificationSendResult.BadGateway>();
        }

        [TestMethod]
        public async Task Send_NotificationServiceThrowsException_ReturnsBadGatewayResult()
        {
            // Arrange
            const string title = "title";
            const string subtitle = "subtitle";
            const string body = "body";
            const string url = "http://www.example.com";

            var request = new NotificationSendRequest
            {
                Title = title,
                Subtitle = subtitle,
                Body = body,
                Url = url
            };

            _mockNotificationClient
                .Setup(x => x.SendNotification(
                    It.Is<NotificationRequest>(y =>
                        y.Title == title &&
                        y.Subtitle == subtitle &&
                        y.Body == body &&
                        y.Url == new Uri(url) &&
                        y.NhsLoginId == NhsLoginId)))
                .ThrowsAsync(new AggregateException("This is an exception"));

            // Act
            var result = await _systemUnderTest.Send(NhsLoginId, request);

            // Assert
            _mockNotificationClient.VerifyAll();

            result.Should().BeOfType<NotificationSendResult.InternalServerError>();
        }
    }
}