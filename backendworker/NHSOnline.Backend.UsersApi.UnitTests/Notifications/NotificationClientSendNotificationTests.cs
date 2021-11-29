using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.UsersApi.Notifications;
using NHSOnline.Backend.UsersApi.Notifications.Models;

namespace NHSOnline.Backend.UsersApi.UnitTests.Notifications
{
    [TestClass]
    public class NotificationClientSendNotificationTests
    {
        private Mock<ILogger<NotificationClient>> _mockLogger;
        private Mock<IAzureNotificationHubWrapperService> _mockWrapperService;
        private Mock<IAzureNotificationHubWrapper> _mockWrapper;
        private Mock<IAzureNotificationHubWrapper> _mockWrapper2;

        private INotificationClient _systemUnderTest;

        private const string InstallationId = "InstallationId";
        private const string NhsLoginId = "NhsLoginId";

        private readonly NotificationRequest _notificationRequest = new NotificationRequest
        {
            Title = "title",
            Subtitle = "subtitle",
            Body = "body",
            Url = new Uri("http://www.example.com"),
            NhsLoginId = NhsLoginId
        };

        private readonly NotificationRequest _scheduledNotificationRequest = new NotificationRequest
        {
            Title = "title",
            Subtitle = "subtitle",
            Body = "body",
            Url = new Uri("http://www.example.com"),
            NhsLoginId = NhsLoginId,
            ScheduledTime = DateTimeOffset.UtcNow.AddHours(1)
        };

        private readonly NotificationResponse _scheduledNotificationResponse = new NotificationResponse
        {
            Scheduled = true,
            NotificationId = "Scheduled Notification ID",
            TrackingId = "Scheduled Tracking ID"
        };

        private readonly NotificationResponse _notificationResponse = new NotificationResponse
        {
            Scheduled = false,
            NotificationId = "Notification ID",
            TrackingId = "Tracking ID"
        };

        [TestInitialize]
        public void TestInitialize()
        {
            _mockLogger = new Mock<ILogger<NotificationClient>>();
            _mockWrapper = new Mock<IAzureNotificationHubWrapper>(MockBehavior.Strict);
            _mockWrapper2 = new Mock<IAzureNotificationHubWrapper>(MockBehavior.Strict);
            _mockWrapperService = new Mock<IAzureNotificationHubWrapperService>(MockBehavior.Strict);

            _systemUnderTest = new NotificationClient(
                _mockLogger.Object,
                _mockWrapperService.Object
            );
        }

        [TestMethod]
        public void SendNotification_NoMatchingWrappers_Throws()
        {
            // Arrange
            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(Enumerable.Empty<IAzureNotificationHubWrapper>());

            // Act
            FluentActions.Invoking(async () => await _systemUnderTest.SendNotification(_notificationRequest))
                .Should().ThrowExactly<InstallationNotFoundException>();

            // Assert
            VerifyMocks();
        }

        [TestMethod]
        public void SendNotification_NoMatchingWrappers_Scheduled_Throws()
        {
            // Arrange
            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(Enumerable.Empty<IAzureNotificationHubWrapper>());

            // Act
            FluentActions.Invoking(async () => await _systemUnderTest.SendNotification(_scheduledNotificationRequest))
                .Should().ThrowExactly<InstallationNotFoundException>();

            // Assert
            VerifyMocks();
        }

        [TestMethod]
        public async Task SendNotification_OneMatchingWrapper_NotificationSent()
        {
            // Arrange
            SetupMockWrapper(_mockWrapper);

            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(new[] { _mockWrapper.Object });

            // Act
            var response = await _systemUnderTest.SendNotification(_notificationRequest);

            // Assert
            response.Should().BeEquivalentTo(_notificationResponse);

            VerifyMocks();
        }

        [TestMethod]
        public async Task SendNotification_OneMatchingWrapper_Scheduled_NotificationSent()
        {
            // Arrange
            SetupMockWrapper(_mockWrapper, scheduled: true);

            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(new[] { _mockWrapper.Object });

            // Act
            var response = await _systemUnderTest.SendNotification(_scheduledNotificationRequest);

            // Assert
            response.Should().BeEquivalentTo(_scheduledNotificationResponse);

            VerifyMocks();
        }

        [TestMethod]
        public void SendNotification_MultipleMatchingWrappers_InstallationNotFoundInAny_Throws()
        {
            // Arrange
            SetupMockWrapper(_mockWrapper, false, true);
            SetupMockWrapper(_mockWrapper2, false, true);

            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(new[] { _mockWrapper.Object, _mockWrapper2.Object });

            // Act
            FluentActions.Invoking(async () => await _systemUnderTest.SendNotification(_notificationRequest))
                .Should().ThrowExactly<InstallationNotFoundException>();

            // Assert
            VerifyMocks();
        }

        [TestMethod]
        public void SendNotification_MultipleMatchingWrappers_InstallationNotFoundInAny_Scheduled_Throws()
        {
            // Arrange
            SetupMockWrapper(_mockWrapper, false, true);
            SetupMockWrapper(_mockWrapper2, false, true);

            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(new[] { _mockWrapper.Object, _mockWrapper2.Object });

            // Act
            FluentActions.Invoking(async () => await _systemUnderTest.SendNotification(_scheduledNotificationRequest))
                .Should().ThrowExactly<InstallationNotFoundException>();

            // Assert
            VerifyMocks();
        }

        [TestMethod]
        public async Task SendNotification_MultipleMatchingWrappers_InstallationFoundInFirstOne_SendsNotification()
        {
            // Arrange
            SetupMockWrapper(_mockWrapper, true, true);
            SetupMockWrapper(_mockWrapper2, false, true);

            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(new[] { _mockWrapper.Object, _mockWrapper2.Object });

            // Act
            var response = await _systemUnderTest.SendNotification(_notificationRequest);

            // Assert
            response.Should().BeEquivalentTo(_notificationResponse);
            _mockWrapperService.VerifyAll();
            _mockWrapper.VerifyAll();
            _mockWrapper2.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task SendNotification_MultipleMatchingWrappers_InstallationFoundInFirstOne_Scheduled_SendsNotification()
        {
            // Arrange
            SetupMockWrapper(_mockWrapper, true, true, true);
            SetupMockWrapper(_mockWrapper2, false, true, true);

            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(new[] { _mockWrapper.Object, _mockWrapper2.Object });

            // Act
            var response = await _systemUnderTest.SendNotification(_scheduledNotificationRequest);

            // Assert
            response.Should().BeEquivalentTo(_scheduledNotificationResponse);
            _mockWrapperService.VerifyAll();
            _mockWrapper.VerifyAll();
            _mockWrapper2.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task SendNotification_MultipleMatchingWrappers_InstallationFoundInSecondOne_SendsNotification()
        {
            // Arrange
            SetupMockWrapper(_mockWrapper, false, true);
            SetupMockWrapper(_mockWrapper2, true, true);

            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(new[] { _mockWrapper.Object, _mockWrapper2.Object });

            // Act
            var response = await _systemUnderTest.SendNotification(_notificationRequest);

            // Assert
            response.Should().BeEquivalentTo(_notificationResponse);
            VerifyMocks();
        }

        [TestMethod]
        public async Task SendNotification_MultipleMatchingWrappers_InstallationFoundInSecondOne_Scheduled_SendsNotification()
        {
            // Arrange
            SetupMockWrapper(_mockWrapper, false, true, true);
            SetupMockWrapper(_mockWrapper2, true, true, true);

            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(new[] { _mockWrapper.Object, _mockWrapper2.Object });

            // Act
            var response = await _systemUnderTest.SendNotification(_scheduledNotificationRequest);

            // Assert
            response.Should().BeEquivalentTo(_scheduledNotificationResponse);
            VerifyMocks();
        }

        [TestMethod]
        public async Task SendNotification_MultipleMatchingWrappers_InstallationFoundInAll_SendsNotification()
        {
            // Arrange
            SetupMockWrapper(_mockWrapper, true, true);
            SetupMockWrapper(_mockWrapper2, true, true);

            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(new[] { _mockWrapper.Object, _mockWrapper2.Object });

            // Act
            var response = await _systemUnderTest.SendNotification(_notificationRequest);

            // Assert
            response.Should().BeEquivalentTo(_notificationResponse);
            _mockWrapperService.VerifyAll();
            _mockWrapper.VerifyAll();
            _mockWrapper2.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task SendNotification_MultipleMatchingWrappers_InstallationFoundInAll_Scheduled_SendsNotification()
        {
            // Arrange
            SetupMockWrapper(_mockWrapper, true, true, true);
            SetupMockWrapper(_mockWrapper2, true, true, true);

            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(new[] { _mockWrapper.Object, _mockWrapper2.Object });

            // Act
            var response = await _systemUnderTest.SendNotification(_scheduledNotificationRequest);

            // Assert
            response.Should().BeEquivalentTo(_scheduledNotificationResponse);
            _mockWrapperService.VerifyAll();
            _mockWrapper.VerifyAll();
            _mockWrapper2.VerifyNoOtherCalls();
        }

        private void SetupMockWrapper(
            Mock<IAzureNotificationHubWrapper> wrapper,
            bool hasInstallations = false,
            bool hasMultipleWrappers = false,
            bool scheduled = false)
        {
            if (hasMultipleWrappers)
            {
                var installationIds = hasInstallations
                    ? new[] { InstallationId }
                    : Array.Empty<string>();

                wrapper
                    .Setup(x => x.GetInstallationIdsByNhsLoginId(NhsLoginId))
                    .ReturnsAsync(installationIds);

                if (hasInstallations)
                {
                    if (scheduled)
                    {
                        wrapper
                            .Setup(x => x.SendScheduledNotification(_scheduledNotificationRequest))
                            .ReturnsAsync(_scheduledNotificationResponse);
                    }
                    else
                    {
                        wrapper
                            .Setup(x => x.SendNotification(_notificationRequest))
                            .ReturnsAsync(_notificationResponse);
                    }
                }
            }
            else
            {
                if (scheduled)
                {
                    wrapper
                        .Setup(x => x.SendScheduledNotification(_scheduledNotificationRequest))
                        .ReturnsAsync(_scheduledNotificationResponse);
                }
                else
                {
                    wrapper
                        .Setup(x => x.SendNotification(_notificationRequest))
                        .ReturnsAsync(_notificationResponse);
                }
            }
        }

        private void VerifyMocks()
        {
            _mockWrapperService.VerifyAll();
            _mockWrapper.VerifyAll();
            _mockWrapper2.VerifyAll();
        }
    }
}