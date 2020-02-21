using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Support.Settings;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support.AspNet.Filters;
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.Support.UnitTests.Http
{
    [TestClass]
    public sealed class HttpTimeoutHandlerTests : IDisposable
    {
        private IFixture _fixture;
        private Mock<IHttpTimeoutConfigurationSettings> _httpTimeoutConfigurationSettings;
        private HttpRequestMessage _httpRequestMessage;
        private MockHttpMessageHandler _mockHttpHandler;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _fixture.Freeze<Mock<ILogger<HttpTimeoutHandler<IHttpRequestIdentifier>>>>();
            
            _httpTimeoutConfigurationSettings = _fixture.Freeze<Mock<IHttpTimeoutConfigurationSettings>>();
            _httpTimeoutConfigurationSettings.Setup(x => x.DefaultHttpTimeoutSeconds)
                .Returns(2);
            
            var mockConfigurationSettings = _fixture.Freeze<Mock<IConfigurationSettings>>();
            mockConfigurationSettings.SetupGet(x => x.DefaultHttpTimeoutSeconds)
                .Returns(2);
            
            _httpRequestMessage = _fixture.Create<HttpRequestMessage>();
            _mockHttpHandler = _fixture.Create<MockHttpMessageHandler>();
        }

        [TestMethod]
        [DataRow("TPP", SourceApi.Tpp)]
        [DataRow("Emis", SourceApi.Emis)]
        [DataRow("Vision", SourceApi.Vision)]
        [DataRow("Microtest", SourceApi.Microtest)]
        [DataRow("OrganDonation", SourceApi.OrganDonation)]
        [DataRow("ServiceJourneyRules", SourceApi.ServiceJourneyRules)]
        [DataRow("CitizenId", SourceApi.NhsLogin)]
        public void SendAsync_ThrowsNhsTimoutException_WhenTaskThrowsOperationCancelledException(string provider,
            SourceApi sourceApi)
        {
            // Arrange
            _mockHttpHandler.Fallback.Respond(async () =>
            {
                await Task.Delay(10000);
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

            var expectedRequestId =
                new HttpRequestIdentity(provider, _httpRequestMessage, sourceApi)
                    .SetCorrelationIdentifier(_fixture.Create<string>());

            var requestIdentifier = _fixture.Freeze<Mock<IHttpRequestIdentifier>>();
            requestIdentifier.Setup(x => x.Identify(_httpRequestMessage))
                .Returns(expectedRequestId);

            var systemUnderTest = _fixture.Create<HttpTimeoutHandler<IHttpRequestIdentifier>>();
            systemUnderTest.InnerHandler = _mockHttpHandler;

            var invoker = new HttpMessageInvoker(systemUnderTest);
            
            // Act
            Func<Task> act = async () => { await invoker.SendAsync(_httpRequestMessage, new CancellationToken()); };

            // Assert
            var nhsTimeoutException = act.Should().Throw<NhsTimeoutException>();
            nhsTimeoutException.Which.Message.Should().Be(expectedRequestId.ToString());
            nhsTimeoutException.Which.SourceApi.Should().Be(sourceApi);
        }

        [TestMethod]
        public async Task SendAsync_ReturnsHttpResponseMessage_WhenSuccessful()
        {
            // Arrange
            var httpContent = _fixture.Create<HttpContent>();
            
            _mockHttpHandler
                .When(_httpRequestMessage.Method, _httpRequestMessage.RequestUri.ToString())
                .Respond(httpContent);

            _mockHttpHandler.Fallback.Respond(async () =>
            {
                await Task.Delay(10000);
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

            var systemUnderTest = _fixture.Create<HttpTimeoutHandler<IHttpRequestIdentifier>>();
            systemUnderTest.InnerHandler = _mockHttpHandler;

            var invoker = new HttpMessageInvoker(systemUnderTest);
            
            // Act
            var result = await invoker.SendAsync(_httpRequestMessage, new CancellationToken());

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Content.Should().BeEquivalentTo(httpContent);
        }

        [TestMethod]
        public async Task SendAsync_EmisExtendedTimeout_TimeoutGreaterThanDefault_Successful()
        {
            // Arrange
            var httpContent = _fixture.Create<HttpContent>();
            
            _mockHttpHandler
                .When(_httpRequestMessage.Method, _httpRequestMessage.RequestUri.ToString())
                .Respond(httpContent);

            _httpRequestMessage.Properties.Add(HttpRequestConstants.CustomTimeout, 6);

            _mockHttpHandler.Fallback.Respond(async () =>
            {
                await Task.Delay(3000);
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

            var systemUnderTest = _fixture.Create<HttpTimeoutHandler<IHttpRequestIdentifier>>();
            systemUnderTest.InnerHandler = _mockHttpHandler;

            var invoker = new HttpMessageInvoker(systemUnderTest);
            
            // Act
            var result = await invoker.SendAsync(_httpRequestMessage, new CancellationToken());

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Content.Should().BeEquivalentTo(httpContent);
        }

        [TestMethod]
        public async Task SendAsync_EmisExtendedTimeout_TimeoutEmisExtendedTimeout_ThrowException()
        {
            // Arrange
            var httpContent = _fixture.Create<HttpContent>();

            _mockHttpHandler
                .When(_httpRequestMessage.Method, _httpRequestMessage.RequestUri.ToString())
                .Respond(httpContent);

            _httpRequestMessage.Properties.Add(HttpRequestConstants.CustomTimeout, 6);

            _mockHttpHandler.Fallback.Respond(async () =>
            {
                await Task.Delay(8000);
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

            var systemUnderTest = _fixture.Create<HttpTimeoutHandler<IHttpRequestIdentifier>>();
            systemUnderTest.InnerHandler = _mockHttpHandler;

            var invoker = new HttpMessageInvoker(systemUnderTest);
            
            // Act
            var result = await invoker.SendAsync(_httpRequestMessage, new CancellationToken());

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Content.Should().BeEquivalentTo(httpContent);
        }

        [TestCleanup]
        public void Dispose()
        {
            _httpRequestMessage?.Dispose();
            _mockHttpHandler?.Dispose();
        }
    }
}