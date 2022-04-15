using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auth;
using NHSOnline.Backend.Auth.AspNet;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Messages.Areas.Messages;
using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.Metrics.EventHub;
using NHSOnline.Backend.PfsApi.Areas.Messages;
using NHSOnline.Backend.Support;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Messages
{
    [TestClass]
    public sealed class MessagesControllerTests : IDisposable
    {
        private const string MessageId = "ae0b4ffd40c44828b884961b";
        private const string NhsNumber = "NHS Number";
        private const string NhsLoginId = "NHS Login ID";

        private MessagesController _systemUnderTest;

        private Mock<IMessageService> _mockMessageService;
        private Mock<IMessagesValidationService> _mockMessagesValidationService;
        private Mock<IEventHubLogger> _mockEventHubLogger;
        private Mock<IMetricLogger<UserSessionMetricContext>> _mockMetricLogger;

        private AccessToken _accessToken;
        private AddMessageRequest _validAddMessageRequest;

        [TestInitialize]
        public void TestInitialize()
        {
            _accessToken = AccessTokenMock.Generate(nhsNumber: NhsNumber);

            var mockAccessTokenProvider = new Mock<IAccessTokenProvider>(MockBehavior.Strict);

            mockAccessTokenProvider
                .SetupGet(x => x.AccessToken)
                .Returns(_accessToken);

            _mockEventHubLogger = new Mock<IEventHubLogger>(MockBehavior.Strict);
            _mockMetricLogger = new Mock<IMetricLogger<UserSessionMetricContext>>(MockBehavior.Strict);
            _mockMessageService = new Mock<IMessageService>(MockBehavior.Strict);
            _mockMessagesValidationService = new Mock<IMessagesValidationService>(MockBehavior.Strict);

            _validAddMessageRequest = new AddMessageRequest();

            _mockMessagesValidationService
                .Setup(x => x.IsMessageRequestValid(_validAddMessageRequest, NhsLoginId))
                .Returns(true);

            _mockMessagesValidationService
                .Setup(x => x.IsPatchRequestValid(It.IsAny<JsonPatchDocument<Message>>(), It.IsAny<string>()))
                .Returns(true);

            _systemUnderTest = new MessagesController(
                _mockMessageService.Object,
                new Mock<ILogger<MessagesController>>().Object,
                _mockMetricLogger.Object,
                _mockEventHubLogger.Object,
                new Mock<IMapper<SenderContext, SenderContextEventLogData>>().Object,
                mockAccessTokenProvider.Object
            );
        }

        [TestMethod]
        public async Task GetMessage_SuccessFound_ReturnsOK()
        {
            // Arrange
            var message = new Message
            {
                Sender = "Sender",
                Body = "Body",
                Read = true
            };

            _mockMessageService
                .Setup(x => x.GetMessage(_accessToken, MessageId))
                .ReturnsAsync(new MessagesResult.Found(new MessagesResponse
                {
                    SenderMessages = new List<SenderMessages>
                    {
                        new SenderMessages { Messages = new List<Message> { message } }
                    }
                }));

            // Act
            var result = await _systemUnderTest.GetMessage(MessageId);

            // Assert
            _mockMessageService.VerifyAll();

            var okObjectResult = result.Should().BeAssignableTo<OkObjectResult>();
            var okObjectResultMessage = okObjectResult.Subject.Value.Should().BeAssignableTo<Message>();
            okObjectResultMessage.Subject.Should().BeEquivalentTo(message);
        }

        [TestMethod]
        public async Task GetMessage_SuccessNone_ReturnsNotFound()
        {
            // Arrange
            _mockMessageService
                .Setup(x => x.GetMessage(It.IsAny<AccessToken>(), MessageId))
                .ReturnsAsync(new MessagesResult.None());

            // Act
            var result = await _systemUnderTest.GetMessage(MessageId);

            // Assert
            _mockMessageService.VerifyAll();

            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [TestMethod]
        public async Task GetMessage_MessageServiceReturnsBadGatewayResult_ReturnsBadGateway()
        {
            // Arrange
            _mockMessageService
                .Setup(x => x.GetMessage(It.IsAny<AccessToken>(), MessageId))
                .ReturnsAsync(new MessagesResult.BadGateway());

            // Act
            var result = await _systemUnderTest.GetMessage(MessageId);

            // Assert
            _mockMessageService.VerifyAll();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task GetMessage_MessageServiceReturnsInternalServerErrorResult_ReturnsInternalServerError()
        {
            // Arrange
            _mockMessageService
                .Setup(x => x.GetMessage(It.IsAny<AccessToken>(), MessageId))
                .ReturnsAsync(new MessagesResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.GetMessage(MessageId);

            // Assert
            _mockMessageService.VerifyAll();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task GetMessage_MessageServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockMessageService
                .Setup(x => x.GetMessage(It.IsAny<AccessToken>(), MessageId))
                .Throws<ArgumentException>();

            // Act
            var result = await _systemUnderTest.GetMessage(MessageId);

            // Assert
            _mockMessageService.VerifyAll();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task GetMessagesMetadata_SuccessFound_ReturnsOK()
        {
            // Arrange
            var response = new MessagesMetadataResponse()
            {
                MessagesMetadata = new MessagesMetadata { UnreadMessageCount = 2 }
            };

            _mockMessageService
                .Setup(x => x.GetMessagesMetadata(_accessToken))
                .ReturnsAsync(new MessagesMetadataResult.Found(response));

            // Act
            var result = await _systemUnderTest.GetMessagesMetadata();

            // Assert
            _mockMessageService.VerifyAll();

            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeAssignableTo<MessagesMetadata>()
                .Subject.Should().BeEquivalentTo(response.MessagesMetadata);
        }

        [TestMethod]
        public async Task GetMessagesMetadata_MessageServiceReturnsBadGatewayResult_ReturnsBadGateway()
        {
            // Arrange
            _mockMessageService
                .Setup(x => x.GetMessagesMetadata(_accessToken))
                .ReturnsAsync(new MessagesMetadataResult.BadGateway());

            // Act
            var result = await _systemUnderTest.GetMessagesMetadata();

            // Assert
            _mockMessageService.VerifyAll();

            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task GetMessagesMetadata_MessageServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockMessageService
                .Setup(x => x.GetMessagesMetadata(_accessToken))
                .Throws<ArgumentException>();

            // Act
            var result = await _systemUnderTest.GetMessagesMetadata();

            // Assert
            _mockMessageService.VerifyAll();

            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestCleanup]
        public void Dispose() => _systemUnderTest?.Dispose();
    }
}
