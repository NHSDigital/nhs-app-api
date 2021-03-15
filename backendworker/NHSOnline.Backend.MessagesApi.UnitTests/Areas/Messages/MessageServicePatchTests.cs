using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.MessagesApi.Areas.Messages;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.MessagesApi.Repository;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Repository;
using UnitTestHelper;

namespace NHSOnline.Backend.MessagesApi.UnitTests.Areas.Messages
{
    [TestClass]
    public class MessageServicePatchTests
    {
        private MessageService _systemUnderTest;
        private Mock<IMessageRepository> _mockMessageRepository;

        private Mock<IMessagesValidationService> _mockMessagesValidationService;
        private string _userMessageId;
        private AccessToken _accessToken;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockMessageRepository = new Mock<IMessageRepository>();
            _userMessageId = "fd9fb3db27402da79fe66515"; //24 digit hex regex

            _mockMessagesValidationService = new Mock<IMessagesValidationService>(MockBehavior.Strict);

            var mockLogger = new Mock<ILogger<MessagesController>>();
            var accessTokenString = JwtToken.Generate(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "NhsLoginId"),
                new Claim("nhs_number", "nhsNumber")
            });

            _accessToken = AccessToken.Parse(mockLogger.Object, accessTokenString);

            _systemUnderTest = new MessageService(
                _mockMessageRepository.Object,
                mockLogger.Object,
                new Mock<IMapper<List<UserMessage>, MessagesResponse>>().Object,
                new Mock<IMapper<List<SummaryMessage>, MessagesResponse>>().Object,
                new Mock<IMapper<AddMessageRequest, string, UserMessage>>().Object,
                _mockMessagesValidationService.Object);
        }

        [TestMethod]
        [DataRow("add", "/read", "/readTime")]
        public async Task Patch_Updated(string operation, string messagePath, string userMessagePath)
        {
            // Arrange
            var jsonPatchDoc = new JsonPatchDocument<Message>();
            jsonPatchDoc.Operations.Add(new Operation<Message>(operation, messagePath, null, true));
            var nowTime = DateTime.UtcNow;
            var userMessageJsonPatch = new JsonPatchDocument<UserMessage>();
            userMessageJsonPatch.Operations.Add(new Operation<UserMessage>(operation, userMessagePath, null, nowTime));
            var userMessages = new List<UserMessage>
            {
                new UserMessage
                {
                    Id = new ObjectId("ae0b4ffd40c44828b884961b5228128e"),
                    CommunicationId = "CommunicationId",
                    TransmissionId = "TransmissionId"
                }
            };

            _mockMessagesValidationService.Setup(x =>
                    x.IsPatchRequestValid(jsonPatchDoc, _userMessageId))
                .Returns(true);

            _mockMessageRepository
                .Setup(x => x.FindMessage(_accessToken.Subject, _userMessageId))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.Found(userMessages));

            _mockMessageRepository.Setup(x => x.UpdateOne(
                    _accessToken.Subject,
                    _userMessageId,
                    It.IsAny<UpdateRecordBuilder<UserMessage>>()))
                .ReturnsAsync(new RepositoryUpdateResult<UserMessage>.Updated());

            // Act
            var result = await _systemUnderTest.UpdateMessage(jsonPatchDoc, _accessToken, _userMessageId);

            // Assert
            _mockMessagesValidationService.VerifyAll();
            _mockMessageRepository.VerifyAll();

            var subject =  result.Should().BeAssignableTo<MessagePatchResult.Updated>().Subject;
            subject.Id.Should().Be("ae0b4ffd40c44828b884961b");
            subject.CommunicationId.Should().Be("CommunicationId");
            subject.TransmissionId.Should().Be("TransmissionId");
        }

        [TestMethod]
        [DataRow("copy", "/read")]
        [DataRow("add", "/sender")]
        public async Task Patch_NotInWhiteList_ReturnsBadRequest(string operation, string path)
        {
            // Arrange
            var jsonPatchDoc = new JsonPatchDocument<Message>();
            jsonPatchDoc.Operations.Add(new Operation<Message>(operation, path, null, "Test"));

            _mockMessagesValidationService.Setup(x =>
                    x.IsPatchRequestValid(jsonPatchDoc, _userMessageId))
                .Returns(true);

            // Act
            var result = await _systemUnderTest.UpdateMessage(jsonPatchDoc, _accessToken, _userMessageId);

            // Assert
            _mockMessagesValidationService.VerifyAll();

            result.Should().BeAssignableTo<MessagePatchResult.BadRequest>();
        }

        [TestMethod]
        public async Task Patch_FindMessageReturnsNotFound_ReturnsNotFound()
        {
            // Arrange
            var jsonPatchDoc = new JsonPatchDocument<Message>();

            _mockMessagesValidationService.Setup(x =>
                    x.IsPatchRequestValid(jsonPatchDoc, _userMessageId))
                .Returns(true);

            _mockMessageRepository
                .Setup(x => x.FindMessage(_accessToken.Subject, _userMessageId))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.NotFound());

            // Act
            var result = await _systemUnderTest.UpdateMessage(jsonPatchDoc, _accessToken, _userMessageId);

            // Assert
            _mockMessageRepository.VerifyAll();
            _mockMessagesValidationService.VerifyAll();

            result.Should().BeAssignableTo<MessagePatchResult.NotFound>();
        }

        [TestMethod]
        public async Task Patch_FindMessageReturnRepositoryError_ReturnsBadGateway()
        {
            // Arrange
            var jsonPatchDoc = new JsonPatchDocument<Message>();

            _mockMessagesValidationService.Setup(x =>
                    x.IsPatchRequestValid(jsonPatchDoc, _userMessageId))
                .Returns(true);

            _mockMessageRepository
                .Setup(x => x.FindMessage(_accessToken.Subject, _userMessageId))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.RepositoryError());

            // Act
            var result = await _systemUnderTest.UpdateMessage(jsonPatchDoc, _accessToken, _userMessageId);

            // Assert
            _mockMessageRepository.VerifyAll();
            _mockMessagesValidationService.VerifyAll();

            result.Should().BeAssignableTo<MessagePatchResult.BadGateway>();
        }

        [TestMethod]
        public async Task Patch_FindMessageThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var jsonPatchDoc = new JsonPatchDocument<Message>();

            _mockMessagesValidationService.Setup(x =>
                    x.IsPatchRequestValid(jsonPatchDoc, _userMessageId))
                .Returns(true);

            _mockMessageRepository
                .Setup(x => x.FindMessage(_accessToken.Subject, _userMessageId))
                .Throws(new MongoException("Test"));

            // Act
            var result = await _systemUnderTest.UpdateMessage(jsonPatchDoc, _accessToken, _userMessageId);

            // Assert
            _mockMessageRepository.VerifyAll();
            _mockMessagesValidationService.VerifyAll();

            result.Should().BeAssignableTo<MessagePatchResult.InternalServerError>();
        }

        [TestMethod]
        public async Task Patch_UpdateOneRepositoryThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var jsonPatchDoc = new JsonPatchDocument<Message>();
            var userMessages = new List<UserMessage>();

            _mockMessagesValidationService.Setup(x =>
                    x.IsPatchRequestValid(jsonPatchDoc, _userMessageId))
                .Returns(true);

            _mockMessageRepository
                .Setup(x => x.FindMessage(_accessToken.Subject, _userMessageId))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.Found(userMessages));

            _mockMessageRepository.Setup(x =>
                    x.UpdateOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UpdateRecordBuilder<UserMessage>>()))
                .Throws(new MongoException("Test"));

            // Act
            var result = await _systemUnderTest.UpdateMessage(jsonPatchDoc, _accessToken, _userMessageId);

            // Assert
            _mockMessageRepository.VerifyAll();
            _mockMessagesValidationService.VerifyAll();

            result.Should().BeAssignableTo<MessagePatchResult.InternalServerError>();
        }

        [TestMethod]
        public async Task Patch_UpdateOneRepositoryReturnsFailure_ReturnsBadGateway()
        {
            // Arrange
            var jsonPatchDoc = new JsonPatchDocument<Message>();
            var userMessages = new List<UserMessage>();

            _mockMessagesValidationService.Setup(x =>
                    x.IsPatchRequestValid(jsonPatchDoc, _userMessageId))
                .Returns(true);

            _mockMessageRepository
                .Setup(x => x.FindMessage(_accessToken.Subject, _userMessageId))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.Found(userMessages));

            _mockMessageRepository.Setup(x =>
                    x.UpdateOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UpdateRecordBuilder<UserMessage>>()))
                .ReturnsAsync(new RepositoryUpdateResult<UserMessage>.RepositoryError());

            // Act
            var result = await _systemUnderTest.UpdateMessage(jsonPatchDoc, _accessToken, _userMessageId);

            // Assert
            _mockMessageRepository.VerifyAll();
            _mockMessagesValidationService.VerifyAll();

            result.Should().BeAssignableTo<MessagePatchResult.BadGateway>();
        }

        [TestMethod]
        public async Task Patch_UpdateOneRepositoryReturnsNotFound_ReturnsNotFound()
        {
            // Arrange
            var jsonPatchDoc = new JsonPatchDocument<Message>();
            var userMessages = new List<UserMessage>();

            _mockMessagesValidationService.Setup(x =>
                    x.IsPatchRequestValid(jsonPatchDoc, _userMessageId))
                .Returns(true);

            _mockMessageRepository
                .Setup(x => x.FindMessage(_accessToken.Subject, _userMessageId))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.Found(userMessages));

            _mockMessageRepository.Setup(x =>
                    x.UpdateOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UpdateRecordBuilder<UserMessage>>()))
                .ReturnsAsync(new RepositoryUpdateResult<UserMessage>.NotFound());

            // Act
            var result = await _systemUnderTest.UpdateMessage(jsonPatchDoc, _accessToken, _userMessageId);

            // Assert
            _mockMessageRepository.VerifyAll();
            _mockMessagesValidationService.VerifyAll();

            result.Should().BeAssignableTo<MessagePatchResult.NotFound>();
        }

        [TestMethod]
        public async Task Patch_UpdateOneRepositoryReturnsNoChange_ReturnsNoChange()
        {
            // Arrange
            var jsonPatchDoc = new JsonPatchDocument<Message>();
            var userMessages = new List<UserMessage>();

            _mockMessagesValidationService.Setup(x =>
                    x.IsPatchRequestValid(jsonPatchDoc, _userMessageId))
                .Returns(true);

            _mockMessageRepository
                .Setup(x => x.FindMessage(_accessToken.Subject, _userMessageId))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.Found(userMessages));

            _mockMessageRepository.Setup(x =>
                    x.UpdateOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UpdateRecordBuilder<UserMessage>>()))
                .ReturnsAsync(new RepositoryUpdateResult<UserMessage>.NoChange());

            // Act
            var result = await _systemUnderTest.UpdateMessage(jsonPatchDoc, _accessToken, _userMessageId);

            // Assert
            _mockMessageRepository.VerifyAll();
            _mockMessagesValidationService.VerifyAll();

            result.Should().BeAssignableTo<MessagePatchResult.NoChange>();
        }

        [TestMethod]
        public async Task Patch_RequestFailsValidation_ReturnsBadRequest()
        {
            // Arrange
            _mockMessagesValidationService.Setup(x =>
                    x.IsPatchRequestValid(It.IsAny<JsonPatchDocument<Message>>(), It.IsAny<string>()))
                .Returns(false);

            // Act
            var result = await _systemUnderTest.UpdateMessage(
                new JsonPatchDocument<Message>(),
                _accessToken,
                _userMessageId);

            // Assert
            _mockMessagesValidationService.VerifyAll();

            result.Should().BeAssignableTo<MessagePatchResult.BadRequest>();
        }
    }
}