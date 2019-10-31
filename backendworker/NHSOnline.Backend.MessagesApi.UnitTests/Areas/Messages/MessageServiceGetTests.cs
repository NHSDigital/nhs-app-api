using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
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
using NHSOnline.Backend.Support;
using UnitTestHelper;

namespace NHSOnline.Backend.MessagesApi.UnitTests.Areas.Messages
{
    public class MessageServiceGetTests
    {
        private IFixture _fixture;
        private MessageService _systemUnderTest;
        private Mock<IMessageRepository> _mockMessageRepository;
        private Mock<IMapper<List<UserMessage>, MessagesResponse>> _mockUserMessagesToResponseMapper;
        private Mock<IMapper<List<SummaryMessage>, MessagesResponse>> _mockSummaryMessagesToResponseMapper;
        private AccessToken _accessToken;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new MessagesApiCustomization());

            _mockMessageRepository = _fixture.Freeze<Mock<IMessageRepository>>();
            var mockLogger = _fixture.Freeze<Mock<ILogger<MessageService>>>();
            var accessTokenString = JwtToken.Generate(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _fixture.Create<string>()),
                new Claim("nhs_number", _fixture.Create<string>())
            });
            
            _mockUserMessagesToResponseMapper = _fixture.Freeze<Mock<IMapper<List<UserMessage>, MessagesResponse>>>();
            _mockSummaryMessagesToResponseMapper =
                _fixture.Freeze<Mock<IMapper<List<SummaryMessage>, MessagesResponse>>>();

            _accessToken = AccessToken.Parse(mockLogger.Object, accessTokenString);
            _systemUnderTest = _fixture.Create<MessageService>();
        }
        
        
        [TestMethod]
        public async Task GetMessages_Some()
        {
            // Arrange
            var response = _fixture.Create<MessagesResponse>();
            _mockUserMessagesToResponseMapper.Setup(x => x.Map(It.IsAny<List<UserMessage>>())).Returns(response);

            // Act
            var result = await _systemUnderTest.GetMessages(_accessToken, _fixture.Create<string>());

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
            var result = await _systemUnderTest.GetMessages(_accessToken, _fixture.Create<string>());

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
            var result = await _systemUnderTest.GetMessages(_accessToken, _fixture.Create<string>());

            // Assert
            _mockUserMessagesToResponseMapper.VerifyAll();
            result.Should().BeAssignableTo<MessagesResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetMessages_RepositoryThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockMessageRepository.Setup(x => x.Find(It.IsAny<string>(), It.IsAny<string>()))
                .Throws<ArgumentException>();

            // Act
            var result = await _systemUnderTest.GetMessages(_accessToken, _fixture.Create<string>());

            // Assert
            _mockMessageRepository.VerifyAll();
            result.Should().BeAssignableTo<MessagesResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetMessages_RepositoryThrowsMongoException_ReturnsBadGateway()
        {
            // Arrange
            _mockMessageRepository.Setup(x => x.Find(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new MongoException("test"));

            // Act
            var result = await _systemUnderTest.GetMessages(_accessToken, _fixture.Create<string>());

            // Assert
            _mockMessageRepository.VerifyAll();
            result.Should().BeAssignableTo<MessagesResult.BadGateway>();
        }

        [TestMethod]
        public async Task GetSummaryMessages_Some()
        {
            // Arrange
            var response = _fixture.Create<MessagesResponse>();
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
            _mockMessageRepository.Setup(x => x.Summary(It.IsAny<string>())).Throws<ArgumentException>();

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
            _mockMessageRepository.Setup(x => x.Summary(It.IsAny<string>()))
                .Throws(new MongoException("test"));

            // Act
            var result = await _systemUnderTest.GetSummaryMessages(_accessToken);

            // Assert
            _mockMessageRepository.VerifyAll();
            result.Should().BeAssignableTo<MessagesResult.BadGateway>();
        }
    }
}