using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auth.AspNet;
using NHSOnline.Backend.MessagesApi.Areas.Messages;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using UnitTestHelper;

namespace NHSOnline.Backend.MessagesApi.UnitTests.Areas.Messages
{
    [TestClass]
    public sealed class MessageLinkClickedControllerTests : IDisposable
    {
        private MessageLinkClickedController _systemUnderTest;

        private Mock<IAccessTokenProvider> _mockAccessTokenProvider;
        private Mock<IMessageLinkClickedService> _mockMessageLinkClickedService;

        private const string MessageId = "MessageId";
        private const string NhsLoginId = "NhsLoginId";
        private readonly Uri _link = new Uri("https://testing.com/valid/url/");

        private MessageLinkClickedRequest _request;

        [TestInitialize]
        public void TestInitialize()
        {
            _request = new MessageLinkClickedRequest
            {
                Link = _link
            };

            _mockAccessTokenProvider = new Mock<IAccessTokenProvider>(MockBehavior.Strict);
            _mockMessageLinkClickedService = new Mock<IMessageLinkClickedService>(MockBehavior.Strict);

            _mockAccessTokenProvider
                .Setup(x => x.AccessToken)
                .Returns(AccessTokenMock.Generate(subject: NhsLoginId));

            _systemUnderTest = new MessageLinkClickedController(
                _mockAccessTokenProvider.Object,
                new Mock<ILogger<MessageLinkClickedController>>().Object,
                _mockMessageLinkClickedService.Object
            );
        }

        [TestMethod]
        public async Task Post_ServiceReturnsSuccess_ReturnsSuccess()
        {
            // Arrange
            _mockMessageLinkClickedService
                .Setup(x => x.LogLinkClicked(NhsLoginId, It.IsAny<MessageLink>()))
                .ReturnsAsync(new MessageLinkClickedResult.Success());

            // Act
            var result = await _systemUnderTest.Post(MessageId, _request);

            // Assert
            VerifySetups();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [TestMethod]
        public async Task Post_ServiceReturnsBadRequest_ReturnsBadRequest()
        {
            // Arrange
            _mockMessageLinkClickedService
                .Setup(x => x.LogLinkClicked(NhsLoginId, It.IsAny<MessageLink>()))
                .ReturnsAsync(new MessageLinkClickedResult.BadRequest());

            // Act
            var result = await _systemUnderTest.Post(MessageId, _request);

            // Assert
            VerifySetups();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [TestMethod]
        public async Task Post_ServiceReturnsBadGateway_ReturnsBadGateway()
        {
            // Arrange
            _mockMessageLinkClickedService
                .Setup(x => x.LogLinkClicked(NhsLoginId, It.IsAny<MessageLink>()))
                .ReturnsAsync(new MessageLinkClickedResult.BadGateway());

            // Act
            var result = await _systemUnderTest.Post(MessageId, _request);

            // Assert
            VerifySetups();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task Post_ServiceReturnsInternalServerError_ReturnsInternalServerError()
        {
            // Arrange
            _mockMessageLinkClickedService
                .Setup(x => x.LogLinkClicked(NhsLoginId, It.IsAny<MessageLink>()))
                .ReturnsAsync(new MessageLinkClickedResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.Post(MessageId, _request);

            // Assert
            VerifySetups();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Post_ServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockMessageLinkClickedService
                .Setup(x => x.LogLinkClicked(NhsLoginId, It.IsAny<MessageLink>()))
                .Throws<Exception>();

            // Act
            var result = await _systemUnderTest.Post(MessageId, _request);

            // Assert
            VerifySetups();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        private void VerifySetups()
        {
            _mockAccessTokenProvider.VerifyAll();
            _mockMessageLinkClickedService.VerifyAll();
        }

        [TestCleanup]
        public void Dispose() => _systemUnderTest?.Dispose();
    }
}
