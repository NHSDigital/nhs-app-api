using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
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
    public class MessageServiceGetTests
    {
        private MessageService _systemUnderTest;
        private Mock<IMessageRepository> _mockMessageRepository;
        private Mock<IMapper<List<UserMessage>, MessagesResponse>> _mockUserMessagesToResponseMapper;
        private Mock<IMapper<List<SummaryMessage>, MessagesResponse>> _mockSummaryMessagesToResponseMapper;
        private AccessToken _accessToken;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockMessageRepository = new Mock<IMessageRepository>();
            var mockLogger = new Moq.Mock<Microsoft.Extensions.Logging.ILogger<MessagesController>>();
            var accessTokenString = JwtToken.Generate(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "NhsLoginId"),
                new Claim("nhs_number", "nhsNumber")
            });

            _mockUserMessagesToResponseMapper = new Mock<IMapper<List<UserMessage>, MessagesResponse>>();
            _mockSummaryMessagesToResponseMapper =new Mock<IMapper<List<SummaryMessage>, MessagesResponse>>();

            _accessToken = AccessToken.Parse(mockLogger.Object, accessTokenString);

            _systemUnderTest = new MessageService(
                _mockMessageRepository.Object,
                mockLogger.Object,
                _mockUserMessagesToResponseMapper.Object,
                _mockSummaryMessagesToResponseMapper.Object,
                new Moq.Mock<IMessagesValidationService>().Object);
        }
        
        
        [TestMethod]
        public async Task GetMessages_Some()
        {
            // Arrange
            var response = new MessagesResponse();
            _mockUserMessagesToResponseMapper.Setup(x => x.Map(It.IsAny<List<UserMessage>>())).Returns(response);

            // Act
            var result = await _systemUnderTest.GetMessages(_accessToken, "sender");

            // Assert
            _mockUserMessagesToResponseMapper.VerifyAll();
            result.Should().BeAssignableTo<MessagesResult.Some>()
                .Subject.Response.Should().BeEquivalentTo(response);
        }

        [TestMethod]
        public async Task GetMessages_None()
        {
            // Arrange
            _mockUserMessagesToResponseMapper.Setup(x => x.Map(It.IsAny<List<UserMessage>>()))
                .Returns(new MessagesResponse());

            // Act
            var result = await _systemUnderTest.GetMessages(_accessToken, "sender");

            // Assert
            _mockUserMessagesToResponseMapper.VerifyAll();
            result.Should().BeAssignableTo<MessagesResult.None>();
        }

        [TestMethod]
        public async Task GetMessages_MapperThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockUserMessagesToResponseMapper.Setup(x => x.Map(It.IsAny<List<UserMessage>>()))
                .Throws<ArgumentException>();

            // Act
            var result = await _systemUnderTest.GetMessages(_accessToken, "sender");

            // Assert
            _mockUserMessagesToResponseMapper.VerifyAll();
            result.Should().BeAssignableTo<MessagesResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetMessages_RepositoryThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockMessageRepository.Setup(x => x.FindMessagesFromSender(It.IsAny<string>(), It.IsAny<string>()))
                .Throws<ArgumentException>();

            // Act
            var result = await _systemUnderTest.GetMessages(_accessToken, "sender");

            // Assert
            _mockMessageRepository.VerifyAll();
            result.Should().BeAssignableTo<MessagesResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetMessages_RepositoryThrowsMongoException_ReturnsBadGateway()
        {
            // Arrange
            _mockMessageRepository.Setup(x => x.FindMessagesFromSender(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new MongoException("test"));

            // Act
            var result = await _systemUnderTest.GetMessages(_accessToken, "sender");

            // Assert
            _mockMessageRepository.VerifyAll();
            result.Should().BeAssignableTo<MessagesResult.BadGateway>();
        }

        [TestMethod]
        public async Task GetSummaryMessages_Some()
        {
            // Arrange
            var response = new MessagesResponse();
            _mockSummaryMessagesToResponseMapper.Setup(x => x.Map(It.IsAny<List<SummaryMessage>>())).Returns(response);

            // Act
            var result = await _systemUnderTest.GetSummaryMessages(_accessToken);

            // Assert
            _mockSummaryMessagesToResponseMapper.VerifyAll();
            result.Should().BeAssignableTo<MessagesResult.Some>()
                .Subject.Response.Should().BeEquivalentTo(response);
        }

        [TestMethod]
        public async Task GetSummaryMessages_None()
        {
            // Arrange
            _mockSummaryMessagesToResponseMapper.Setup(x => x.Map(It.IsAny<List<SummaryMessage>>()))
                .Returns(new MessagesResponse());

            // Act
            var result = await _systemUnderTest.GetSummaryMessages(_accessToken);

            // Assert
            _mockSummaryMessagesToResponseMapper.VerifyAll();
            result.Should().BeAssignableTo<MessagesResult.None>();
        }

        [TestMethod]
        public async Task GetSummaryMessages_MapperThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockSummaryMessagesToResponseMapper.Setup(x => x.Map(It.IsAny<List<SummaryMessage>>()))
                .Throws<ArgumentException>();

            // Act
            var result = await _systemUnderTest.GetSummaryMessages(_accessToken);

            // Assert
            _mockSummaryMessagesToResponseMapper.VerifyAll();
            result.Should().BeAssignableTo<MessagesResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetSummaryMessages_RepositoryThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockMessageRepository.Setup(x => x.FindAllForUser(It.IsAny<string>())).Throws<ArgumentException>();

            // Act
            var result = await _systemUnderTest.GetSummaryMessages(_accessToken);

            // Assert
            _mockMessageRepository.VerifyAll();
            result.Should().BeAssignableTo<MessagesResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetSummaryMessages_RepositoryThrowsMongoException_ReturnsBadGateway()
        {
            // Arrange
            _mockMessageRepository.Setup(x => x.FindAllForUser(It.IsAny<string>()))
                .Throws(new MongoException("test"));

            // Act
            var result = await _systemUnderTest.GetSummaryMessages(_accessToken);

            // Assert
            _mockMessageRepository.VerifyAll();
            result.Should().BeAssignableTo<MessagesResult.BadGateway>();
        }
    }
}