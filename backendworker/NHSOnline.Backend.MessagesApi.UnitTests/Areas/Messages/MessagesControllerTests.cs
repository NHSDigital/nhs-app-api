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
using NHSOnline.Backend.Auth.AspNet;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.MessagesApi.Areas.Messages;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.Metrics;
using UnitTestHelper;

namespace NHSOnline.Backend.MessagesApi.UnitTests.Areas.Messages
{
    [TestClass]
    public sealed class MessagesControllerTests: IDisposable
    {
        private MessagesController _systemUnderTest;
        private Mock<IMessageService> _mockMessageService;
        private Mock<IMessagesValidationService> _mockMessagesValidationService;

        private AddMessageRequest _validAddMessageRequest;
        private Mock<IMetricLogger> _mockMetricLogger;
        private const string NhsNumber = "NhsNumber";
        private const string OdsCode = "OdsCode";
        private const string MessageId = "MessageId";
        private const string NhsLoginId = "Nhs login id";

        [TestInitialize]
        public void TestInitialize()
        {
            var mockAccessTokenProvider = new Mock<IAccessTokenProvider>();
            mockAccessTokenProvider.SetupGet(x => x.AccessToken)
                .Returns(AccessTokenMock.Generate(nhsNumber: NhsNumber));

            _mockMetricLogger = new Mock<IMetricLogger>();

            _mockMessageService = new Mock<IMessageService>();
            _validAddMessageRequest = new AddMessageRequest();

            _mockMessagesValidationService = new Mock<IMessagesValidationService>();
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
                mockAccessTokenProvider.Object);
        }

        [TestMethod]
        public async Task Post_Success()
        {
            // Arrange
            var messageResult = new MessageResult.Success(MessageId);
            var expectedResponse = new AddMessageResponse
            {
                MessageId = MessageId
            };

            _mockMessageService.Setup(x => x.Send(_validAddMessageRequest, NhsLoginId))
                .ReturnsAsync(messageResult);

            // Act
            var result = await _systemUnderTest.Post(_validAddMessageRequest, NhsLoginId);

            // Assert
            _mockMessageService.VerifyAll();
            var statusCodeResult = result.Should().BeAssignableTo<CreatedResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status201Created);
            statusCodeResult.Subject.Value.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public async Task Post_WhenSendMessageReturnsBadGateway_ReturnsServiceUnavailable()
        {
            // Arrange
            _mockMessageService.Setup(x => x.Send(_validAddMessageRequest, NhsLoginId))
                .ReturnsAsync(new MessageResult.BadGateway());

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
                .ReturnsAsync(new MessageResult.InternalServerError());

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
            _mockMessageService.Setup(x => x.Send(It.IsAny<AddMessageRequest>(), NhsLoginId ))
                .ReturnsAsync(new MessageResult.BadRequest());

            // Act
            var result = await _systemUnderTest.Post(null, NhsLoginId);

            // Assert
            result.Should().BeAssignableTo<BadRequestResult>();
        }

        [TestMethod]
        public async Task Post_MessageRequestIsInvalid_ReturnsBadRequest()
        {
            // Arrange
            _mockMessageService.Setup(x => x.Send(It.IsAny<AddMessageRequest>(), NhsLoginId ))
                .ReturnsAsync(new MessageResult.BadRequest());

            // Act
            var result = await _systemUnderTest.Post(_validAddMessageRequest, NhsLoginId);

            // Assert
            result.Should().BeAssignableTo<BadRequestResult>();
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
        public async Task Patch_Success()
        {
            // Arrange
            var userProfile = MockUserProfileSetup(OdsCode, NhsNumber);
            _mockMessageService.Setup(x =>
                    x.UpdateMessage(It.IsAny<JsonPatchDocument<Message>>(), It.IsAny<AccessToken>(), It.IsAny<string>()))
                .ReturnsAsync(new MessagePatchResult.Updated());

            // Act
            var result = await _systemUnderTest.Patch(new JsonPatchDocument<Message>(), "message id", userProfile);

            // Assert
            _mockMessageService.VerifyAll();
            _mockMetricLogger.Verify(x => x.MessageRead(), Times.Once);

            result.Should().BeAssignableTo<NoContentResult>();
        }

        [TestMethod]
        public async Task Patch_MessageServiceReturnsNotFound_ReturnsNotFound()
        {
            // Arrange
            var userProfile = MockUserProfileSetup(OdsCode, NhsNumber);
            _mockMessageService.Setup(x =>
                    x.UpdateMessage(It.IsAny<JsonPatchDocument<Message>>(), It.IsAny<AccessToken>(), It.IsAny<string>()))
                .ReturnsAsync(new MessagePatchResult.NotFound());

            // Act
            var result = await _systemUnderTest.Patch(new JsonPatchDocument<Message>(), "message id", userProfile);

            // Assert
            _mockMessageService.VerifyAll();
            _mockMetricLogger.VerifyNoOtherCalls();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [TestMethod]
        public async Task Patch_MessageServiceReturnsBadGatewayResult_ReturnsBadGateway()
        {
            // Arrange
            var userProfile = MockUserProfileSetup(OdsCode, NhsNumber);
            _mockMessageService.Setup(x =>
                    x.UpdateMessage(It.IsAny<JsonPatchDocument<Message>>(), It.IsAny<AccessToken>(), It.IsAny<string>()))
                .ReturnsAsync(new MessagePatchResult.BadGateway());

            // Act
            var result = await _systemUnderTest.Patch(new JsonPatchDocument<Message>(), "message id", userProfile);

            // Assert
            _mockMessageService.VerifyAll();
            _mockMetricLogger.VerifyNoOtherCalls();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task Patch_MessageServiceReturnsInternalServerErrorResult_ReturnsInternalServerError()
        {
            // Arrange
            var userProfile = MockUserProfileSetup(OdsCode, NhsNumber);
            _mockMessageService.Setup(x =>
                    x.UpdateMessage(It.IsAny<JsonPatchDocument<Message>>(), It.IsAny<AccessToken>(), It.IsAny<string>()))
                .ReturnsAsync(new MessagePatchResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.Patch(new JsonPatchDocument<Message>(), "message id", userProfile);

            // Assert
            _mockMessageService.VerifyAll();
            _mockMetricLogger.VerifyNoOtherCalls();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Patch_MessageServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var userProfile = MockUserProfileSetup(OdsCode, NhsNumber);
            _mockMessageService.Setup(x =>
                    x.UpdateMessage(It.IsAny<JsonPatchDocument<Message>>(), It.IsAny<AccessToken>(), It.IsAny<string>()))
                .Throws<ArgumentException>();

            // Act
            var result = await _systemUnderTest.Patch(new JsonPatchDocument<Message>(), "message id", userProfile);

            // Assert
            _mockMessageService.VerifyAll();
            _mockMetricLogger.VerifyNoOtherCalls();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Patch_PatchRequestIsInvalid_ReturnsBadRequest()
        {
            // Arrange
            var userProfile = MockUserProfileSetup(OdsCode, NhsNumber);

            // Act
            _mockMessageService.Setup(x => x.UpdateMessage(
                    It.IsAny<JsonPatchDocument<Message>>(), It.IsAny<AccessToken>(), It.IsAny<string>()))
                .ReturnsAsync(new MessagePatchResult.BadRequest());

            var result = await _systemUnderTest.Patch(null, "message id", userProfile);

            // Assert
            _mockMetricLogger.VerifyNoOtherCalls();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [TestCleanup]
        public void Dispose() => _systemUnderTest?.Dispose();


        private UserProfile MockUserProfileSetup(string odsCode, string nhsNumber)
        {
            var userInfo = new UserInfo
            {
                GpIntegrationCredentials = { OdsCode = odsCode },
                NhsNumber = nhsNumber,
            };
            return new UserProfile(userInfo, "Access token", "Refresh Token");
        }
    }
}
