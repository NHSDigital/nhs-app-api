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
using NHSOnline.Backend.PfsApi.UnitTests.Extensions;
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.PfsApi.UnitTests.Messages
{
    [TestClass]
    public sealed class MessagesClientTests : IDisposable
    {
        private IMessagesClient _systemUnderTest;

        private MockHttpMessageHandler _mockHttpHandler;
        private Mock<ILogger<MessagesClient>> _mockLogger;
        private Mock<IMessagesApiConfig> _mockConfiguration;

        private static readonly Uri BaseUri = new Uri("https://base_url", UriKind.Absolute);
        private const string MessageId = "MessageId";
        private const string NhsLoginId = "NhsLoginId";
        private readonly string _resourceUri = $"/v1/api/{NhsLoginId}/messages";

        private readonly AddMessageRequest _messageRequest = new AddMessageRequest
        {
            Body = "Body",
            Sender = "Sender",
            Version = 1,
            SenderContext = new AddMessageSenderContext
            {
                CampaignId = "CampaignId",
                NhsLoginId = NhsLoginId,
                SupplierId = "SupplierId"
            }
        };

        [TestInitialize]
        public void TestInitialize()
        {
            _mockHttpHandler = new MockHttpMessageHandler();
            _mockLogger = new Mock<ILogger<MessagesClient>>();
            _mockConfiguration = new Mock<IMessagesApiConfig>(MockBehavior.Strict);

            _mockConfiguration.SetupGet(x => x.ApiKey).Returns("ApiKey");
            _mockConfiguration.SetupGet(x => x.BaseUrl).Returns(BaseUri);
            _mockConfiguration.SetupGet(x => x.ResourceUrl).Returns(_resourceUri);

            _systemUnderTest = new MessagesClient(
                _mockLogger.Object,
                _mockConfiguration.Object,
                new MessagesHttpClient(_mockHttpHandler.ToHttpClient(), _mockConfiguration.Object)
            );
        }

        [TestMethod]
        public async Task Post_ReturnsCreated()
        {
            // Arrange
            using var body = new StringContent(JsonConvert.SerializeObject(new AddMessageResponse
            {
                MessageId = MessageId
            }));

            _mockHttpHandler
                .WhenRequest(HttpMethod.Post, BaseUri, _resourceUri)
                .Respond(HttpStatusCode.Created, body);

            // Act
            var response = await _systemUnderTest.Post(_messageRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.HasSuccessResponse.Should().Be(true);
            response.MessageId.Should().Be(MessageId);
        }

        [TestMethod]
        [DataRow(HttpStatusCode.InternalServerError)]
        [DataRow(HttpStatusCode.BadGateway)]
        [DataRow(HttpStatusCode.Forbidden)]
        public async Task Post_ErrorResponseReceived_ReturnsError(HttpStatusCode httpStatusCode)
        {
            // Arrange
            _mockHttpHandler
                .WhenRequest(HttpMethod.Post, BaseUri, _resourceUri)
                .Respond(httpStatusCode);

            // Act
            var response = await _systemUnderTest.Post(_messageRequest);

            // Assert
            response.StatusCode.Should().Be(httpStatusCode);
            response.HasSuccessResponse.Should().Be(false);
            response.MessageId.Should().BeNull();
        }

        public void Dispose()
        {
            _mockHttpHandler.Dispose();
        }
    }
}
