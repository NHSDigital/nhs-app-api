using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auth.AspNet.ApiKey;

namespace NHSOnline.Backend.Auth.UnitTests.AspNet.ApiKey
{
    [TestClass]
    public sealed class ApiKeyAuthenticationHandlerTests
    {
        private Mock<IOptionsMonitor<ApiKeyAuthenticationOptions>> _options;
        private Mock<ILoggerFactory> _loggerFactory;
        private Mock<UrlEncoder> _encoder;
        private Mock<ISystemClock> _clock;
        private Mock<IGetApiKeyQuery> _principalProvider;
        private ApiKeyAuthenticationHandler _handler;

        [TestInitialize]
        public void TestInitialize()
        {
            _options = new Mock<IOptionsMonitor<ApiKeyAuthenticationOptions>>();

            var logger = new Mock<ILogger<ApiKeyAuthenticationHandler>>();
            _loggerFactory = new Mock<ILoggerFactory>();
            _loggerFactory.Setup(x => x.CreateLogger(It.IsAny<String>())).Returns(logger.Object);

            _encoder = new Mock<UrlEncoder>();
            _clock = new Mock<ISystemClock>();
            _principalProvider = new Mock<IGetApiKeyQuery>();

            _handler = new ApiKeyAuthenticationHandler(_options.Object, _loggerFactory.Object, _encoder.Object,
                _clock.Object, _principalProvider.Object);
        }

        [TestMethod]
        public async Task Success()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var validTestKey = "testKey";
            var secureApiKey = new SecureApiKey("testOwner", validTestKey);
            context.Request.Headers.Add("X-Api-Key", validTestKey);
            _principalProvider.Setup(x => x.Execute(It.IsAny<string>())).ReturnsAsync(secureApiKey);

            await _handler.InitializeAsync(
                new AuthenticationScheme("API Key", null, typeof(ApiKeyAuthenticationHandler)),
                context);

            // Act
            var result = await _handler.AuthenticateAsync();

            // Assert
            Assert.IsTrue(result.Succeeded);
        }

        [TestMethod]
        public async Task Failure_InvalidApiKey()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers.Add("X-Api-Key", "InvalidValue");
            var secureApiKey = new SecureApiKey("testOwner", "ValidValue");
            _principalProvider.Setup(x => x.Execute(It.IsAny<string>())).ReturnsAsync((SecureApiKey) null);

            await _handler.InitializeAsync(
                new AuthenticationScheme("API Key", null, typeof(ApiKeyAuthenticationHandler)),
                context);

            // Act
            var result = await _handler.AuthenticateAsync();

            // Assert
            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual("Invalid API Key provided.", result.Failure.Message);
        }

        [TestMethod]
        public async Task NoResult_EmptyApiKey()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers.Add("X-Api-Key", string.Empty);

            await _handler.InitializeAsync(new AuthenticationScheme("API Key",
                    null,
                    typeof(ApiKeyAuthenticationHandler)),
                context);

            // Act
            var result = await _handler.AuthenticateAsync();

            // Assert
            Assert.IsTrue(result.None);
        }

        [TestMethod]
        public async Task NoResult_IncorrectHeaderKey()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers.Add("X-Api-Key-Error", "Value");

            await _handler.InitializeAsync(new AuthenticationScheme("API Key",
                    null,
                    typeof(ApiKeyAuthenticationHandler)),
                context);

            // Act
            var result = await _handler.AuthenticateAsync();

            // Assert
            Assert.IsTrue(result.None);
        }
    }
}