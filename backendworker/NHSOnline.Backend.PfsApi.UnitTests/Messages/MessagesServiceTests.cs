using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using NHSOnline.Backend.PfsApi.Messages;
using NHSOnline.Backend.PfsApi.Messages.Models;

namespace NHSOnline.Backend.PfsApi.UnitTests.Messages
{
    [TestClass]
    public class MessagesServiceTests
    {
        private MessagesService _systemUnderTest;

        private Mock<ILogger<MessagesService>> _mockLogger;
        private Mock<IMessagesClient> _mockMessagesClient;
        private Mock<IMessagesServiceConfig> _mockMessagesServiceConfig;

        private const string CampaignId = "CampaignId";
        private const string MessageId = "MessageId";
        private const string NhsLoginId = "NhsLoginId";

        [TestInitialize]
        public void TestInitialize()
        {
            _mockLogger = new Mock<ILogger<MessagesService>>();
            _mockMessagesClient = new Mock<IMessagesClient>(MockBehavior.Strict);
            _mockMessagesServiceConfig = new Mock<IMessagesServiceConfig>(MockBehavior.Strict);

            _systemUnderTest = new MessagesService(
                _mockLogger.Object,
                _mockMessagesClient.Object,
                _mockMessagesServiceConfig.Object
            );
        }

        [TestMethod]
        public async Task SendIntroductoryMessage_MessagesTurnedOff_NoActionTaken()
        {
            // Arrange
            SetupConfig(sendIntroductoryMessage: false);

            // Act
            var result = await _systemUnderTest.SendIntroductoryMessage(NhsLoginId);

            // Assert
            VerifyAsserts();
            result.Should().BeOfType<MessagesResult.NoActionTaken>();
        }

        [TestMethod]
        public async Task SendIntroductoryMessage_MessagesTurnedOn_Success()
        {
            // Arrange
            SetupConfig(sendIntroductoryMessage: true);

            var response = new AddMessageResponse
            {
                MessageId = MessageId
            };

            _mockMessagesClient
                .Setup(x => x.Post(It.Is<AddMessageRequest>(r => r.SenderContext.NhsLoginId == NhsLoginId)))
                .ReturnsAsync(BuildResponse(HttpStatusCode.Created, response));

            // Act
            var result = await _systemUnderTest.SendIntroductoryMessage(NhsLoginId);

            // Assert
            VerifyAsserts();
            result.Should().BeOfType<MessagesResult.Success>();
        }

        [TestMethod]
        [DataRow(HttpStatusCode.InternalServerError)]
        [DataRow(HttpStatusCode.BadGateway)]
        [DataRow(HttpStatusCode.Forbidden)]
        public async Task SendIntroductoryMessage_ClientResponseNotSuccessful_ReturnsBadGateway(HttpStatusCode httpStatusCode)
        {
            // Arrange
            SetupConfig(sendIntroductoryMessage: true);

            _mockMessagesClient
                .Setup(x => x.Post(It.Is<AddMessageRequest>(r => r.SenderContext.NhsLoginId == NhsLoginId)))
                .ReturnsAsync(BuildResponse(httpStatusCode));

            // Act
            var result = await _systemUnderTest.SendIntroductoryMessage(NhsLoginId);

            // Assert
            VerifyAsserts();
            result.Should().BeOfType<MessagesResult.BadGateway>();
        }

        [TestMethod]
        public async Task SendIntroductoryMessage_ClientThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            SetupConfig(sendIntroductoryMessage: true);

            _mockMessagesClient
                .Setup(x => x.Post(It.Is<AddMessageRequest>(r => r.SenderContext.NhsLoginId == NhsLoginId)))
                .Throws<Exception>();

            // Act
            var result = await _systemUnderTest.SendIntroductoryMessage(NhsLoginId);

            // Assert
            _mockMessagesClient.VerifyAll();
            result.Should().BeAssignableTo<MessagesResult.InternalServerError>();
        }

        private void SetupConfig(bool sendIntroductoryMessage)
        {
            _mockMessagesServiceConfig.Setup(x => x.SendIntroductoryMessage).Returns(sendIntroductoryMessage);

            if (sendIntroductoryMessage)
            {
                _mockMessagesServiceConfig.Setup(x => x.Body).Returns("Body");
                _mockMessagesServiceConfig.Setup(x => x.CampaignId).Returns(CampaignId);
            }
        }

        private void VerifyAsserts()
        {
            _mockMessagesClient.VerifyAll();
            _mockMessagesServiceConfig.VerifyAll();
        }

        private MessagesResponse BuildResponse(HttpStatusCode statusCode, AddMessageResponse messageResponse = null)
        {
            using var response = new HttpResponseMessage(statusCode);

            if (messageResponse != null)
            {
                response.Content = new StringContent(JsonConvert.SerializeObject(messageResponse));
            }

            return new MessagesResponse(response, _mockLogger.Object);
        }
    }
}
