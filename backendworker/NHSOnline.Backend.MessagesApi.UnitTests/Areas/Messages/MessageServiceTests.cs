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
    [TestClass]
    public class MessageServiceTests
    {
        private IFixture _fixture;
        private MessageService _systemUnderTest;
        private Mock<IMessageRepository> _mockMessageRepository;
        private string _nhsLoginId;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _mockMessageRepository = _fixture.Freeze<Mock<IMessageRepository>>();
            _nhsLoginId = _fixture.Create<string>();
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

            // Act
            var result = await _systemUnderTest.Send(request, _nhsLoginId);

            // Assert
            _mockMessageRepository.VerifyAll();
            result.Should().BeAssignableTo<MessageResult.Success>();
            actualUserMessage.Should().BeEquivalentTo(new UserMessage
            {
                NhsLoginId = _nhsLoginId,
                Sender = request.Sender,
                Version = request.Version,
                Body = request.Body
            });
        }

        [TestMethod]
        public async Task Send_RepositoryThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var request = _fixture.Create<AddMessageRequest>();
            _mockMessageRepository.Setup(x => x.Create(It.IsAny<UserMessage>())).Throws(new ArgumentException("Test"));

            // Act
            var result = await _systemUnderTest.Send(request, _nhsLoginId);

            // Assert
            _mockMessageRepository.VerifyAll();
            result.Should().BeAssignableTo<MessageResult.InternalServerError>();
        }

        [TestMethod]
        public async Task Send_RepositoryThrowsMongoException_ReturnsBadGateway()
        {
            // Arrange
            var request = _fixture.Create<AddMessageRequest>();
            _mockMessageRepository.Setup(x => x.Create(It.IsAny<UserMessage>())).Throws(new MongoException("Test"));

            // Act
            var result = await _systemUnderTest.Send(request, _nhsLoginId);

            // Assert
            _mockMessageRepository.VerifyAll();
            result.Should().BeAssignableTo<MessageResult.BadGateway>();
        }
    }
}