using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Mappers;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;

namespace NHSOnline.Backend.MessagesApi.UnitTests.Areas.Messages.Mappers
{
    [TestClass]
    public class MessagesResponseMapperTests
    {
        private IFixture _fixture;
        private MessagesResponseMapper _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new MessagesApiCustomization());

            _systemUnderTest = _fixture.Create<MessagesResponseMapper>();
        }

        [TestMethod]
        public void Map_WithUserMessages_MapsToResponse()
        {
            // Arrange
            const string sender = "test sender";
            var currentMessage = _fixture.Build<UserMessage>()
                .With(x => x.Sender, sender)
                .With(x => x.SentTime, DateTime.UtcNow)
                .With(x => x.ReadTime, DateTime.UtcNow)
                .Create();

            var oldestMessage = _fixture.Build<UserMessage>()
                .With(x => x.Sender, sender)
                .With(x => x.SentTime, DateTime.UtcNow.AddSeconds(-10))
                .With(x => x.ReadTime, default(DateTime?))
                .Create();

            var latestMessage = _fixture.Build<UserMessage>()
                .With(x => x.Sender, sender)
                .With(x => x.SentTime, DateTime.UtcNow.AddSeconds(10))
                .With(x => x.ReadTime, default(DateTime?))
                .Create();

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
            var currentMessage = _fixture.Build<SummaryMessage>()
                .With(x => x.SentTime, DateTime.UtcNow)
                .Create();

            var oldestMessage = _fixture.Build<SummaryMessage>()
                .With(x => x.SentTime, DateTime.UtcNow.AddSeconds(-10))
                .Create();

            var latestMessage = _fixture.Build<SummaryMessage>()
                .With(x => x.SentTime, DateTime.UtcNow.AddSeconds(10))
                .Create();

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
                SentTime = m.SentTime,
            }).ToList();
    }
}