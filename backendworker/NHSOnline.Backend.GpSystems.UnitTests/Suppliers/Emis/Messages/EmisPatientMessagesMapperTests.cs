using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Messages.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Messages;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Messages;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.Messages
{
    [TestClass]
    public class EmisPatientMessagesMapperTests
    {
        private IFixture _fixture;

        private EmisPatientMessagesMapper _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _systemUnderTest = _fixture.Create<EmisPatientMessagesMapper>();
        }

        [TestMethod]
        public void Map_WhenCalledHappyPath_ReturnsMappedGetPatientMessagesResponse()
        {
            // Arrange
            var messagesGetResponse = _fixture.Create<MessagesGetResponse>();

            // Act
            var result = _systemUnderTest.Map(messagesGetResponse);

            // Assert
            result.Should().BeEquivalentTo(new GetPatientMessagesResponse
            {
                MessageSummaries = messagesGetResponse.Messages.Select(m => new PatientMessageSummary
                {
                    Id = m.MessageId.ToString(CultureInfo.InvariantCulture),
                    Recipient = m.Recipients.Select(r => r.Name).ToList().First(),
                    LastMessageDateTime = m.LastReplyDateTime,
                    Subject = m.Subject,
                    ReplyCount = m.ReplyCount,
                    HasUnreadReplies = m.HasUnreadReplies
                }).ToList()
            });
        }

        [TestMethod]
        public void Map_WhenCalledWithSuccessResponse_OrdersMessagesByDateDesc()
        {
            // Arrange
            var messagesGetResponse = new MessagesGetResponse
            {
                Messages = Enumerable.Range(1, 3).Select(i => new MessageSummary
                {
                    HasUnreadReplies = true,
                    LastReplyDateTime = $"{i}",
                    LastReplyFromDisplayName = _fixture.Create<string>(),
                    MessageId = _fixture.Create<int>(),
                    Recipients = _fixture.Create<List<UserMessageRecipient>>(),
                    ReplyCount = _fixture.Create<int>(),
                    SentDateTime = _fixture.Create<string>(),
                    Subject = _fixture.Create<string>()
                }).ToList()
            };

            // Act
            var result = _systemUnderTest.Map(messagesGetResponse);

            // Assert
            result.MessageSummaries.Select(m => m.LastMessageDateTime).ToList().Should().BeEquivalentTo(new List<string>{"3", "2", "1"});
        }

        [TestMethod]
        public void Map_WhenCalledWithSuccessResponseWithoutLastReplyDateTime_SetsDateToSentDateTime()
        {
            // Arrange
            var messageMetaData = _fixture.Create<MessageSummary>();
            var sentDateTime = _fixture.Create<string>();

            messageMetaData.SentDateTime = sentDateTime;
            messageMetaData.LastReplyDateTime = null;

            var messagesGetResponse = new MessagesGetResponse
            {
                Messages = new List<MessageSummary>{ messageMetaData }
            };

            // Act
            var result = _systemUnderTest.Map(messagesGetResponse);

            // Assert
            result.MessageSummaries.First().LastMessageDateTime.Should().Be(sentDateTime);
        }

        [TestMethod]
        public void Map_WhenCalledWithResponseWithNoMessages_ReturnsNull()
        {
            // Act
            var result = _systemUnderTest.Map(new MessagesGetResponse());

            // Assert
            result.Should().Be(null);
        }
    }
}