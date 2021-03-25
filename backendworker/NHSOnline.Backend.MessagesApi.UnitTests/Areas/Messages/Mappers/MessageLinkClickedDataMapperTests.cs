using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Mappers;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.MessagesApi.UnitTests.Areas.Messages.Mappers
{
    [TestClass]
    public class MessageLinkClickedDataMapperTests
    {
        private MessageLinkClickedDataMapper _systemUnderTest;

        private Mock<ILogger<MessageLinkClickedDataMapper>> _mockLogger;

        private const string MessageId = "MessageId";
        private const string Link = "https://testing.com/valid/url/";
        private const string CampaignId = "CampaignId";
        private const string CommunicationId = "CommunicationId";
        private const string TransmissionId = "TransmissionId";
        private const string SenderContextCommunicationId = "SenderContextCommunicationId";
        private const string SenderContextTransmissionId = "SenderContextTransmissionId";

        private MessageLink _messageLink;
        private RepositoryFindResult<UserMessage>.Found _found;
        private Uri _link;

        [TestInitialize]
        public void Setup()
        {
            _link = new Uri(Link);

            _messageLink = new MessageLink
            {
                MessageId = MessageId,
                Link = _link
            };

            _found = new RepositoryFindResult<UserMessage>.Found(new [] { new UserMessage() });

            _mockLogger = new Mock<ILogger<MessageLinkClickedDataMapper>>();

            _systemUnderTest = new MessageLinkClickedDataMapper(_mockLogger.Object);
        }

        [TestMethod]
        public void Map_MessageLinkIsNull_ThrowsError()
        {
            Assert.ThrowsException<NullReferenceException>(() => _systemUnderTest.Map(null, _found));
        }

        [TestMethod]
        public void Map_MessageLinkMessageIdIsNull_ThrowsError()
        {
            _messageLink = new MessageLink
            {
                MessageId = null,
                Link = new Uri(Link)
            };

            Assert.ThrowsException<AggregateException>(() => _systemUnderTest.Map(_messageLink, _found));
        }

        [TestMethod]
        public void Map_MessageLinkMessageIdIsBlank_ThrowsError()
        {
            _messageLink = new MessageLink
            {
                MessageId = string.Empty,
                Link = new Uri(Link)
            };

            Assert.ThrowsException<AggregateException>(() => _systemUnderTest.Map(_messageLink, _found));
        }

        [TestMethod]
        public void Map_MessageLinkLinkIsNull_ThrowsError()
        {
            _messageLink = new MessageLink
            {
                MessageId = MessageId,
                Link = null
            };

            Assert.ThrowsException<AggregateException>(() => _systemUnderTest.Map(_messageLink, _found));
        }

        [TestMethod]
        public void Map_FoundIsNull_ValidData()
        {
            var output = _systemUnderTest.Map(_messageLink, null);

            Assert.AreEqual(MessageId, output.MessageId);
            Assert.AreEqual(_link, output.Link);
            Assert.AreEqual(null, output.CampaignId);
            Assert.AreEqual(null, output.CommunicationId);
            Assert.AreEqual(null, output.TransmissionId);
        }

        [TestMethod]
        public void Map_UserMessageIsNull_ValidData()
        {
            var found = new RepositoryFindResult<UserMessage>.Found(new List<UserMessage> { null });

            var output = _systemUnderTest.Map(_messageLink, found);

            Assert.AreEqual(MessageId, output.MessageId);
            Assert.AreEqual(_link, output.Link);
            Assert.AreEqual(null, output.CampaignId);
            Assert.AreEqual(null, output.CommunicationId);
            Assert.AreEqual(null, output.TransmissionId);
        }

        [TestMethod]
        public void Map_UserMessageSenderContextIsNull_ValidData()
        {
            var found = new RepositoryFindResult<UserMessage>.Found(new[]
            {
                new UserMessage
                {
                    CommunicationId = CommunicationId,
                    TransmissionId = TransmissionId,
                    SenderContext = null
                }
            });

            var output = _systemUnderTest.Map(_messageLink, found);

            Assert.AreEqual(MessageId, output.MessageId);
            Assert.AreEqual(_link, output.Link);
            Assert.AreEqual(null, output.CampaignId);
            Assert.AreEqual(CommunicationId, output.CommunicationId);
            Assert.AreEqual(TransmissionId, output.TransmissionId);
        }

        [TestMethod]
        public void Map_UserMessageSenderContextIsPopulated_ValidData()
        {
            var found = new RepositoryFindResult<UserMessage>.Found(new []
            {
                new UserMessage
                {
                    CommunicationId = CommunicationId,
                    TransmissionId = TransmissionId,
                    SenderContext = new SenderContext
                    {
                        CampaignId = CampaignId,
                        CommunicationId = SenderContextCommunicationId,
                        TransmissionId = SenderContextTransmissionId
                    }
                }
            });

            var output = _systemUnderTest.Map(_messageLink, found);

            Assert.AreEqual(MessageId, output.MessageId);
            Assert.AreEqual(_link, output.Link);
            Assert.AreEqual(CampaignId, output.CampaignId);
            Assert.AreEqual(SenderContextCommunicationId, output.CommunicationId);
            Assert.AreEqual(SenderContextTransmissionId, output.TransmissionId);
        }
    }
}
