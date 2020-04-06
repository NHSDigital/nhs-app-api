using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Messages.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientPracticeMessaging;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientPracticeMessaging;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.PatientPracticeMessaging
{
    [TestClass]
    public class TppPatientPracticeMessagesMapperTests
    {
        private IFixture _fixture;

        private TppPatientMessagesMapper _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _systemUnderTest = _fixture.Create<TppPatientMessagesMapper>();
        }

        [TestMethod]
        public void Map_WhenCalledWithSuccessResponse_ReturnsMappedGetPatientMessagesResponse()
        {
            // Arrange
            var message = new Message
            {
                MessageId = "1",
                ConversationId = "1",
                MessageText = _fixture.Create<string>(),
                ReadString = "y",
                IncomingString = "y",
                Recipient = _fixture.Create<string>(),
                Sender = _fixture.Create<string>(),
                Sent = "2020-02-03T11:08:32.0Z"
            };

            var messagesViewReply = new MessagesViewReply
            {
                Messages = new List<Message>{ message }
            };

            // Act
            var result = _systemUnderTest.Map(messagesViewReply);

            // Assert
            result.Should().BeEquivalentTo(new GetPatientMessagesResponse
            {
                MessageSummaries = messagesViewReply.Messages.Select(m => new PatientMessageSummary
                {
                    MessageId = m.MessageId,
                    ConversationId = m.ConversationId,
                    Content = m.MessageText,
                    Recipient = m.Recipient,
                    LastMessageDateTime = "2020-02-03T11:08:32",
                    HasUnreadReplies = false,
                    UnreadCount = 0,
                    Sender = m.Sender,
                    SentDateTime = "2020-02-03T11:08:32",
                    Replies = new List<MessageReply>(),
                    OutboundMessage = true
                }).ToList()
            });
        }

        [TestMethod]
        public void Map_WhenCalledWithSuccessResponse_ReturnsMappedGetPatientMessagesResponseWithReplies()
        {
            // Arrange
            var message = new Message
            {
                MessageId = "1",
                ConversationId = "1",
                MessageText = _fixture.Create<string>(),
                ReadString = "y",
                IncomingString = "y",
                Recipient = _fixture.Create<string>(),
                Sender = _fixture.Create<string>(),
                Sent = "2020-04-01T12:00:00Z"
            };

            var messageFirstReply = new Message
            {
                MessageId = "2",
                ConversationId = "1",
                MessageText = "test 1",
                ReadString = "y",
                IncomingString = "y",
                Recipient = _fixture.Create<string>(),
                Sender = _fixture.Create<string>(),
                Sent = "2020-04-02T12:00:00Z"
            };

            var messageSecondReply = new Message
            {
                MessageId = "3",
                ConversationId = "1",
                MessageText = "test 2",
                ReadString = "y",
                IncomingString = "y",
                Recipient = _fixture.Create<string>(),
                Sender = _fixture.Create<string>(),
                Sent = "2020-04-03T12:00:00Z"
            };

            var messagesViewReply = new MessagesViewReply
            {
                Messages = new List<Message>{ message, messageFirstReply, messageSecondReply }
            };

            // Act
            var result = _systemUnderTest.Map(messagesViewReply);

            var replies = new List<MessageReply>
            {
                new MessageReply
                {
                    Sender = messageFirstReply.Sender,
                    SentDateTime = messageFirstReply.Sent,
                    IsUnread = messageFirstReply.Read == YesNo.n,
                    ReplyContent = messageFirstReply.MessageText,
                    OutboundMessage = messageFirstReply.Incoming == YesNo.y
                },
                new MessageReply
                {
                    Sender = messageSecondReply.Sender,
                    SentDateTime = messageSecondReply.Sent,
                    IsUnread = messageSecondReply.Read == YesNo.n,
                    ReplyContent = messageSecondReply.MessageText,
                    OutboundMessage = messageSecondReply.Incoming == YesNo.y
                }
            };

            // Assert
            result.MessageSummaries[0].Replies.Count.Should().Be(2);
            result.MessageSummaries[0].Replies[0].ReplyContent.Should().Be(replies[0].ReplyContent);
            result.MessageSummaries[0].Replies[1].ReplyContent.Should().Be(replies[1].ReplyContent);

        }

        [TestMethod]
        public void Map_WhenCalledWithSuccessResponse_OrdersMessagesByDateDesc()
        {
            // Arrange
            var messagesViewReply = new MessagesViewReply
            {
                Messages = Enumerable.Range(1, 3).Select(i => new Message
                {
                    Sent = $"2018-0{i}-01T12:00:00Z",
                    MessageId = $"{i}",
                    ConversationId = $"{i}",
                    MessageText = _fixture.Create<string>(),
                    Recipient = _fixture.Create<string>(),
                    Sender = _fixture.Create<string>(),
                    ReadString = "y",
                    IncomingString = "y"
                }).ToList()
            };

            // Act
            var result = _systemUnderTest.Map(messagesViewReply);

            // Assert
            result.MessageSummaries.Select(m => m.MessageId)
                .ToList().Should().BeEquivalentTo(new List<string>{"3", "2", "1"});
        }

        [TestMethod]
        public void Map_WhenCalledWithSuccessResponseWithNotIncomingAndUniqueConversationId_SetsRecipientToSender()
        {
            // Arrange
            var message = new Message
            {
                MessageId = "123",
                ConversationId = "123",
                Sender = "Test sender",
                Sent = "2018-01-01T12:00:00Z"
            };


            var messagesViewReply = new MessagesViewReply
            {
                Messages = new List<Message>{ message }
            };

            // Act
            var result = _systemUnderTest.Map(messagesViewReply);

            // Assert
            result.MessageSummaries
                .First().Recipient
                .Should()
                .Be("Test sender");
        }

        [TestMethod]
        public void Map_WhenCalledWithSuccessResponseWithUnreadReplies_SetsUnreadRepliesAndUnreadCount()
        {
            // Arrange
            var parentMessage = new Message
            {
                MessageId = "1",
                ConversationId = "1",
                MessageText = "This is the first message",
                ReadString = "y",
                Recipient = "Test Recipient",
                Sender = "Test Sender",
                Sent = "2018-01-01T12:00:00Z"
            };

            var childMessage = new Message
            {
                MessageId = "2",
                ConversationId = "1",
                MessageText = "This is the reply",
                Recipient = "Test recipient",
                Sender = "Test Sender",
                Sent = "2018-01-01T12:20:00Z"
            };

            var messagesViewReply = new MessagesViewReply
            {
                Messages = new List<Message>{  parentMessage, childMessage }
            };

            // Act
            var result = _systemUnderTest.Map(messagesViewReply);

            // Assert
            result.MessageSummaries.First().HasUnreadReplies.Should().BeTrue();
            result.MessageSummaries.First().UnreadCount.Should().Be(1);
        }

        [TestMethod]
        public void Map_WhenCalledWithSuccessResponseWithDeletedFlag_ShouldNotBeReturned()
        {
            // Arrange
            var deletedMessage = new Message
            {
                MessageId = "1",
                ConversationId = "1",
                MessageText = "This is the first message",
                ReadString = "y",
                IncomingString = "y",
                Recipient = "Test Recipient",
                Sender = "Test Sender",
                DeletedString = "y"
            };

            var messagesViewReply = new MessagesViewReply
            {
                Messages = new List<Message>{ deletedMessage }
            };

            // Act
            var result = _systemUnderTest.Map(messagesViewReply);

            // Assert
            result.MessageSummaries.Should().BeEmpty();
        }

        [TestMethod]
        public void Map_WhenCalledWithResponseWithNoMessages_ReturnsEmptyArray()
        {
            // Act
            var result = _systemUnderTest.Map(new MessagesViewReply());

            // Assert
            result.MessageSummaries.Should().BeEmpty();
        }
    }
}