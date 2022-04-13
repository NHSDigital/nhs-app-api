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

        [TestInitialize]
        public void TestInitialize()
        {
            _systemUnderTest = new MessagesResponseMapper(new Mock<ILogger<MessagesResponseMapper>>().Object);
        }

        [TestMethod]
        public void Map_WithUserMessages_MapsToResponse()
        {
            // Arrange
            const string sender = "test sender";
            var currentMessage = new UserMessage
            {
                Sender = sender,
                SentTime = DateTime.UtcNow,
                ReadTime = DateTime.UtcNow,
                Body = "123"
            };

            var oldestMessage = new UserMessage
            {
                Sender = sender,
                SentTime = DateTime.UtcNow.AddSeconds(-10),
                ReadTime = default,
                Body = "456"
            };

            var latestMessage = new UserMessage
            {
                Sender = sender,
                SentTime = DateTime.UtcNow.AddSeconds(10),
                ReadTime = default,
                Body = "789"
            };

            // Act
            var result = _systemUnderTest.Map(new List<UserMessage> { currentMessage, oldestMessage, latestMessage });

            // Assert
            result.SenderMessages.Should().NotBeEmpty();
            result.SenderMessages.Should().HaveCount(1);
            var resultSenderMessage = result.SenderMessages.First();
            resultSenderMessage.Sender.Should().Be(sender);
            resultSenderMessage.UnreadCount.Should().Be(2);

            var resultMessages = resultSenderMessage.Messages;
            resultMessages.Should().NotBeNull();
            resultMessages.Should().HaveCount(3);

            var expectedMessages = MapToMessages(latestMessage, currentMessage, oldestMessage);

            resultMessages[0].Should().BeEquivalentTo(expectedMessages[0]);
            resultMessages[1].Should().BeEquivalentTo(expectedMessages[1]);
            resultMessages[2].Should().BeEquivalentTo(expectedMessages[2]);
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
            var currentMessage = new SummaryMessage { SentTime = DateTime.UtcNow };

            var oldestMessage = new SummaryMessage { SentTime = DateTime.UtcNow.AddSeconds(-10) };

            var latestMessage = new SummaryMessage { SentTime = DateTime.UtcNow.AddSeconds(10) };

            // Act
            var result = _systemUnderTest.Map(new List<SummaryMessage>
                { currentMessage, oldestMessage, latestMessage });

            // Assert
            result.SenderMessages.Should().NotBeEmpty();
            result.SenderMessages.Should().HaveCount(3);
            result.SenderMessages.Should().BeEquivalentTo(new List<SenderMessages>
            {
                new SenderMessages
                {
                    Sender = latestMessage.Sender,
                    UnreadCount = latestMessage.UnreadCount,
                    Messages =  MapToMessages(latestMessage)
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
                },
            });
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
                       $"metus id velit ullamcorper pulvinar. Vestibulum fermen#"
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
                Body = body
            };

            // Act
            var result = _systemUnderTest.Map(new List<UserMessage> {message});

            // Assert
            result.Should().NotBeNull();
            result.SenderMessages.Should().ContainSingle();
            var resultMessage = result.SenderMessages.First().Messages.Should().ContainSingle().Subject;
            resultMessage.Body.Should().Be(body);
        }

        [TestMethod]
        public void Map_WithUserMessage_MapsToResponse()
        {
            // Arrange
            var sentTime = DateTime.UtcNow;
            var message = new UserMessage
            {
                Id = new ObjectId("ae0b4ffd40c44828b884961b"),
                Sender = "test sender",
                SentTime = sentTime,
                ReadTime = default,
                Body = $"Nam quis nulla. Integer malesuada. In in enim a arcu imperdiet malesuada. Sed vel lectus. " +
                       $"Donec odio urna, tempus molestie, porttitor ut, iaculis quis, sem. Phasellus rhoncus. Aenean id " +
                       $"metus id velit ullamcorper pulvinar. Vestibulum fermen#",
                Version = 3
            };

            // Act
            var result = _systemUnderTest.Map(message);

            // Assert
            result.Should().NotBeNull();
            result.SenderMessages.Should().ContainSingle();
            var resultMessage = result.SenderMessages.First().Messages.Should().ContainSingle().Subject;

            resultMessage.Id.Should().Be("ae0b4ffd40c44828b884961b");
            resultMessage.Sender.Should().Be("test sender");
            resultMessage.SentTime.Should().Be(sentTime);
            resultMessage.Read.Should().Be(false);
            resultMessage.Body.Should().Be(
                $"Nam quis nulla. Integer malesuada. In in enim a arcu imperdiet malesuada. Sed vel lectus. Donec odio " +
                $"urna, tempus molestie, porttitor ut, iaculis quis, sem. Phasellus rhoncus. Aenean id metus id velit " +
                $"ullamcorper pulvinar. Vestibulum fermen#");
            resultMessage.Version.Should().Be(3);
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
                Sender = m.Sender,
                Version = m.Version,
                Body = m.Body,
                Read = m.ReadTime.HasValue,
                SentTime = m.SentTime
            }).ToList();
    }
}