using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.NotificationHubs.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Notifications;

namespace NHSOnline.Backend.UsersApi.UnitTests.Notifications
{
    [TestClass]
    public class NotificationClientNotificationOutcomeTests
    {
        private const string HubPath = "hubPath";
        private const string NotificationId = "notificationId";
        private Mock<ILogger<NotificationClient>> _mockLogger;
        private Mock<IAzureNotificationHubWrapper> _mockWrapper;
        private Mock<IAzureNotificationHubWrapperService> _mockWrapperService;
        private INotificationClient _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockLogger = new Mock<ILogger<NotificationClient>>();
            _mockWrapper = new Mock<IAzureNotificationHubWrapper>(MockBehavior.Strict);
            _mockWrapperService = new Mock<IAzureNotificationHubWrapperService>(MockBehavior.Strict);

            _systemUnderTest = new NotificationClient(
                _mockLogger.Object,
                _mockWrapperService.Object
            );
        }

        [TestMethod]
        public void GetNotificationOutcome_NoMatchingWrappers_ThrowsException_Wrapper_Rethrows_Exception()
        {
            _mockWrapperService
                .Setup(x => x.Hub(HubPath))
                .Throws(new NotificationHubNotFoundException(nameof(HubPath)));

            Func<Task> act = async () =>
                await _systemUnderTest.GetNotificationOutcomeDetails(NotificationId, HubPath);

            act.Should().Throw<NotificationHubNotFoundException>();
            _mockWrapperService.VerifyAll();
            _mockWrapper.VerifyAll();
        }

        [TestMethod]
        public void GetNotificationOutcome_HubClient_Throws_Exception_Wrapper_Rethrows_Exception()
        {
            _mockWrapperService
                .Setup(x => x.Hub(HubPath))
                .Returns(_mockWrapper.Object);

            _mockWrapper.Setup(s => s.GetNotificationOutcomeDetails(NotificationId))
                .ThrowsAsync(MessagingExceptionFactory.CreateMessagingException());

            Func<Task> act = async () =>
                await _systemUnderTest.GetNotificationOutcomeDetails(NotificationId, HubPath);

            act.Should().Throw<MessagingException>();
            _mockWrapperService.VerifyAll();
            _mockWrapper.VerifyAll();
        }

        [TestMethod]
        public async Task GetNotificationOutcome_OneMatchingWrapper_NotificationOutcomeFound_CompletesSuccessfully()
        {
            var expectedNotificationOutcomeResponse = new NotificationOutcomeResponse
            {
                State = "Completed",
                EndTime = DateTime.UtcNow.AddMinutes(2),
                EnqueueTime = DateTime.UtcNow.AddHours(-1),
                StartTime = DateTime.UtcNow.AddMinutes(1),
                PnsErrorDetailsUri = "ErrorDetailUri",
                PlatformOutcomes = new List<PlatformOutcome>
                {
                    new PlatformOutcome { Count = 1, Outcome = "Success", Platform = "iOS" },
                    new PlatformOutcome { Count = 1, Outcome = "Skipped", Platform = "iOS" },
                    new PlatformOutcome { Count = 2, Outcome = "ExpiredChannel", Platform = "Android" },
                    new PlatformOutcome { Count = 2, Outcome = "BadChannel", Platform = "Android" }
                }
            };

            _mockWrapperService
                .Setup(x => x.Hub(HubPath))
                .Returns(_mockWrapper.Object);
            _mockWrapper.Setup(s => s.GetNotificationOutcomeDetails(NotificationId))
                .ReturnsAsync(expectedNotificationOutcomeResponse);

            var response = await _systemUnderTest.GetNotificationOutcomeDetails(NotificationId, HubPath);

            response.Should().Be(expectedNotificationOutcomeResponse);
            _mockWrapperService.VerifyAll();
            _mockWrapper.VerifyAll();
        }
    }
}