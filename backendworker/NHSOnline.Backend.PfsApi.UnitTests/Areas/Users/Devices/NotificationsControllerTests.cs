using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Metrics.EventHub;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.PfsApi.Areas.Users.Devices;
using NHSOnline.Backend.Users.Areas.Devices.Models;
using NHSOnline.Backend.Users.Notifications;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Users.Devices
{
    [TestClass]
    public sealed class NotificationsControllerTests : IDisposable
    {
        private const string NhsLoginId = "NhsLoginId";
        private Mock<IEventHubLogger> _mockEventHubLogger;
        private Mock<INotificationService> _mockNotificationService;
        private NotificationsController _systemUnderTest;

        public void Dispose()
        {
            _systemUnderTest?.Dispose();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _mockNotificationService = new Mock<INotificationService>(MockBehavior.Strict);
            _mockEventHubLogger = new Mock<IEventHubLogger>(MockBehavior.Strict);

            _systemUnderTest = new NotificationsController(
                _mockNotificationService.Object,
                new Mock<ILogger<NotificationsController>>().Object,
                _mockEventHubLogger.Object,
                new Mock<IMapper<AddNotificationSenderContext, SenderContextEventLogData>>().Object
            );
        }

        [TestMethod]
        public async Task Post_SendNotification_ValidRequest_Returns202Accepted()
        {
            // Arrange
            var sendRequest = new NotificationSendRequest
            {
                Title = "title",
                Subtitle = "subtitle",
                Body = "body",
                Url = "https://www.example.com"
            };

            var notificationResponse = new NotificationSendResponse
            {
                Scheduled = false,
                NotificationId = "Notification ID",
                HubPath = "Hub Path"
            };

            _mockNotificationService
                .Setup(x => x.Send(NhsLoginId, sendRequest))
                .ReturnsAsync(new NotificationSendResult.Success(notificationResponse));

            _mockEventHubLogger
                .Setup(x => x.NotificationEnqueued(It.IsNotNull<NotificationEnqueuedEventLogData>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _systemUnderTest.Post(NhsLoginId, sendRequest);

            // Assert
            VerifyMocks();

            var acceptedResult = result.Should().BeOfType<AcceptedResult>();
            acceptedResult.Subject.Value.Should().BeEquivalentTo(notificationResponse);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task Post_SendNotification_NoTitleRequest_Returns202Accepted(string title)
        {
            // Arrange
            var sendRequest = new NotificationSendRequest
            {
                Title = title,
                Subtitle = "subtitle",
                Body = "body",
                Url = "https://www.example.com"
            };

            var notificationResponse = new NotificationSendResponse
            {
                Scheduled = true,
                NotificationId = "Notification ID",
                HubPath = "Hub Path"
            };

            _mockNotificationService
                .Setup(x => x.Send(NhsLoginId, sendRequest))
                .ReturnsAsync(new NotificationSendResult.Success(notificationResponse));

            _mockEventHubLogger
                .Setup(x => x.NotificationEnqueued(It.IsNotNull<NotificationEnqueuedEventLogData>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _systemUnderTest.Post(NhsLoginId, sendRequest);

            // Assert
            VerifyMocks();

            var acceptedResult = result.Should().BeOfType<AcceptedResult>();
            acceptedResult.Subject.Value.Should().BeEquivalentTo(notificationResponse);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task Post_SendNotification_NoSubtitleRequest_Returns202Accepted(string subtitle)
        {
            // Arrange
            var sendRequest = new NotificationSendRequest
            {
                Title = "title",
                Subtitle = subtitle,
                Body = "body",
                Url = "https://www.example.com"
            };

            var notificationResponse = new NotificationSendResponse
            {
                Scheduled = false,
                NotificationId = "Notification ID",
                HubPath = "Hub Path"
            };

            _mockNotificationService
                .Setup(x => x.Send(NhsLoginId, sendRequest))
                .ReturnsAsync(new NotificationSendResult.Success(notificationResponse));

            _mockEventHubLogger
                .Setup(x => x.NotificationEnqueued(It.IsNotNull<NotificationEnqueuedEventLogData>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _systemUnderTest.Post(NhsLoginId, sendRequest);

            // Assert
            VerifyMocks();

            var acceptedResult = result.Should().BeOfType<AcceptedResult>();
            acceptedResult.Subject.Value.Should().BeEquivalentTo(notificationResponse);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task Post_SendNotification_NoUrlRequest_Returns202Accepted(string url)
        {
            // Arrange
            var sendRequest = new NotificationSendRequest
            {
                Title = "title",
                Subtitle = "subtitle",
                Body = "body",
                Url = url
            };

            var notificationResponse = new NotificationSendResponse
            {
                Scheduled = false,
                NotificationId = "Notification ID",
                HubPath = "Hub Path"
            };

            _mockNotificationService
                .Setup(x => x.Send(NhsLoginId, sendRequest))
                .ReturnsAsync(new NotificationSendResult.Success(notificationResponse));

            _mockEventHubLogger
                .Setup(x => x.NotificationEnqueued(It.IsNotNull<NotificationEnqueuedEventLogData>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _systemUnderTest.Post(NhsLoginId, sendRequest);

            // Assert
            VerifyMocks();

            var acceptedResult = result.Should().BeOfType<AcceptedResult>();
            acceptedResult.Subject.Value.Should().BeEquivalentTo(notificationResponse);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task Post_SendNotification_NoBodyRequest_Returns400BadRequest(string body)
        {
            // Arrange
            var sendRequest = new NotificationSendRequest
            {
                Title = "title",
                Subtitle = "subtitle",
                Body = body,
                Url = "https://www.example.com"
            };

            // Act
            var result = await _systemUnderTest.Post(NhsLoginId, sendRequest);

            // Assert
            VerifyMocks();

            result.Should().BeOfType<BadRequestResult>();
        }

        [TestMethod]
        public async Task Post_SendNotification_InvalidUrlRequest_Returns400BadRequest()
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
            VerifyMocks();

            result.Should().BeOfType<BadRequestResult>();
        }

        [TestMethod]
        public async Task Post_SendNotification_NotificationServiceReturnConflict_Returns409Conflict()
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
                .ReturnsAsync(new NotificationSendResult.Conflict());

            // Act
            var result = await _systemUnderTest.Post(NhsLoginId, sendRequest);

            // Assert
            VerifyMocks();

            result.Should().BeOfType<ConflictResult>();
        }

        [TestMethod]
        public async Task Post_SendNotification_NotificationServiceReturnBadGateway_Returns502BadGateway()
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
            VerifyMocks();

            var subject = result.Should().BeOfType<StatusCodeResult>().Subject;
            subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task
            Post_SendNotification_NotificationServiceReturnInternalServerError_Returns500InternalServerError()
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
            VerifyMocks();

            var subject = result.Should().BeOfType<StatusCodeResult>().Subject;
            subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [DataTestMethod]
        [DataRow("", DisplayName = "Empty hub path")]
        [DataRow(null, DisplayName = "Null hub path")]
        public async Task Get_NotificationOutcome_InvalidHubPath_Returns400BadRequest(string hubPath)
        {
            // Act
            var result = await _systemUnderTest.GetNotificationOutcomeDetails("notificationId", hubPath);

            // Assert
            VerifyMocks();

            result.Should().BeOfType<BadRequestResult>();
        }

        [DataTestMethod]
        [DataRow("", DisplayName = "Empty notification id")]
        [DataRow(null, DisplayName = "Null notification id")]
        public async Task Get_NotificationOutcome_InvalidNotificationId_Returns400BadRequest(string notificationId)
        {
            // Act
            var result = await _systemUnderTest.GetNotificationOutcomeDetails(notificationId, "hubPath");

            // Assert
            VerifyMocks();

            result.Should().BeOfType<BadRequestResult>();
        }

        [TestMethod]
        public async Task
            Get_NotificationOutcome_NotificationService_ReturnInternalServerError_Returns500InternalServerError()
        {
            // Arrange
            _mockNotificationService
                .Setup(x => x.GetNotificationOutcomeDetails("notificationId", "hubPath"))
                .ReturnsAsync(new NotificationOutcomeResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.GetNotificationOutcomeDetails("notificationId", "hubPath");

            // Assert
            VerifyMocks();

            var subject = result.Should().BeOfType<StatusCodeResult>().Subject;
            subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Get_NotificationOutcome_NotificationService_ReturnBadGateway_Returns502BadGateway()
        {
            // Arrange
            _mockNotificationService
                .Setup(x => x.GetNotificationOutcomeDetails("notificationId", "hubPath"))
                .ReturnsAsync(new NotificationOutcomeResult.BadGateway());

            // Act
            var result = await _systemUnderTest.GetNotificationOutcomeDetails("notificationId", "hubPath");

            // Assert
            VerifyMocks();

            var subject = result.Should().BeOfType<StatusCodeResult>().Subject;
            subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task Get_NotificationOutcome_NotificationService_ReturnNotFound_Returns404NotFound()
        {
            // Arrange
            _mockNotificationService
                .Setup(x => x.GetNotificationOutcomeDetails("notificationId", "hubPath"))
                .ReturnsAsync(new NotificationOutcomeResult.NotFound());

            // Act
            var result = await _systemUnderTest.GetNotificationOutcomeDetails("notificationId", "hubPath");

            // Assert
            VerifyMocks();

            var subject = result.Should().BeOfType<StatusCodeResult>().Subject;
            subject.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [TestMethod]
        public async Task Get_NotificationOutcome_Success_Returns_Expected_Response()
        {
            // Arrange
            var expectedNotificationOutcomeResponse = new NotificationOutcomeResponse
            {
                State = "Completed",
                EndTime = DateTime.UtcNow.AddMinutes(2),
                EnqueueTime = DateTime.UtcNow.AddHours(-1),
                StartTime = DateTime.UtcNow.AddMinutes(1),
                PnsErrorDetailsUri = "ErrorDetailsUri",
                PlatformOutcomes = new List<PlatformOutcome>
                {
                    new PlatformOutcome { Count = 1, Outcome = "Success", Platform = "iOS" },
                    new PlatformOutcome { Count = 1, Outcome = "Skipped", Platform = "iOS" },
                    new PlatformOutcome { Count = 2, Outcome = "ExpiredChannel", Platform = "Android" },
                    new PlatformOutcome { Count = 2, Outcome = "BadChannel", Platform = "Android" }
                }
            };

            _mockNotificationService
                .Setup(x => x.GetNotificationOutcomeDetails("notificationId", "hubPath"))
                .ReturnsAsync(new NotificationOutcomeResult.Success(expectedNotificationOutcomeResponse));

            // Act
            var result = await _systemUnderTest.GetNotificationOutcomeDetails("notificationId", "hubPath");

            // Assert
            VerifyMocks();

            var value = result.Should().BeAssignableTo<OkObjectResult>().Subject;
            value.StatusCode.Should().Be(StatusCodes.Status200OK);
            value.Value.Should().BeEquivalentTo(expectedNotificationOutcomeResponse);
        }

        private void VerifyMocks()
        {
            _mockNotificationService.VerifyAll();
            _mockEventHubLogger.VerifyAll();
            _mockEventHubLogger.VerifyNoOtherCalls();
        }
    }
}