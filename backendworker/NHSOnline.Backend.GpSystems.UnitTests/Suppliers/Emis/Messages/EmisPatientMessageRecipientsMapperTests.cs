using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Messages.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Messages;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Messages;
using UnitTestHelper;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.Messages
{
    [TestClass]
    public class EmisPatientMessageRecipientsMapperTests
    {
        private IFixture _fixture;
        private EmisPatientMessageRecipientsMapper _mapper;

        private Mock<ILogger<IEmisPatientMessageRecipientsMapper>> _mockLogger;

        private MessageResponseRecipient _recipientOne;
        private MessageResponseRecipient _recipientTwo;
        private MessageRecipient _expectedRecipientOne;
        private MessageRecipient _expectedRecipientTwo;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _mockLogger = _fixture.Create<Mock<ILogger<IEmisPatientMessageRecipientsMapper>>>();

            _recipientOne = _fixture.Create<MessageResponseRecipient>();
            _recipientTwo = _fixture.Create<MessageResponseRecipient>();

            _mapper = new EmisPatientMessageRecipientsMapper(_mockLogger.Object);
        }

        [TestMethod]
        public void Map_WhenCalledWithResponseWithDuplicates_RemovesDuplicateRecipientsAndLogsDuplicateIds()
        {

            _expectedRecipientOne = new MessageRecipient
            {
                Name = _recipientOne.Name,
                RecipientIdentifier = _recipientOne.RecipientGuid
            };
            _expectedRecipientTwo = new MessageRecipient
            {
                Name = _recipientTwo.Name,
                RecipientIdentifier = _recipientTwo.RecipientGuid
            };

            var recipientsResponse = new MessageRecipientsResponse
            {
                MessageRecipients = new List<MessageResponseRecipient>
                {
                    _recipientOne, _recipientTwo, _recipientOne, _recipientOne
                }
            };

            var expectedResult = new PatientPracticeMessageRecipients()
            {
                MessageRecipients = new List<MessageRecipient>
                {
                    _expectedRecipientOne, _expectedRecipientTwo
                }
            };

            var result = _mapper.Map(recipientsResponse);

            result.MessageRecipients.Count.Should().Be(2);
            result.MessageRecipients[0].Name.Should().Be(_expectedRecipientOne.Name);
            result.MessageRecipients[0].RecipientIdentifier.Should().Be(_expectedRecipientOne.RecipientIdentifier);
            result.MessageRecipients[1].Name.Should().Be(_expectedRecipientTwo.Name);
            result.MessageRecipients[1].RecipientIdentifier.Should().Be(_expectedRecipientTwo.RecipientIdentifier);
            _mockLogger.VerifyLogger(LogLevel.Information, $"Duplicate recipient id {_recipientOne.RecipientGuid} removed from response", Times.Exactly(2));
            _mockLogger.VerifyLogger(LogLevel.Information, "Number of mapped recipients: 2", Times.Once());
        }

        [TestMethod]
        public void Map_WhenResponseHasNoDuplicates_RemovesNoRecipientsAndDoesNotLogIds()
        {
            var recipientsResponse = new MessageRecipientsResponse
            {
                MessageRecipients = new List<MessageResponseRecipient>
                {
                    _recipientOne, _recipientTwo
                }
            };

            var result = _mapper.Map(recipientsResponse);

            result.MessageRecipients.Count.Should().Be(2);
            result.MessageRecipients[0].Name.Should().Be(_recipientOne.Name);
            result.MessageRecipients[0].RecipientIdentifier.Should().Be(_recipientOne.RecipientGuid);
            result.MessageRecipients[1].Name.Should().Be(_recipientTwo.Name);
            result.MessageRecipients[1].RecipientIdentifier.Should().Be(_recipientTwo.RecipientGuid);
            _mockLogger.VerifyLogger(LogLevel.Information, Times.Once());
            _mockLogger.VerifyLogger(LogLevel.Information, "Number of mapped recipients: 2", Times.Once());
        }

        [TestMethod]
        public void Map_WhenResponseHasNoRecipients_ReturnsResponseWithNoRecipients()
        {
            var recipientsResponse = new MessageRecipientsResponse
            {
                MessageRecipients = new List<MessageResponseRecipient>()
            };

            var result = _mapper.Map(recipientsResponse);

            CollectionAssert.AreEqual(recipientsResponse.MessageRecipients, result.MessageRecipients);
            _mockLogger.VerifyLogger(LogLevel.Information, Times.Once());
            _mockLogger.VerifyLogger(LogLevel.Information, "Number of mapped recipients: 0", Times.Once());
        }

        [TestMethod]
        public void Map_WhenResponseIsNull_ReturnsResponseWithNullRecipients()
        {
            var result = _mapper.Map(null);

            Assert.IsNull(result.MessageRecipients);
            _mockLogger.VerifyLogger(LogLevel.Information, "Number of mapped recipients: 0", Times.Once());
        }

        [TestMethod]
        public void Map_WhenMessageRecipientsAreNull_ReturnsResponseWithNullRecipients()
        {
            var result = _mapper.Map(new MessageRecipientsResponse());

            Assert.IsNull(result.MessageRecipients);
            _mockLogger.VerifyLogger(LogLevel.Information, "Number of mapped recipients: 0", Times.Once());
        }
    }
}