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
        public void SendAsync_ThrowsTimoutException_WhenTaskThrowsOperationCancelledException()
        {
            var httpRequestMessage = _fixture.Create<HttpRequestMessage>();
            
            var mockHttpHandler = new MockHttpMessageHandler();
            mockHttpHandler.Fallback.Respond(async () =>
            {
                await Task.Delay(10000);
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

            var expectedRequestId = 
                new HttpRequestIdentity(_fixture.Create<string>(), httpRequestMessage)
                    .SetCorrelationIdentifier(_fixture.Create<string>());

            _requestIdentifier
                .Setup(x => x.Identify(httpRequestMessage))
                .Returns(expectedRequestId);

            var systemUnderTest = _fixture.Create<HttpTimeoutHandler<IHttpRequestIdentifier>>();
            systemUnderTest.InnerHandler = mockHttpHandler;

            var invoker = new HttpMessageInvoker(systemUnderTest);
            Func<Task> act = async () => { await invoker.SendAsync(httpRequestMessage, new CancellationToken()); };

            act.Should().Throw<TimeoutException>().Which.Message.Should().Be(expectedRequestId.ToString());
            
            mockHttpHandler.Dispose();
        }
        
        [TestMethod]
        public async Task SendAsync_ReturnsHttpResponseMessage_WhenSuccessful()
        {
            var httpRequestMessage = _fixture.Create<HttpRequestMessage>();
            var httpContent = _fixture.Create<HttpContent>();
            
            var mockHttpHandler = new MockHttpMessageHandler();
            mockHttpHandler
                .When(httpRequestMessage.Method, httpRequestMessage.RequestUri.ToString())
                .Respond(httpContent);
                
            mockHttpHandler.Fallback.Respond(async () =>
            {
                await Task.Delay(10000);
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

            var systemUnderTest = _fixture.Create<HttpTimeoutHandler<IHttpRequestIdentifier>>();
            systemUnderTest.InnerHandler = mockHttpHandler;

            var invoker = new HttpMessageInvoker(systemUnderTest);
            var result = await invoker.SendAsync(httpRequestMessage, new CancellationToken());

            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Content.Should().BeEquivalentTo(httpContent);
            
            mockHttpHandler.Dispose();
        }
        
        [TestMethod]
        public async Task SendAsync_EmisExtendedTimeout_TimeoutGreaterThanDefault_Successful()
        {
            var httpRequestMessage = _fixture.Create<HttpRequestMessage>();
            var httpContent = _fixture.Create<HttpContent>();
            
            var mockHttpHandler = new MockHttpMessageHandler();
            mockHttpHandler
                .When(httpRequestMessage.Method, httpRequestMessage.RequestUri.ToString())
                .Respond(httpContent);

            httpRequestMessage.Properties.Add(HttpRequestConstants.CustomTimeout,
                _configurationSettings.EmisExtendedHttpTimeoutSeconds);
            
            mockHttpHandler.Fallback.Respond(async () =>
            {
                await Task.Delay(3000);
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

            var systemUnderTest = _fixture.Create<HttpTimeoutHandler<IHttpRequestIdentifier>>();
            systemUnderTest.InnerHandler = mockHttpHandler;

            var invoker = new HttpMessageInvoker(systemUnderTest);
            var result = await invoker.SendAsync(httpRequestMessage, new CancellationToken());

            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Content.Should().BeEquivalentTo(httpContent);
            
            mockHttpHandler.Dispose();
        }
        
        [TestMethod]
        public async Task SendAsync_EmisExtendedTimeout_TimeoutEmisExtendedTimeout_ThrowException()
        {
            var httpRequestMessage = _fixture.Create<HttpRequestMessage>();
            var httpContent = _fixture.Create<HttpContent>();
            
            var mockHttpHandler = new MockHttpMessageHandler();
            mockHttpHandler
                .When(httpRequestMessage.Method, httpRequestMessage.RequestUri.ToString())
                .Respond(httpContent);

            httpRequestMessage.Properties.Add(HttpRequestConstants.CustomTimeout,
                _configurationSettings.EmisExtendedHttpTimeoutSeconds);
            
            mockHttpHandler.Fallback.Respond(async () =>
            {
                await Task.Delay(8000);
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

            var systemUnderTest = _fixture.Create<HttpTimeoutHandler<IHttpRequestIdentifier>>();
            systemUnderTest.InnerHandler = mockHttpHandler;

            var invoker = new HttpMessageInvoker(systemUnderTest);
            var result = await invoker.SendAsync(httpRequestMessage, new CancellationToken());

            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Content.Should().BeEquivalentTo(httpContent);
            
            mockHttpHandler.Dispose();
        }
    }
}