using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
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
using NHSOnline.Backend.Messages.Areas.Messages;
using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Messages.Repository;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support;
using UnitTestHelper;

namespace NHSOnline.Backend.Messages.UnitTests.Areas.Messages
{
    [TestClass]
    public class MessageServicePatchTests
    {
        private MessageService _systemUnderTest;
        private Mock<IMessageRepository> _mockMessageRepository;
        private Mock<IMessagesValidationService> _mockMessagesValidationService;
        private Mock<IMapper<Operation<Message>, MessagePatchType>> _mockPatchMessageTypeMapper;
        private string _userMessageId;
        private AccessToken _accessToken;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockMessageRepository = new Mock<IMessageRepository>();
            _userMessageId = "fd9fb3db27402da79fe66515"; //24 digit hex regex

            _mockMessagesValidationService = new Mock<IMessagesValidationService>(MockBehavior.Strict);
            _mockPatchMessageTypeMapper = new Mock<IMapper<Operation<Message>, MessagePatchType>>(MockBehavior.Strict);

            var mockLogger = new Mock<ILogger<MessageService>>();
            var accessTokenString = JwtToken.Generate(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "NhsLoginId"),
                new Claim("nhs_number", "nhsNumber")
            });

            _accessToken = AccessToken.Parse(mockLogger.Object, accessTokenString);

            _systemUnderTest = new MessageService(
                _mockMessageRepository.Object,
                mockLogger.Object,
                new Mock<IMapper<List<UserMessage>, MessagesResponse>>(MockBehavior.Strict).Object,
                new Mock<IMapper<UserMessage, MessagesResponse>>(MockBehavior.Strict).Object,
                new Mock<IMapper<List<SummaryMessage>, MessagesResponse>>(MockBehavior.Strict).Object,
                new Mock<IMapper<AddMessageRequest, string, UserMessage>>(MockBehavior.Strict).Object,
                _mockPatchMessageTypeMapper.Object,
                _mockMessagesValidationService.Object);
        }

        [TestMethod]
        public async Task Patch_Updated_For_MessageRead()
        {
            // Arrange
            var jsonPatchDoc = new JsonPatchDocument<Message>();
            jsonPatchDoc.Operations.Add(new Operation<Message>("add", "/read", null, true));
            _mockPatchMessageTypeMapper.Setup(s => s.Map(jsonPatchDoc.Operations.FirstOrDefault()))
                .Returns(MessagePatchType.Read);

            var userMessage = new UserMessage
            {
                Id = new ObjectId("ae0b4ffd40c44828b884961b5228128e")
            };

            var messagesToFilter = new List<UserMessage>
            {
                new UserMessage { NhsLoginId = "NhsLoginIdNullReadTime", ReadTime = null },
                new UserMessage { NhsLoginId = "NhsLoginIdNonNullReadTime", ReadTime = DateTime.Now }
            };

            _mockMessagesValidationService.Setup(x =>
                    x.IsPatchRequestValid(jsonPatchDoc, _userMessageId))
                .Returns(true);

            _mockPatchMessageTypeMapper.Setup(s => s.Map(jsonPatchDoc.Operations.FirstOrDefault()))
                .Returns(MessagePatchType.Read);

            List<Expression<Func<UserMessage, bool>>> filters = null;

            _mockMessageRepository.Setup(x => x.UpdateOne(
                    _accessToken.Subject,
                    _userMessageId,
                    It.IsAny<(List<Expression<Func<UserMessage, bool>>>, UpdateRecordBuilder<UserMessage>)>()))
                .Callback<string, string, (List<Expression<Func<UserMessage, bool>>>, UpdateRecordBuilder<UserMessage>)>(
                    (nhsLoginId, messageId, thing) => filters = thing.Item1)
                .ReturnsAsync(new RepositoryUpdateResult<UserMessage>.Updated());

            _mockMessageRepository
                .Setup(x => x.FindMessage(_accessToken.Subject, _userMessageId))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.Found(new[] { userMessage }));

            // Act
            var result = await _systemUnderTest.UpdateMessage(jsonPatchDoc, _accessToken, _userMessageId);

            // Assert
            _mockMessagesValidationService.VerifyAll();
            _mockMessageRepository.VerifyAll();
            _mockPatchMessageTypeMapper.VerifyAll();

            filters.Should().HaveCount(1);

            var filteredMessages = messagesToFilter.Where(filters.First().Compile());
            filteredMessages.Should().HaveCount(1);
            filteredMessages.Should().Contain(u => u.NhsLoginId == "NhsLoginIdNullReadTime");

            var subject = result.Should().BeAssignableTo<MessagePatchResult.Updated>().Subject;
            subject.UserMessage.Should().Be(userMessage);
        }

        [DataTestMethod]
        [DataRow(true, DisplayName = "Valid reply code")]
        [DataRow(false, DisplayName = "Invalid reply code")]
        public async Task Patch_Updated_For_MessageReply(bool isValidReply)
        {
            // Arrange
            var jsonPatchDoc = new JsonPatchDocument<Message>();
            var validCode = "YES";
            if (isValidReply)
            {
                jsonPatchDoc.Operations.Add(new Operation<Message>("add", "/reply/response", null, validCode));
            }
            else
            {
                jsonPatchDoc.Operations.Add(new Operation<Message>("add", "/reply/response", null, "TEST_CODE"));
            }

            var userMessage = new UserMessage
            {
                Id = new ObjectId("ae0b4ffd40c44828b884961b5228128e")
            };

            var messagesToFilter = new List<UserMessage>
            {
                new UserMessage
                {
                    NhsLoginId = "NhsLoginIdNullResponse",
                    Reply = new UserMessageReply
                    {
                        Options = new List<UserReplyOption>()
                        {
                            new UserReplyOption() { Code = validCode, Display = "Yes" },
                            new UserReplyOption() { Code = "NO", Display = "No" }
                        }
                    }
                },
                new UserMessage
                {
                    NhsLoginId = "NhsLoginIdNonNullResponse",
                    Reply = new UserMessageReply
                    {
                        Options = new List<UserReplyOption>()
                        {
                            new UserReplyOption() { Code = validCode, Display = "Yes" },
                            new UserReplyOption() { Code = "NO", Display = "No" }
                        },
                        Response = "YES",
                        ResponseDateTime = DateTime.Now
                    }
                }
            };

            _mockMessagesValidationService.Setup(x =>
                    x.IsPatchRequestValid(jsonPatchDoc, _userMessageId))
                .Returns(true);

            _mockPatchMessageTypeMapper.Setup(s => s.Map(jsonPatchDoc.Operations.FirstOrDefault()))
                .Returns(MessagePatchType.Reply);

            List<Expression<Func<UserMessage, bool>>> filters = null;

            _mockMessageRepository.Setup(x => x.UpdateOne(
                    _accessToken.Subject,
                    _userMessageId,
                    It.IsAny<(List<Expression<Func<UserMessage, bool>>>, UpdateRecordBuilder<UserMessage>)>()))
                .Callback<string, string, (List<Expression<Func<UserMessage, bool>>>, UpdateRecordBuilder<UserMessage>)>(
                    (nhsLoginId, messageId, thing) => filters = thing.Item1)
                .ReturnsAsync(new RepositoryUpdateResult<UserMessage>.Updated());

            _mockMessageRepository
                .Setup(x => x.FindMessage(_accessToken.Subject, _userMessageId))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.Found(new[] { userMessage }));

            // Act
            var result = await _systemUnderTest.UpdateMessage(jsonPatchDoc, _accessToken, _userMessageId);

            // Assert
            _mockMessagesValidationService.VerifyAll();
            _mockMessageRepository.VerifyAll();
            _mockPatchMessageTypeMapper.VerifyAll();

            filters.Should().HaveCount(1);
            var filteredMessages = messagesToFilter.Where(filters.First().Compile());

            if (isValidReply)
            {
                filteredMessages.Should().HaveCount(1);
                filteredMessages.Should().Contain(u => u.NhsLoginId == "NhsLoginIdNullResponse");
                var subject = result.Should().BeAssignableTo<MessagePatchResult.Updated>().Subject;
                subject.UserMessage.Should().Be(userMessage);
            }
            else
            {
                filteredMessages.Should().HaveCount(0);
            }
        }

        [TestMethod]
        public async Task Patch_UpdateOneRepositoryThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var jsonPatchDoc = new JsonPatchDocument<Message>();

            _mockMessagesValidationService.Setup(x =>
                    x.IsPatchRequestValid(jsonPatchDoc, _userMessageId))
                .Returns(true);

            _mockPatchMessageTypeMapper.Setup(s => s.Map(jsonPatchDoc.Operations.FirstOrDefault()))
                .Returns(MessagePatchType.Read);

            _mockMessageRepository.Setup(x =>
                    x.UpdateOne(It.IsAny<string>(), It.IsAny<string>(),
                        It.IsAny<(List<Expression<Func<UserMessage, bool>>>, UpdateRecordBuilder<UserMessage>)>()))
                .Throws(new MongoException("Test"));


            // Act
            var result = await _systemUnderTest.UpdateMessage(jsonPatchDoc, _accessToken, _userMessageId);

            // Assert
            _mockMessageRepository.VerifyAll();
            _mockMessagesValidationService.VerifyAll();
            _mockPatchMessageTypeMapper.VerifyAll();

            result.Should().BeAssignableTo<MessagePatchResult.InternalServerError>();
        }

        [TestMethod]
        public async Task Patch_UpdateOneRepositoryReturnsFailure_ReturnsBadGateway()
        {
            // Arrange
            var jsonPatchDoc = new JsonPatchDocument<Message>();

            _mockMessagesValidationService.Setup(x =>
                    x.IsPatchRequestValid(jsonPatchDoc, _userMessageId))
                .Returns(true);

            _mockPatchMessageTypeMapper.Setup(s => s.Map(jsonPatchDoc.Operations.FirstOrDefault()))
                .Returns(MessagePatchType.Read);

            _mockMessageRepository.Setup(x =>
                    x.UpdateOne(It.IsAny<string>(), It.IsAny<string>(),
                        It.IsAny<(List<Expression<Func<UserMessage, bool>>>, UpdateRecordBuilder<UserMessage>)>()))
                .ReturnsAsync(new RepositoryUpdateResult<UserMessage>.RepositoryError());

            // Act
            var result = await _systemUnderTest.UpdateMessage(jsonPatchDoc, _accessToken, _userMessageId);

            // Assert
            _mockMessageRepository.VerifyAll();
            _mockMessagesValidationService.VerifyAll();
            _mockPatchMessageTypeMapper.VerifyAll();

            result.Should().BeAssignableTo<MessagePatchResult.BadGateway>();
        }

        [TestMethod]
        public async Task Patch_UpdateOneRepositoryReturnsNotFound_ReturnsNotFound()
        {
            // Arrange
            var jsonPatchDoc = new JsonPatchDocument<Message>();

            _mockMessagesValidationService.Setup(x =>
                    x.IsPatchRequestValid(jsonPatchDoc, _userMessageId))
                .Returns(true);

            _mockPatchMessageTypeMapper.Setup(s => s.Map(jsonPatchDoc.Operations.FirstOrDefault()))
                .Returns(MessagePatchType.Read);

            _mockMessageRepository.Setup(x =>
                    x.UpdateOne(It.IsAny<string>(), It.IsAny<string>(),
                        It.IsAny<(List<Expression<Func<UserMessage, bool>>>, UpdateRecordBuilder<UserMessage>)>()))
                .ReturnsAsync(new RepositoryUpdateResult<UserMessage>.NotFound());

            // Act
            var result = await _systemUnderTest.UpdateMessage(jsonPatchDoc, _accessToken, _userMessageId);

            // Assert
            _mockMessageRepository.VerifyAll();
            _mockMessagesValidationService.VerifyAll();
            _mockPatchMessageTypeMapper.VerifyAll();

            result.Should().BeAssignableTo<MessagePatchResult.NotFound>();
        }

        [TestMethod]
        public async Task Patch_UpdateOneRepositoryReturnsNoChange_ReturnsNoChange()
        {
            // Arrange
            var jsonPatchDoc = new JsonPatchDocument<Message>();

            _mockMessagesValidationService.Setup(x =>
                    x.IsPatchRequestValid(jsonPatchDoc, _userMessageId))
                .Returns(true);

            _mockPatchMessageTypeMapper.Setup(s => s.Map(jsonPatchDoc.Operations.FirstOrDefault()))
                .Returns(MessagePatchType.Read);

            _mockMessageRepository.Setup(x =>
                    x.UpdateOne(It.IsAny<string>(), It.IsAny<string>(),
                        It.IsAny<(List<Expression<Func<UserMessage, bool>>>, UpdateRecordBuilder<UserMessage>)>()))
                .ReturnsAsync(new RepositoryUpdateResult<UserMessage>.NoChange());

            // Act
            var result = await _systemUnderTest.UpdateMessage(jsonPatchDoc, _accessToken, _userMessageId);

            // Assert
            _mockMessageRepository.VerifyAll();
            _mockMessagesValidationService.VerifyAll();
            _mockPatchMessageTypeMapper.VerifyAll();

            result.Should().BeAssignableTo<MessagePatchResult.NoChange>();
        }

        [TestMethod]
        public async Task Patch_FindMessageReturnsNotFound_ReturnsNotFound()
        {
            // Arrange
            var jsonPatchDoc = new JsonPatchDocument<Message>();

            _mockMessagesValidationService.Setup(x =>
                    x.IsPatchRequestValid(jsonPatchDoc, _userMessageId))
                .Returns(true);

            _mockPatchMessageTypeMapper.Setup(s => s.Map(jsonPatchDoc.Operations.FirstOrDefault()))
                .Returns(MessagePatchType.Read);

            _mockMessageRepository.Setup(x => x.UpdateOne(
                    _accessToken.Subject,
                    _userMessageId,
                    It.IsAny<(List<Expression<Func<UserMessage, bool>>>, UpdateRecordBuilder<UserMessage>)>()))
                .ReturnsAsync(new RepositoryUpdateResult<UserMessage>.Updated());

            _mockMessageRepository
                .Setup(x => x.FindMessage(_accessToken.Subject, _userMessageId))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.NotFound());

            // Act
            var result = await _systemUnderTest.UpdateMessage(jsonPatchDoc, _accessToken, _userMessageId);

            // Assert
            _mockMessageRepository.VerifyAll();
            _mockMessagesValidationService.VerifyAll();
            _mockPatchMessageTypeMapper.VerifyAll();

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

            _mockPatchMessageTypeMapper.Setup(s => s.Map(jsonPatchDoc.Operations.FirstOrDefault()))
                .Returns(MessagePatchType.Read);

            _mockMessageRepository.Setup(x => x.UpdateOne(
                    _accessToken.Subject,
                    _userMessageId,
                    It.IsAny<(List<Expression<Func<UserMessage, bool>>>, UpdateRecordBuilder<UserMessage>)>()))
                .ReturnsAsync(new RepositoryUpdateResult<UserMessage>.Updated());

            _mockMessageRepository
                .Setup(x => x.FindMessage(_accessToken.Subject, _userMessageId))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.RepositoryError());

            // Act
            var result = await _systemUnderTest.UpdateMessage(jsonPatchDoc, _accessToken, _userMessageId);

            // Assert
            _mockMessageRepository.VerifyAll();
            _mockMessagesValidationService.VerifyAll();
            _mockPatchMessageTypeMapper.VerifyAll();

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

            _mockPatchMessageTypeMapper.Setup(s => s.Map(jsonPatchDoc.Operations.FirstOrDefault()))
                .Returns(MessagePatchType.Read);

            _mockMessageRepository.Setup(x => x.UpdateOne(
                    _accessToken.Subject,
                    _userMessageId,
                    It.IsAny<(List<Expression<Func<UserMessage, bool>>>, UpdateRecordBuilder<UserMessage>)>()))
                .ReturnsAsync(new RepositoryUpdateResult<UserMessage>.Updated());

            _mockMessageRepository
                .Setup(x => x.FindMessage(_accessToken.Subject, _userMessageId))
                .Throws(new MongoException("Test"));

            // Act
            var result = await _systemUnderTest.UpdateMessage(jsonPatchDoc, _accessToken, _userMessageId);

            // Assert
            _mockMessageRepository.VerifyAll();
            _mockMessagesValidationService.VerifyAll();
            _mockPatchMessageTypeMapper.VerifyAll();

            result.Should().BeAssignableTo<MessagePatchResult.InternalServerError>();
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

        [TestMethod]
        public async Task Patch_RequestMapsToUnknownOperation_ReturnsBadRequest()
        {
            // Arrange
            _mockMessagesValidationService.Setup(x =>
                    x.IsPatchRequestValid(It.IsAny<JsonPatchDocument<Message>>(), It.IsAny<string>()))
                .Returns(true);

            _mockPatchMessageTypeMapper.Setup(s => s.Map(It.IsAny<Operation<Message>>()))
                .Returns(MessagePatchType.Unknown);

            // Act
            var result = await _systemUnderTest.UpdateMessage(
                new JsonPatchDocument<Message>(),
                _accessToken,
                _userMessageId);

            // Assert
            _mockMessagesValidationService.VerifyAll();
            _mockPatchMessageTypeMapper.VerifyAll();

            result.Should().BeAssignableTo<MessagePatchResult.BadRequest>();
        }
    }
}