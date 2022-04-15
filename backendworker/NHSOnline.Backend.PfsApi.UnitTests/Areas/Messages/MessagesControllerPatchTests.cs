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
    public sealed class MessagesControllerPatchTests : IDisposable
    {
        private const string MessageId = "ae0b4ffd40c44828b884961b";
        private const string NhsNumber = "NHS Number";

        private MessagesController _systemUnderTest;
        private AccessToken _accessToken;

        private Mock<IMessageService> _mockMessageService;
        private Mock<IEventHubLogger> _mockEventHubLogger;
        private Mock<IMetricLogger<UserSessionMetricContext>> _mockMetricLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            _accessToken = AccessTokenMock.Generate(nhsNumber: NhsNumber);

            var mockAccessTokenProvider = new Mock<IAccessTokenProvider>(MockBehavior.Strict);

            mockAccessTokenProvider
                .SetupGet(x => x.AccessToken)
                .Returns(_accessToken);

            _mockEventHubLogger = new Mock<IEventHubLogger>();
            _mockMetricLogger = new Mock<IMetricLogger<UserSessionMetricContext>>();
            _mockMessageService = new Mock<IMessageService>(MockBehavior.Strict);

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
        public async Task Patch_MessageRead_MessageServiceReturnsUpdated_ReturnsNoContent()
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
                    ReadTime = DateTime.UtcNow,
                    SenderContext = new SenderContext
                    {
                        CampaignId = campaignId,
                        CommunicationId = communicationId,
                        SupplierId = supplierId,
                        TransmissionId = transmissionId
                    }
                }, MessagePatchType.Read));

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
        public async Task Patch_MessageRead_WithNoSenderContext_DoesNotSendEventLog()
        {
            // Arrange
            _mockMessageService.Setup(x =>
                    x.UpdateMessage(It.IsAny<JsonPatchDocument<Message>>(), It.IsAny<AccessToken>(),
                        It.IsAny<string>()))
                .ReturnsAsync(new MessagePatchResult.Updated(new UserMessage
                {
                    Id = new ObjectId(MessageId),
                    ReadTime = new DateTime(2021, 04, 22, 01, 05, 25),
                    SenderContext = null
                }, MessagePatchType.Read));

            _mockMetricLogger.Setup(x => x.MessageRead(It.Is<MessageReadData>(mrd =>
                    mrd.CommunicationId == null &&
                    mrd.TransmissionId == null &&
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
        public async Task Patch_MessageReply_MessageServiceReturnsUpdated_ReturnsNoContent()
        {
            // Arrange
            const string response = "response";

            _mockMessageService.Setup(x =>
                    x.UpdateMessage(It.IsAny<JsonPatchDocument<Message>>(), It.IsAny<AccessToken>(),
                        It.IsAny<string>()))
                .ReturnsAsync(new MessagePatchResult.Updated(new UserMessage
                {
                    Id = new ObjectId(MessageId),
                    Reply = new UserMessageReply()
                    {
                        Response = response,
                        ResponseDateTime = DateTime.Now
                    },
                    SenderContext = new SenderContext()
                }, MessagePatchType.Reply));

            MessageReplyEventLogData messageReplyEventLogData = null;
            _mockEventHubLogger.Setup(x => x.MessageReply(It.IsAny<MessageReplyEventLogData>()))
                .Callback<MessageReplyEventLogData>(x => messageReplyEventLogData = x)
                .Returns(Task.CompletedTask);
            

            // Act
            var result = await _systemUnderTest.Patch(new JsonPatchDocument<Message>(), MessageId);

            // Assert
            _mockMessageService.VerifyAll();
            _mockEventHubLogger.VerifyAll();

            result.Should().BeAssignableTo<NoContentResult>();

            messageReplyEventLogData.Should().NotBeNull();
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