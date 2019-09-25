using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
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
        private AddMessageRequest _validAddMessageRequest;
        private string _nhsLoginId;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            var mockHttpContext =  HttpContextGetAccessTokenHelper.CreateMockHttpContext(_fixture);

            _nhsLoginId =  _fixture.Create<string>();

            _mockMessageService = _fixture.Freeze<Mock<IMessageService>>();
            _validAddMessageRequest = _fixture.Create<AddMessageRequest>();

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
            // Act
            var result = await _systemUnderTest.Post(null, _nhsLoginId);
            
            // Assert
            result.Should().BeAssignableTo<BadRequestResult>();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("            ")]
        [DataRow("")]
        public async Task Post_NHSLoginIdIsInvalid_ReturnsBadRequest(string nhsLoginId)
        {
            // Act
            var result = await _systemUnderTest.Post(_validAddMessageRequest, nhsLoginId);
            
            // Assert
            result.Should().BeAssignableTo<BadRequestResult>();
        }

        [DataTestMethod]
        [DataRow( "    ", "    ")]
        [DataRow(null, "body")]
        [DataRow("sender", null)]
        public async Task Post_MessageBodyIsInvalid_ReturnsBadRequest(string sender, string body)
        {
            // Arrange
            var invalidMessage = new AddMessageRequest
            {
                Sender = sender,
                Body = body
            };
            
            // Act
            var result = await _systemUnderTest.Post(invalidMessage, _nhsLoginId);
            
            // Assert
            result.Should().BeAssignableTo<BadRequestResult>();
        }

        [TestMethod]
        public async Task Get_SuccessSome()
        {
            // Arrange
            var message = MessageHelpers.MockUserMessage(_fixture);
            var messages = new[] { message }.ToList();
            _mockMessageService.Setup(x => x.GetMessages(It.IsAny<AccessToken>()))
                .ReturnsAsync(new MessagesResult.Some(messages));

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _mockMessageService.VerifyAll();
            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeAssignableTo<IEnumerable<UserMessage>>()
                .Subject.Should().BeEquivalentTo(messages);
        }

        [TestMethod]
        public async Task Get_SuccessNone()
        {
            // Arrange
            _mockMessageService.Setup(x => x.GetMessages(It.IsAny<AccessToken>()))
                .ReturnsAsync(new MessagesResult.None());

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _mockMessageService.VerifyAll();
            result.Should().BeAssignableTo<NoContentResult>();
        }

        [TestMethod]
        public async Task Get_MessageServiceReturnsBadGatewayResult_ReturnsBadGateway()
        {
            // Arrange
            _mockMessageService.Setup(x => x.GetMessages(It.IsAny<AccessToken>()))
                .ReturnsAsync(new MessagesResult.BadGateway());

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _mockMessageService.VerifyAll();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task Get_MessageServiceReturnsInternalServerErrorResult_ReturnsInternalServerError()
        {
            // Arrange
            _mockMessageService.Setup(x => x.GetMessages(It.IsAny<AccessToken>()))
                .ReturnsAsync(new MessagesResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _mockMessageService.VerifyAll();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Get_MessageServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockMessageService.Setup(x => x.GetMessages(It.IsAny<AccessToken>()))
                .Throws<ArgumentException>();

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _mockMessageService.VerifyAll();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestCleanup]
        public void Dispose()
        {
            _systemUnderTest?.Dispose();
        }
    }
}
