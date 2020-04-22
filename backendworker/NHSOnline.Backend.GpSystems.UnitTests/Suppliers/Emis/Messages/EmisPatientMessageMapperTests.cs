using System.Collections.Generic;
using System.Globalization;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Messages.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Messages;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.Messages
{
    [TestClass]
    public class EmisPatientMessageMapperTests
    {
        private IFixture _fixture;

        private EmisPatientMessageMapper _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _systemUnderTest = _fixture.Create<EmisPatientMessageMapper>();
        }

        [TestMethod]
        public void Map_WhenCalledHappyPath_ReturnsMappedGetPatientMessageResponse()
        {
            // Arrange
            var recipients = new UserMessageRecipient
            {
                Name = "Recipient 1",
            };

            var reply = new MessageReply
            {
                Sender = "Test sender",
                ReplyContent = "This is a reply",
                OutboundMessage = false,
                IsUnread = true,
                SentDateTime = _fixture.Create<string>(),
                AttachmentId = null,
            };

            var messageGetResponse = new MessageGetResponse
            {
                Message = new MessageDetails
                {
                    MessageId = 1,
                    SentDateTime = _fixture.Create<string>(),
                    Subject = _fixture.Create<string>(),
                    Content = _fixture.Create<string>(),
                    Recipients = new List<UserMessageRecipient>{ recipients },
                    MessageReplies = new List<MessageReply>{ reply },
                }
            };

            messageGetResponse.Message.MessageReplies = new List<MessageReply>{ reply };

            // Act
            var result = _systemUnderTest.Map(messageGetResponse);

            // Assert
            result.Should().BeEquivalentTo(new GetPatientMessageResponse
            {
                MessageDetails =
                {
                    MessageId = messageGetResponse.Message.MessageId?.ToString(CultureInfo.InvariantCulture),
                    Subject = messageGetResponse.Message.Subject,
                    Recipient = messageGetResponse.Message.Recipients[0].Name,
                    Replies = messageGetResponse.Message.MessageReplies,
                    Content = messageGetResponse.Message.Content,
                    SentDateTime = messageGetResponse.Message.SentDateTime,
                    OutboundMessage = true
                }
            });
        }
    }
}