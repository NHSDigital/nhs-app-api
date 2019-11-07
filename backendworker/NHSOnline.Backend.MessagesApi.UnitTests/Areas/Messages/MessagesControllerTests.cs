using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.MessagesApi.Areas.Messages;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using UnitTestHelper;

namespace NHSOnline.Backend.MessagesApi.UnitTests.Areas.Messages
{
    [TestClass]
    public class MessagesControllerTests
    {
        private IFixture _fixture;
        private MessagesController _systemUnderTest;
        private Mock<IMessageService> _mockMessageService;
        private Mock<IMessagesValidationService> _mockMessagesValidationService;

        private AddMessageRequest _validAddMessageRequest;
        private string _nhsLoginId;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization())
                .Customize(new MessagesApiCustomization());

            var mockHttpContext = HttpContextGetAccessTokenHelper.CreateMockHttpContext(_fixture);

            _nhsLoginId = _fixture.Create<string>();
         
            _mockMessageService = _fixture.Freeze<Mock<IMessageService>>();
            _validAddMessageRequest = _fixture.Create<AddMessageRequest>();

            _mockMessagesValidationService = _fixture.Freeze<Mock<IMessagesValidationService>>();
            _mockMessagesValidationService.Setup(x => x.IsMessageRequestValid(_validAddMessageRequest, _nhsLoginId))
                .Returns(true);
            
            _mockMessagesValidationService.Setup(x => 
                    x.IsPatchRequestValid(It.IsAny<JsonPatchDocument<Message>>(), It.IsAny<string>()))
                .Returns(true);
            
            _systemUnderTest = _fixture.Create<MessagesController>();
            _systemUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };
        }

        [TestMethod]
        public async Task Post_Success()
        {
            // Arrange
            _mockMessageService.Setup(x => x.Send(_validAddMessageRequest, _nhsLoginId))
                .ReturnsAsync(new MessageResult.Success());

            // Act
            var result = await _systemUnderTest.Post(_validAddMessageRequest, _nhsLoginId);

            // Assert
            _mockMessageService.VerifyAll();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status201Created);
        }

        [TestMethod]
        public async Task Post_WhenSendMessageReturnsBadGateway_ReturnsServiceUnavailable()
        {
            // Arrange
            _mockMessageService.Setup(x => x.Send(_validAddMessageRequest, _nhsLoginId))
                .ReturnsAsync(new MessageResult.BadGateway());

            // Act
            var result = await _systemUnderTest.Post(_validAddMessageRequest, _nhsLoginId);
            
            // Assert
            _mockMessageService.VerifyAll();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task Post_WhenSendMessageReturnsInternalServerError_ReturnsInternalServerError()
        {
            // Arrange
            _mockMessageService.Setup(x => x.Send(_validAddMessageRequest, _nhsLoginId))
                .ReturnsAsync(new MessageResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.Post(_validAddMessageRequest, _nhsLoginId);

            // Assert
            _mockMessageService.VerifyAll();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Post_SendMessageException_ReturnsInternalServerError()
        {
            // Arrange
            _mockMessageService.Setup(x => x.Send(_validAddMessageRequest, _nhsLoginId))
                .Throws(new ArgumentException("test"));

            // Act
            var result = await _systemUnderTest.Post(_validAddMessageRequest, _nhsLoginId);
            
            // Assert
            _mockMessageService.VerifyAll();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }
        
        [TestMethod]
        public async Task Post_MessageIsNull_ReturnsBadRequest()
        {
            // Arrange
            _mockMessageService.Setup(x => x.Send(It.IsAny<AddMessageRequest>(), _nhsLoginId ))
                .ReturnsAsync(new MessageResult.BadRequest());
            
            // Act
            var result = await _systemUnderTest.Post(null, _nhsLoginId);
            
            // Assert
            result.Should().BeAssignableTo<BadRequestResult>();
        }

        [TestMethod]
        public async Task Post_MessageRequestIsInvalid_ReturnsBadRequest()
        {
            // Arrange
            _mockMessageService.Setup(x => x.Send(It.IsAny<AddMessageRequest>(), _nhsLoginId ))
                .ReturnsAsync(new MessageResult.BadRequest());
            
            // Act
            var result = await _systemUnderTest.Post(_validAddMessageRequest, _nhsLoginId);
            
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
            var response = _fixture.Create<MessagesResponse>();

            _mockMessageService.Setup(x => x.GetMessages(It.IsAny<AccessToken>(), It.IsAny<string>()))
                .ReturnsAsync(new MessagesResult.Some(response));

            // Act
            var result = await _systemUnderTest.Get(_fixture.Create<string>());

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
            var result = await _systemUnderTest.Get(_fixture.Create<string>());

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
            var result = await _systemUnderTest.Get(_fixture.Create<string>());

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
            var result = await _systemUnderTest.Get(_fixture.Create<string>());

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
            var result = await _systemUnderTest.Get(_fixture.Create<string>());

            // Assert
            _mockMessageService.VerifyAll();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Get_WithSummary_SuccessSome()
        {
            // Arrange
            var response = _fixture.Create<MessagesResponse>();
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
            _mockMessageService.Setup(x =>
                    x.PatchMessage(It.IsAny<JsonPatchDocument<Message>>(), It.IsAny<AccessToken>(), It.IsAny<string>()))
                .ReturnsAsync(new MessagePatchResult.Updated());

            // Act
            var result = await _systemUnderTest.Patch(_fixture.Create<JsonPatchDocument<Message>>(), _fixture.Create<string>());

            // Assert
            _mockMessageService.VerifyAll();
            result.Should().BeAssignableTo<NoContentResult>();
        }

        [TestMethod]
        public async Task Patch_MessageServiceReturnsNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockMessageService.Setup(x =>
                    x.PatchMessage(It.IsAny<JsonPatchDocument<Message>>(), It.IsAny<AccessToken>(), It.IsAny<string>()))
                .ReturnsAsync(new MessagePatchResult.NotFound());

            // Act
            var result = await _systemUnderTest.Patch(_fixture.Create<JsonPatchDocument<Message>>(), _fixture.Create<string>());

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
                    x.PatchMessage(It.IsAny<JsonPatchDocument<Message>>(), It.IsAny<AccessToken>(), It.IsAny<string>()))
                .ReturnsAsync(new MessagePatchResult.BadGateway());

            // Act
            var result = await _systemUnderTest.Patch(_fixture.Create<JsonPatchDocument<Message>>(), _fixture.Create<string>());

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
                    x.PatchMessage(It.IsAny<JsonPatchDocument<Message>>(), It.IsAny<AccessToken>(), It.IsAny<string>()))
                .ReturnsAsync(new MessagePatchResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.Patch(_fixture.Create<JsonPatchDocument<Message>>(), _fixture.Create<string>());
            
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
                    x.PatchMessage(It.IsAny<JsonPatchDocument<Message>>(), It.IsAny<AccessToken>(), It.IsAny<string>()))
                .Throws<ArgumentException>();

            // Act
            var result = await _systemUnderTest.Patch(_fixture.Create<JsonPatchDocument<Message>>(), _fixture.Create<string>());
            
            // Assert
            _mockMessageService.VerifyAll();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Patch_PatchRequestIsInvalid_ReturnsBadRequest()
        {
            // Act
            _mockMessageService.Setup(x => x.PatchMessage(
                    It.IsAny<JsonPatchDocument<Message>>(), It.IsAny<AccessToken>(), It.IsAny<string>()))
                .ReturnsAsync(new MessagePatchResult.BadRequest());
            
            var result = await _systemUnderTest.Patch(null, _fixture.Create<string>());

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [TestCleanup]
        public void Dispose()
        {
            _systemUnderTest?.Dispose();
        }
    }
}
