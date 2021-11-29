using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Metrics.EventHub;

namespace NHSOnline.Backend.Metrics.UnitTests.EventHub
{
    [TestClass]
    public class EventHubLoggerTests
    {
        private const string EnvironmentName = "TestEnv";

        private EventHubLogger _systemUnderTest;
        private Mock<IEventHubClient> _mockPidEventHubClient;
        private Mock<IEventHubClient> _mockNonPidEventHubClient;
        private Mock<IEventHubLoggerConfiguration> _mockEventHubLoggerConfiguration;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockPidEventHubClient = new Mock<IEventHubClient>(MockBehavior.Strict);
            _mockPidEventHubClient.SetupGet(x => x.EventHub).Returns(EventHubs.CommsHubPid);

            _mockNonPidEventHubClient = new Mock<IEventHubClient>(MockBehavior.Strict);
            _mockNonPidEventHubClient.SetupGet(x => x.EventHub).Returns(EventHubs.CommsHubNonPid);

            _mockEventHubLoggerConfiguration = new Mock<IEventHubLoggerConfiguration>(MockBehavior.Strict);

            _mockEventHubLoggerConfiguration.SetupGet(x => x.EnvironmentName)
                .Returns(EnvironmentName);

            _systemUnderTest = new EventHubLogger(
                _mockEventHubLoggerConfiguration.Object,
                new[] { _mockPidEventHubClient.Object, _mockNonPidEventHubClient.Object });
        }

        [TestMethod]
        public async Task MessageCreated_LogsMessageEventLogDataToPidAndNonPidEventHubs()
        {
            // Arrange
            var messageCreatedData = new MessageCreatedEventLogData(
                "Message ID",
                new MessageSenderContextEventLogData(
                    "Supplier ID",
                    "Communication ID",
                    "Transmission ID",
                    new DateTime(2021, 04, 22, 01, 05, 25),
                    "Request Reference",
                    "Campaign ID",
                    "Ods Code",
                    "NHS Number",
                    "NHS Login ID"
                )
            );

            var loggedPidData = string.Empty;
            var loggedNonPidData = string.Empty;

            _mockNonPidEventHubClient.Setup(x => x.WriteToEventHub(It.IsAny<string>()))
                .Callback<string>(x => loggedNonPidData = x)
                .Returns(Task.CompletedTask);
            _mockNonPidEventHubClient.SetupGet(x => x.PidAllowed).Returns(false);

            _mockPidEventHubClient.Setup(x => x.WriteToEventHub(It.IsAny<string>()))
                .Callback<string>(x => loggedPidData = x)
                .Returns(Task.CompletedTask);
            _mockPidEventHubClient.SetupGet(x => x.PidAllowed).Returns(true);

            // Act
            await _systemUnderTest.MessageCreated(messageCreatedData);

            // Assert
            VerifyMocks();

            loggedNonPidData.Split(' ').Should().HaveCount(12);
            AssertTimeStamp(loggedNonPidData);
            AssertContains(loggedNonPidData, "Action=MessageCreated");
            AssertContains(loggedNonPidData, "EnvironmentName=TestEnv");
            AssertContains(loggedNonPidData, "MessageId=Message+ID");
            AssertContains(loggedNonPidData, "SupplierId=Supplier+ID");
            AssertContains(loggedNonPidData, "CommunicationId=Communication+ID");
            AssertContains(loggedNonPidData, "TransmissionId=Transmission+ID");
            AssertContains(loggedNonPidData, "CommunicationCreatedDateTime=2021-04-22T01%3a05%3a25%3a000");
            AssertContains(loggedNonPidData, "RequestReference=Request+Reference");
            AssertContains(loggedNonPidData, "CampaignId=Campaign+ID");
            AssertContains(loggedNonPidData, "OdsCode=Ods+Code");
            AssertContains(loggedNonPidData, "NhsLoginId=NHS+Login+ID");
            AssertDoesNotContain(loggedNonPidData, "NhsNumber=NHS+Number");

            loggedPidData.Split(' ').Should().HaveCount(13);
            AssertTimeStamp(loggedPidData);
            AssertContains(loggedPidData, "Action=MessageCreated");
            AssertContains(loggedPidData, "EnvironmentName=TestEnv");
            AssertContains(loggedPidData, "MessageId=Message+ID");
            AssertContains(loggedPidData, "SupplierId=Supplier+ID");
            AssertContains(loggedPidData, "CommunicationId=Communication+ID");
            AssertContains(loggedPidData, "TransmissionId=Transmission+ID");
            AssertContains(loggedPidData, "CommunicationCreatedDateTime=2021-04-22T01%3a05%3a25%3a000");
            AssertContains(loggedPidData, "RequestReference=Request+Reference");
            AssertContains(loggedPidData, "CampaignId=Campaign+ID");
            AssertContains(loggedPidData, "OdsCode=Ods+Code");
            AssertContains(loggedPidData, "NhsLoginId=NHS+Login+ID");
            AssertContains(loggedPidData, "NhsNumber=NHS+Number");
        }

        [TestMethod]
        public async Task MessageRead_LogsMessageEventLogDataToPidAndNonPidEventHubs()
        {
            // Arrange
            var messageReadData = new MessageReadEventLogData(
                "Message ID",
                new MessageSenderContextEventLogData(
                    "Supplier ID",
                    "Communication ID",
                    "Transmission ID",
                    new DateTime(2021, 04, 22, 01, 05, 25),
                    "Request Reference",
                    "Campaign ID",
                    "Ods Code",
                    "NHS Number",
                    "NHS Login ID"
                )
            );

            var loggedPidData = string.Empty;
            var loggedNonPidData = string.Empty;

            _mockNonPidEventHubClient.Setup(x => x.WriteToEventHub(It.IsAny<string>()))
                .Callback<string>(x => loggedNonPidData = x)
                .Returns(Task.CompletedTask);
            _mockNonPidEventHubClient.SetupGet(x => x.PidAllowed).Returns(false);

            _mockPidEventHubClient.Setup(x => x.WriteToEventHub(It.IsAny<string>()))
                .Callback<string>(x => loggedPidData = x)
                .Returns(Task.CompletedTask);
            _mockPidEventHubClient.SetupGet(x => x.PidAllowed).Returns(true);

            // Act
            await _systemUnderTest.MessageRead(messageReadData);

            // Assert
            VerifyMocks();

            loggedNonPidData.Split(' ').Should().HaveCount(12);
            AssertTimeStamp(loggedNonPidData);
            AssertContains(loggedNonPidData, "Action=MessageRead");
            AssertContains(loggedNonPidData, "EnvironmentName=TestEnv");
            AssertContains(loggedNonPidData, "MessageId=Message+ID");
            AssertContains(loggedNonPidData, "SupplierId=Supplier+ID");
            AssertContains(loggedNonPidData, "CommunicationId=Communication+ID");
            AssertContains(loggedNonPidData, "TransmissionId=Transmission+ID");
            AssertContains(loggedNonPidData, "CommunicationCreatedDateTime=2021-04-22T01%3a05%3a25%3a000");
            AssertContains(loggedNonPidData, "RequestReference=Request+Reference");
            AssertContains(loggedNonPidData, "CampaignId=Campaign+ID");
            AssertContains(loggedNonPidData, "OdsCode=Ods+Code");
            AssertContains(loggedNonPidData, "NhsLoginId=NHS+Login+ID");
            AssertDoesNotContain(loggedNonPidData, "NhsNumber=NHS+Number");

            loggedPidData.Split(' ').Should().HaveCount(13);
            AssertTimeStamp(loggedPidData);
            AssertContains(loggedPidData, "Action=MessageRead");
            AssertContains(loggedPidData, "EnvironmentName=TestEnv");
            AssertContains(loggedPidData, "MessageId=Message+ID");
            AssertContains(loggedPidData, "SupplierId=Supplier+ID");
            AssertContains(loggedPidData, "CommunicationId=Communication+ID");
            AssertContains(loggedPidData, "TransmissionId=Transmission+ID");
            AssertContains(loggedPidData, "CommunicationCreatedDateTime=2021-04-22T01%3a05%3a25%3a000");
            AssertContains(loggedPidData, "RequestReference=Request+Reference");
            AssertContains(loggedPidData, "CampaignId=Campaign+ID");
            AssertContains(loggedPidData, "OdsCode=Ods+Code");
            AssertContains(loggedPidData, "NhsLoginId=NHS+Login+ID");
            AssertContains(loggedPidData, "NhsNumber=NHS+Number");
        }

        [TestMethod]
        public async Task NotificationEnqueued_LogsNotificationEnqueuedEventLogDataToNonPidEventHubs()
        {
            // Arrange
            var notificationEnqueuedData = new NotificationEnqueuedEventLogData(
                "Nhs Login ID", "Notification ID", "Tracking ID", true);

            var loggedNonPidData = string.Empty;

            _mockNonPidEventHubClient.Setup(x => x.WriteToEventHub(It.IsAny<string>()))
                .Callback<string>(x => loggedNonPidData = x)
                .Returns(Task.CompletedTask);
            _mockNonPidEventHubClient.SetupGet(x => x.PidAllowed).Returns(false);

            // Act
            await _systemUnderTest.NotificationEnqueued(notificationEnqueuedData);

            // Assert
            VerifyMocks();

            loggedNonPidData.Split(' ').Should().HaveCount(7);
            AssertTimeStamp(loggedNonPidData);
            AssertContains(loggedNonPidData, "Action=NotificationEnqueued");
            AssertContains(loggedNonPidData, "EnvironmentName=TestEnv");
            AssertContains(loggedNonPidData, "NhsLoginId=Nhs+Login+ID");
            AssertContains(loggedNonPidData, "NotificationId=Notification+ID");
            AssertContains(loggedNonPidData, "TrackingId=Tracking+ID");
            AssertContains(loggedNonPidData, "Scheduled=True");
        }

        private static void AssertContains(string logData, string expected)
        {
            logData.Split(' ').Should().Contain(expected);
        }

        private static void AssertDoesNotContain(string logData, string expectedAbsent)
        {
            logData.Split(' ').Should().NotContain(expectedAbsent);
        }

        private static void AssertTimeStamp(string data)
        {
            data.Split(' ')[0]
                .Should().MatchRegex(@"^Timestamp=\d\d\d\d-\d\d-\d\dT\d\d:\d\d:\d\d\:\d\d\d$");
        }

        private void VerifyMocks()
        {
            _mockEventHubLoggerConfiguration.VerifyAll();
            _mockPidEventHubClient.VerifyAll();
            _mockNonPidEventHubClient.VerifyAll();
        }
    }
}