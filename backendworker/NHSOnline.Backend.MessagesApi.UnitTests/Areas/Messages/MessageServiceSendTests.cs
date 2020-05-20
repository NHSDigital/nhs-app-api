using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Moq;
using NHSOnline.Backend.MessagesApi.Areas.Messages;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.MessagesApi.Repository;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.MessagesApi.UnitTests.Areas.Messages
{
    public class MessageServiceSendTests
    {
        private MessageService _systemUnderTest;
        private Mock<IMessageRepository> _mockMessageRepository;
        private Mock<IMessagesValidationService> _mockMessagesValidationService;
        private string _nhsLoginId;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockMessageRepository = new Mock<IMessageRepository>();
            _nhsLoginId = "NHSLoginId";

            _mockMessagesValidationService = new Mock<IMessagesValidationService>();

            _systemUnderTest = new MessageService(
                _mockMessageRepository.Object,
                new Mock<ILogger<MessagesController>>().Object,
                new Mock<IMapper<List<UserMessage>, MessagesResponse>>().Object,
                new Mock<IMapper<List<SummaryMessage>, MessagesResponse>>().Object,
                _mockMessagesValidationService.Object);
        }

        [TestMethod]
        public async Task Send_Success()
        {
            // Arrange
            UserMessage actualUserMessage = null;
            var request = new AddMessageRequest();

            _mockMessageRepository.Setup(x => x.Create(It.IsAny<UserMessage>()))
                .Callback<UserMessage>(u => actualUserMessage = u)
                .ReturnsAsync(new RepositoryCreateResult<UserMessage>.Created(actualUserMessage));

            _mockMessagesValidationService.Setup(x =>
                    x.IsMessageRequestValid(request, _nhsLoginId))
                .Returns(true);

            // Act
            var result = await _systemUnderTest.Send(request, _nhsLoginId);

            // Assert
            _mockMessageRepository.VerifyAll();
            _mockMessagesValidationService.VerifyAll();
            result.Should().BeAssignableTo<MessageResult.Success>();
            actualUserMessage.NhsLoginId.Should().BeEquivalentTo(_nhsLoginId);
            actualUserMessage.Sender.Should().BeEquivalentTo(request.Sender);
            actualUserMessage.Version.Should().Be(request.Version);
            actualUserMessage.Body.Should().BeEquivalentTo(request.Body);
            actualUserMessage.SentTime.Should().BeCloseTo(DateTime.UtcNow, precision: 60);
        }

        [TestMethod]
        public async Task Send_RepositoryThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var request = new AddMessageRequest();
            _mockMessageRepository.Setup(x => x.Create(
                It.IsAny<UserMessage>())).Throws(new ArgumentException("Test"));

            _mockMessagesValidationService.Setup(x =>
                    x.IsMessageRequestValid(request, _nhsLoginId))
                .Returns(true);

            // Act
            var result = await _systemUnderTest.Send(request, _nhsLoginId);

            // Assert
            _mockMessageRepository.VerifyAll();
            _mockMessagesValidationService.VerifyAll();
            result.Should().BeAssignableTo<MessageResult.InternalServerError>();
        }

        [TestMethod]
        public async Task Send_RepositoryThrowsMongoException_ReturnsBadGateway()
        {
            // Arrange
            var request = new AddMessageRequest();
            _mockMessageRepository.Setup(x => x.Create(It.IsAny<UserMessage>()))
                .Throws(new MongoException("Test"));

            _mockMessagesValidationService.Setup(x =>
                    x.IsMessageRequestValid(request, _nhsLoginId))
                .Returns(true);

            // Act
            var result = await _systemUnderTest.Send(request, _nhsLoginId);

            // Assert
            _mockMessageRepository.VerifyAll();
            _mockMessagesValidationService.VerifyAll();
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
                _nhsLoginId);

            // Assert
            _mockMessagesValidationService.VerifyAll();
            result.Should().BeAssignableTo<MessageResult.BadRequest>();
        }
    }
}