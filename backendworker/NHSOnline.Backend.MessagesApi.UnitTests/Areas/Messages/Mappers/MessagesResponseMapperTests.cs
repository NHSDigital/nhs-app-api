using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Mappers;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;

namespace NHSOnline.Backend.MessagesApi.UnitTests.Areas.Messages.Mappers
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
                ReadTime = DateTime.UtcNow
            };

            var oldestMessage = new UserMessage
            {
                Sender = sender,
                SentTime = DateTime.UtcNow.AddSeconds(-10),
                ReadTime = default(DateTime?)
            };

            var latestMessage = new UserMessage
            {
                Sender = sender,
                SentTime = DateTime.UtcNow.AddSeconds(10),
                ReadTime = default(DateTime?)
            };

            // Act
            var result = _systemUnderTest.Map(new List<UserMessage> { currentMessage, oldestMessage, latestMessage });

            // Assert
            result.Should().NotBeEmpty();
            result.Should().HaveCount(1);
            result.First().Should().BeEquivalentTo(new SenderMessages
            {
                Sender = sender,
                UnreadCount = 2,
                Messages = MapToMessages(latestMessage, currentMessage, oldestMessage)
            });
        }

        [TestMethod]
        public void Map_WithNoUserMessages_MapsToEmptyResponse()
        {
            // Act
            var result = _systemUnderTest.Map(new List<UserMessage>());

            // Assert
            result.Should().BeEmpty();
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
            result.Should().NotBeEmpty();
            result.Should().HaveCount(3);
            result.Should().BeEquivalentTo(new List<SenderMessages>
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
            result.Should().BeEmpty();
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

        private List<Message> MapToMessages(params UserMessage[] userMessages)
            => userMessages.Select(m => new Message
            {
                Id = m.Id,
                Sender = m.Sender,
                Version = m.Version,
                Body = m.Body,
                Read = m.ReadTime.HasValue,
                SentTime = m.SentTime
            }).ToList();
    }
}