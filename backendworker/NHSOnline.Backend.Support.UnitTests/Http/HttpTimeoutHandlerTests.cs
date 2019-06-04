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
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.Support.UnitTests.Http
{
    [TestClass]
    public class HttpTimeoutHandlerTests
    {
        private IFixture _fixture;
        private Mock<IHttpRequestIdentifier> _requestIdentifier;
        private ConfigurationSettings _configurationSettings;
        private EmisConfigurationSettings _emisConfigurationSettings;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _fixture.Freeze<Mock<ILogger<HttpTimeoutHandler<IHttpRequestIdentifier>>>>();
            _requestIdentifier = _fixture.Freeze<Mock<IHttpRequestIdentifier>>();
            
            _configurationSettings = _fixture.Create<ConfigurationSettings>();
            _emisConfigurationSettings = _fixture.Create<EmisConfigurationSettings>();
            _configurationSettings.DefaultHttpTimeoutSeconds = 2;
            _emisConfigurationSettings.EmisExtendedHttpTimeoutSeconds = 6;

            _fixture.Inject(_configurationSettings);

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
                _emisConfigurationSettings.EmisExtendedHttpTimeoutSeconds);
            
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
                _emisConfigurationSettings.EmisExtendedHttpTimeoutSeconds);
            
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