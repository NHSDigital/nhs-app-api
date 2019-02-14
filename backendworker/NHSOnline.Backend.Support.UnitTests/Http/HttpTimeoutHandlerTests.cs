using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Support.Settings;
using NHSOnline.Backend.Support.Http;
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.Support.UnitTests.Http
{
    [TestClass]
    public class HttpTimeoutHandlerTests
    {
        private IFixture _fixture;
        private Mock<IOptions<ConfigurationSettings>> _settings;
        private Mock<IHttpRequestIdentifier> _requestIdentifier;
        private ConfigurationSettings _configurationSettings;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _fixture.Freeze<Mock<ILogger<HttpTimeoutHandler<IHttpRequestIdentifier>>>>();
            _requestIdentifier = _fixture.Freeze<Mock<IHttpRequestIdentifier>>();
            _settings = _fixture.Freeze<Mock<IOptions<ConfigurationSettings>>>();
            
            _configurationSettings = _fixture.Create<ConfigurationSettings>();
            _configurationSettings.DefaultHttpTimeoutSeconds = 2;
            _configurationSettings.EmisExtendedHttpTimeoutSeconds = 6;

            _settings
                .Setup(x => x.Value)
                .Returns(_configurationSettings);
        }

        [TestMethod]
        public void SendAsyc_ThrowsTimoutException_WhenTaskThrowsOperationCancelledException()
        {
            var httpRequestMessage = _fixture.Create<HttpRequestMessage>();
            
            MockHttpMessageHandler _mockHttpHandler = new MockHttpMessageHandler();
            _mockHttpHandler.Fallback.Respond(async () =>
            {
                await Task.Delay(10000);
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

            var expectedRequestId = new HttpRequestIdentity
            {
                Provider = _fixture.Create<string>(),
                Identifier = _fixture.Create<string>(),
                Method = _fixture.Create<string>(),
                RequestUrl = _fixture.Create<Uri>()
            };

            _requestIdentifier
                .Setup(x => x.Identify(httpRequestMessage))
                .Returns(expectedRequestId);

            var systemUnderTest = _fixture.Create<HttpTimeoutHandler<IHttpRequestIdentifier>>();
            systemUnderTest.InnerHandler = _mockHttpHandler;

            var invoker = new HttpMessageInvoker(systemUnderTest);
            Func<Task> act = async () => { await invoker.SendAsync(httpRequestMessage, new CancellationToken()); };

            act.Should().Throw<TimeoutException>().Which.Message.Should().Be(expectedRequestId.ToString());
            
            _mockHttpHandler.Dispose();
        }
        
        [TestMethod]
        public async Task SendAsyc_ReturnsHttpResponseMessage_WhenSuccessful()
        {
            var httpRequestMessage = _fixture.Create<HttpRequestMessage>();
            var httpContent = _fixture.Create<HttpContent>();
            
            MockHttpMessageHandler _mockHttpHandler = new MockHttpMessageHandler();
            _mockHttpHandler
                .When(httpRequestMessage.Method, httpRequestMessage.RequestUri.ToString())
                .Respond(httpContent);
                
            _mockHttpHandler.Fallback.Respond(async () =>
            {
                await Task.Delay(10000);
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

            var systemUnderTest = _fixture.Create<HttpTimeoutHandler<IHttpRequestIdentifier>>();
            systemUnderTest.InnerHandler = _mockHttpHandler;

            var invoker = new HttpMessageInvoker(systemUnderTest);
            var result = await invoker.SendAsync(httpRequestMessage, new CancellationToken());

            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Content.Should().BeEquivalentTo(httpContent);
            
            _mockHttpHandler.Dispose();
        }
        
        [TestMethod]
        public async Task SendAsyc_EmisExtendedTimeout_TimeoutGreaterThanDefault_Successful()
        {
            var httpRequestMessage = _fixture.Create<HttpRequestMessage>();
            var httpContent = _fixture.Create<HttpContent>();
            
            MockHttpMessageHandler _mockHttpHandler = new MockHttpMessageHandler();
            _mockHttpHandler
                .When(httpRequestMessage.Method, httpRequestMessage.RequestUri.ToString())
                .Respond(httpContent);

            httpRequestMessage.Properties.Add(HttpRequestConstants.CustomTimeout,
                _configurationSettings.EmisExtendedHttpTimeoutSeconds);
            
            _mockHttpHandler.Fallback.Respond(async () =>
            {
                await Task.Delay(3000);
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

            var systemUnderTest = _fixture.Create<HttpTimeoutHandler<IHttpRequestIdentifier>>();
            systemUnderTest.InnerHandler = _mockHttpHandler;

            var invoker = new HttpMessageInvoker(systemUnderTest);
            var result = await invoker.SendAsync(httpRequestMessage, new CancellationToken());

            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Content.Should().BeEquivalentTo(httpContent);
            
            _mockHttpHandler.Dispose();
        }
        
        [TestMethod]
        public async Task SendAsyc_EmisExtendedTimeout_TimeoutEmisExtendedTimeout_ThrowException()
        {
            var httpRequestMessage = _fixture.Create<HttpRequestMessage>();
            var httpContent = _fixture.Create<HttpContent>();
            
            MockHttpMessageHandler _mockHttpHandler = new MockHttpMessageHandler();
            _mockHttpHandler
                .When(httpRequestMessage.Method, httpRequestMessage.RequestUri.ToString())
                .Respond(httpContent);

            httpRequestMessage.Properties.Add(HttpRequestConstants.CustomTimeout,
                _configurationSettings.EmisExtendedHttpTimeoutSeconds);
            
            _mockHttpHandler.Fallback.Respond(async () =>
            {
                await Task.Delay(8000);
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

            var systemUnderTest = _fixture.Create<HttpTimeoutHandler<IHttpRequestIdentifier>>();
            systemUnderTest.InnerHandler = _mockHttpHandler;

            var invoker = new HttpMessageInvoker(systemUnderTest);
            var result = await invoker.SendAsync(httpRequestMessage, new CancellationToken());

            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Content.Should().BeEquivalentTo(httpContent);
            
            _mockHttpHandler.Dispose();
        }
    }
}