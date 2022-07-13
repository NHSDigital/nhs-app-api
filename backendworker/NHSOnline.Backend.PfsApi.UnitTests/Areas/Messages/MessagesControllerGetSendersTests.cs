using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public sealed class MessagesControllerGetSendersTests : IDisposable
    {
        private const string NhsNumber = "NHS Number";

        private MessagesController _systemUnderTest;
        private AccessToken _accessToken;

        private Mock<IMessageService> _mockMessageService;
        private Mock<IEventHubLogger> _mockEventHubLogger;
        private Mock<IMetricLogger> _mockMetricLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            _accessToken = AccessTokenMock.Generate(nhsNumber: NhsNumber);

            var mockAccessTokenProvider = new Mock<IAccessTokenProvider>(MockBehavior.Strict);

            mockAccessTokenProvider
                .SetupGet(x => x.AccessToken)
                .Returns(_accessToken);

            _mockEventHubLogger = new Mock<IEventHubLogger>();
            _mockMetricLogger = new Mock<IMetricLogger>();
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
        [DataRow(null, null, false)]
        [DataRow("sender", "senderId", false)]
        [DataRow("sender", null, true)]
        [DataRow(null, "senderId", true)]
        [DataRow("sender", "senderId", true)]
        public async Task Get_ArgumentsAreNotValid_ReturnsBadRequest(string sender, string senderId, bool summary)
        {
            // Act
            var result = await _systemUnderTest.Get(sender, senderId, summary);

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [TestMethod]
        public async Task Get_WithSender_SuccessFound()
        {
            // Arrange
            var response = new MessagesResponse
            {
                SenderMessages = new List<SenderMessages>
                {
                    new SenderMessages { Sender = "Test Sender", UnreadCount = 1 }
                }
            };

            _mockMessageService.Setup(x => x.GetMessagesBySender(It.IsAny<AccessToken>(), It.IsAny<string>()))
                .ReturnsAsync(new MessagesResult.Found(response));

            // Act
            var result = await _systemUnderTest.Get("sender");

            // Assert
            _mockMessageService.VerifyAll();

            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeAssignableTo<List<SenderMessages>>()
                .Subject.Should().BeEquivalentTo(response.SenderMessages);
        }

        [TestMethod]
        public async Task Get_WithSender_SuccessNone()
        {
            // Arrange
            _mockMessageService.Setup(x => x.GetMessagesBySender(It.IsAny<AccessToken>(), It.IsAny<string>()))
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
            _mockMessageService.Setup(x => x.GetMessagesBySender(It.IsAny<AccessToken>(), It.IsAny<string>()))
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
            _mockMessageService.Setup(x => x.GetMessagesBySender(It.IsAny<AccessToken>(), It.IsAny<string>()))
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
            _mockMessageService.Setup(x => x.GetMessagesBySender(It.IsAny<AccessToken>(), It.IsAny<string>()))
                .Throws<ArgumentException>();

            // Act
            var result = await _systemUnderTest.Get("sender");

            // Assert
            _mockMessageService.VerifyAll();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Get_WithSenderId_SuccessFound()
        {
            // Arrange
            var response = new MessagesResponse
            {
                SenderMessages = new List<SenderMessages>
                {
                    new SenderMessages { Sender = "Test Sender", UnreadCount = 1 }
                }
            };

            _mockMessageService.Setup(x => x.GetMessagesBySenderId(It.IsAny<AccessToken>(), It.IsAny<string>()))
                .ReturnsAsync(new MessagesResult.Found(response));

            // Act
            var result = await _systemUnderTest.Get(senderId: "senderId");

            // Assert
            _mockMessageService.VerifyAll();

            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeAssignableTo<List<SenderMessages>>()
                .Subject.Should().BeEquivalentTo(response.SenderMessages);
        }

        [TestMethod]
        public async Task Get_WithSenderId_SuccessNone()
        {
            // Arrange
            _mockMessageService.Setup(x => x.GetMessagesBySenderId(It.IsAny<AccessToken>(), It.IsAny<string>()))
                .ReturnsAsync(new MessagesResult.None());

            // Act
            var result = await _systemUnderTest.Get(senderId: "senderId");

            // Assert
            _mockMessageService.VerifyAll();

            result.Should().BeAssignableTo<NoContentResult>();
        }

        [TestMethod]
        public async Task Get_WithSenderId_MessageServiceReturnsBadGatewayResult_ReturnsBadGateway()
        {
            // Arrange
            _mockMessageService.Setup(x => x.GetMessagesBySenderId(It.IsAny<AccessToken>(), It.IsAny<string>()))
                .ReturnsAsync(new MessagesResult.BadGateway());

            // Act
            var result = await _systemUnderTest.Get(senderId: "senderId");

            // Assert
            _mockMessageService.VerifyAll();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task Get_WithSenderId_MessageServiceReturnsInternalServerErrorResult_ReturnsInternalServerError()
        {
            // Arrange
            _mockMessageService.Setup(x => x.GetMessagesBySenderId(It.IsAny<AccessToken>(), It.IsAny<string>()))
                .ReturnsAsync(new MessagesResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.Get(senderId: "senderId");

            // Assert
            _mockMessageService.VerifyAll();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Get_WithSenderId_MessageServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockMessageService.Setup(x => x.GetMessagesBySenderId(It.IsAny<AccessToken>(), It.IsAny<string>()))
                .Throws<ArgumentException>();

            // Act
            var result = await _systemUnderTest.Get(senderId: "senderId");

            // Assert
            _mockMessageService.VerifyAll();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Get_WithSummary_SuccessFound()
        {
            // Arrange
            var response = new MessagesResponse
            {
                SenderMessages = new List<SenderMessages>
                {
                    new SenderMessages { Sender = "Test Sender", UnreadCount = 1 }
                }
            };

            _mockMessageService.Setup(x => x.GetSummaryMessages(It.IsAny<AccessToken>()))
                .ReturnsAsync(new MessagesResult.Found(response));

            // Act
            var result = await _systemUnderTest.Get(summary: true);

            // Assert
            _mockMessageService.VerifyAll();

            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeAssignableTo<List<SenderMessages>>()
                .Subject.Should().BeEquivalentTo(response.SenderMessages);
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
        public async Task GetSendersV2_WhenFound_ReturnsOk()
        {
            // Arrange
            var response = new UserSendersResponse
            {
                Senders = new List<UserSender>
                {
                    new UserSender
                    {
                        Id = "Sender Id",
                        Name = "Sender Name",
                        UnreadCount = 1
                    }
                }
            };

            _mockMessageService
                .Setup(x => x.GetSendersV2(It.IsAny<AccessToken>()))
                .ReturnsAsync(new UserSendersResult.Found(response));

            // Act
            var result = await _systemUnderTest.GetSendersV2();

            // Assert
            _mockMessageService.VerifyAll();

            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeAssignableTo<UserSendersResponse>()
                .Subject.Should().BeEquivalentTo(response);
        }

        [TestMethod]
        public async Task GetSendersV2_WhenNotFound_ReturnsNoContent()
        {
            // Arrange
            _mockMessageService
                .Setup(x => x.GetSendersV2(It.IsAny<AccessToken>()))
                .ReturnsAsync(new UserSendersResult.None());

            // Act
            var result = await _systemUnderTest.GetSendersV2();

            // Assert
            _mockMessageService.VerifyAll();

            result.Should().BeAssignableTo<NoContentResult>();
        }

        [TestMethod]
        public async Task GetSendersV2_WhenMessageServiceReturnsBadGatewayResult_ReturnsBadGateway()
        {
            // Arrange
            _mockMessageService
                .Setup(x => x.GetSendersV2(It.IsAny<AccessToken>()))
                .ReturnsAsync(new UserSendersResult.BadGateway());

            // Act
            var result = await _systemUnderTest.GetSendersV2();

            // Assert
            _mockMessageService.VerifyAll();

            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task GetSendersV2_WhenMessageServiceReturnsInternalServerErrorResult_ReturnsInternalServerError()
        {
            // Arrange
            _mockMessageService
                .Setup(x => x.GetSendersV2(It.IsAny<AccessToken>()))
                .ReturnsAsync(new UserSendersResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.GetSendersV2();

            // Assert
            _mockMessageService.VerifyAll();

            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task GetSenders_WhenFound_ReturnsOk()
        {
            // Arrange
            var response = new UserSendersResponse
            {
                Senders = new List<UserSender>
                {
                    new UserSender
                    {
                        Name = "Sender Name",
                        UnreadCount = 1
                    }
                }
            };

            _mockMessageService
                .Setup(x => x.GetSenders(It.IsAny<AccessToken>()))
                .ReturnsAsync(new UserSendersResult.Found(response));

            // Act
            var result = await _systemUnderTest.GetSenders();

            // Assert
            _mockMessageService.VerifyAll();

            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeAssignableTo<UserSendersResponse>()
                .Subject.Should().BeEquivalentTo(response);
        }

        [TestMethod]
        public async Task GetSenders_WhenNotFound_ReturnsNoContent()
        {
            // Arrange
            _mockMessageService
                .Setup(x => x.GetSenders(It.IsAny<AccessToken>()))
                .ReturnsAsync(new UserSendersResult.None());

            // Act
            var result = await _systemUnderTest.GetSenders();

            // Assert
            _mockMessageService.VerifyAll();

            result.Should().BeAssignableTo<NoContentResult>();
        }

        [TestMethod]
        public async Task GetSenders_WhenMessageServiceReturnsBadGatewayResult_ReturnsBadGateway()
        {
            // Arrange
            _mockMessageService
                .Setup(x => x.GetSenders(It.IsAny<AccessToken>()))
                .ReturnsAsync(new UserSendersResult.BadGateway());

            // Act
            var result = await _systemUnderTest.GetSenders();

            // Assert
            _mockMessageService.VerifyAll();

            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task GetSenders_WhenMessageServiceReturnsInternalServerErrorResult_ReturnsInternalServerError()
        {
            // Arrange
            _mockMessageService
                .Setup(x => x.GetSenders(It.IsAny<AccessToken>()))
                .ReturnsAsync(new UserSendersResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.GetSenders();

            // Assert
            _mockMessageService.VerifyAll();

            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestCleanup]
        public void Dispose() => _systemUnderTest?.Dispose();
    }
}