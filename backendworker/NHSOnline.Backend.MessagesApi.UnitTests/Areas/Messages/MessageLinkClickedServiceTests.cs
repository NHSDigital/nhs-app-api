using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.MessagesApi.Areas.Messages;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.MessagesApi.Repository;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.MessagesApi.UnitTests.Areas.Messages
{
    [TestClass]
    public class MessageLinkClickedServiceTests 
    {
        private MessageLinkClickedService _systemUnderTest;

        private Mock<IMapper<MessageLink, RepositoryFindResult<UserMessage>.Found, MessageLinkClickedData>> _mockMapper;
        private Mock<IMessageLinkClickedValidationService> _mockValidationService;
        private Mock<IMessageRepository> _mockMessageRepository;
        private Mock<IMetricLogger> _mockMetricLogger;

        private const string NhsLoginId = "NhsLoginId";
        private const string MessageId = "MessageId";
        private readonly Uri _link = new Uri("https://www.testing.com/valid/url/");

        private MessageLink _messageLink;
        private MessageLinkClickedData _data;

        [TestInitialize]
        public void TestInitialize()
        {
            _messageLink = new MessageLink
            {
                MessageId = MessageId,
                Link = _link
            };

            _data = new MessageLinkClickedData(MessageId, _link, null, null, null);

            _mockMapper = new Mock<IMapper<MessageLink, RepositoryFindResult<UserMessage>.Found, MessageLinkClickedData>>(MockBehavior.Strict);
            _mockValidationService = new Mock<IMessageLinkClickedValidationService>(MockBehavior.Strict);
            _mockMessageRepository = new Mock<IMessageRepository>(MockBehavior.Strict);
            _mockMetricLogger = new Mock<IMetricLogger>(MockBehavior.Strict);

            _systemUnderTest = new MessageLinkClickedService(
                new Mock<ILogger<MessageLinkClickedService>>().Object,
                _mockMapper.Object,
                _mockValidationService.Object,
                _mockMessageRepository.Object,
                _mockMetricLogger.Object
            );
        }

        [TestMethod]
        public async Task LogLinkClicked_ValidationFails_ReturnsBadRequest()
        {
            // Arrange
            _mockValidationService
                .Setup(x => x.IsServiceRequestValid(NhsLoginId, _messageLink))
                .Returns(false);

            // Act
            var result = await _systemUnderTest.LogLinkClicked(NhsLoginId, _messageLink);

            // Assert
            VerifySetups();

            result.Should().BeOfType(typeof(MessageLinkClickedResult.BadRequest));
        }

        [TestMethod]
        public async Task LogLinkClicked_ValidationThrowsError_ReturnsInternalServerError()
        {
            // Arrange
            _mockValidationService
                .Setup(x => x.IsServiceRequestValid(NhsLoginId, _messageLink))
                .Throws<Exception>();

            // Act
            var result = await _systemUnderTest.LogLinkClicked(NhsLoginId, _messageLink);

            // Assert
            VerifySetups();

            result.Should().BeOfType(typeof(MessageLinkClickedResult.InternalServerError));
        }

        [TestMethod]
        public async Task LogLinkClicked_RepositoryReturnsNotFound_ReturnsBadRequest()
        {
            // Arrange
            _mockValidationService
                .Setup(x => x.IsServiceRequestValid(NhsLoginId, _messageLink))
                .Returns(true);

            _mockMessageRepository
                .Setup(x => x.FindMessage(NhsLoginId, MessageId))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.NotFound());

            // Act
            var result = await _systemUnderTest.LogLinkClicked(NhsLoginId, _messageLink);

            // Assert
            VerifySetups();

            result.Should().BeOfType(typeof(MessageLinkClickedResult.BadRequest));
        }

        [TestMethod]
        public async Task LogLinkClicked_RepositoryReturnsError_ReturnsBadGateway()
        {
            // Arrange
            _mockValidationService
                .Setup(x => x.IsServiceRequestValid(NhsLoginId, _messageLink))
                .Returns(true);

            _mockMessageRepository
                .Setup(x => x.FindMessage(NhsLoginId, MessageId))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.RepositoryError());

            // Act
            var result = await _systemUnderTest.LogLinkClicked(NhsLoginId, _messageLink);

            // Assert
            VerifySetups();

            result.Should().BeOfType(typeof(MessageLinkClickedResult.BadGateway));
        }

        [TestMethod]
        public async Task LogLinkClicked_RepositoryThrowsError_ReturnsInternalServerError()
        {
            // Arrange
            _mockValidationService
                .Setup(x => x.IsServiceRequestValid(NhsLoginId, _messageLink))
                .Returns(true);

            _mockMessageRepository
                .Setup(x => x.FindMessage(NhsLoginId, MessageId))
                .Throws<Exception>();

            // Act
            var result = await _systemUnderTest.LogLinkClicked(NhsLoginId, _messageLink);

            // Assert
            VerifySetups();

            result.Should().BeOfType(typeof(MessageLinkClickedResult.InternalServerError));
        }

        [TestMethod]
        public async Task LogLinkClicked_MapperThrowsError_ReturnsInternalServerError()
        {
            // Arrange
            _mockValidationService
                .Setup(x => x.IsServiceRequestValid(NhsLoginId, _messageLink))
                .Returns(true);

            var found = new RepositoryFindResult<UserMessage>.Found(null);

            _mockMessageRepository
                .Setup(x => x.FindMessage(NhsLoginId, MessageId))
                .ReturnsAsync(found);

            _mockMapper
                .Setup(x => x.Map(_messageLink, found))
                .Throws<Exception>();

            // Act
            var result = await _systemUnderTest.LogLinkClicked(NhsLoginId, _messageLink);

            // Assert
            VerifySetups();

            result.Should().BeOfType(typeof(MessageLinkClickedResult.InternalServerError));
        }

        [TestMethod]
        public async Task LogLinkClicked_MetricLoggerThrowsError_ReturnsInternalServerError()
        {
            // Arrange
            _mockValidationService
                .Setup(x => x.IsServiceRequestValid(NhsLoginId, _messageLink))
                .Returns(true);

            var found = new RepositoryFindResult<UserMessage>.Found(null);

            _mockMessageRepository
                .Setup(x => x.FindMessage(NhsLoginId, MessageId))
                .ReturnsAsync(found);

            _mockMapper
                .Setup(x => x.Map(_messageLink, found))
                .Returns(_data);

            _mockMetricLogger
                .Setup(x => x.MessageLinkClicked(_data))
                .Throws<Exception>();

            // Act
            var result = await _systemUnderTest.LogLinkClicked(NhsLoginId, _messageLink);

            // Assert
            VerifySetups();

            result.Should().BeOfType(typeof(MessageLinkClickedResult.InternalServerError));
        }

        [TestMethod]
        public async Task LogLinkClicked_MetricLoggerSucceeds_ReturnsSuccess()
        {
            // Arrange
            _mockValidationService
                .Setup(x => x.IsServiceRequestValid(NhsLoginId, _messageLink))
                .Returns(true);

            var found = new RepositoryFindResult<UserMessage>.Found(null);

            _mockMessageRepository
                .Setup(x => x.FindMessage(NhsLoginId, MessageId))
                .ReturnsAsync(found);

            _mockMapper
                .Setup(x => x.Map(_messageLink, found))
                .Returns(_data);

            _mockMetricLogger
                .Setup(x => x.MessageLinkClicked(_data))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _systemUnderTest.LogLinkClicked(NhsLoginId, _messageLink);

            // Assert
            VerifySetups();

            result.Should().BeOfType(typeof(MessageLinkClickedResult.Success));
        }

        private void VerifySetups()
        {
            _mockValidationService.VerifyAll();
            _mockMessageRepository.VerifyAll();
            _mockMapper.VerifyAll();
            _mockMetricLogger.VerifyAll();
        }
    }
}
