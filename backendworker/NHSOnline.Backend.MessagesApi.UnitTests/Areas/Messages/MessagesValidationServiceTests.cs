using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.MessagesApi.Areas.Messages;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;

namespace NHSOnline.Backend.MessagesApi.UnitTests.Areas.Messages
{
    [TestClass]
    public class MessagesValidationServiceTests
    {
        private MessagesValidationService _systemUnderTest;
        private Fixture _fixture;
        private string _userMessageId;
        private JsonPatchDocument<Message> _jsonPatchDoc;
        private Operation<Message> _validPatchOperation;
        
        private AddMessageRequest _validAddMessageRequest;
        private string _nhsLoginId;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture();
            var logger = new Mock<ILogger<MessagesValidationService>>();
            _systemUnderTest = new MessagesValidationService(logger.Object);
            _userMessageId = "823c13304df9cbc0e744241c"; //24 digit hex regex
            _jsonPatchDoc = new JsonPatchDocument<Message>();
            _validPatchOperation = _fixture.Create<Operation<Message>>();

            _validAddMessageRequest = _fixture.Create<AddMessageRequest>();
            _nhsLoginId = _fixture.Create<string>();
        }
        
        [TestMethod]
        public void IsPatchRequestValid_ValidData_ReturnsTrue()
        {
            // Arrange
            _jsonPatchDoc.Operations.Add(_validPatchOperation);

            // Act
            var result = _systemUnderTest.IsPatchRequestValid(_jsonPatchDoc, _userMessageId);

            // Assert
            result.Should().BeTrue();
        }
        
        [TestMethod]
        public void IsPatchRequestValid_EmptyJsonPatchOperations_ReturnsFalse()
        {
            // Act
            var result = _systemUnderTest.IsPatchRequestValid(_jsonPatchDoc, _userMessageId);

            // Assert
            result.Should().BeFalse();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("            ")]
        [DataRow("")]
        public void IsPatchRequestValid_InvalidJsonPatchOperation_ReturnsFalse(string op)
        {
            // Arrange
            _validPatchOperation.op = op;
            _jsonPatchDoc.Operations.Add(_validPatchOperation);
            
            // Act
            var result = _systemUnderTest.IsPatchRequestValid(_jsonPatchDoc, _userMessageId);

            // Assert
            result.Should().BeFalse();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("            ")]
        [DataRow("")]
        public void IsPatchRequestValid_InvalidAndValidJsonPatchOperation_ReturnsFalse(string op)
        {
            // Arrange
            var invalidPatchOperation = _validPatchOperation;
            invalidPatchOperation.op = op;
            
            _jsonPatchDoc.Operations.Add(_validPatchOperation);
            _jsonPatchDoc.Operations.Add(invalidPatchOperation);
            
            // Act
            var result = _systemUnderTest.IsPatchRequestValid(_jsonPatchDoc, _userMessageId);

            // Assert
            result.Should().BeFalse();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("            ")]
        [DataRow("")]
        public void IsPatchRequestValid_InvalidMessageId_ReturnsFalse(string userMessageId)
        {
            // Arrange
            _jsonPatchDoc.Operations.Add(_validPatchOperation);

            // Act
            var result = _systemUnderTest.IsPatchRequestValid(_jsonPatchDoc, userMessageId);

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsMessageRequestValid_ValidData_ReturnsTrue()
        {
            // Act
            var result = _systemUnderTest.IsMessageRequestValid(_validAddMessageRequest, _nhsLoginId);

            // Assert
            result.Should().BeTrue();
        }
        
        [TestMethod]
        public void IsMessageRequestValid_NullAddMessageRequest_ReturnsFalse()
        {
            // Act
            var result = _systemUnderTest.IsMessageRequestValid(null, _nhsLoginId);

            // Assert
            result.Should().BeFalse();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("            ")]
        [DataRow("")]
        public void IsMessageRequestValid_InvalidMessageSender_ReturnsFalse(string messageSender)
        {
            // Arrange
            _validAddMessageRequest.Sender = messageSender;
            
            // Act
            var result = _systemUnderTest.IsMessageRequestValid(_validAddMessageRequest, _nhsLoginId);

            // Assert
            result.Should().BeFalse();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("            ")]
        [DataRow("")]
        public void IsMessageRequestValid_InvalidMessageBody_ReturnsFalse(string messageBody)
        {
            // Arrange
            _validAddMessageRequest.Body = messageBody;
            
            // Act
            var result = _systemUnderTest.IsMessageRequestValid(_validAddMessageRequest, _nhsLoginId);

            // Assert
            result.Should().BeFalse();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("            ")]
        [DataRow("")]
        public void IsMessageRequestValid_InvalidNhsId_ReturnsFalse(string nhsLoginId)
        {
            // Act
            var result = _systemUnderTest.IsMessageRequestValid(_validAddMessageRequest, nhsLoginId);

            // Assert
            result.Should().BeFalse();
        }
        
        [DataTestMethod]
        [DataRow("something","something", null)]
        [DataRow("something",null, null)]
        [DataRow("something",null, "something")]
        [DataRow(null, "something", "something")]
        [DataRow(null, null, "something")]
        [DataRow(null, "something", null)]
        [DataRow(null, null, null)]
        public void IsMessageRequestValid_invalid_ReturnsFalse(string body, string sender, string nhsLoginId)
        {
            //Arrange
            _validAddMessageRequest.Body = body;
            _validAddMessageRequest.Sender = sender;
            
            // Act
            var result = _systemUnderTest.IsMessageRequestValid(_validAddMessageRequest, nhsLoginId);

            // Assert
            result.Should().BeFalse();
        }
    }
}