using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.NotificationHubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Users.Notifications;

namespace NHSOnline.Backend.Users.UnitTests.Notifications
{
    [TestClass]
    public class AzureNotificationHubWrapperTests
    {
        private Mock<INotificationHubClient> _mockNotificationHubClient;
        private Mock<INotificationHubClientFactory> _mockNotificationHubClientFactory;
        private IAzureNotificationHubWrapper _systemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            _mockNotificationHubClientFactory = new Mock<INotificationHubClientFactory>(MockBehavior.Strict);
            _mockNotificationHubClient = new Mock<INotificationHubClient>(MockBehavior.Strict);
            _mockNotificationHubClientFactory
                .Setup(s => s.CreateClientFromConnectionString(It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(_mockNotificationHubClient.Object);

            var hubConfiguration = new AzureNotificationHubConfiguration(string.Empty,
                string.Empty, string.Empty,
                string.Empty, string.Empty, int.MaxValue);
            var notificationsConfiguration = new NotificationsConfiguration(true, 10);

            _systemUnderTest = new AzureNotificationHubWrapper(hubConfiguration, notificationsConfiguration, _mockNotificationHubClientFactory.Object);
        }

        [TestMethod]
        public async Task NotificationOutcome_Is_Parsed_When_FcmOutComeCollection_IsNull()
        {
            var dateTime = new DateTime(2022, 04, 05);
            var expectedNotificationDetails = new NotificationDetails
            {
                State = NotificationOutcomeState.Abandoned,
                EndTime = dateTime,
                StartTime = null,
                EnqueueTime = dateTime.AddHours(-2),
                FcmOutcomeCounts = null,
                ApnsOutcomeCounts = new NotificationOutcomeCollection
                {
                    { "InvalidNotificationSize", 1 },
                    { "PnsInterfaceError", 0 }
                }
            };

            _mockNotificationHubClient
                .Setup(s => s.GetNotificationOutcomeDetailsAsync("notificationId"))
                .ReturnsAsync(expectedNotificationDetails);

            var notificationResponse = await _systemUnderTest.GetNotificationOutcomeDetails("notificationId");

            notificationResponse.State.Should().Be("Abandoned");
            notificationResponse.StartTime.Should().BeNull();
            notificationResponse.EndTime.Should().Be(dateTime);
            notificationResponse.EnqueueTime.Should().Be(dateTime.AddHours(-2));

            var detailedOutComeCounts = notificationResponse.PlatformOutcomes.ToList();

            detailedOutComeCounts.Count.Should().Be(2);

            detailedOutComeCounts[0].Outcome.Should().Be("InvalidNotificationSize");
            detailedOutComeCounts[0].Count.Should().Be(1);
            detailedOutComeCounts[0].Platform.Should().Be("iOS");

            detailedOutComeCounts[1].Outcome.Should().Be("PnsInterfaceError");
            detailedOutComeCounts[1].Count.Should().Be(0);
            detailedOutComeCounts[1].Platform.Should().Be("iOS");

            VerifyAllMocks();
        }

        [TestMethod]
        public async Task NotificationOutcome_Is_Parsed_When_ApnOutComeCollection_IsNull()
        {
            var dateTime = new DateTime(2022, 04, 05);
            var expectedNotificationDetails = new NotificationDetails
            {
                State = NotificationOutcomeState.Cancelled,
                EndTime = dateTime,
                StartTime = null,
                EnqueueTime = dateTime.AddHours(-2),
                ApnsOutcomeCounts = null,
                FcmOutcomeCounts = new NotificationOutcomeCollection
                {
                    { "InvalidNotificationSize", 1 },
                    { "PnsInterfaceError", 0 }
                }
            };

            _mockNotificationHubClient
                .Setup(s => s.GetNotificationOutcomeDetailsAsync("notificationId"))
                .ReturnsAsync(expectedNotificationDetails);

            var notificationResponse = await _systemUnderTest.GetNotificationOutcomeDetails("notificationId");

            notificationResponse.State.Should().Be("Cancelled");
            notificationResponse.StartTime.Should().BeNull();
            notificationResponse.EndTime.Should().Be(dateTime);
            notificationResponse.EnqueueTime.Should().Be(dateTime.AddHours(-2));

            var detailedOutComeCounts = notificationResponse.PlatformOutcomes.ToList();

            detailedOutComeCounts.Count.Should().Be(2);

            detailedOutComeCounts[0].Outcome.Should().Be("InvalidNotificationSize");
            detailedOutComeCounts[0].Count.Should().Be(1);
            detailedOutComeCounts[0].Platform.Should().Be("Android");

            detailedOutComeCounts[1].Outcome.Should().Be("PnsInterfaceError");
            detailedOutComeCounts[1].Count.Should().Be(0);
            detailedOutComeCounts[1].Platform.Should().Be("Android");

            VerifyAllMocks();
        }

        [TestMethod]
        public async Task NotificationOutcome_Is_Returned_On_Successful_Parsing()
        {
            var dateTime = new DateTime(2022, 04, 05);
            var expectedNotificationDetails = new NotificationDetails
            {
                State = NotificationOutcomeState.Cancelled,
                EndTime = dateTime,
                StartTime = dateTime.AddHours(-1),
                EnqueueTime = dateTime.AddHours(-2),
                PnsErrorDetailsUri = "ErrorDetailsUri",
                ApnsOutcomeCounts = new NotificationOutcomeCollection
                {
                    { "InvalidNotificationSize", 1 }
                },
                FcmOutcomeCounts = new NotificationOutcomeCollection
                {
                    { "PnsInterfaceError", 2 }
                }
            };

            _mockNotificationHubClient
                .Setup(s => s.GetNotificationOutcomeDetailsAsync("notificationId"))
                .ReturnsAsync(expectedNotificationDetails);

            var notificationResponse = await _systemUnderTest.GetNotificationOutcomeDetails("notificationId");

            notificationResponse.State.Should().Be("Cancelled");
            notificationResponse.StartTime.Should().Be(dateTime.AddHours(-1));
            notificationResponse.EndTime.Should().Be(dateTime);
            notificationResponse.EnqueueTime.Should().Be(dateTime.AddHours(-2));
            notificationResponse.PnsErrorDetailsUri.Should().NotBeNullOrEmpty();

            var detailedOutComeCounts = notificationResponse.PlatformOutcomes.ToList();

            detailedOutComeCounts.Count.Should().Be(2);

            detailedOutComeCounts[0].Outcome.Should().Be("InvalidNotificationSize");
            detailedOutComeCounts[0].Count.Should().Be(1);
            detailedOutComeCounts[0].Platform.Should().Be("iOS");

            detailedOutComeCounts[1].Outcome.Should().Be("PnsInterfaceError");
            detailedOutComeCounts[1].Count.Should().Be(2);
            detailedOutComeCounts[1].Platform.Should().Be("Android");

            VerifyAllMocks();
        }

        private void VerifyAllMocks()
        {
            _mockNotificationHubClientFactory.VerifyAll();
            _mockNotificationHubClient.VerifyAll();
        }
    }
}