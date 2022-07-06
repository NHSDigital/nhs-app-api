using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using Moq;
using NHSOnline.Backend.Messages.Areas.Messages;
using NHSOnline.Backend.Messages.Areas.Messages.Models;
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
        private const string Body = "Body";

        private const string SenderName = "NHS App";
        private const string SupplierId = "278d3b75-3498-4d68-8991-506d0006e46f";
        // private const string SenderId = "NHSAPP";        // NHSO-20472 / NHSO-21187
        private const int Version = 1;

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

            AddMessageRequest messageRequest = null;
            _mockMessageService
                .Setup(x => x.Send(It.IsAny<AddMessageRequest>(), NhsLoginId))
                .Callback<AddMessageRequest, string>((amr, login) => messageRequest = amr)
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
            VerifyMessageRequest(messageRequest);
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

            AddMessageRequest messageRequest = null;
            _mockMessageService
                .Setup(x => x.Send(It.IsAny<AddMessageRequest>(), NhsLoginId))
                .Callback<AddMessageRequest, string>((amr, login) => messageRequest = amr)
                .ReturnsAsync(response);

            // Act
            var result = await _systemUnderTest.SendIntroductoryMessage(NhsLoginId);

            // Assert
            VerifyAsserts();
            result.Should().BeOfType<MessagesResult.InternalServerError>();
            VerifyMessageRequest(messageRequest);
        }

        private void SetupConfig(bool sendIntroductoryMessage)
        {
            _mockIntroMessagesServiceConfig.Setup(x => x.SendIntroductoryMessage).Returns(sendIntroductoryMessage);

            if (sendIntroductoryMessage)
            {
                _mockIntroMessagesServiceConfig.Setup(x => x.Body).Returns(Body);
                _mockIntroMessagesServiceConfig.Setup(x => x.CampaignId).Returns(CampaignId);
                // _mockIntroMessagesServiceConfig.Setup(x => x.SenderId).Returns(SenderId);    // NHSO-20472 / NHSO-21187
            }
        }

        private void VerifyAsserts()
        {
            _mockIntroMessagesServiceConfig.VerifyAll();
        }

        private static void VerifyMessageRequest(AddMessageRequest messageRequest)
        {
            messageRequest.Sender.Should().Be(SenderName);
            messageRequest.Body.Should().Be(Body);
            messageRequest.Version.Should().Be(Version);
            messageRequest.SenderContext.CampaignId.Should().Be(CampaignId);
            // messageRequest.SenderContext.SenderId.Should().Be(SenderId); // NHSO-20472 / NHSO-21187
            messageRequest.SenderContext.NhsLoginId.Should().Be(NhsLoginId);
            messageRequest.SenderContext.SupplierId.Should().Be(SupplierId);
        }
    }
}
