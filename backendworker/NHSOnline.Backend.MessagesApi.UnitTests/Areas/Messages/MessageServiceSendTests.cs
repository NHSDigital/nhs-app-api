using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using NHSOnline.Backend.MessagesApi.Areas.Messages;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.MessagesApi.Repository;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.MessagesApi.UnitTests.Areas.Messages
{
    [TestClass]
    public class MessageServiceSendTests
    {
        private const string NhsLoginId = "NHSLoginId";

        private MessageService _systemUnderTest;
        private Mock<IMessageRepository> _mockMessageRepository;
        private Mock<IMessagesValidationService> _mockMessagesValidationService;
        private Mock<IMapper<AddMessageRequest, string, UserMessage>> _mockAddMessageToUserMessageMapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockMessageRepository = new Mock<IMessageRepository>();

            _mockMessagesValidationService = new Mock<IMessagesValidationService>(MockBehavior.Strict);
            _mockAddMessageToUserMessageMapper =
                new Mock<IMapper<AddMessageRequest, string, UserMessage>>(MockBehavior.Strict);

            _systemUnderTest = new MessageService(
                _mockMessageRepository.Object,
                new Mock<ILogger<MessagesController>>().Object,
                new Mock<IMapper<List<UserMessage>, MessagesResponse>>().Object,
                new Mock<IMapper<List<SummaryMessage>, MessagesResponse>>().Object,
                _mockAddMessageToUserMessageMapper.Object,
                _mockMessagesValidationService.Object);
        }

        [TestMethod]
        public async Task Send_Success()
        {
            // Arrange
            var request = new AddMessageRequest
            {
                Version = 1,
                Body = "Body",
                Sender = "Sender",
                CommunicationId = "CommunicationId",
                TransmissionId = "TransmissionId"
            };

            var mappedUserMessage = new UserMessage();

            var returnedUserMessage = new UserMessage
            {
                Id = ObjectId.GenerateNewId(),
            };

            _mockAddMessageToUserMessageMapper.Setup(x => x.Map(request, NhsLoginId))
                .Returns(mappedUserMessage);

            _mockMessageRepository.Setup(x => x.Create(mappedUserMessage))
                .ReturnsAsync(new RepositoryCreateResult<UserMessage>.Created(returnedUserMessage));

            _mockMessagesValidationService.Setup(x =>
                    x.IsMessageRequestValid(request, NhsLoginId))
                .Returns(true);

            // Act
            var result = await _systemUnderTest.Send(request, NhsLoginId);

            // Assert
            VerifyAll();

            result.Should().BeAssignableTo<MessageResult.Success>()
                .Subject.Response.MessageId.Should().Be(returnedUserMessage.Id.ToString());
        }

        [TestMethod]
        public async Task Send_RepositoryThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var request = new AddMessageRequest();
            var mappedUserMessage = new UserMessage();

            _mockAddMessageToUserMessageMapper.Setup(x => x.Map(request, NhsLoginId))
                .Returns(mappedUserMessage);

            _mockMessageRepository.Setup(x => x.Create(mappedUserMessage))
                .Throws(new ArgumentException("Test"));

            _mockMessagesValidationService.Setup(x =>
                    x.IsMessageRequestValid(request, NhsLoginId))
                .Returns(true);

            // Act
            var result = await _systemUnderTest.Send(request, NhsLoginId);

            // Assert
            VerifyAll();

            result.Should().BeAssignableTo<MessageResult.InternalServerError>();
        }

        [TestMethod]
        public async Task Send_RepositoryThrowsMongoException_ReturnsInternalServerError()
        {
            // Arrange
            var request = new AddMessageRequest();
            var mappedUserMessage = new UserMessage();

            _mockAddMessageToUserMessageMapper.Setup(x => x.Map(request, NhsLoginId))
                .Returns(mappedUserMessage);

            _mockMessageRepository.Setup(x => x.Create(mappedUserMessage))
                .Throws(new MongoException("Test"));

            _mockMessagesValidationService.Setup(x =>
                    x.IsMessageRequestValid(request, NhsLoginId))
                .Returns(true);

            // Act
            var result = await _systemUnderTest.Send(request, NhsLoginId);

            // Assert
            VerifyAll();

            result.Should().BeAssignableTo<MessageResult.InternalServerError>();
        }

        [TestMethod]
        public async Task Send_RepositoryReturnsError_ReturnsBadGateway()
        {
            // Arrange
            var request = new AddMessageRequest();
            var mappedUserMessage = new UserMessage();

            _mockAddMessageToUserMessageMapper.Setup(x => x.Map(request, NhsLoginId))
                .Returns(mappedUserMessage);

            _mockMessageRepository.Setup(x => x.Create(mappedUserMessage))
                .ReturnsAsync(new RepositoryCreateResult<UserMessage>.RepositoryError());

            _mockMessagesValidationService.Setup(x =>
                    x.IsMessageRequestValid(request, NhsLoginId))
                .Returns(true);

            // Act
            var result = await _systemUnderTest.Send(request, NhsLoginId);

            // Assert
            VerifyAll();

            result.Should().BeAssignableTo<MessageResult.BadGateway>();
        }

        [TestMethod]
        public async Task Send_RequestFailsValidation_ReturnsBadRequest()
        {
            // Arrange
            _mockMessagesValidationService.Setup(x =>
                    x.IsMessageRequestValid(It.IsAny<AddMessageRequest>(), It.IsAny<string>()))
                .Returns(false);

            // Act
            var result = await _systemUnderTest.Send(
                new AddMessageRequest(),
                NhsLoginId);

            // Assert
            VerifyAll();

            result.Should().BeAssignableTo<MessageResult.BadRequest>();
        }

        private void VerifyAll()
        {
            _mockAddMessageToUserMessageMapper.VerifyAll();
            _mockMessagesValidationService.VerifyAll();
            _mockMessagesValidationService.VerifyAll();
        }
    }
}