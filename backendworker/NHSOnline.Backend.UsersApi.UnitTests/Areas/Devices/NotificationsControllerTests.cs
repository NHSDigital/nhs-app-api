using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.UsersApi.Areas.Devices;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Notifications;

namespace NHSOnline.Backend.UsersApi.UnitTests.Areas.Devices
{
    [TestClass]
    public sealed class NotificationsControllerTests : IDisposable
    {
        private NotificationsController _systemUnderTest;
        private Mock<INotificationService> _mockNotificationService;
        private const string NhsLoginId = "NhsLoginId";

        [TestInitialize]
        public void TestInitialize()
        {
            _mockNotificationService = new Mock<INotificationService>(MockBehavior.Strict);

            _systemUnderTest = new NotificationsController(_mockNotificationService.Object,
                new Mock<ILogger<NotificationsController>>().Object);
        }

        [TestMethod]
        public async Task Post_ValidRequest_Returns202Accepted()
        {
            // Arrange
            var sendRequest = new NotificationSendRequest
            {
                Title = "title",
                Subtitle = "subtitle",
                Body = "body",
                Url = "http://www.example.com"
            };

            _mockNotificationService
                .Setup(x => x.Send(NhsLoginId, sendRequest))
                .ReturnsAsync(new NotificationSendResult.Success());

            // Act
            var result = await _systemUnderTest.Post(NhsLoginId, sendRequest);

            // Assert
            _mockNotificationService.VerifyAll();

            result.Should().BeOfType<AcceptedResult>();
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task Post_NoTitleRequest_Returns202Accepted(string title)
        {
            // Arrange
            var sendRequest = new NotificationSendRequest
            {
                Title = title,
                Subtitle = "subtitle",
                Body = "body",
                Url = "http://www.example.com"
            };

            _mockNotificationService
                .Setup(x => x.Send(NhsLoginId, sendRequest))
                .ReturnsAsync(new NotificationSendResult.Success());

            // Act
            var result = await _systemUnderTest.Post(NhsLoginId, sendRequest);

            // Assert
            _mockNotificationService.VerifyAll();

            result.Should().BeOfType<AcceptedResult>();
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task Post_NoSubtitleRequest_Returns202Accepted(string subtitle)
        {
            // Arrange
            var sendRequest = new NotificationSendRequest
            {
                Title = "title",
                Subtitle = subtitle,
                Body = "body",
                Url = "http://www.example.com"
            };

            _mockNotificationService
                .Setup(x => x.Send(NhsLoginId, sendRequest))
                .ReturnsAsync(new NotificationSendResult.Success());

            // Act
            var result = await _systemUnderTest.Post(NhsLoginId, sendRequest);

            // Assert
            _mockNotificationService.VerifyAll();

            result.Should().BeOfType<AcceptedResult>();
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task Post_NoUrlRequest_Returns202Accepted(string url)
        {
            // Arrange
            var sendRequest = new NotificationSendRequest
            {
                Title = "title",
                Subtitle = "subtitle",
                Body = "body",
                Url = url
            };

            _mockNotificationService
                .Setup(x => x.Send(NhsLoginId, sendRequest))
                .ReturnsAsync(new NotificationSendResult.Success());

            // Act
            var result = await _systemUnderTest.Post(NhsLoginId, sendRequest);

            // Assert
            _mockNotificationService.VerifyAll();

            result.Should().BeOfType<AcceptedResult>();
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task Post_NoBodyRequest_Returns400BadRequest(string body)
        {
            // Arrange
            var sendRequest = new NotificationSendRequest
            {
                Title = "title",
                Subtitle = "subtitle",
                Body = body,
                Url = "http://www.example.com"
            };

            // Act
            var result = await _systemUnderTest.Post(NhsLoginId, sendRequest);

            // Assert
            _mockNotificationService.VerifyAll();

            result.Should().BeOfType<BadRequestResult>();
        }

        [TestMethod]
        public async Task Post_InvalidUrlRequest_Returns400BadRequest()
        {
            // Arrange
            var sendRequest = new NotificationSendRequest
            {
                Title = "title",
                Subtitle = "subtitle",
                Body = "body",
                Url = "this is not the url you are looking for"
            };

            // Act
            var result = await _systemUnderTest.Post(NhsLoginId, sendRequest);

            // Assert
            _mockNotificationService.VerifyAll();

            result.Should().BeOfType<BadRequestResult>();
        }

        [TestMethod]
        public async Task Post_NotificationServiceReturnBadGateway_Returns502BadGateway()
        {
            // Arrange
            var sendRequest = new NotificationSendRequest
            {
                Title = "title",
                Subtitle = "subtitle",
                Body = "body",
                Url = ""
            };
            _mockNotificationService
                .Setup(x => x.Send(NhsLoginId, sendRequest))
                .ReturnsAsync(new NotificationSendResult.BadGateway());

            // Act
            var result = await _systemUnderTest.Post(NhsLoginId, sendRequest);

            // Assert
            _mockNotificationService.VerifyAll();

            var subject = result.Should().BeOfType<StatusCodeResult>().Subject;
            subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task Post_NotificationServiceReturnInternalServerError_Returns500InternalServerError()
        {
            // Arrange
            var sendRequest = new NotificationSendRequest
            {
                Title = "title",
                Subtitle = "subtitle",
                Body = "body",
                Url = ""
            };
            _mockNotificationService
                .Setup(x => x.Send(NhsLoginId, sendRequest))
                .ReturnsAsync(new NotificationSendResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.Post(NhsLoginId, sendRequest);

            // Assert
            _mockNotificationService.VerifyAll();

            var subject = result.Should().BeOfType<StatusCodeResult>().Subject;
            subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        public void Dispose()
        {
            _systemUnderTest?.Dispose();
        }
    }
}