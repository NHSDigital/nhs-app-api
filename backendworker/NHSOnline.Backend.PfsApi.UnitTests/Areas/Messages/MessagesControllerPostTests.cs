using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using Moq;
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
    public sealed class MessagesControllerPostTests : IDisposable
    {
        private const string MessageId = "ae0b4ffd40c44828b884961b";
        private const string NhsNumber = "NHS Number";
        private const string NhsLoginId = "NHS Login ID";

        private MessagesController _systemUnderTest;

        private Mock<IMessageService> _mockMessageService;
        private Mock<IMessagesValidationService> _mockMessagesValidationService;
        private Mock<IEventHubLogger> _mockEventHubLogger;
        private Mock<IMetricLogger> _mockMetricLogger;

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
            _mockMetricLogger = new Mock<IMetricLogger>(MockBehavior.Strict);
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
        public async Task Post_Success()
        {
            // Arrange
            var messageResult = new AddMessageResult.Success(
                new UserMessage
                {
                    Id = new ObjectId(MessageId),
                    Timestamp = new DateTime(2021, 04, 22, 01, 05, 25),
                    SenderContext = new SenderContext()
                }
            );
            var expectedResponse = new AddMessageResponse
            {
                MessageId = MessageId
            };

            _mockMessageService.Setup(x => x.Send(_validAddMessageRequest, NhsLoginId))
                .ReturnsAsync(messageResult);

            MessageCreatedEventLogData messageCreatedEventLogData = null;
            _mockEventHubLogger.Setup(x => x.MessageCreated(It.IsAny<MessageCreatedEventLogData>()))
                .Callback<MessageCreatedEventLogData>(x => messageCreatedEventLogData = x)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _systemUnderTest.Post(_validAddMessageRequest, NhsLoginId);

            // Assert
            _mockMessageService.VerifyAll();
            _mockEventHubLogger.VerifyAll();

            var statusCodeResult = result.Should().BeAssignableTo<CreatedResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status201Created);
            statusCodeResult.Subject.Value.Should().BeEquivalentTo(expectedResponse);

            messageCreatedEventLogData.Should().NotBeNull();
        }

        [TestMethod]
        public async Task Post_WithNoSenderContext_DoesNotSendEventLog()
        {
            // Arrange
            var messageResult = new AddMessageResult.Success(
                new UserMessage
                {
                    Id = new ObjectId(MessageId),
                    Timestamp = new DateTime(2021, 04, 22, 01, 05, 25),
                    SenderContext = null
                }
            );

            _mockMessageService.Setup(x => x.Send(_validAddMessageRequest, NhsLoginId))
                .ReturnsAsync(messageResult);

            // Act
            var result = await _systemUnderTest.Post(_validAddMessageRequest, NhsLoginId);

            // Assert
            _mockMessageService.VerifyAll();
            _mockEventHubLogger.VerifyNoOtherCalls();

            result.Should().BeAssignableTo<CreatedResult>();
        }

        [TestMethod]
        public async Task Post_WhenSendMessageReturnsBadGateway_ReturnsServiceUnavailable()
        {
            // Arrange
            _mockMessageService.Setup(x => x.Send(_validAddMessageRequest, NhsLoginId))
                .ReturnsAsync(new AddMessageResult.BadGateway());

            // Act
            var result = await _systemUnderTest.Post(_validAddMessageRequest, NhsLoginId);

            // Assert
            _mockMessageService.VerifyAll();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task Post_WhenSendMessageReturnsInternalServerError_ReturnsInternalServerError()
        {
            // Arrange
            _mockMessageService.Setup(x => x.Send(_validAddMessageRequest, NhsLoginId))
                .ReturnsAsync(new AddMessageResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.Post(_validAddMessageRequest, NhsLoginId);

            // Assert
            _mockMessageService.VerifyAll();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Post_SendMessageException_ReturnsInternalServerError()
        {
            // Arrange
            _mockMessageService.Setup(x => x.Send(_validAddMessageRequest, NhsLoginId))
                .Throws(new ArgumentException("test"));

            // Act
            var result = await _systemUnderTest.Post(_validAddMessageRequest, NhsLoginId);

            // Assert
            _mockMessageService.VerifyAll();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Post_MessageIsNull_ReturnsBadRequest()
        {
            // Arrange
            _mockMessageService.Setup(x => x.Send(It.IsAny<AddMessageRequest>(), NhsLoginId))
                .ReturnsAsync(new AddMessageResult.BadRequest());

            // Act
            var result = await _systemUnderTest.Post(null, NhsLoginId);

            // Assert
            _mockMessageService.VerifyAll();

            result.Should().BeAssignableTo<BadRequestResult>();
        }

        [TestMethod]
        public async Task Post_MessageRequestIsInvalid_ReturnsBadRequest()
        {
            // Arrange
            _mockMessageService.Setup(x => x.Send(It.IsAny<AddMessageRequest>(), NhsLoginId))
                .ReturnsAsync(new AddMessageResult.BadRequest());

            // Act
            var result = await _systemUnderTest.Post(_validAddMessageRequest, NhsLoginId);

            // Assert
            _mockMessageService.VerifyAll();

            result.Should().BeAssignableTo<BadRequestResult>();
        }

        [TestCleanup]
        public void Dispose() => _systemUnderTest?.Dispose();
    }
}
