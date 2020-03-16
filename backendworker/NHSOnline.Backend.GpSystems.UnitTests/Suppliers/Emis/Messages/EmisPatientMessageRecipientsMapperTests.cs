using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Messages.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Messages;
using UnitTestHelper;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.Messages
{
    [TestClass]
    public class EmisPatientMessageRecipientsMapperTests
    {
        private IFixture _fixture;
        private EmisPatientMessageRecipientsMapper _mapper;

        private Mock<ILogger<IEmisPatientMessageRecipientsMapper>> _mockLogger;

        private MessageRecipient _recipientOne;
        private MessageRecipient _recipientTwo;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _mockLogger = _fixture.Create<Mock<ILogger<IEmisPatientMessageRecipientsMapper>>>();

            _recipientOne = _fixture.Create<MessageRecipient>();
            _recipientTwo = _fixture.Create<MessageRecipient>();

            _mapper = new EmisPatientMessageRecipientsMapper(_mockLogger.Object);
        }

        [TestMethod]
        public void Map_WhenCalledWithResponseWithDuplicates_RemovesDuplicateRecipientsAndLogsDuplicateIds()
        {

            var recipientsResponse = new MessageRecipientsResponse
            {
                MessageRecipients = new List<MessageRecipient>
                {
                    _recipientOne, _recipientTwo, _recipientOne, _recipientOne
                }
            };

            var expectedResult = new MessageRecipientsResponse
            {
                MessageRecipients = new List<MessageRecipient>
                {
                    _recipientOne, _recipientTwo
                }
            };

            var result = _mapper.Map(recipientsResponse);

            CollectionAssert.AreEqual(expectedResult.MessageRecipients, result.MessageRecipients);
            _mockLogger.VerifyLogger(LogLevel.Information, $"Duplicate recipient id {_recipientOne.RecipientIdentifier} removed from response", Times.Exactly(2));
            _mockLogger.VerifyLogger(LogLevel.Information, "Number of mapped recipients: 2", Times.Once());
        }

        [TestMethod]
        public void Map_WhenResponseHasNoDuplicates_RemovesNoRecipientsAndDoesNotLogIds()
        {
            var recipientsResponse = new MessageRecipientsResponse
            {
                MessageRecipients = new List<MessageRecipient>
                {
                    _recipientOne, _recipientTwo
                }
            };

            var result = _mapper.Map(recipientsResponse);

            CollectionAssert.AreEqual(recipientsResponse.MessageRecipients, result.MessageRecipients);
            _mockLogger.VerifyLogger(LogLevel.Information, Times.Once());
            _mockLogger.VerifyLogger(LogLevel.Information, "Number of mapped recipients: 2", Times.Once());
        }

        [TestMethod]
        public void Map_WhenResponseHasNoRecipients_ReturnsResponseWithNoRecipients()
        {
            var recipientsResponse = new MessageRecipientsResponse
            {
                MessageRecipients = new List<MessageRecipient>()
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