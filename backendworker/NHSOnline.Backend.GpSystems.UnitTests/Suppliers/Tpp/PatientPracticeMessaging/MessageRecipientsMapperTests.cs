using System.Collections.Generic;
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
    public class MessageRecipientsMapperTests
    {
        private IFixture _fixture;
        private MessageRecipientsMapper _mapper;
        private Item _recipientOne;
        private Item _recipientTwo;
        private MessageRecipient _mappedRecipientOne;
        private MessageRecipient _mappedRecipientTwo;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _mapper = new MessageRecipientsMapper();
            _recipientOne = _fixture.Create<Item>();
            _recipientTwo = _fixture.Create<Item>();
            _mappedRecipientOne = new MessageRecipient
            {
                Name = _recipientOne.ItemText,
                RecipientIdentifier = _recipientOne.Id
            };
            _mappedRecipientTwo = new MessageRecipient
            {
                Name = _recipientTwo.ItemText,
                RecipientIdentifier = _recipientTwo.Id
            };
        }

        [TestMethod]
        public void MapMessageRecipients__ReturnsResult()
        {
            var messageRecipientsResponse = new MessageRecipientsReply
            {
                Items = new List<Item>
                {
                    _recipientOne,
                    _recipientTwo
                }
            };

            var expectedResult = new MessageRecipientsResponse
            {
                MessageRecipients = new List<MessageRecipient>
                {
                    _mappedRecipientOne, _mappedRecipientTwo
                }
            };

            // Act
            var tppDcrEvents = _mapper.Map(messageRecipientsResponse);

            // Assert
            tppDcrEvents.Should().NotBeNull();
            tppDcrEvents.Should().BeEquivalentTo(expectedResult);
        }
    }
}