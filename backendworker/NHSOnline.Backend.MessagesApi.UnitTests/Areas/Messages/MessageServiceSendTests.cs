using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Moq;
using NHSOnline.Backend.MessagesApi.Areas.Messages;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.MessagesApi.Repository;

namespace NHSOnline.Backend.MessagesApi.UnitTests.Areas.Messages
{
    public class MessageServiceSendTests
    {
        private IFixture _fixture;
        private MessageService _systemUnderTest;
        private Mock<IMessageRepository> _mockMessageRepository;
        private Mock<IMessagesValidationService> _mockMessagesValidationService;
        private string _nhsLoginId;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new MessagesApiCustomization());

            _mockMessageRepository = _fixture.Freeze<Mock<IMessageRepository>>();
            _nhsLoginId = _fixture.Create<string>();

            _mockMessagesValidationService = _fixture.Freeze<Mock<IMessagesValidationService>>();
            _systemUnderTest = _fixture.Create<MessageService>();
        }
        
        [TestMethod]
        public async Task Send_Success()
        {
            // Arrange
            UserMessage actualUserMessage = null;
            var request = _fixture.Create<AddMessageRequest>();

            _mockMessageRepository.Setup(x => x.Create(It.IsAny<UserMessage>()))
                .Callback<UserMessage>(u => actualUserMessage = u)
                .Returns(Task.CompletedTask);
            
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
            actualUserMessage.SentTime.Should().BeCloseTo(DateTime.Now, precision: 60);
        }

        [TestMethod]
        public async Task Send_RepositoryThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var request = _fixture.Create<AddMessageRequest>();
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
            var request = _fixture.Create<AddMessageRequest>();
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
                _fixture.Create<AddMessageRequest>(), 
                _fixture.Create<string>());
            
            // Assert
            _mockMessagesValidationService.VerifyAll();
            result.Should().BeAssignableTo<MessageResult.BadRequest>();
        }
    }
}