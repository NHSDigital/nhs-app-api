using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using Moq;
using NHSOnline.Backend.MessagesApi.Areas.Messages;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.Metrics.EventHub;
using NHSOnline.Backend.PfsApi.Messages;
using NHSOnline.Backend.Support;
using MessagesResult = NHSOnline.Backend.PfsApi.Messages.MessagesResult;

namespace NHSOnline.Backend.PfsApi.UnitTests.Messages
{
    [TestClass]
    public class IntroMessagesServiceTests
    {
        private IntroMessagesService _systemUnderTest;

        private Mock<ILogger<IntroMessagesService>> _mockLogger;
        private Mock<IIntroMessagesServiceConfig> _mockIntroMessagesServiceConfig;
        private Mock<IMessageService> _mockMessageService;
        private Mock<IEventHubLogger> _mockEventHubLogger;
        private Mock<IMapper<SenderContext, SenderContextEventLogData>> _mockSenderContextEventLogDataMapper;

        private const string CampaignId = "CampaignId";
        private const string MessageId = "ae0b4ffd40c44828b884961b";
        private const string NhsLoginId = "NhsLoginId";
        private const string SupplierId = "DrDre";
        private const string CommunicationId = "Compton";
        private const string TransmissionId = "All in a Day's Work";
        private const string RequestReference = "I'm Working";
        private const string OdsCode = "Easy E";
        private const string NhsNumber = "123456789";

        [TestInitialize]
        public void TestInitialize()
        {
            _mockLogger = new Mock<ILogger<IntroMessagesService>>();
            _mockIntroMessagesServiceConfig = new Mock<IIntroMessagesServiceConfig>(MockBehavior.Strict);
            _mockMessageService = new Mock<IMessageService>(MockBehavior.Strict);
            _mockEventHubLogger = new Mock<IEventHubLogger>(MockBehavior.Strict);
            _mockSenderContextEventLogDataMapper =
                new Mock<IMapper<SenderContext, SenderContextEventLogData>>(MockBehavior.Strict);

            _systemUnderTest = new IntroMessagesService(
                _mockLogger.Object,
                _mockIntroMessagesServiceConfig.Object,
                _mockMessageService.Object,
                _mockEventHubLogger.Object,
                _mockSenderContextEventLogDataMapper.Object
                );
        }

        [TestMethod]
        public async Task SendIntroductoryMessage_MessagesTurnedOff_NoActionTaken()
        {
            // Arrange
            SetupConfig(sendIntroductoryMessage: false);

            // Act
            var result = await _systemUnderTest.SendIntroductoryMessage(NhsLoginId);

            // Assert
            VerifyAsserts();
            result.Should().BeOfType<MessagesResult.NoActionTaken>();
        }

        [TestMethod]
        public async Task SendIntroductoryMessage_MessagesTurnedOn_Success()
        {
            // Arrange
            SetupConfig(sendIntroductoryMessage: true);

            var senderContext = new SenderContext();

            var response = new AddMessageResult.Success(
                new UserMessage
                {
                    Id = new ObjectId(MessageId),
                    Timestamp = new DateTime(2021, 04, 22, 01, 05, 25),
                    SenderContext = senderContext
                }
            );

            _mockMessageService
                .Setup(x => x.Send(
                    It.Is<AddMessageRequest>(r => r.SenderContext.NhsLoginId == NhsLoginId),
                    NhsLoginId))
                .ReturnsAsync(response);

            _mockSenderContextEventLogDataMapper
                .Setup(x => x.Map(senderContext))
                .Returns(It.IsAny<SenderContextEventLogData>());

            _mockEventHubLogger
                .Setup(x => x.MessageCreated(It.IsAny<MessageCreatedEventLogData>()));

            // Act
            var result = await _systemUnderTest.SendIntroductoryMessage(NhsLoginId);

            // Assert
            VerifyAsserts();
            result.Should().BeOfType<MessagesResult.Success>();
        }

        [TestMethod]
        [DataRow(typeof(AddMessageResult.BadGateway))]
        [DataRow(typeof(AddMessageResult.BadRequest))]
        [DataRow(typeof(AddMessageResult.InternalServerError))]
        public async Task SendIntroductoryMessage_MessageServiceError_ReturnsInternalServerError(Type addMessageResultType)
        {
            // Arrange
            SetupConfig(sendIntroductoryMessage: true);

            var response = (AddMessageResult)Activator.CreateInstance(addMessageResultType);

            _mockMessageService
                .Setup(x => x.Send(
                    It.Is<AddMessageRequest>(r => r.SenderContext.NhsLoginId == NhsLoginId),
                    NhsLoginId))
                .ReturnsAsync(response);

            // Act
            var result = await _systemUnderTest.SendIntroductoryMessage(NhsLoginId);

            // Assert
            VerifyAsserts();
            result.Should().BeOfType<MessagesResult.InternalServerError>();
        }

        private void SetupConfig(bool sendIntroductoryMessage)
        {
            _mockIntroMessagesServiceConfig.Setup(x => x.SendIntroductoryMessage).Returns(sendIntroductoryMessage);

            if (sendIntroductoryMessage)
            {
                _mockIntroMessagesServiceConfig.Setup(x => x.Body).Returns("Body");
                _mockIntroMessagesServiceConfig.Setup(x => x.CampaignId).Returns(CampaignId);
            }
        }

        private void VerifyAsserts()
        {
            _mockIntroMessagesServiceConfig.VerifyAll();
        }

        private SenderContextEventLogData CreateEventLogData()
        {
            return new SenderContextEventLogData(
                SupplierId,
                CommunicationId,
                TransmissionId,
                DateTime.Today,
                RequestReference,
                CampaignId,
                OdsCode,
                NhsNumber,
                NhsLoginId
            );
        }
    }
}
