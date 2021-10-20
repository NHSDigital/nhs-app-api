using System;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Mappers;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.Support.Hasher;

namespace NHSOnline.Backend.MessagesApi.UnitTests.Areas.Messages.Mappers
{
    [TestClass]
    public class MessageLinkClickedDataMapperTests
    {
        private MessageLinkClickedDataMapper _systemUnderTest;

        private Mock<ILogger<MessageLinkClickedDataMapper>> _mockLogger;
        private Mock<IHashingService> _mockHashingService;

        private const string MessageId = "MessageId";
        private const string Link = "https://testing.com/valid/url/";
        private const string CampaignId = "CampaignId";
        private const string CommunicationId = "CommunicationId";
        private const string TransmissionId = "TransmissionId";

        private MessageLink _messageLink;
        private UserMessage _userMessage;
        private readonly Uri _link = new Uri(Link);

        [TestInitialize]
        public void Setup()
        {
            _messageLink = new MessageLink
            {
                MessageId = MessageId,
                Link = _link
            };

            _userMessage = new UserMessage
            {
                SenderContext = new SenderContext
                {
                    CampaignId = CampaignId,
                    CommunicationId = CommunicationId,
                    TransmissionId = TransmissionId
                }
            };

            _mockLogger = new Mock<ILogger<MessageLinkClickedDataMapper>>();
            _mockHashingService = new Mock<IHashingService>(MockBehavior.Strict);

            _systemUnderTest = new MessageLinkClickedDataMapper(_mockLogger.Object, _mockHashingService.Object);
        }

        [TestMethod]
        public void Map_MessageLinkIsNull_ThrowsError()
        {
            Assert.ThrowsException<NullReferenceException>(() => _systemUnderTest.Map(null, _userMessage));

            VerifyMocks();
        }

        [TestMethod]
        public void Map_MessageLinkMessageIdIsNull_ThrowsError()
        {
            _messageLink = new MessageLink
            {
                MessageId = null,
                Link = _link
            };

            Assert.ThrowsException<AggregateException>(() => _systemUnderTest.Map(_messageLink, _userMessage));

            VerifyMocks();
        }

        [TestMethod]
        public void Map_MessageLinkMessageIdIsBlank_ThrowsError()
        {
            _messageLink = new MessageLink
            {
                MessageId = string.Empty,
                Link = _link
            };

            Assert.ThrowsException<AggregateException>(() => _systemUnderTest.Map(_messageLink, _userMessage));

            VerifyMocks();
        }

        [TestMethod]
        public void Map_MessageLinkLinkIsNull_ThrowsError()
        {
            _messageLink = new MessageLink
            {
                MessageId = MessageId,
                Link = null
            };

            Assert.ThrowsException<AggregateException>(() => _systemUnderTest.Map(_messageLink, _userMessage));

            VerifyMocks();
        }

        [TestMethod]
        public void Map_UserMessageIsNull_ThrowsError()
        {
            Assert.ThrowsException<AggregateException>(() => _systemUnderTest.Map(_messageLink, null));

            VerifyMocks();
        }

        [TestMethod]
        public void Map_UserMessageSenderContextIsNull_ValidDataWithUnhashedLink()
        {
            _userMessage.SenderContext = null;

            var output = _systemUnderTest.Map(_messageLink, _userMessage);

            Assert.AreEqual(MessageId, output.MessageId);
            Assert.AreEqual(Link, output.Link);
            Assert.AreEqual(null, output.CampaignId);
            Assert.AreEqual(null, output.CommunicationId);
            Assert.AreEqual(null, output.TransmissionId);

            VerifyMocks();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void Map_UserMessageSenderContextIsPopulatedWithNullOrEmptyCommunicationId_ValidDataWithUnhashedLink(string communicationId)
        {
            _userMessage.SenderContext.CommunicationId = communicationId;

            var output = _systemUnderTest.Map(_messageLink, _userMessage);

            Assert.AreEqual(MessageId, output.MessageId);
            Assert.AreEqual(Link, output.Link);
            Assert.AreEqual(CampaignId, output.CampaignId);
            Assert.AreEqual(communicationId, output.CommunicationId);
            Assert.AreEqual(TransmissionId, output.TransmissionId);

            VerifyMocks();
        }

        [TestMethod]
        public void Map_UserMessageSenderContextIsPopulatedWithCommunicationId_ValidDataWithHashedLink()
        {
            const string hashedLink = "hashedLink";
            _mockHashingService.Setup(x => x.Hash(Link)).Returns(hashedLink);

            var output = _systemUnderTest.Map(_messageLink, _userMessage);

            Assert.AreEqual(MessageId, output.MessageId);
            Assert.AreEqual(hashedLink, output.Link);
            Assert.AreEqual(CampaignId, output.CampaignId);
            Assert.AreEqual(CommunicationId, output.CommunicationId);
            Assert.AreEqual(TransmissionId, output.TransmissionId);

            VerifyMocks();
        }

        private void VerifyMocks()
        {
            _mockHashingService.VerifyAll();
        }
    }
}
