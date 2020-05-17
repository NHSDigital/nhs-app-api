using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Moq;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.MessagesApi.Areas.Messages;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.MessagesApi.Repository;
using NHSOnline.Backend.Support;
using UnitTestHelper;

namespace NHSOnline.Backend.MessagesApi.UnitTests.Areas.Messages
{
    [TestClass]
    public class MessageServicePatchTests
    {
        private IFixture _fixture;
        private MessageService _systemUnderTest;
        private Mock<IMessageRepository> _mockMessageRepository;
        private Mock<IMapper<JsonPatchDocument<Message>, JsonPatchDocument<UserMessage>>> _mockMessageToUserMessageJsonPatch;
        private Mock<IMessagesValidationService> _mockMessagesValidationService;
        private string _userMessageId;
        private AccessToken _accessToken;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new MessagesApiCustomization());

            _mockMessageRepository = _fixture.Freeze<Mock<IMessageRepository>>();
            _fixture.Create<string>();
            _userMessageId = _fixture.CreateStringFromRegex("^[0-9a-f]{24}$"); //24 digit hex regex

            _mockMessagesValidationService = _fixture.Freeze<Mock<IMessagesValidationService>>();

            _mockMessageToUserMessageJsonPatch =
                _fixture.Freeze<Mock<IMapper<JsonPatchDocument<Message>, JsonPatchDocument<UserMessage>>>>();
            _fixture.Freeze<Mock<IMapper<List<UserMessage>, MessagesResponse>>>();
            _fixture.Freeze<Mock<IMapper<List<SummaryMessage>, MessagesResponse>>>();

            var mockLogger = _fixture.Freeze<Mock<ILogger<MessageService>>>();
            var accessTokenString = JwtToken.Generate(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _fixture.Create<string>()),
                new Claim("nhs_number", _fixture.Create<string>())
            });

            _accessToken = AccessToken.Parse(mockLogger.Object, accessTokenString);

            _systemUnderTest = _fixture.Create<MessageService>();
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
            UserMessage updatedMessage = null;

            _mockMessagesValidationService.Setup(x =>
                    x.IsPatchRequestValid(jsonPatchDoc, _userMessageId))
                .Returns(true);

            _mockMessageToUserMessageJsonPatch.Setup(x => x.Map(jsonPatchDoc))
                .Returns(userMessageJsonPatch);

            _mockMessageRepository.Setup(x => x.UpdateOne(It.IsAny<UserMessage>()))
                .Callback<UserMessage>(u => updatedMessage = u)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _systemUnderTest.PatchMessage(jsonPatchDoc, _accessToken, _userMessageId);

            // Assert
            _mockMessagesValidationService.VerifyAll();
            _mockMessageToUserMessageJsonPatch.VerifyAll();
            _mockMessageRepository.VerifyAll();
            result.Should().BeAssignableTo<MessagePatchResult.Updated>();
            updatedMessage.ReadTime.Should().Be(nowTime);
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
            var result = await _systemUnderTest.PatchMessage(jsonPatchDoc, _accessToken, _userMessageId);

            // Assert
            _mockMessagesValidationService.VerifyAll();
            _mockMessageToUserMessageJsonPatch.VerifyAll();
            result.Should().BeAssignableTo<MessagePatchResult.BadRequest>();
        }

        [TestMethod]
        public async Task Patch_MessageDoesNotExists_ReturnsNotFound()
        {
            // Arrange
            var jsonPatchDoc = new JsonPatchDocument<Message>();
            _mockMessageRepository
                .Setup(x => x.FindOne(It.IsAny<string>(),It.IsAny<string>()))
                .ReturnsAsync((UserMessage)null);

            _mockMessagesValidationService.Setup(x =>
                    x.IsPatchRequestValid(jsonPatchDoc, _userMessageId))
                .Returns(true);

            // Act
            var result = await _systemUnderTest.PatchMessage(jsonPatchDoc, _accessToken, _userMessageId);

            // Assert
            _mockMessageRepository.VerifyAll();
            _mockMessagesValidationService.VerifyAll();
            result.Should().BeAssignableTo<MessagePatchResult.NotFound>();
        }

        [TestMethod]
        public async Task Patch_RepositoryThrowsMongoException_ReturnsInternalServerError()
        {
            // Arrange
            var jsonPatchDoc = new JsonPatchDocument<Message>();
            _mockMessageRepository
                .Setup(x => x.FindOne(It.IsAny<string>(), It.IsAny<string>()))
                .Throws<ArgumentException>();

            _mockMessagesValidationService.Setup(x =>
                    x.IsPatchRequestValid(jsonPatchDoc, _userMessageId))
                .Returns(true);

            // Act
            var result = await _systemUnderTest.PatchMessage(jsonPatchDoc, _accessToken, _userMessageId);

            // Assert
            _mockMessageRepository.VerifyAll();
            _mockMessagesValidationService.VerifyAll();
            result.Should().BeAssignableTo<MessagePatchResult.InternalServerError>();
        }

        [TestMethod]
        public async Task Patch_RepositoryThrowsMongoException_ReturnsBadGateway()
        {
            // Arrange
            var jsonPatchDoc = new JsonPatchDocument<Message>();
            _mockMessageRepository.Setup(x => x.UpdateOne(It.IsAny<UserMessage>()))
                .Throws(new MongoException("Test"));

            _mockMessagesValidationService.Setup(x =>
                    x.IsPatchRequestValid(jsonPatchDoc, _userMessageId))
                .Returns(true);

            // Act
            var result = await _systemUnderTest.PatchMessage(jsonPatchDoc, _accessToken, _userMessageId);

            // Assert
            _mockMessagesValidationService.VerifyAll();
            _mockMessageRepository.VerifyAll();
            result.Should().BeAssignableTo<MessagePatchResult.BadGateway>();
        }

        [TestMethod]
        public async Task Patch_RequestFailsValidation_ReturnsBadRequest()
        {
            // Arrange
            _mockMessagesValidationService.Setup(x =>
                    x.IsPatchRequestValid(It.IsAny<JsonPatchDocument<Message>>(), It.IsAny<string>()))
                .Returns(false);

            // Act
            var result = await _systemUnderTest.PatchMessage(
                    _fixture.Create<JsonPatchDocument<Message>>(),
                    _accessToken,
                _fixture.Create<string>());

            // Assert
            _mockMessagesValidationService.VerifyAll();
            result.Should().BeAssignableTo<MessagePatchResult.BadRequest>();
        }
    }
}