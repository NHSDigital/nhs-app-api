using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Moq;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.MessagesApi.Areas.Messages;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.MessagesApi.Repository;
using UnitTestHelper;

namespace NHSOnline.Backend.MessagesApi.UnitTests.Areas.Messages
{
    [TestClass]
    public class MessageServiceTests
    {
        private IFixture _fixture;
        private MessageService _systemUnderTest;
        private Mock<IMessageRepository> _mockMessageRepository;
        private string _nhsLoginId;
        private AccessToken _accessToken;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());
            _mockMessageRepository = _fixture.Freeze<Mock<IMessageRepository>>();
            _nhsLoginId = _fixture.Create<string>();
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

        [TestMethod]
        public async Task GetMessages_Some()
        {
            // Arrange
            var message = MessageHelpers.MockUserMessage(_fixture);
            var messages = new[] { message }.ToList();
            _mockMessageRepository.Setup(x => x.Find(It.IsAny<string>())).ReturnsAsync(messages);

            // Act
            var result = await _systemUnderTest.GetMessages(_accessToken);

            // Assert
            _mockMessageRepository.VerifyAll();
            result.Should().BeAssignableTo<MessagesResult.Some>()
                .Subject.Messages.Should().BeEquivalentTo(messages);
        }

        [TestMethod]
        public async Task GetMessages_None()
        {
            // Arrange
            var messages = new List<UserMessage>();
            _mockMessageRepository.Setup(x => x.Find(It.IsAny<string>())).ReturnsAsync(messages);

            // Act
            var result = await _systemUnderTest.GetMessages(_accessToken);

            // Assert
            _mockMessageRepository.VerifyAll();
            result.Should().BeAssignableTo<MessagesResult.None>();
        }

        [TestMethod]
        public async Task Get_MessageServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockMessageRepository.Setup(x => x.Find(It.IsAny<string>()))
                .Throws<ArgumentException>();

            // Act
            var result = await _systemUnderTest.GetMessages(_accessToken);

            // Assert
            _mockMessageRepository.VerifyAll();
            result.Should().BeAssignableTo<MessagesResult.InternalServerError>();
        }

        [TestMethod]
        public async Task Get_MessageServiceThrowsException_ReturnsBadGateway()
        {
            // Arrange
            _mockMessageRepository.Setup(x => x.Find(It.IsAny<string>()))
                .Throws(new MongoException("test"));

            // Act
            var result = await _systemUnderTest.GetMessages(_accessToken);

            // Assert
            _mockMessageRepository.VerifyAll();
            result.Should().BeAssignableTo<MessagesResult.BadGateway>();
        }
    }
}