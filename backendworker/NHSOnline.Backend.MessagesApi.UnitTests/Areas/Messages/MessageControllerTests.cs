using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.MessagesApi.Areas.Messages;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using UnitTestHelper;

namespace NHSOnline.Backend.MessagesApi.UnitTests.Areas.Messages
{
    [TestClass]
    public class MessageControllerTests
    {
        private IFixture _fixture;
        private MessageController _systemUnderTest;
        private Mock<IMessageService> _mockMessageService;
        private AddMessageRequest _validAddMessageRequest;
        private string _nhsLoginId;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());
            
            _nhsLoginId =  _fixture.Create<string>();

            _mockMessageService = _fixture.Freeze<Mock<IMessageService>>();
            _validAddMessageRequest = _fixture.Create<AddMessageRequest>();

            _systemUnderTest = _fixture.Create<MessageController>();
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

        [TestCleanup]
        public void Dispose()
        {
            _systemUnderTest?.Dispose();
        }
    }
}