using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class MessageServiceGetTests
    {
        private const string Body = "Body";
        private const string MessageId = "MessageId";
        private const string NhsLoginId = "NhsLoginId";
        private const string NhsNumber = "NhsNumber";
        private const string Sender = "Sender";
        private const string SenderId = "SenderId";

        private IMessageService _systemUnderTest;
        private Mock<IMessagesConfiguration> _mockMessagesConfiguration;
        private Mock<IMessageRepository> _mockMessageRepository;
        private Mock<IMapper<List<UserMessage>, MessagesResponse>> _mockUserMessagesToResponseMapper;
        private Mock<IMapper<UserMessage, MessagesResponse>> _mockUserMessageToResponseMapper;
        private Mock<IMapper<List<SummaryMessage>, MessagesResponse>> _mockSummaryMessagesToResponseMapper;
        private Mock<IMessagesValidationService> _mockValidationService;
        private AccessToken _accessToken;

        private readonly MessagesResponse _response = new MessagesResponse
        {
            SenderMessages = new List<SenderMessages>
            {
                new SenderMessages { Sender = Sender }
            }
        };
        private readonly SummaryMessage _summaryMessage = new SummaryMessage { Body = Body, Sender = Sender};
        private readonly UserMessage _userMessage = new UserMessage { Body = Body, Sender = Sender };

        [TestInitialize]
        public void TestInitialize()
        {
            var accessTokenString = JwtToken.Generate(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, NhsLoginId),
                new Claim("nhs_number", NhsNumber)
            });
            _mockMessagesConfiguration = new Mock<IMessagesConfiguration>(MockBehavior.Strict);
            _mockMessageRepository = new Mock<IMessageRepository>(MockBehavior.Strict);
            _mockUserMessagesToResponseMapper = new Mock<IMapper<List<UserMessage>, MessagesResponse>>(MockBehavior.Strict);
            _mockUserMessageToResponseMapper =  new Mock<IMapper<UserMessage, MessagesResponse>>(MockBehavior.Strict);
            _mockSummaryMessagesToResponseMapper = new Mock<IMapper<List<SummaryMessage>, MessagesResponse>>(MockBehavior.Strict);
            _mockValidationService = new Mock<IMessagesValidationService>(MockBehavior.Strict);

            var mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<MessageService>>();

            _accessToken = AccessToken.Parse(mockLogger.Object, accessTokenString);

            _systemUnderTest = new MessageService(
                _mockMessagesConfiguration.Object,
                _mockMessageRepository.Object,
                mockLogger.Object,
                _mockUserMessagesToResponseMapper.Object,
                _mockUserMessageToResponseMapper.Object,
                _mockSummaryMessagesToResponseMapper.Object,
                new Mock<IMapper<AddMessageRequest, string, UserMessage>>().Object,
                _mockValidationService.Object);
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
        public async Task GetMessage_Found()
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

            _mockUserMessageToResponseMapper
                .Setup(x => x.Map(It.IsAny<UserMessage>()))
                .Returns(_response);

            // Act
            var result = await _systemUnderTest.GetMessage(_accessToken, MessageId);

            // Assert
            VerifySetups();

            result.Should().BeAssignableTo<MessagesResult.Found>()
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

            _mockUserMessageToResponseMapper
                .Setup(x => x.Map(It.IsAny<UserMessage>()))
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
        public async Task GetMessagesBySender_Found()
        {
            // Arrange
            _mockMessageRepository
                .Setup(x => x.FindMessagesFromSenderByName(_accessToken.Subject, Sender))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.Found(new[]
                {
                    _userMessage
                }));

            _mockUserMessagesToResponseMapper
                .Setup(x => x.Map(It.IsAny<List<UserMessage>>()))
                .Returns(_response);

            // Act
            var result = await _systemUnderTest.GetMessagesBySender(_accessToken, Sender);

            // Assert
            VerifySetups();

            result.Should().BeAssignableTo<MessagesResult.Found>()
                .Subject.Response.Should().BeEquivalentTo(_response);
        }

        [TestMethod]
        public async Task GetMessagesBySender_None()
        {
            // Arrange
            _mockMessageRepository
                .Setup(x => x.FindMessagesFromSenderByName(_accessToken.Subject, Sender))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.NotFound());

            // Act
            var result = await _systemUnderTest.GetMessagesBySender(_accessToken, Sender);

            // Assert
            VerifySetups();

            result.Should().BeAssignableTo<MessagesResult.None>();
        }

        [TestMethod]
        public async Task GetMessagesBySender_MapperThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockMessageRepository
                .Setup(x => x.FindMessagesFromSenderByName(_accessToken.Subject, Sender))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.Found(new[]
                {
                    _userMessage
                }));

            _mockUserMessagesToResponseMapper
                .Setup(x => x.Map(It.IsAny<List<UserMessage>>()))
                .Throws<ArgumentException>();

            // Act
            var result = await _systemUnderTest.GetMessagesBySender(_accessToken, Sender);

            // Assert
            VerifySetups();

            result.Should().BeAssignableTo<MessagesResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetMessagesBySender_RepositoryThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockMessageRepository
                .Setup(x => x.FindMessagesFromSenderByName(NhsLoginId, Sender))
                .Throws<ArgumentException>();

            // Act
            var result = await _systemUnderTest.GetMessagesBySender(_accessToken, Sender);

            // Assert
            VerifySetups();

            result.Should().BeAssignableTo<MessagesResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetMessagesBySender_RepositoryReturnsError_ReturnsBadGateway()
        {
            // Arrange
            _mockMessageRepository
                .Setup(x => x.FindMessagesFromSenderByName(NhsLoginId, Sender))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.RepositoryError());

            // Act
            var result = await _systemUnderTest.GetMessagesBySender(_accessToken, Sender);

            // Assert
            VerifySetups();

            result.Should().BeAssignableTo<MessagesResult.BadGateway>();
        }

        [TestMethod]
        public async Task GetMessagesBySenderId_Found()
        {
            // Arrange
            _mockMessageRepository
                .Setup(x => x.FindMessagesFromSenderById(_accessToken.Subject, SenderId))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.Found(new[]
                {
                    _userMessage
                }));

            _mockUserMessagesToResponseMapper
                .Setup(x => x.Map(It.IsAny<List<UserMessage>>()))
                .Returns(_response);

            // Act
            var result = await _systemUnderTest.GetMessagesBySenderId(_accessToken, SenderId);

            // Assert
            VerifySetups();

            result.Should().BeAssignableTo<MessagesResult.Found>()
                .Subject.Response.Should().BeEquivalentTo(_response);
        }

        [TestMethod]
        public async Task GetMessagesBySenderId_None()
        {
            // Arrange
            _mockMessageRepository
                .Setup(x => x.FindMessagesFromSenderById(_accessToken.Subject, SenderId))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.NotFound());

            // Act
            var result = await _systemUnderTest.GetMessagesBySenderId(_accessToken, SenderId);

            // Assert
            VerifySetups();

            result.Should().BeAssignableTo<MessagesResult.None>();
        }

        [TestMethod]
        public async Task GetMessagesBySenderId_MapperThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockMessageRepository
                .Setup(x => x.FindMessagesFromSenderById(_accessToken.Subject, SenderId))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.Found(new[]
                {
                    _userMessage
                }));

            _mockUserMessagesToResponseMapper
                .Setup(x => x.Map(It.IsAny<List<UserMessage>>()))
                .Throws<ArgumentException>();

            // Act
            var result = await _systemUnderTest.GetMessagesBySenderId(_accessToken, SenderId);

            // Assert
            VerifySetups();

            result.Should().BeAssignableTo<MessagesResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetMessagesBySenderId_RepositoryThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockMessageRepository
                .Setup(x => x.FindMessagesFromSenderById(NhsLoginId, SenderId))
                .Throws<ArgumentException>();

            // Act
            var result = await _systemUnderTest.GetMessagesBySenderId(_accessToken, SenderId);

            // Assert
            VerifySetups();

            result.Should().BeAssignableTo<MessagesResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetMessagesBySenderId_RepositoryReturnsError_ReturnsBadGateway()
        {
            // Arrange
            _mockMessageRepository
                .Setup(x => x.FindMessagesFromSenderById(NhsLoginId, SenderId))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.RepositoryError());

            // Act
            var result = await _systemUnderTest.GetMessagesBySenderId(_accessToken, SenderId);

            // Assert
            VerifySetups();

            result.Should().BeAssignableTo<MessagesResult.BadGateway>();
        }

        [TestMethod]
        public async Task GetSummaryMessages_Found()
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

            result.Should().BeAssignableTo<MessagesResult.Found>()
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

        [TestMethod]
        public async Task GetSendersV2_WhenSenderIdDisabled_FoundResultGroupBySenderName()
        {
            // Arrange
            _mockMessageRepository
                .Setup(x => x.FindAllForUser(NhsLoginId))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.Found(new[]
                {
                    new UserMessage { Sender = "First", SentTime = DateTime.UtcNow.AddSeconds(-20), ReadTime = DateTime.UtcNow },
                    new UserMessage { Sender = "Second", SentTime = DateTime.UtcNow.AddSeconds(-10), ReadTime = DateTime.UtcNow },
                    new UserMessage { Sender = "Third", SentTime = DateTime.UtcNow },
                    new UserMessage { Sender = "First", SentTime = DateTime.UtcNow.AddSeconds(10) },
                    new UserMessage { Sender = "Third", SentTime = DateTime.UtcNow.AddSeconds(20) },
                }));

            _mockMessagesConfiguration
                .Setup(x => x.SenderIdEnabled)
                .Returns(false);

            // Act
            var result = await _systemUnderTest.GetSendersV2(_accessToken);

            // Assert
            VerifySetups();

            var response = result.Should().BeAssignableTo<UserSendersResult.Found>().Subject.Response;
            response.Should().NotBeNull();
            response.Senders.Should().HaveCount(3);
            response.Senders[0].Should().BeEquivalentTo(new UserSender {  Name = "Third", UnreadCount = 2 });
            response.Senders[1].Should().BeEquivalentTo(new UserSender {  Name = "First", UnreadCount = 1 });
            response.Senders[2].Should().BeEquivalentTo(new UserSender {  Name = "Second", UnreadCount = 0 });
        }

        [TestMethod]
        public async Task GetSendersV2_WhenSenderIdEnabled_FoundResultGroupBySenderId()
        {
            // Arrange
            _mockMessageRepository
                .Setup(x => x.FindAllForUser(NhsLoginId))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.Found(new[]
                {
                    new UserMessage
                    {
                        Sender = "First",
                        SentTime = DateTime.UtcNow.AddSeconds(-20),
                        ReadTime = DateTime.UtcNow,
                        SenderContext = new SenderContext { SenderId = "SenderIdOne" }
                    },
                    new UserMessage
                    {
                        Sender = "Second",
                        SentTime = DateTime.UtcNow.AddSeconds(-10),
                        ReadTime = DateTime.UtcNow,
                        SenderContext = new SenderContext { SenderId = "SenderIdTwo" }
                    },
                    new UserMessage
                    {
                        Sender = "Third",
                        SentTime = DateTime.UtcNow,
                        SenderContext = new SenderContext { SenderId = "SenderIdThree" }
                    },
                    new UserMessage
                    {
                        Sender = "First Sender",
                        SentTime = DateTime.UtcNow.AddSeconds(10),
                        SenderContext = new SenderContext { SenderId = "SenderIdOne" }
                    },
                    new UserMessage
                    {
                        Sender = "Third Sender",
                        SentTime = DateTime.UtcNow.AddSeconds(20),
                        SenderContext = new SenderContext { SenderId = "SenderIdThree" }
                    },
                }));

            _mockMessagesConfiguration
                .Setup(x => x.SenderIdEnabled)
                .Returns(true);

            // Act
            var result = await _systemUnderTest.GetSendersV2(_accessToken);

            // Assert
            VerifySetups();

            var response = result.Should().BeAssignableTo<UserSendersResult.Found>().Subject.Response;
            response.Should().NotBeNull();
            response.Senders.Should().HaveCount(3);
            response.Senders[0].Should().BeEquivalentTo(new UserSender {  Id = "SenderIdThree", Name = "Third Sender", UnreadCount = 2 });
            response.Senders[1].Should().BeEquivalentTo(new UserSender {  Id = "SenderIdOne", Name = "First Sender", UnreadCount = 1 });
            response.Senders[2].Should().BeEquivalentTo(new UserSender {  Id = "SenderIdTwo", Name = "Second", UnreadCount = 0 });
        }

        [DataTestMethod]
        [DataRow(true, DisplayName = "SenderIdEnabled")]
        [DataRow(false, DisplayName = "SenderIdDisabled")]
        public async Task GetSendersV2_None(bool isSenderIdEnabled)
        {
            // Arrange
            _mockMessageRepository
                .Setup(x => x.FindAllForUser(NhsLoginId))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.NotFound());

            _mockMessagesConfiguration
                .Setup(x => x.SenderIdEnabled)
                .Returns(isSenderIdEnabled);

            // Act
            var result = await _systemUnderTest.GetSendersV2(_accessToken);

            // Assert
            VerifySetups();
            result.Should().BeAssignableTo<UserSendersResult.None>();
        }

        [DataTestMethod]
        [DataRow(true, DisplayName = "SenderIdEnabled")]
        [DataRow(false, DisplayName = "SenderIdDisabled")]
        public async Task GetSendersV2_InternalServerError(bool isSenderIdEnabled)
        {
            // Arrange
            _mockMessageRepository
                .Setup(x => x.FindAllForUser(NhsLoginId))
                .Throws<ArgumentException>();

            _mockMessagesConfiguration
                .Setup(x => x.SenderIdEnabled)
                .Returns(isSenderIdEnabled);

            // Act
            var result = await _systemUnderTest.GetSendersV2(_accessToken);

            // Assert
            VerifySetups();

            result.Should().BeAssignableTo<UserSendersResult.InternalServerError>();
        }

        [DataTestMethod]
        [DataRow(true, DisplayName = "SenderIdEnabled")]
        [DataRow(false, DisplayName = "SenderIdDisabled")]
        public async Task GetSendersV2_BadGatewayError(bool isSenderIdEnabled)
        {
            // Arrange
            _mockMessageRepository
                .Setup(x => x.FindAllForUser(NhsLoginId))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.RepositoryError());

            _mockMessagesConfiguration
                .Setup(x => x.SenderIdEnabled)
                .Returns(isSenderIdEnabled);

            // Act
            var result = await _systemUnderTest.GetSendersV2(_accessToken);

            // Assert
            VerifySetups();

            result.Should().BeAssignableTo<UserSendersResult.BadGateway>();
        }

        [TestMethod]
        public async Task GetSenders_Found()
        {
            // Arrange
            _mockMessageRepository
                .Setup(x => x.FindAllForUserV1(NhsLoginId))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.Found(new[]
                {
                    new UserMessage { Sender = "First", SentTime = DateTime.UtcNow.AddSeconds(-20), ReadTime = DateTime.UtcNow },
                    new UserMessage { Sender = "Second", SentTime = DateTime.UtcNow.AddSeconds(-10), ReadTime = DateTime.UtcNow },
                    new UserMessage { Sender = "Third", SentTime = DateTime.UtcNow },
                    new UserMessage { Sender = "First", SentTime = DateTime.UtcNow.AddSeconds(10) },
                    new UserMessage { Sender = "Third", SentTime = DateTime.UtcNow.AddSeconds(20) },
                }));

            // Act
            var result = await _systemUnderTest.GetSenders(_accessToken);

            // Assert
            VerifySetups();

            var response = result.Should().BeAssignableTo<UserSendersResult.Found>().Subject.Response;
            response.Should().NotBeNull();
            response.Senders.Should().HaveCount(3);
            response.Senders[0].Should().BeEquivalentTo(new UserSender {  Name = "Third", UnreadCount = 2 });
            response.Senders[1].Should().BeEquivalentTo(new UserSender {  Name = "First", UnreadCount = 1 });
            response.Senders[2].Should().BeEquivalentTo(new UserSender {  Name = "Second", UnreadCount = 0 });
        }

        [TestMethod]
        public async Task GetSenders_None()
        {
            // Arrange
            _mockMessageRepository
                .Setup(x => x.FindAllForUserV1(NhsLoginId))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.NotFound());

            // Act
            var result = await _systemUnderTest.GetSenders(_accessToken);

            // Assert
            VerifySetups();
            result.Should().BeAssignableTo<UserSendersResult.None>();
        }

        [TestMethod]
        public async Task GetSenders_InternalServerError()
        {
            // Arrange
            _mockMessageRepository
                .Setup(x => x.FindAllForUserV1(NhsLoginId))
                .Throws<ArgumentException>();

            // Act
            var result = await _systemUnderTest.GetSenders(_accessToken);

            // Assert
            VerifySetups();

            result.Should().BeAssignableTo<UserSendersResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetSenders_BadGatewayError()
        {
            // Arrange
            _mockMessageRepository
                .Setup(x => x.FindAllForUserV1(NhsLoginId))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.RepositoryError());

            // Act
            var result = await _systemUnderTest.GetSenders(_accessToken);

            // Assert
            VerifySetups();

            result.Should().BeAssignableTo<UserSendersResult.BadGateway>();
        }

        private void VerifySetups()
        {
            _mockMessageRepository.VerifyAll();
            _mockSummaryMessagesToResponseMapper.VerifyAll();
            _mockUserMessagesToResponseMapper.VerifyAll();
            _mockUserMessageToResponseMapper.VerifyAll();
            _mockValidationService.VerifyAll();
        }
    }
}