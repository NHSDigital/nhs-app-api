using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.MessagesApi.Areas.Messages;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.MessagesApi.Repository;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support;
using UnitTestHelper;

namespace NHSOnline.Backend.MessagesApi.UnitTests.Areas.Messages
{
    [TestClass]
    public class MessageServiceGetTests
    {
        private const string Body = "Body";
        private const string MessageId = "MessageId";
        private const string NhsLoginId = "NhsLoginId";
        private const string NhsNumber = "NhsNumber";
        private const string Sender = "Sender";

        private IMessageService _systemUnderTest;

        private Mock<IMessageRepository> _mockMessageRepository;
        private Mock<IMapper<List<UserMessage>, MessagesResponse>> _mockUserMessagesToResponseMapper;
        private Mock<IMapper<List<SummaryMessage>, MessagesResponse>> _mockSummaryMessagesToResponseMapper;
        private Mock<IMessagesValidationService> _mockValidationService;
        private AccessToken _accessToken;

        private readonly MessagesResponse _response = new MessagesResponse(new[] { new SenderMessages { Sender = Sender } });
        private readonly SummaryMessage _summaryMessage = new SummaryMessage { Body = Body };
        private readonly UserMessage _userMessage = new UserMessage { Body = Body };

        [TestInitialize]
        public void TestInitialize()
        {
            var accessTokenString = JwtToken.Generate(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, NhsLoginId),
                new Claim("nhs_number", NhsNumber)
            });

            _mockMessageRepository = new Mock<IMessageRepository>(MockBehavior.Strict);
            _mockUserMessagesToResponseMapper = new Mock<IMapper<List<UserMessage>, MessagesResponse>>(MockBehavior.Strict);
            _mockSummaryMessagesToResponseMapper = new Mock<IMapper<List<SummaryMessage>, MessagesResponse>>(MockBehavior.Strict);
            _mockValidationService = new Mock<IMessagesValidationService>(MockBehavior.Strict);

            var mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<MessagesController>>();

            _accessToken = AccessToken.Parse(mockLogger.Object, accessTokenString);

            _systemUnderTest = new MessageService(
                _mockMessageRepository.Object,
                mockLogger.Object,
                _mockUserMessagesToResponseMapper.Object,
                _mockSummaryMessagesToResponseMapper.Object,
                new Mock<IMapper<AddMessageRequest, string, UserMessage>>().Object,
                _mockValidationService.Object
            );
        }

        [TestMethod]
        public async Task GetMessage_ValidationServiceReturnsFalse_ReturnsBadRequest()
        {
            // Arrange
            _mockValidationService
                .Setup(x => x.IsMessageIdValid(MessageId))
                .Returns(false);

            // Act
            var result = await _systemUnderTest.GetMessage(_accessToken, MessageId);

            // Assert
            VerifySetups();

            result.Should().BeAssignableTo<MessagesResult.BadRequest>();
        }

        [TestMethod]
        public async Task GetMessage_Some()
        {
            // Arrange
            _mockValidationService
                .Setup(x => x.IsMessageIdValid(MessageId))
                .Returns(true);

            _mockMessageRepository
                .Setup(x => x.FindMessage(_accessToken.Subject, MessageId))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.Found(new[]
                {
                    _userMessage
                }));

            _mockUserMessagesToResponseMapper
                .Setup(x => x.Map(It.IsAny<List<UserMessage>>()))
                .Returns(_response);

            // Act
            var result = await _systemUnderTest.GetMessage(_accessToken, MessageId);

            // Assert
            VerifySetups();

            result.Should().BeAssignableTo<MessagesResult.Some>()
                .Subject.Response.Should().BeEquivalentTo(_response);
        }

        [TestMethod]
        public async Task GetMessage_None()
        {
            // Arrange
            _mockValidationService
                .Setup(x => x.IsMessageIdValid(MessageId))
                .Returns(true);

            _mockMessageRepository
                .Setup(x => x.FindMessage(_accessToken.Subject, MessageId))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.NotFound());

            // Act
            var result = await _systemUnderTest.GetMessage(_accessToken, MessageId);

            // Assert
            VerifySetups();

            result.Should().BeAssignableTo<MessagesResult.None>();
        }

        [TestMethod]
        public async Task GetMessage_MapperThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockValidationService
                .Setup(x => x.IsMessageIdValid(MessageId))
                .Returns(true);

            _mockMessageRepository
                .Setup(x => x.FindMessage(NhsLoginId, MessageId))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.Found(new[]
                {
                    _userMessage
                }));

            _mockUserMessagesToResponseMapper
                .Setup(x => x.Map(It.IsAny<List<UserMessage>>()))
                .Throws<ArgumentException>();

            // Act
            var result = await _systemUnderTest.GetMessage(_accessToken, MessageId);

            // Assert
            VerifySetups();

            result.Should().BeAssignableTo<MessagesResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetMessage_RepositoryThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockValidationService
                .Setup(x => x.IsMessageIdValid(MessageId))
                .Returns(true);

            _mockMessageRepository
                .Setup(x => x.FindMessage(NhsLoginId, MessageId))
                .Throws<ArgumentException>();

            // Act
            var result = await _systemUnderTest.GetMessage(_accessToken, MessageId);

            // Assert
            VerifySetups();

            result.Should().BeAssignableTo<MessagesResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetMessage_RepositoryReturnsError_ReturnsBadGateway()
        {
            // Arrange
            _mockValidationService
                .Setup(x => x.IsMessageIdValid(MessageId))
                .Returns(true);

            _mockMessageRepository
                .Setup(x => x.FindMessage(NhsLoginId, MessageId))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.RepositoryError());

            // Act
            var result = await _systemUnderTest.GetMessage(_accessToken, MessageId);

            // Assert
            VerifySetups();

            result.Should().BeAssignableTo<MessagesResult.BadGateway>();
        }

        [TestMethod]
        public async Task GetMessages_Some()
        {
            // Arrange
            _mockMessageRepository
                .Setup(x => x.FindMessagesFromSender(_accessToken.Subject, Sender))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.Found(new[]
                {
                    _userMessage
                }));

            _mockUserMessagesToResponseMapper
                .Setup(x => x.Map(It.IsAny<List<UserMessage>>()))
                .Returns(_response);

            // Act
            var result = await _systemUnderTest.GetMessages(_accessToken, Sender);

            // Assert
            VerifySetups();

            result.Should().BeAssignableTo<MessagesResult.Some>()
                .Subject.Response.Should().BeEquivalentTo(_response);
        }

        [TestMethod]
        public async Task GetMessages_None()
        {
            // Arrange
            _mockMessageRepository
                .Setup(x => x.FindMessagesFromSender(_accessToken.Subject, Sender))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.NotFound());

            // Act
            var result = await _systemUnderTest.GetMessages(_accessToken, Sender);

            // Assert
            VerifySetups();

            result.Should().BeAssignableTo<MessagesResult.None>();
        }

        [TestMethod]
        public async Task GetMessages_MapperThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockMessageRepository
                .Setup(x => x.FindMessagesFromSender(_accessToken.Subject, Sender))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.Found(new[]
                {
                    _userMessage
                }));

            _mockUserMessagesToResponseMapper
                .Setup(x => x.Map(It.IsAny<List<UserMessage>>()))
                .Throws<ArgumentException>();

            // Act
            var result = await _systemUnderTest.GetMessages(_accessToken, Sender);

            // Assert
            VerifySetups();

            result.Should().BeAssignableTo<MessagesResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetMessages_RepositoryThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockMessageRepository
                .Setup(x => x.FindMessagesFromSender(NhsLoginId, Sender))
                .Throws<ArgumentException>();

            // Act
            var result = await _systemUnderTest.GetMessages(_accessToken, Sender);

            // Assert
            VerifySetups();

            result.Should().BeAssignableTo<MessagesResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetMessages_RepositoryReturnsError_ReturnsBadGateway()
        {
            // Arrange
            _mockMessageRepository
                .Setup(x => x.FindMessagesFromSender(NhsLoginId, Sender))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.RepositoryError());

            // Act
            var result = await _systemUnderTest.GetMessages(_accessToken, Sender);

            // Assert
            VerifySetups();

            result.Should().BeAssignableTo<MessagesResult.BadGateway>();
        }

        [TestMethod]
        public async Task GetSummaryMessages_Some()
        {
            // Arrange
            _mockMessageRepository
                .Setup(x => x.FindAllForUser(_accessToken.Subject))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.Found(new[]
                {
                    _summaryMessage
                }));

            _mockSummaryMessagesToResponseMapper
                .Setup(x => x.Map(It.IsAny<List<SummaryMessage>>()))
                .Returns(_response);

            // Act
            var result = await _systemUnderTest.GetSummaryMessages(_accessToken);

            // Assert
            VerifySetups();

            result.Should().BeAssignableTo<MessagesResult.Some>()
                .Subject.Response.Should().BeEquivalentTo(_response);
        }

        [TestMethod]
        public async Task GetSummaryMessages_None()
        {
            // Arrange
            _mockMessageRepository
                .Setup(x => x.FindAllForUser(_accessToken.Subject))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.NotFound());

            // Act
            var result = await _systemUnderTest.GetSummaryMessages(_accessToken);

            // Assert
            VerifySetups();

            result.Should().BeAssignableTo<MessagesResult.None>();
        }

        [TestMethod]
        public async Task GetSummaryMessages_MapperThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockMessageRepository
                .Setup(x => x.FindAllForUser(_accessToken.Subject))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.Found(new[]
                {
                    _summaryMessage
                }));

            _mockSummaryMessagesToResponseMapper
                .Setup(x => x.Map(It.IsAny<List<SummaryMessage>>()))
                .Throws<ArgumentException>();

            // Act
            var result = await _systemUnderTest.GetSummaryMessages(_accessToken);

            // Assert
            VerifySetups();

            result.Should().BeAssignableTo<MessagesResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetSummaryMessages_RepositoryThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockMessageRepository
                .Setup(x => x.FindAllForUser(NhsLoginId))
                .Throws<ArgumentException>();

            // Act
            var result = await _systemUnderTest.GetSummaryMessages(_accessToken);

            // Assert
            VerifySetups();

            result.Should().BeAssignableTo<MessagesResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetSummaryMessages_RepositoryReturnsError_ReturnsBadGateway()
        {
            // Arrange
            _mockMessageRepository
                .Setup(x => x.FindAllForUser(NhsLoginId))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.RepositoryError());

            // Act
            var result = await _systemUnderTest.GetSummaryMessages(_accessToken);

            // Assert
            VerifySetups();

            result.Should().BeAssignableTo<MessagesResult.BadGateway>();
        }

        private void VerifySetups()
        {
            _mockMessageRepository.VerifyAll();
            _mockSummaryMessagesToResponseMapper.VerifyAll();
            _mockUserMessagesToResponseMapper.VerifyAll();
            _mockValidationService.VerifyAll();
        }
    }
}