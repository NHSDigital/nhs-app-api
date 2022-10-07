using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Messages.Areas.Messages;
using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Messages.Repository;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Users.Areas.Devices.Models;
using NHSOnline.Backend.Users.Notifications;
using NHSOnline.Backend.Users.Notifications.Models;

namespace NHSOnline.Backend.Users.UnitTests.Notifications
{
    [TestClass]
    public class NotificationServiceTests
    {
        private INotificationService _systemUnderTest;
        private Mock<INotificationClient> _mockNotificationClient;
        private Mock<INotificationsConfiguration> _mockNotificationsConfiguration;
        private Mock<IMessageService> _mockMessageService;

        private const string NhsLoginId = "NhsLoginId";

        [TestInitialize]
        public void TestInitialize()
        {
            _mockNotificationClient = new Mock<INotificationClient>(MockBehavior.Strict);
            _mockNotificationsConfiguration = new Mock<INotificationsConfiguration>(MockBehavior.Strict);
            _mockMessageService = new Mock<IMessageService>(MockBehavior.Strict);

            _mockNotificationsConfiguration
                .SetupGet(c => c.IosBadgeCountEnabled)
                .Returns(false);
            _mockMessageService
                .Setup(r => r.GetUnreadMessageCount(NhsLoginId))
                .ReturnsAsync(new UnreadMessageCountResult.Success(5));

            _systemUnderTest = new NotificationService(
                new Mock<ILogger<NotificationService>>().Object,
                _mockNotificationClient.Object,
                _mockNotificationsConfiguration.Object,
                _mockMessageService.Object);
        }

        [TestMethod]
        public async Task Send_ReturnsSuccessResult()
        {
            // Arrange
            const string title = "title";
            const string subtitle = "subtitle";
            const string body = "body";
            const string url = "https://www.example.com";

            var request = new NotificationSendRequest
            {
                Title = title,
                Subtitle = subtitle,
                Body = body,
                Url = url,
            };

            var response = new NotificationSendResponse
            {
                Scheduled = false,
                NotificationId = "Notification ID",
                HubPath = "Hub Path"
            };

            _mockNotificationClient
                .Setup(x => x.SendNotification(
                    It.Is<NotificationRequest>(y =>
                        y.Title == title &&
                        y.Subtitle == subtitle &&
                        y.Body == body &&
                        y.Url == new Uri(url) &&
                        y.NhsLoginId == NhsLoginId)))
                .ReturnsAsync(response);

            // Act
            var result = await _systemUnderTest.Send(NhsLoginId, request);

            // Assert
            _mockNotificationClient.VerifyAll();

            var successResult = result.Should().BeOfType<NotificationSendResult.Success>().Subject;
            successResult.NotificationSendResponse.Should().BeEquivalentTo(response);
        }

        [TestMethod]
        public async Task Send_RequestTitleIsNull_ReturnsSuccessResult()
        {
            // Arrange
            const string subtitle = "subtitle";
            const string body = "body";
            const string url = "https://www.example.com";

            var request = new NotificationSendRequest
            {
                Title = null,
                Subtitle = subtitle,
                Body = body,
                Url = url
            };

            var response = new NotificationSendResponse
            {
                Scheduled = false,
                NotificationId = "Notification ID",
                HubPath = "Hub Path"
            };

            _mockNotificationClient
                .Setup(x => x.SendNotification(
                    It.Is<NotificationRequest>(y =>
                        y.Title == null &&
                        y.Subtitle == subtitle &&
                        y.Body == body &&
                        y.Url == new Uri(url) &&
                        y.NhsLoginId == NhsLoginId)))
                .ReturnsAsync(response);

            // Act
            var result = await _systemUnderTest.Send(NhsLoginId, request);

            // Assert
            _mockNotificationClient.VerifyAll();

            var successResult = result.Should().BeOfType<NotificationSendResult.Success>().Subject;
            successResult.NotificationSendResponse.Should().BeEquivalentTo(response);
        }

        [TestMethod]
        public async Task Send_RequestSubtitleIsNull_ReturnsSuccessResult()
        {
            // Arrange
            const string title = "title";
            const string body = "body";
            const string url = "https://www.example.com";

            var request = new NotificationSendRequest
            {
                Title = title,
                Subtitle = null,
                Body = body,
                Url = url
            };

            var response = new NotificationSendResponse
            {
                Scheduled = false,
                NotificationId = "Notification ID",
                HubPath = "Hub Path"
            };

            _mockNotificationClient
                .Setup(x => x.SendNotification(
                    It.Is<NotificationRequest>(y =>
                        y.Title == title &&
                        y.Subtitle == null &&
                        y.Body == body &&
                        y.Url == new Uri(url) &&
                        y.NhsLoginId == NhsLoginId)))
                .ReturnsAsync(response);

            // Act
            var result = await _systemUnderTest.Send(NhsLoginId, request);

            // Assert
            _mockNotificationClient.VerifyAll();

            result.Should().BeOfType<NotificationSendResult.Success>();

            var successResult = result.Should().BeOfType<NotificationSendResult.Success>().Subject;
            successResult.NotificationSendResponse.Should().BeEquivalentTo(response);
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

            var response = new NotificationSendResponse
            {
                Scheduled = false,
                NotificationId = "Notification ID",
                HubPath = "Hub Path"
            };

            _mockNotificationClient
                .Setup(x => x.SendNotification(
                    It.Is<NotificationRequest>(y =>
                        y.Title == title &&
                        y.Subtitle == subtitle &&
                        y.Body == body &&
                        y.Url == null &&
                        y.NhsLoginId == NhsLoginId)))
                .ReturnsAsync(response);

            // Act
            var result = await _systemUnderTest.Send(NhsLoginId, request);

            // Assert
            _mockNotificationClient.VerifyAll();

            var successResult = result.Should().BeOfType<NotificationSendResult.Success>().Subject;
            successResult.NotificationSendResponse.Should().BeEquivalentTo(response);
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
                Url = "   https://www.example.com  "
            };

            var response = new NotificationSendResponse
            {
                Scheduled = false,
                NotificationId = "Notification ID",
                HubPath = "Hub Path"
            };

            _mockNotificationClient
                .Setup(x => x.SendNotification(
                    It.Is<NotificationRequest>(y =>
                        y.Title == "title" &&
                        y.Subtitle == "subtitle" &&
                        y.Body == "body" &&
                        y.Url == new Uri("https://www.example.com") &&
                        y.NhsLoginId == NhsLoginId)))
                .ReturnsAsync(response);

            // Act
            var result = await _systemUnderTest.Send(NhsLoginId, request);

            // Assert
            _mockNotificationClient.VerifyAll();

            var successResult = result.Should().BeOfType<NotificationSendResult.Success>().Subject;
            successResult.NotificationSendResponse.Should().BeEquivalentTo(response);
        }

        [TestMethod]
        public async Task Send_NotificationServiceThrowsInstallationNotFoundException_ReturnsConflictResult()
        {
            // Arrange
            const string title = "title";
            const string subtitle = "subtitle";
            const string body = "body";
            const string url = "https://www.example.com";

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
                .ThrowsAsync(new InstallationNotFoundException());

            // Act
            var result = await _systemUnderTest.Send(NhsLoginId, request);

            // Assert
            _mockNotificationClient.VerifyAll();

            result.Should().BeOfType<NotificationSendResult.Conflict>();
        }

        [TestMethod]
        public async Task Send_NotificationServiceThrowsMessagingException_ReturnsBadGatewayResult()
        {
            // Arrange
            const string title = "title";
            const string subtitle = "subtitle";
            const string body = "body";
            const string url = "https://www.example.com";

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
                .ThrowsAsync(MessagingExceptionFactory.CreateMessagingException());

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
            const string url = "https://www.example.com";

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
            const string url = "https://www.example.com";

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

        [TestMethod]
        public async Task Send_ScheduledTimeSpecified_Success()
        {
            // Arrange
            const string title = "title";
            const string subtitle = "subtitle";
            const string body = "body";
            const string url = "https://www.example.com";
            DateTimeOffset? scheduledTime = DateTimeOffset.Now.AddHours(1);

            var request = new NotificationSendRequest
            {
                Title = title,
                Subtitle = subtitle,
                Body = body,
                Url = url,
                ScheduledTime = scheduledTime
            };

            var response = new NotificationSendResponse
            {
                Scheduled = true,
                NotificationId = "Notification ID",
                HubPath = "Hub Path"
            };

            _mockNotificationClient
                .Setup(x => x.SendNotification(
                    It.Is<NotificationRequest>(y =>
                        y.Title == title &&
                        y.Subtitle == subtitle &&
                        y.Body == body &&
                        y.Url == new Uri(url) &&
                        y.NhsLoginId == NhsLoginId &&
                        y.ScheduledTime == scheduledTime)))
                .ReturnsAsync(response);

            // Act
            var result = await _systemUnderTest.Send(NhsLoginId, request);

            // Assert
            _mockNotificationClient.VerifyAll();

            result.Should().BeOfType<NotificationSendResult.Success>();

            var successResult = result.Should().BeOfType<NotificationSendResult.Success>().Subject;
            successResult.NotificationSendResponse.Should().BeEquivalentTo(response);
        }

        [TestMethod]
        public async Task Send_SendsMinusOneBadgeCountAndDoesNotFetchUnreadCount_WhenFeatureDisabled()
        {
            // Arrange
            const string title = "title";
            const string subtitle = "subtitle";
            const string body = "body";
            const string url = "https://www.example.com";

            var request = new NotificationSendRequest
            {
                Title = title,
                Subtitle = subtitle,
                Body = body,
                Url = url,
            };

            var response = new NotificationSendResponse
            {
                Scheduled = false,
                NotificationId = "Notification ID",
                HubPath = "Hub Path"
            };

            _mockNotificationClient
                .Setup(x => x.SendNotification(
                    It.Is<NotificationRequest>(y =>
                        y.Title == title &&
                        y.Subtitle == subtitle &&
                        y.Body == body &&
                        y.Url == new Uri(url) &&
                        y.NhsLoginId == NhsLoginId &&
                        y.BadgeCount == 0)))
                .ReturnsAsync(response);

            // Act
            var result = await _systemUnderTest.Send(NhsLoginId, request);

            // Assert
            _mockNotificationClient.VerifyAll();
            _mockMessageService.VerifyNoOtherCalls();

            var successResult = result.Should().BeOfType<NotificationSendResult.Success>().Subject;
            successResult.NotificationSendResponse.Should().BeEquivalentTo(response);
        }

        [TestMethod]
        public async Task Send_SendsMinusOneBadgeCount_WhenRepositoryReturnsError()
        {
            // Arrange
            const string title = "title";
            const string subtitle = "subtitle";
            const string body = "body";
            const string url = "https://www.example.com";

            var request = new NotificationSendRequest
            {
                Title = title,
                Subtitle = subtitle,
                Body = body,
                Url = url,
            };

            var response = new NotificationSendResponse
            {
                Scheduled = false,
                NotificationId = "Notification ID",
                HubPath = "Hub Path"
            };

            _mockNotificationsConfiguration
                .SetupGet(c => c.IosBadgeCountEnabled)
                .Returns(true);

            _mockMessageService
                .Setup(r => r.GetUnreadMessageCount(NhsLoginId))
                .ReturnsAsync(new UnreadMessageCountResult.Failure());

            _mockNotificationClient
                .Setup(x => x.SendNotification(
                    It.Is<NotificationRequest>(y =>
                        y.Title == title &&
                        y.Subtitle == subtitle &&
                        y.Body == body &&
                        y.Url == new Uri(url) &&
                        y.NhsLoginId == NhsLoginId &&
                        y.BadgeCount == 0)))
                .ReturnsAsync(response);

            // Act
            var result = await _systemUnderTest.Send(NhsLoginId, request);

            // Assert
            _mockNotificationClient.VerifyAll();
            _mockMessageService.VerifyAll();

            var successResult = result.Should().BeOfType<NotificationSendResult.Success>().Subject;
            successResult.NotificationSendResponse.Should().BeEquivalentTo(response);
        }

        [TestMethod]
        public async Task Send_SendsCountUnreadMessages_WhenRepositoryReturnsFound()
        {
            // Arrange
            const string title = "title";
            const string subtitle = "subtitle";
            const string body = "body";
            const string url = "https://www.example.com";

            var request = new NotificationSendRequest
            {
                Title = title,
                Subtitle = subtitle,
                Body = body,
                Url = url,
            };

            var response = new NotificationSendResponse
            {
                Scheduled = false,
                NotificationId = "Notification ID",
                HubPath = "Hub Path"
            };

            _mockNotificationsConfiguration
                .SetupGet(c => c.IosBadgeCountEnabled)
                .Returns(true);

            _mockMessageService
                .Setup(r => r.GetUnreadMessageCount(NhsLoginId))
                .ReturnsAsync(new UnreadMessageCountResult.Success(5));

            _mockNotificationClient
                .Setup(x => x.SendNotification(
                    It.Is<NotificationRequest>(y =>
                        y.Title == title &&
                        y.Subtitle == subtitle &&
                        y.Body == body &&
                        y.Url == new Uri(url) &&
                        y.NhsLoginId == NhsLoginId &&
                        y.BadgeCount == 5)))
                .ReturnsAsync(response);

            // Act
            var result = await _systemUnderTest.Send(NhsLoginId, request);

            // Assert
            _mockNotificationClient.VerifyAll();
            _mockMessageService.VerifyAll();

            var successResult = result.Should().BeOfType<NotificationSendResult.Success>().Subject;
            successResult.NotificationSendResponse.Should().BeEquivalentTo(response);
        }

        [TestMethod]
        public async Task NotificationOutcome_NotificationServiceThrows_HttpRequestException_Returns_BadGatewayResult()
        {
            // Arrange
            _mockNotificationClient
                .Setup(x => x.GetNotificationOutcomeDetails("notificationId", "hubPath"))
                .ThrowsAsync(new HttpRequestException("Failed request"));

            // Act
            var result = await _systemUnderTest.GetNotificationOutcomeDetails("notificationId", "hubPath");

            // Assert
            _mockNotificationClient.VerifyAll();

            result.Should().BeOfType<NotificationOutcomeResult.BadGateway>();
        }

        [TestMethod]
        public async Task NotificationOutcome_NotificationServiceThrows_HubNotFound_Returns_NotFoundResult()
        {
            // Arrange
            _mockNotificationClient
                .Setup(x => x.GetNotificationOutcomeDetails("notificationId", "hubPath"))
                .ThrowsAsync(new NotificationHubNotFoundException("Hub not found"));

            // Act
            var result = await _systemUnderTest.GetNotificationOutcomeDetails("notificationId", "hubPath");

            // Assert
            _mockNotificationClient.VerifyAll();

            result.Should().BeOfType<NotificationOutcomeResult.NotFound>();
        }

        [TestMethod]
        public async Task NotificationOutcome_NotificationServiceThrows_MessagingEntityNotFoundException_Returns_NotFoundResult()
        {
            // Arrange
            _mockNotificationClient
                .Setup(x => x.GetNotificationOutcomeDetails("notificationId", "hubPath"))
                .ThrowsAsync(MessagingExceptionFactory.CreateMessagingEntityNotFoundException());

            // Act
            var result = await _systemUnderTest.GetNotificationOutcomeDetails("notificationId", "hubPath");

            // Assert
            _mockNotificationClient.VerifyAll();

            result.Should().BeOfType<NotificationOutcomeResult.NotFound>();
        }

        [TestMethod]
        public async Task NotificationOutcome_NotificationServiceThrows_MessagingException_Returns_BadGatewayResult()
        {
            // Arrange
            _mockNotificationClient
                .Setup(x => x.GetNotificationOutcomeDetails("notificationId", "hubPath"))
                .ThrowsAsync(MessagingExceptionFactory.CreateMessagingException());

            // Act
            var result = await _systemUnderTest.GetNotificationOutcomeDetails("notificationId", "hubPath");

            // Assert
            _mockNotificationClient.VerifyAll();

            result.Should().BeOfType<NotificationOutcomeResult.BadGateway>();
        }

        [TestMethod]
        public async Task NotificationOutcome_NotificationServiceThrows_Exception_Generic_CatchBlock_Handler_Returns_InternalServerError()
        {
            // Arrange
            _mockNotificationClient
                .Setup(x => x.GetNotificationOutcomeDetails("notificationId", "hubPath"))
                .ThrowsAsync(new AggregateException());

            // Act
            var result = await _systemUnderTest.GetNotificationOutcomeDetails("notificationId", "hubPath");

            // Assert
            _mockNotificationClient.VerifyAll();

            result.Should().BeOfType<NotificationOutcomeResult.InternalServerError>();
        }

        [TestMethod]
        public async Task NotificationOutcome_NotificationService_Success()
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

            _mockNotificationClient
                .Setup(x => x.GetNotificationOutcomeDetails("notificationId", "hubPath"))
                .ReturnsAsync(expectedNotificationOutcomeResponse);

            // Act
            var result = await _systemUnderTest.GetNotificationOutcomeDetails("notificationId", "hubPath");

            // Assert
            _mockNotificationClient.VerifyAll();
            result.Should().BeOfType<NotificationOutcomeResult.Success>();
            var successResult = result.Should().BeOfType<NotificationOutcomeResult.Success>().Subject;
            successResult.NotificationOutcomeResponse.Should().BeEquivalentTo(expectedNotificationOutcomeResponse);
        }
    }
}