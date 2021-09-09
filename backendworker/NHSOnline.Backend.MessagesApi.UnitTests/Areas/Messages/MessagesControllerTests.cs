using System;
using System.Collections.Generic;
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
using NHSOnline.Backend.MessagesApi.Areas.Messages;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.Metrics.EventHub;
using NHSOnline.Backend.Support;
using UnitTestHelper;

namespace NHSOnline.Backend.MessagesApi.UnitTests.Areas.Messages
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
                new Mock<IMapper<SenderContext, MessageSenderContextEventLogData>>().Object,
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

        [TestMethod]
        public async Task GetMessage_SuccessSome_ReturnsOK()
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
                .ReturnsAsync(new MessagesResult.Some(new MessagesResponse
                {
                    new SenderMessages { Messages = new List<Message> { message } }
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
        [DataRow(null, false)]
        [DataRow("sender", true)]
        public async Task Get_ArgumentsAreNotValid_ReturnsBadRequest(string sender, bool summary)
        {
            // Act
            var result = await _systemUnderTest.Get(sender, summary);

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [TestMethod]
        public async Task Get_WithSender_SuccessSome()
        {
            // Arrange
            var response = new MessagesResponse();

            _mockMessageService.Setup(x => x.GetMessages(It.IsAny<AccessToken>(), It.IsAny<string>()))
                .ReturnsAsync(new MessagesResult.Some(response));

            // Act
            var result = await _systemUnderTest.Get("sender");

            // Assert
            _mockMessageService.VerifyAll();

            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeAssignableTo<IEnumerable<SenderMessages>>()
                .Subject.Should().BeEquivalentTo(response);
        }

        [TestMethod]
        public async Task Get_WithSender_SuccessNone()
        {
            // Arrange
            _mockMessageService.Setup(x => x.GetMessages(It.IsAny<AccessToken>(), It.IsAny<string>()))
                .ReturnsAsync(new MessagesResult.None());

            // Act
            var result = await _systemUnderTest.Get("sender");

            // Assert
            _mockMessageService.VerifyAll();

            result.Should().BeAssignableTo<NoContentResult>();
        }

        [TestMethod]
        public async Task Get_WithSender_MessageServiceReturnsBadGatewayResult_ReturnsBadGateway()
        {
            // Arrange
            _mockMessageService.Setup(x => x.GetMessages(It.IsAny<AccessToken>(), It.IsAny<string>()))
                .ReturnsAsync(new MessagesResult.BadGateway());

            // Act
            var result = await _systemUnderTest.Get("sender");

            // Assert
            _mockMessageService.VerifyAll();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task Get_WithSender_MessageServiceReturnsInternalServerErrorResult_ReturnsInternalServerError()
        {
            // Arrange
            _mockMessageService.Setup(x => x.GetMessages(It.IsAny<AccessToken>(), It.IsAny<string>()))
                .ReturnsAsync(new MessagesResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.Get("sender");

            // Assert
            _mockMessageService.VerifyAll();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Get_WithSender_MessageServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockMessageService.Setup(x => x.GetMessages(It.IsAny<AccessToken>(), It.IsAny<string>()))
                .Throws<ArgumentException>();

            // Act
            var result = await _systemUnderTest.Get("sender");

            // Assert
            _mockMessageService.VerifyAll();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Get_WithSummary_SuccessSome()
        {
            // Arrange
            var response = new MessagesResponse();
            _mockMessageService.Setup(x => x.GetSummaryMessages(It.IsAny<AccessToken>()))
                .ReturnsAsync(new MessagesResult.Some(response));

            // Act
            var result = await _systemUnderTest.Get(summary: true);

            // Assert
            _mockMessageService.VerifyAll();

            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeAssignableTo<IEnumerable<SenderMessages>>()
                .Subject.Should().BeEquivalentTo(response);
        }

        [TestMethod]
        public async Task Get_WithSummary_SuccessNone()
        {
            // Arrange
            _mockMessageService.Setup(x => x.GetSummaryMessages(It.IsAny<AccessToken>()))
                .ReturnsAsync(new MessagesResult.None());

            // Act
            var result = await _systemUnderTest.Get(summary: true);

            // Assert
            _mockMessageService.VerifyAll();

            result.Should().BeAssignableTo<NoContentResult>();
        }

        [TestMethod]
        public async Task Get_WithSummary_MessageServiceReturnsBadGatewayResult_ReturnsBadGateway()
        {
            // Arrange
            _mockMessageService.Setup(x => x.GetSummaryMessages(It.IsAny<AccessToken>()))
                .ReturnsAsync(new MessagesResult.BadGateway());

            // Act
            var result = await _systemUnderTest.Get(summary: true);

            // Assert
            _mockMessageService.VerifyAll();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task Get_WithSummary_MessageServiceReturnsInternalServerErrorResult_ReturnsInternalServerError()
        {
            // Arrange
            _mockMessageService.Setup(x => x.GetSummaryMessages(It.IsAny<AccessToken>()))
                .ReturnsAsync(new MessagesResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.Get(summary: true);

            // Assert
            _mockMessageService.VerifyAll();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Get_WithSummary_MessageServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockMessageService.Setup(x => x.GetSummaryMessages(It.IsAny<AccessToken>()))
                .Throws<ArgumentException>();

            // Act
            var result = await _systemUnderTest.Get(summary: true);

            // Assert
            _mockMessageService.VerifyAll();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Patch_MessageServiceReturnsUpdated_ReturnsNoContent()
        {
            // Arrange
            const string communicationId = "communicationId_2345";
            const string transmissionId = "transmissionId_3456";
            const string campaignId = "campaignId_1234";
            const string supplierId = "supplierId_5678";

            _mockMessageService.Setup(x =>
                    x.UpdateMessage(It.IsAny<JsonPatchDocument<Message>>(), It.IsAny<AccessToken>(),
                        It.IsAny<string>()))
                .ReturnsAsync(new MessagePatchResult.Updated(new UserMessage
                {
                    Id = new ObjectId(MessageId),
                    CommunicationId = communicationId,
                    TransmissionId = transmissionId,
                    ReadTime = DateTime.UtcNow,
                    SenderContext = new SenderContext
                    {
                        CampaignId = campaignId,
                        SupplierId = supplierId
                    }
                }));

            MessageReadEventLogData messageReadEventLogData = null;
            _mockEventHubLogger.Setup(x => x.MessageRead(It.IsAny<MessageReadEventLogData>()))
                .Callback<MessageReadEventLogData>(x => messageReadEventLogData = x)
                .Returns(Task.CompletedTask);

            _mockMetricLogger.Setup(x => x.MessageRead(It.Is<MessageReadData>(mrd =>
                    mrd.CommunicationId == communicationId &&
                    mrd.TransmissionId == transmissionId &&
                    mrd.MessageId == MessageId &&
                    mrd.CampaignId == campaignId &&
                    mrd.SupplierId == supplierId)))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _systemUnderTest.Patch(new JsonPatchDocument<Message>(), MessageId);

            // Assert
            _mockMessageService.VerifyAll();
            _mockMetricLogger.VerifyAll();
            _mockEventHubLogger.VerifyAll();

            result.Should().BeAssignableTo<NoContentResult>();

            messageReadEventLogData.Should().NotBeNull();
        }

        [TestMethod]
        public async Task Patch_WithNoSenderContext_DoesNotSendEventLog()
        {
            // Arrange
            const string communicationId = "communicationId_2345";
            const string transmissionId = "transmissionId_3456";

            _mockMessageService.Setup(x =>
                    x.UpdateMessage(It.IsAny<JsonPatchDocument<Message>>(), It.IsAny<AccessToken>(),
                        It.IsAny<string>()))
                .ReturnsAsync(new MessagePatchResult.Updated(new UserMessage
                {
                    Id = new ObjectId(MessageId),
                    CommunicationId = communicationId,
                    TransmissionId = transmissionId,
                    ReadTime = new DateTime(2021, 04, 22, 01, 05, 25),
                    SenderContext = null
                }));

            _mockMetricLogger.Setup(x => x.MessageRead(It.Is<MessageReadData>(mrd =>
                    mrd.CommunicationId == communicationId &&
                    mrd.TransmissionId == transmissionId &&
                    mrd.MessageId == MessageId)))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _systemUnderTest.Patch(new JsonPatchDocument<Message>(), MessageId);

            // Assert
            _mockMessageService.VerifyAll();
            _mockMetricLogger.VerifyAll();
            _mockEventHubLogger.VerifyNoOtherCalls();

            result.Should().BeAssignableTo<NoContentResult>();
        }

        [TestMethod]
        public async Task Patch_MessageServiceReturnsNoChange_ReturnsNoContent()
        {
            // Arrange
            const string messageId = "id_1234";

            _mockMessageService.Setup(x =>
                    x.UpdateMessage(It.IsAny<JsonPatchDocument<Message>>(), It.IsAny<AccessToken>(),
                        It.IsAny<string>()))
                .ReturnsAsync(new MessagePatchResult.NoChange());

            // Act
            var result = await _systemUnderTest.Patch(new JsonPatchDocument<Message>(), messageId);

            // Assert
            _mockMessageService.VerifyAll();

            result.Should().BeAssignableTo<NoContentResult>();
        }

        [TestMethod]
        public async Task Patch_MessageServiceReturnsNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockMessageService.Setup(x =>
                    x.UpdateMessage(It.IsAny<JsonPatchDocument<Message>>(), It.IsAny<AccessToken>(),
                        It.IsAny<string>()))
                .ReturnsAsync(new MessagePatchResult.NotFound());

            // Act
            var result = await _systemUnderTest.Patch(new JsonPatchDocument<Message>(), "message id");

            // Assert
            _mockMessageService.VerifyAll();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [TestMethod]
        public async Task Patch_MessageServiceReturnsBadGatewayResult_ReturnsBadGateway()
        {
            // Arrange
            _mockMessageService.Setup(x =>
                    x.UpdateMessage(It.IsAny<JsonPatchDocument<Message>>(), It.IsAny<AccessToken>(),
                        It.IsAny<string>()))
                .ReturnsAsync(new MessagePatchResult.BadGateway());

            // Act
            var result = await _systemUnderTest.Patch(new JsonPatchDocument<Message>(), "message id");

            // Assert
            _mockMessageService.VerifyAll();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task Patch_MessageServiceReturnsInternalServerErrorResult_ReturnsInternalServerError()
        {
            // Arrange
            _mockMessageService.Setup(x =>
                    x.UpdateMessage(It.IsAny<JsonPatchDocument<Message>>(), It.IsAny<AccessToken>(),
                        It.IsAny<string>()))
                .ReturnsAsync(new MessagePatchResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.Patch(new JsonPatchDocument<Message>(), "message id");

            // Assert
            _mockMessageService.VerifyAll();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Patch_MessageServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockMessageService.Setup(x =>
                    x.UpdateMessage(It.IsAny<JsonPatchDocument<Message>>(), It.IsAny<AccessToken>(),
                        It.IsAny<string>()))
                .Throws<ArgumentException>();

            // Act
            var result = await _systemUnderTest.Patch(new JsonPatchDocument<Message>(), "message id");

            // Assert
            _mockMessageService.VerifyAll();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Patch_PatchRequestIsInvalid_ReturnsBadRequest()
        {
            // Act
            _mockMessageService.Setup(x => x.UpdateMessage(
                    It.IsAny<JsonPatchDocument<Message>>(), It.IsAny<AccessToken>(), It.IsAny<string>()))
                .ReturnsAsync(new MessagePatchResult.BadRequest());

            var result = await _systemUnderTest.Patch(null, "message id");

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [TestCleanup]
        public void Dispose() => _systemUnderTest?.Dispose();
    }
}