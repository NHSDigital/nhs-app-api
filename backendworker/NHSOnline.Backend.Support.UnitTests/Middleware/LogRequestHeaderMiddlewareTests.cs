using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Support.Middleware;

namespace NHSOnline.Backend.Support.UnitTests.Middleware
{
    [TestClass]
    public sealed class LogRequestHeaderMiddlewareTests
    {
        private IFixture _fixture;
        private LogRequestHeaderMiddleware _systemUnderTest;
        private RequestDelegate _next;
        private Mock<ILogger<LogRequestHeaderMiddleware>> _logger;
        private Mock<ILoggerFactory> _loggerFactory;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            
            _next = _ => Task.CompletedTask;

            _logger = _fixture.Freeze<Mock<ILogger<LogRequestHeaderMiddleware>>>();
            
            _loggerFactory = _fixture.Freeze<Mock<ILoggerFactory>>();
            _loggerFactory
                .Setup(x => x.CreateLogger(It.IsAny<string>()))
                .Returns(_logger.Object);
        }
        
        [TestMethod]
        public async Task Invoke_WhenHeaderIsNotPresent_DoesNotAddTheLogMessageToLoggerScope()
        {
            // Arrange
            _logger
                .Setup(x => x.BeginScope(It.IsAny<string>()))
                .Throws<InvalidOperationException>();

            var options = _fixture.Create<OptionsWrapper<LogRequestHeaderOptions>>();
            
            var context = new DefaultHttpContext();

            _systemUnderTest = new LogRequestHeaderMiddleware(_next, _loggerFactory.Object, options);
            // Act
            await _systemUnderTest.Invoke(context);

            // Assert
            context.Response.StatusCode.Should()
                .Be((int)HttpStatusCode.OK);
        }
        
        [TestMethod]
        [DataRow("{value}","Test")]
        [DataRow("CorrelationId:{value}","CorrelationId:Test")]
        [DataRow("CorrelationId:{value}xyz","CorrelationId:Testxyz")]
        public async Task Invoke_TemplateWithOnlyValuePlaceHolder_AddsTheLogMessageToLoggerScope(
            string logTemplate, string expectedValue)
        {
            var headerName = _fixture.Create<string>();
            // Arrange
            _logger
                .Setup(x => x.BeginScope(expectedValue))
                .Returns(_fixture.Create<IDisposable>())
                .Verifiable();
            
            var options = new OptionsWrapper<LogRequestHeaderOptions>(
                new LogRequestHeaderOptions
                {
                    HeaderName = headerName,
                    LogTemplate = logTemplate
                });
            
            var context = new DefaultHttpContext();
            context.Request.Headers.Add(headerName, "Test");

            _systemUnderTest = new LogRequestHeaderMiddleware(_next, _loggerFactory.Object, options);
            
            // Act
            await _systemUnderTest.Invoke(context);

            // Assert
            _logger.Verify();
        }
        
        [TestMethod]
        [DataRow("","")]
        [DataRow("foobar","foobar")]
        public async Task Invoke_TemplateNoPlaceHoldersOrEmpty_AddsTheLogMessageToLoggerScope(
            string logTemplate, string expectedValue)
        {
            var headerName = _fixture.Create<string>();
            var headerValue = _fixture.Create<string>();
            // Arrange
            _logger
                .Setup(x => x.BeginScope(expectedValue))
                .Returns(_fixture.Create<IDisposable>())
                .Verifiable();
            
            var options = new OptionsWrapper<LogRequestHeaderOptions>(
                new LogRequestHeaderOptions
                {
                    HeaderName = headerName,
                    LogTemplate = logTemplate
                });
            
            var context = new DefaultHttpContext();
            context.Request.Headers.Add(headerName, headerValue);

            _systemUnderTest = new LogRequestHeaderMiddleware(_next, _loggerFactory.Object, options);
            // Act
            await _systemUnderTest.Invoke(context);

            // Assert
            _logger.Verify();
        }
        
        [TestMethod]
        public async Task Invoke_TemplateWithNameAndValuePlaceHolders_AddsTheLogMessageToLoggerScope()
        {
            var logTemplate = "{name}={value}";
            var headerName = _fixture.Create<string>();
            var headerValue = _fixture.Create<string>();

            var expectedResult = $"{headerName}={headerValue}";
            // Arrange
            _logger
                .Setup(x => x.BeginScope(expectedResult))
                .Returns(_fixture.Create<IDisposable>())
                .Verifiable();
            
            var options = new OptionsWrapper<LogRequestHeaderOptions>(
                new LogRequestHeaderOptions
                {
                    HeaderName = headerName,
                    LogTemplate = logTemplate
                });
            
            var context = new DefaultHttpContext();
            context.Request.Headers.Add(headerName, headerValue);

            _systemUnderTest = new LogRequestHeaderMiddleware(_next, _loggerFactory.Object, options);
            // Act
            await _systemUnderTest.Invoke(context);

            // Assert
            _logger.Verify();
        }
        
        [TestMethod]
        public void Invoke_TheTemplateContainsMultipleValuePlaceHolders_ItThrowsAnArgumentException()
        {
            // Arrange  
            var options = new OptionsWrapper<LogRequestHeaderOptions>(
                new LogRequestHeaderOptions
                {
                    HeaderName = "foo",
                    LogTemplate = "bar:{value}{value}"
                });
            
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new LogRequestHeaderMiddleware(_next, _loggerFactory.Object, options);
            
            // Act
            act.Should().Throw<ArgumentException>()
                .WithMessage("LogTemplate contains more than one instance of {value}");
        }
        
        [TestMethod]
        public void Invoke_WhenNoOptionsWrapperIsProvided_ItThrowsArgumentException()
        {
            // Arrange  
            OptionsWrapper<LogRequestHeaderOptions> options = null;
            
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new LogRequestHeaderMiddleware(_next, _loggerFactory.Object, options);
            
            // Act
            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("options");
        }
        
        [TestMethod]
        public void Invoke_WhenNoOptionsAreProvided_ItThrowsArgumentException()
        {
            // Arrange  
            var options = new OptionsWrapper<LogRequestHeaderOptions>(
                null);
            
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new LogRequestHeaderMiddleware(_next, _loggerFactory.Object, options);
            
            // Act
            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("options");
        }
    }
}
