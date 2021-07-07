using System;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task SendNotification_NoMatchingWrappers_NoActionTaken()
        {
            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(Enumerable.Empty<IAzureNotificationHubWrapper>());

            await _systemUnderTest.SendNotification(_notificationRequest);

            VerifySetups();
        }

        [TestMethod]
        public async Task SendNotification_OneMatchingWrapper_InstallationNotFound_NoActionTaken()
        {
            SetupMockWrapper(_mockWrapper, false);

            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(new[] { _mockWrapper.Object });

            await _systemUnderTest.SendNotification(_notificationRequest);

            VerifySetups();
        }

        [TestMethod]
        public async Task SendNotification_OneMatchingWrapper_InstallationFound_SendsNotification()
        {
            SetupMockWrapper(_mockWrapper, true);

            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(new[] { _mockWrapper.Object });

            await _systemUnderTest.SendNotification(_notificationRequest);

            VerifySetups();
        }

        [TestMethod]
        public async Task SendNotification_MultipleMatchingWrappers_InstallationNotFoundInAny_NoActionTaken()
        {
            SetupMockWrapper(_mockWrapper, false, true);
            SetupMockWrapper(_mockWrapper2, false, true);

            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(new[] { _mockWrapper.Object, _mockWrapper2.Object });

            await _systemUnderTest.SendNotification(_notificationRequest);

            VerifySetups();
        }

        [TestMethod]
        public async Task SendNotification_MultipleMatchingWrappers_InstallationFoundInFirstOne_SendsNotification()
        {
            SetupMockWrapper(_mockWrapper, true, true);
            SetupMockWrapper(_mockWrapper2, false, true);

            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(new[] { _mockWrapper.Object, _mockWrapper2.Object });

            await _systemUnderTest.SendNotification(_notificationRequest);

            _mockWrapperService.VerifyAll();
            _mockWrapper.VerifyAll();
            _mockWrapper2.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task SendNotification_MultipleMatchingWrappers_InstallationFoundInSecondOne_SendsNotification()
        {
            SetupMockWrapper(_mockWrapper, false, true);
            SetupMockWrapper(_mockWrapper2, true, true);

            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(new[] { _mockWrapper.Object, _mockWrapper2.Object });

            await _systemUnderTest.SendNotification(_notificationRequest);

            VerifySetups();
        }

        [TestMethod]
        public async Task SendNotification_MultipleMatchingWrappers_InstallationFoundInAll_SendsNotification()
        {
            SetupMockWrapper(_mockWrapper, true, true);
            SetupMockWrapper(_mockWrapper2, true, true);

            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(new[] { _mockWrapper.Object, _mockWrapper2.Object });

            await _systemUnderTest.SendNotification(_notificationRequest);

            _mockWrapperService.VerifyAll();
            _mockWrapper.VerifyAll();
            _mockWrapper2.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task SendNotification_NullScheduled_SendsNotification()
        {
            //Arrange
            SetupMockWrapper(_mockWrapper, true);
            _notificationRequest.ScheduledTime = null;

            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(new[] { _mockWrapper.Object });

            //Act
            await _systemUnderTest.SendNotification(_notificationRequest);

            //Assert
            VerifySetups();
        }

        [TestMethod]
        public async Task SendNotification_ScheduledTimeSpecified_SchedulesNotification()
        {
            // Arrange
            _notificationRequest.ScheduledTime = DateTimeOffset.Now.AddHours(1);

            SetupMockWrapper(_mockWrapper, true);

            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(new[] { _mockWrapper.Object });

            // Act
            await _systemUnderTest.SendNotification(_notificationRequest);

            // Assert
            VerifySetups();
        }

        private void SetupMockWrapper(
            Mock<IAzureNotificationHubWrapper> wrapper,
            bool hasInstallations,
            bool hasMultipleWrappers = false
        )
        {
            bool isScheduled = _notificationRequest.ScheduledTime != null;

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
                    if (isScheduled)
                    {
                        wrapper
                            .Setup(x => x.SendScheduledNotification(_notificationRequest))
                            .Returns(Task.CompletedTask);
                    }
                    else
                    {
                        wrapper
                            .Setup(x => x.SendNotification(_notificationRequest))
                            .Returns(Task.CompletedTask);
                    }
                }
            }
            else
            {
                if (isScheduled)
                {
                    wrapper
                        .Setup(x => x.SendScheduledNotification(_notificationRequest))
                        .Returns(Task.CompletedTask);
                }
                else
                {
                    wrapper
                        .Setup(x => x.SendNotification(_notificationRequest))
                        .Returns(Task.CompletedTask);
                }
            }
        }

        private void VerifySetups()
        {
            _mockWrapperService.VerifyAll();
            _mockWrapper.VerifyAll();
            _mockWrapper2.VerifyAll();
        }
    }
}