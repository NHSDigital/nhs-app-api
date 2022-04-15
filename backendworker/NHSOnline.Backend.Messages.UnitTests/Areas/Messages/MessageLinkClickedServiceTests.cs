using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using Moq;
using NHSOnline.Backend.Messages.Areas.Messages;
using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Messages.Repository;
using NHSOnline.Backend.Auth;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.Metrics.EventHub;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Hasher;

namespace NHSOnline.Backend.Messages.UnitTests.Areas.Messages
{
    [TestClass]
    public class MessageLinkClickedServiceTests
    {
        private MessageLinkClickedService _systemUnderTest;

        private Mock<IMapper<SenderContext, SenderContextEventLogData>> _mockSenderContextEventLogDataMapper;
        private Mock<IMessageLinkClickedValidationService> _mockValidationService;
        private Mock<IMessageRepository> _mockMessageRepository;
        private Mock<IMetricLogger<UserSessionMetricContext>> _mockMetricLogger;
        private Mock<IEventHubLogger> _mockEventHubLogger;
        private Mock<IHashingService> _mockHashingService;

        private const string NhsLoginId = "NhsLoginId";
        private const string MessageId = "61e31ce61a33d95515728826";
        private const string Link = "https://www.testing.com/valid/url/";
        private const string CampaignId = "CampaignId";
        private const string CommunicationId = "CommunicationId";
        private const string TransmissionId = "TransmissionId";

        private MessageLink _messageLink;
        private UserMessage _userMessage;
        private SenderContextEventLogData _senderContextEventLogData;

        [TestInitialize]
        public void TestInitialize()
        {
            _messageLink = new MessageLink
            {
                MessageId = MessageId,
                Link = Link
            };

            _userMessage = new UserMessage
            {
                Id = new ObjectId(MessageId),
                SenderContext = new SenderContext
                {
                    CampaignId = CampaignId,
                    CommunicationId = CommunicationId,
                    TransmissionId = TransmissionId
                }
            };

            _senderContextEventLogData = new SenderContextEventLogData(null, null, CommunicationId, TransmissionId, null,
                null, CampaignId, null, null, null);

            _mockSenderContextEventLogDataMapper =
                new Mock<IMapper<SenderContext, SenderContextEventLogData>>(MockBehavior.Strict);
            _mockValidationService = new Mock<IMessageLinkClickedValidationService>(MockBehavior.Strict);
            _mockMessageRepository = new Mock<IMessageRepository>(MockBehavior.Strict);
            _mockMetricLogger = new Mock<IMetricLogger<UserSessionMetricContext>>(MockBehavior.Strict);
            _mockEventHubLogger = new Mock<IEventHubLogger>(MockBehavior.Strict);
            _mockHashingService = new Mock<IHashingService>(MockBehavior.Strict);

            _systemUnderTest = new MessageLinkClickedService(
                new Mock<ILogger<MessageLinkClickedService>>().Object,
                _mockSenderContextEventLogDataMapper.Object,
                _mockValidationService.Object,
                _mockMessageRepository.Object,
                _mockMetricLogger.Object,
                _mockEventHubLogger.Object,
                _mockHashingService.Object
            );
        }

        [TestMethod]
        public async Task LogLinkClicked_ValidationFails_ReturnsBadRequest()
        {
            // Arrange
            _mockValidationService
                .Setup(x => x.IsServiceRequestValid(NhsLoginId, _messageLink))
                .Returns(false);

            // Act
            var result = await _systemUnderTest.LogLinkClicked(NhsLoginId, _messageLink);

            // Assert
            VerifyMocks();

            result.Should().BeOfType(typeof(MessageLinkClickedResult.BadRequest));
        }

        [TestMethod]
        public async Task LogLinkClicked_ValidationThrowsError_ReturnsInternalServerError()
        {
            // Arrange
            _mockValidationService
                .Setup(x => x.IsServiceRequestValid(NhsLoginId, _messageLink))
                .Throws<Exception>();

            // Act
            var result = await _systemUnderTest.LogLinkClicked(NhsLoginId, _messageLink);

            // Assert
            VerifyMocks();

            result.Should().BeOfType(typeof(MessageLinkClickedResult.InternalServerError));
        }

        [TestMethod]
        public async Task LogLinkClicked_RepositoryReturnsNotFound_ReturnsBadRequest()
        {
            // Arrange
            _mockValidationService
                .Setup(x => x.IsServiceRequestValid(NhsLoginId, _messageLink))
                .Returns(true);

            _mockMessageRepository
                .Setup(x => x.FindMessage(NhsLoginId, MessageId))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.NotFound());

            // Act
            var result = await _systemUnderTest.LogLinkClicked(NhsLoginId, _messageLink);

            // Assert
            VerifyMocks();

            result.Should().BeOfType(typeof(MessageLinkClickedResult.BadRequest));
        }

        [TestMethod]
        public async Task LogLinkClicked_RepositoryReturnsError_ReturnsBadGateway()
        {
            // Arrange
            _mockValidationService
                .Setup(x => x.IsServiceRequestValid(NhsLoginId, _messageLink))
                .Returns(true);

            _mockMessageRepository
                .Setup(x => x.FindMessage(NhsLoginId, MessageId))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.RepositoryError());

            // Act
            var result = await _systemUnderTest.LogLinkClicked(NhsLoginId, _messageLink);

            // Assert
            VerifyMocks();

            result.Should().BeOfType(typeof(MessageLinkClickedResult.BadGateway));
        }

        [TestMethod]
        public async Task LogLinkClicked_RepositoryThrowsError_ReturnsInternalServerError()
        {
            // Arrange
            _mockValidationService
                .Setup(x => x.IsServiceRequestValid(NhsLoginId, _messageLink))
                .Returns(true);

            _mockMessageRepository
                .Setup(x => x.FindMessage(NhsLoginId, MessageId))
                .Throws<Exception>();

            // Act
            var result = await _systemUnderTest.LogLinkClicked(NhsLoginId, _messageLink);

            // Assert
            VerifyMocks();

            result.Should().BeOfType(typeof(MessageLinkClickedResult.InternalServerError));
        }

        [TestMethod]
        public async Task LogLinkClicked_MetricLoggerThrowsError_ReturnsInternalServerError()
        {
            // Arrange
            _mockValidationService
                .Setup(x => x.IsServiceRequestValid(NhsLoginId, _messageLink))
                .Returns(true);

            var found = new RepositoryFindResult<UserMessage>.Found(new[] { _userMessage });

            _mockMessageRepository
                .Setup(x => x.FindMessage(NhsLoginId, MessageId))
                .ReturnsAsync(found);

            const string hashedLink = "hashedLink";
            _mockHashingService.Setup(x => x.Hash(Link)).Returns(hashedLink);

            _mockSenderContextEventLogDataMapper
                .Setup(x => x.Map(_userMessage.SenderContext))
                .Returns(_senderContextEventLogData);

            _mockEventHubLogger
                .Setup(x => x.MessageLinkClicked(It.IsAny<MessageLinkClickedEventLogData>()))
                .Returns(Task.CompletedTask);

            _mockMetricLogger
                .Setup(x => x.MessageLinkClicked(It.IsAny<MessageLinkClickedData>()))
                .Throws<Exception>();

            // Act
            var result = await _systemUnderTest.LogLinkClicked(NhsLoginId, _messageLink);

            // Assert
            VerifyMocks();

            result.Should().BeOfType(typeof(MessageLinkClickedResult.InternalServerError));
        }

        [TestMethod]
        public async Task LogLinkClicked_EventHubLoggerThrowsError_ReturnsInternalServerError()
        {
            // Arrange
            _mockValidationService
                .Setup(x => x.IsServiceRequestValid(NhsLoginId, _messageLink))
                .Returns(true);

            var found = new RepositoryFindResult<UserMessage>.Found(new[] { _userMessage });

            _mockMessageRepository
                .Setup(x => x.FindMessage(NhsLoginId, MessageId))
                .ReturnsAsync(found);

            const string hashedLink = "hashedLink";
            _mockHashingService.Setup(x => x.Hash(Link)).Returns(hashedLink);

            _mockSenderContextEventLogDataMapper
                .Setup(x => x.Map(_userMessage.SenderContext))
                .Returns(_senderContextEventLogData);

            _mockEventHubLogger
                .Setup(x => x.MessageLinkClicked(It.IsAny<MessageLinkClickedEventLogData>()))
                .Throws<Exception>();

            // Act
            var result = await _systemUnderTest.LogLinkClicked(NhsLoginId, _messageLink);

            // Assert
            VerifyMocks();

            result.Should().BeOfType(typeof(MessageLinkClickedResult.InternalServerError));
        }

        [TestMethod]
        public async Task LogLinkClicked_UserMessageSenderContextNull_LogsDataWithUnhashedLink()
        {
            // Arrange
            _mockValidationService
                .Setup(x => x.IsServiceRequestValid(NhsLoginId, _messageLink))
                .Returns(true);

            _userMessage.SenderContext = null;
            var found = new RepositoryFindResult<UserMessage>.Found(new[]{_userMessage});

            _mockMessageRepository
                .Setup(x => x.FindMessage(NhsLoginId, MessageId))
                .ReturnsAsync(found);

            MessageLinkClickedData metricLoggerData = null;
            _mockMetricLogger
                .Setup(x => x.MessageLinkClicked(It.IsAny<MessageLinkClickedData>()))
                .Callback<MessageLinkClickedData>(x => metricLoggerData = x)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _systemUnderTest.LogLinkClicked(NhsLoginId, _messageLink);

            // Assert
            metricLoggerData.Link.Should().Be(Link);
            metricLoggerData.MessageId.Should().Be(MessageId);
            metricLoggerData.CampaignId.Should().Be(null);
            metricLoggerData.CommunicationId.Should().Be(null);
            metricLoggerData.TransmissionId.Should().Be(null);

            VerifyMocks();

            result.Should().BeOfType(typeof(MessageLinkClickedResult.Success));
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public async Task LogLinkClicked_UserMessageSenderContextIsPopulatedWithNullOrEmptyCommunicationId_LogsDataWithUnhashedLink(string communicationId)
        {
            // Arrange
            _userMessage.SenderContext.CommunicationId = communicationId;

            _mockValidationService
                .Setup(x => x.IsServiceRequestValid(NhsLoginId, _messageLink))
                .Returns(true);

            _userMessage.SenderContext = null;
            var found = new RepositoryFindResult<UserMessage>.Found(new[]{_userMessage});

            _mockMessageRepository
                .Setup(x => x.FindMessage(NhsLoginId, MessageId))
                .ReturnsAsync(found);

            MessageLinkClickedData metricLoggerData = null;
            _mockMetricLogger
                .Setup(x => x.MessageLinkClicked(It.IsAny<MessageLinkClickedData>()))
                .Callback<MessageLinkClickedData>(x => metricLoggerData = x)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _systemUnderTest.LogLinkClicked(NhsLoginId, _messageLink);

            // Assert
            metricLoggerData.Link.Should().Be(Link);
            metricLoggerData.MessageId.Should().Be(MessageId);
            metricLoggerData.CampaignId.Should().Be(null);
            metricLoggerData.CommunicationId.Should().Be(null);
            metricLoggerData.TransmissionId.Should().Be(null);

            VerifyMocks();

            result.Should().BeOfType(typeof(MessageLinkClickedResult.Success));
        }

        [TestMethod]
        public async Task LogLinkClicked_UserMessageSenderContextIsPopulatedWithCommunicationId_LogsDataWithHashedLink()
        {
            // Arrange
            _mockValidationService
                .Setup(x => x.IsServiceRequestValid(NhsLoginId, _messageLink))
                .Returns(true);

            var found = new RepositoryFindResult<UserMessage>.Found(new[]{_userMessage});

            _mockMessageRepository
                .Setup(x => x.FindMessage(NhsLoginId, MessageId))
                .ReturnsAsync(found);

            const string hashedLink = "hashedLink";
            _mockHashingService.Setup(x => x.Hash(Link)).Returns(hashedLink);

            _mockSenderContextEventLogDataMapper
                .Setup(x => x.Map(_userMessage.SenderContext))
                .Returns(_senderContextEventLogData);

            MessageLinkClickedData metricLoggerData = null;
            _mockMetricLogger
                .Setup(x => x.MessageLinkClicked(It.IsAny<MessageLinkClickedData>()))
                .Callback<MessageLinkClickedData>(x => metricLoggerData = x)
                .Returns(Task.CompletedTask);

            MessageLinkClickedEventLogData eventLogData = null;
            _mockEventHubLogger
                .Setup(x => x.MessageLinkClicked(It.IsAny<MessageLinkClickedEventLogData>()))
                .Callback<MessageLinkClickedEventLogData>(x => eventLogData = x)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _systemUnderTest.LogLinkClicked(NhsLoginId, _messageLink);

            // Assert
            eventLogData.Link.Should().Be(hashedLink);
            eventLogData.MessageId.Should().Be(MessageId);
            eventLogData.SenderContextEventLogData.Should().Be(_senderContextEventLogData);

            metricLoggerData.Link.Should().Be(hashedLink);
            metricLoggerData.MessageId.Should().Be(MessageId);
            metricLoggerData.CampaignId.Should().Be(CampaignId);
            metricLoggerData.CommunicationId.Should().Be(CommunicationId);
            metricLoggerData.TransmissionId.Should().Be(TransmissionId);

            VerifyMocks();

            result.Should().BeOfType(typeof(MessageLinkClickedResult.Success));
        }

        private void VerifyMocks()
        {
            _mockValidationService.VerifyAll();
            _mockMessageRepository.VerifyAll();
            _mockSenderContextEventLogDataMapper.VerifyAll();
            _mockMetricLogger.VerifyAll();
            _mockEventHubLogger.VerifyAll();
            _mockHashingService.VerifyAll();
        }
    }
}
