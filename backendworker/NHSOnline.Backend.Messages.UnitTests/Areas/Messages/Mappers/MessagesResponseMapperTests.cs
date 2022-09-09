using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using Moq;
using NHSOnline.Backend.Messages.Areas.Messages.Mappers;
using NHSOnline.Backend.Messages.Areas.Messages.Models;

namespace NHSOnline.Backend.Messages.UnitTests.Areas.Messages.Mappers
{
    [TestClass]
    public class MessagesResponseMapperTests
    {
        private MessagesResponseMapper _systemUnderTest;

        private UserMessageReply _messageReply;
        private UserMessageReply _noMessageReply;
        private UserMessageReply _noMessageReplyResponse;

        private enum MessageReplyOptions
        {
            MessageReply,
            NoMessageReply,
            NoMessageReplyResponse
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _systemUnderTest = new MessagesResponseMapper(new Mock<ILogger<MessagesResponseMapper>>().Object);

            _messageReply = new UserMessageReply
            {
                Options = new List<UserReplyOption>()
                {
                    new UserReplyOption() { Code = "SMOKE", Display = "SMOKE" },
                    new UserReplyOption() { Code = "NO", Display = "NO" },
                    new UserReplyOption() { Code = "NEVER", Display = "NEVER" }
                },
                Response = "NO",
                ResponseDateTime = DateTime.UtcNow
            };
            _noMessageReply = null;
            _noMessageReplyResponse = new UserMessageReply
            {
                Options = new List<UserReplyOption>()
                {
                    new UserReplyOption() { Code = "CANCEL", Display = "CANCEL" },
                },
                Response = string.Empty,
                ResponseDateTime = null
            };
        }

        [TestMethod]
        public void Map_WithUserMessages_MapsToResponse()
        {
            // Arrange
            const string senderName = "test sender";
            const string senderId = "TEST_SENDER";

            var currentMessage = new UserMessage
            {
                Sender = senderName,
                SentTime = DateTime.UtcNow,
                ReadTime = DateTime.UtcNow,
                Body = "123",
                SenderContext = new SenderContext
                {
                    SenderId = senderId
                },
                Reply = _messageReply
            };

            var oldestMessage = new UserMessage
            {
                Sender = senderName,
                SentTime = DateTime.UtcNow.AddSeconds(-10),
                ReadTime = default,
                Body = "456",
                SenderContext = new SenderContext
                {
                    SenderId = senderId
                },
                Reply = _noMessageReply
            };

            var latestMessage = new UserMessage
            {
                Sender = senderName,
                SentTime = DateTime.UtcNow.AddSeconds(10),
                ReadTime = default,
                Body = "789",
                SenderContext = new SenderContext
                {
                    SenderId = senderId
                },
                Reply = _noMessageReplyResponse
            };

            // Act
            var result = _systemUnderTest.Map(new List<UserMessage> { currentMessage, oldestMessage, latestMessage });

            // Assert
            result.SenderMessages.Should().NotBeEmpty();
            result.SenderMessages.Should().HaveCount(1);
            var resultSenderMessage = result.SenderMessages.First();
            resultSenderMessage.Sender.Should().Be(senderName);
            resultSenderMessage.UnreadCount.Should().Be(2);

            var resultMessages = resultSenderMessage.Messages;
            resultMessages.Should().NotBeNull();
            resultMessages.Should().HaveCount(3);

            var expectedMessages = MapToMessages(latestMessage, currentMessage, oldestMessage);

            resultMessages[0].Should().BeEquivalentTo(expectedMessages[0]);
            resultMessages[1].Should().BeEquivalentTo(expectedMessages[1]);
            resultMessages[2].Should().BeEquivalentTo(expectedMessages[2]);

            VerifyMessageReply( resultMessages[0], MessageReplyOptions.NoMessageReplyResponse);
            VerifyMessageReply( resultMessages[1], MessageReplyOptions.MessageReply);
            VerifyMessageReply( resultMessages[2], MessageReplyOptions.NoMessageReply);
        }

        private static void VerifyMessageReply(Message resultMessage, MessageReplyOptions messageReplyOptions)
        {
            switch (@messageReplyOptions)
            {
                case MessageReplyOptions.MessageReply:
                    resultMessage.Reply.Should().NotBeNull();
                    resultMessage.Reply.Options.Should().HaveCount(3);
                    resultMessage.Reply.Options[0].Code.Should().NotBeNullOrEmpty();
                    resultMessage.Reply.Options[1].Code.Should().NotBeNullOrEmpty();
                    resultMessage.Reply.Options[2].Code.Should().NotBeNullOrEmpty();
                    resultMessage.Reply.Response.Should().NotBeNullOrEmpty();
                    resultMessage.Reply.ResponseDateTime.Should().NotBeNull();
                    break;
                case MessageReplyOptions.NoMessageReply:
                    resultMessage.Reply.Should().BeNull();
                    break;
                case MessageReplyOptions.NoMessageReplyResponse:
                    resultMessage.Reply.Should().NotBeNull();
                    resultMessage.Reply.Options.Should().HaveCount(1);
                    resultMessage.Reply.Options[0].Code.Should().NotBeNullOrEmpty();
                    resultMessage.Reply.Response.Should().BeNullOrEmpty();
                    resultMessage.Reply.ResponseDateTime.Should().BeNull();
                    break;
                default:
                    break;
            }
        }

        [TestMethod]
        public void Map_WithNoUserMessages_MapsToEmptyResponse()
        {
            // Act
            var result = _systemUnderTest.Map(new List<UserMessage>());

            // Assert
            result.SenderMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void Map_WhenUserMessagesIsNull_ThrowsException()
        {
            // Act
            Action act = () => _systemUnderTest.Map(default(List<UserMessage>));

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("source");
        }

        [TestMethod]
        public void Map_WithSummaryMessages_MapsToResponse()
        {
            // Arrange
            var currentMessage = new SummaryMessage
            {
                SentTime = DateTime.UtcNow ,
                SenderContext = new SenderContext { SenderId = "TEST_SENDER" },
                Reply = _messageReply
            };

            var oldestMessage = new SummaryMessage
            {
                SentTime = DateTime.UtcNow.AddSeconds(-10),
                SenderContext = new SenderContext { SenderId = "TEST_SENDER" },
                Reply = _noMessageReply
            };

            var latestMessage = new SummaryMessage
            {
                SentTime = DateTime.UtcNow.AddSeconds(10),
                SenderContext = new SenderContext { SenderId = "TEST_SENDER" },
                Reply = _noMessageReplyResponse
            };
            var expectedMessages = new List<SenderMessages>
            {
                new SenderMessages
                {
                    Sender = latestMessage.Sender,
                    UnreadCount = latestMessage.UnreadCount,
                    Messages = MapToMessages(latestMessage),
                },
                new SenderMessages
                {
                    Sender = currentMessage.Sender,
                    UnreadCount = currentMessage.UnreadCount,
                    Messages = MapToMessages(currentMessage)
                },
                new SenderMessages
                {
                    Sender = oldestMessage.Sender,
                    UnreadCount = oldestMessage.UnreadCount,
                    Messages = MapToMessages(oldestMessage)
                }
            };
            // Act
            var result = _systemUnderTest.Map(new List<SummaryMessage>
                { currentMessage, oldestMessage, latestMessage });

            // Assert
            result.SenderMessages.Should().NotBeEmpty();
            result.SenderMessages.Should().HaveCount(3);
            result.SenderMessages.Should().BeEquivalentTo(expectedMessages);

            VerifyMessageReply(result.SenderMessages[0].Messages[0], MessageReplyOptions.NoMessageReplyResponse);
            VerifyMessageReply(result.SenderMessages[1].Messages[0], MessageReplyOptions.MessageReply);
            VerifyMessageReply(result.SenderMessages[2].Messages[0], MessageReplyOptions.NoMessageReply);
        }

        [TestMethod]
        public void Map_WithNoSummaryMessages_MapsToEmptyResponse()
        {
            // Act
            var result = _systemUnderTest.Map(new List<SummaryMessage>());

            // Assert
            result.SenderMessages.Should().BeEmpty();
        }

        [TestMethod]
        public void Map_WhenSummaryMessagesIsNull_ThrowsException()
        {
            // Act
            Action act = () => _systemUnderTest.Map(default(List<SummaryMessage>));

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("source");
        }

        [TestMethod]
        public void Map_WithUserMessages_WhenBodyExceeds240Chars_TruncateBody()
        {
            // Arrange
            var message = new UserMessage
            {
                Sender = "test sender",
                SentTime = DateTime.UtcNow.AddSeconds(-10),
                ReadTime = default,
                Body = $"Nam quis nulla. Integer malesuada. In in enim a arcu imperdiet malesuada. Sed vel lectus. " +
                       $"Donec odio urna, tempus molestie, porttitor ut, iaculis quis, sem. Phasellus rhoncus. Aenean id " +
                       $"metus id velit ullamcorper pulvinar. Vestibulum fermen#",
                SenderContext = new SenderContext
                {
                    SenderId = "TEST_SENDER"
                },
                Reply = new UserMessageReply
                {
                    Options = new List<UserReplyOption>()
                    {
                        new UserReplyOption() { Code = "CANCEL", Display = "CANCEL" },
                    },
                    Response = string.Empty,
                    ResponseDateTime = null
                }
            };

            // Act
            var result = _systemUnderTest.Map(new List<UserMessage> {message});

            // Assert
            result.Should().NotBeNull();
            result.SenderMessages.Should().ContainSingle();
            var resultMessage = result.SenderMessages.First().Messages.Should().ContainSingle().Subject;
            resultMessage.Body.Should().Be($"Nam quis nulla. Integer malesuada. In in enim a arcu imperdiet malesuada. Sed vel lectus. " +
                                           $"Donec odio urna, tempus molestie, porttitor ut, iaculis quis, sem. Phasellus rhoncus. Aenean id " +
                                           $"metus id velit ullamcorper pulvinar. Vestibulum fermen");
            var resultMessageCheck = result.SenderMessages.First();
            VerifyMessageReply(resultMessageCheck.Messages[0], MessageReplyOptions.NoMessageReplyResponse);
        }

        [DataTestMethod]
        [DataRow("Nam quis nulla. Integer malesuada. In in enim a arcu imperdiet malesuada. Sed vel lectus. " +
                 "Donec odio urna, tempus molestie, porttitor ut, iaculis quis, sem. Phasellus rhoncus. Aenean id " +
                 "metus id velit ullamcorper pulvinar. Vestibulum fermen")]
        [DataRow("Nam quis nulla. Integer malesuada. In in enim a arcu imperdiet malesuada.")]
        public void Map_WithUserMessages_WhenBodyLessThanOrEquals240Chars_DoesNotTruncateBody(string body)
        {
            // Arrange
            var message = new UserMessage
            {
                Sender = "test sender",
                SentTime = DateTime.UtcNow.AddSeconds(-10),
                ReadTime = default,
                Body = body,
                SenderContext = new SenderContext
                {
                    SenderId = "TEST_SENDER"
                },
                Reply = new UserMessageReply
                {
                    Options = new List<UserReplyOption>()
                    {
                        new UserReplyOption() { Code = "CANCEL", Display = "CANCEL" },
                    },
                    Response = string.Empty,
                    ResponseDateTime = null
                }
            };

            // Act
            var result = _systemUnderTest.Map(new List<UserMessage> {message});

            // Assert
            result.Should().NotBeNull();
            result.SenderMessages.Should().ContainSingle();
            var resultMessage = result.SenderMessages.First().Messages.Should().ContainSingle().Subject;
            resultMessage.Body.Should().Be(body);
            var resultMessageCheck = result.SenderMessages.First();
            VerifyMessageReply(resultMessageCheck.Messages[0], MessageReplyOptions.NoMessageReplyResponse);
        }

        [TestMethod]
        public void Map_WithUserMessage_MapsToResponse()
        {
            // Arrange
            var sentTime = DateTime.UtcNow;
            var message = new UserMessage
            {
                Id = new ObjectId("ae0b4ffd40c44828b884961b"),
                SenderContext = new SenderContext { SenderId = "TEST_SENDER" },
                Sender = "test sender",
                SentTime = sentTime,
                ReadTime = default,
                Body = $"Nam quis nulla. Integer malesuada. In in enim a arcu imperdiet malesuada. Sed vel lectus. " +
                       $"Donec odio urna, tempus molestie, porttitor ut, iaculis quis, sem. Phasellus rhoncus. Aenean id " +
                       $"metus id velit ullamcorper pulvinar. Vestibulum fermen#",
                Version = 3,
                Reply = new UserMessageReply
                {
                    Options = new List<UserReplyOption>()
                    {
                        new UserReplyOption() { Code = "CANCEL", Display = "CANCEL" },
                    },
                    Response = string.Empty,
                    ResponseDateTime = null
                }
            };

            // Act
            var result = _systemUnderTest.Map(message);

            // Assert
            result.Should().NotBeNull();
            result.SenderMessages.Should().ContainSingle();
            var resultMessage = result.SenderMessages.First().Messages.Should().ContainSingle().Subject;

            resultMessage.Id.Should().Be("ae0b4ffd40c44828b884961b");
            resultMessage.SenderId.Should().Be("TEST_SENDER");
            resultMessage.Sender.Should().Be("test sender");
            resultMessage.SentTime.Should().Be(sentTime);
            resultMessage.Read.Should().Be(false);
            resultMessage.Body.Should().Be(
                $"Nam quis nulla. Integer malesuada. In in enim a arcu imperdiet malesuada. Sed vel lectus. Donec odio " +
                $"urna, tempus molestie, porttitor ut, iaculis quis, sem. Phasellus rhoncus. Aenean id metus id velit " +
                $"ullamcorper pulvinar. Vestibulum fermen#");
            resultMessage.Version.Should().Be(3);
            var resultMessageCheck = result.SenderMessages.First();
            VerifyMessageReply(resultMessageCheck.Messages[0], MessageReplyOptions.NoMessageReplyResponse);
        }

        [TestMethod]
        public void Map_WhenUserMessageIsNull_ThrowsException()
        {
            // Arrange, Act, Assert
            FluentActions
                .Invoking(() => _systemUnderTest.Map(default(UserMessage)))
                .Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("source");
        }

        private List<Message> MapToMessages(params UserMessage[] userMessages)
            => userMessages.Select(m => new Message
            {
                Id = m.Id.ToString(),
                SenderId = m.SenderContext.SenderId,
                Sender = m.Sender,
                Version = m.Version,
                Body = m.Body,
                Read = m.ReadTime.HasValue,
                SentTime = m.SentTime,
                Reply = MapMessageReply(m.Reply)
            }).ToList();

        private MessageReply MapMessageReply(UserMessageReply userMessageReply)
        {
            if (userMessageReply != null)
            {
                return new MessageReply()
                {
                    Options = MapMessageReplyOptions(userMessageReply.Options),
                    Response = userMessageReply.Response,
                    ResponseDateTime = userMessageReply.ResponseDateTime
                };
            }
            return null;
        }

        private List<ReplyOption> MapMessageReplyOptions(IReadOnlyCollection<UserReplyOption> userReplyOption)
        {
            var options = userReplyOption?.Select(o => new ReplyOption
                {
                    Code = o.Code,
                    Display = o.Display
                })
                .ToList();
            return options;
        }
    }
}