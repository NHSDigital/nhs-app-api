using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Support.AspNet.Filters;
using UnitTestHelper;

namespace NHSOnline.Backend.Support.UnitTests.AspNet.Filters
{
    [TestClass]
    public class UnhandledExceptionFilterAttributeTests
    {
        private UnhandledExceptionFilterAttribute _systemUnderTest;
        
        private Mock<HttpContext> _mockHttpContext;
        private ActionContext _actionContext;
        private Mock<IErrorReferenceGenerator> _mockServiceDeskErrorReferenceGenerator;
        private Mock<ILogger<UnhandledExceptionFilterAttribute>> _mockLogger;
        private IFixture _fixture;

        [TestInitialize]
        public void TestInitialise()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            
            _mockLogger = _fixture.Freeze<Mock<ILogger<UnhandledExceptionFilterAttribute>>>();
            _mockServiceDeskErrorReferenceGenerator = _fixture.Freeze<Mock<IErrorReferenceGenerator>>();

            _systemUnderTest = _fixture.Create<UnhandledExceptionFilterAttribute>();
            
            var mockAuthenticationService = new Mock<IAuthenticationService>();
            var mockServiceProvider = new Mock<IServiceProvider>();
            
            _mockHttpContext = new Mock<HttpContext>();

            mockServiceProvider
                .Setup(x => x.GetService(typeof(IAuthenticationService)))
                .Returns(mockAuthenticationService.Object);
            _mockHttpContext.SetupGet(x => x.RequestServices).Returns(mockServiceProvider.Object);
            _actionContext = new ActionContext
            {
                HttpContext = _mockHttpContext.Object,
                RouteData = new Microsoft.AspNetCore.Routing.RouteData(),
                ActionDescriptor = new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor(),
            };
        }

        [TestMethod]
        public void OnException_LogsAndReturnsExpectedObjectResult()
        {
            // Arrange
            var exception = new DivideByZeroException(_fixture.Create<string>());
            var exceptionContext = new ExceptionContext(_actionContext, new List<IFilterMetadata>())
            {
                Exception = exception,
                HttpContext = _mockHttpContext.Object
            };
            
            var serviceDeskReference = _fixture.Create<string>();

            _mockServiceDeskErrorReferenceGenerator.Setup(x =>
                x.GenerateAndLogErrorReference(It.IsAny<ErrorTypes.UnhandledError>()))
                .Returns(serviceDeskReference);

            var expectedResponse = new PfsErrorResponse
            {
                ServiceDeskReference = serviceDeskReference
            };

            // Act
            _systemUnderTest.OnException(exceptionContext);

            // Assert
            using (new AssertionScope())
            {
                var objectResult = exceptionContext.Result.Should().BeOfType<ObjectResult>().Subject;
                objectResult.Value.Should().BeEquivalentTo(expectedResponse);
                objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
                exceptionContext.ExceptionHandled.Should().BeTrue();
                _mockLogger.VerifyLogger(LogLevel.Error, "Unhandled Exception", 
                    It.Is<DivideByZeroException>(x => x.Message.Equals(exception.Message, StringComparison.OrdinalIgnoreCase)), 
                    Times.Once());
            }
        }

        [TestMethod]
        public void OnException_ExceptionAlreadyHandled_ResultUnchanged()
        {
            // Arrange
            var exceptionContext = new ExceptionContext(_actionContext, new List<IFilterMetadata>())
            {
                Exception = new DivideByZeroException(),
                ExceptionHandled = true,
                Result = new StatusCodeResult(StatusCodes.Status418ImATeapot)
            };
            
            // Act
            _systemUnderTest.OnException(exceptionContext);
            
            // Assert
            exceptionContext.Result.Should().BeOfType<StatusCodeResult>().Subject
                .StatusCode.Should().Be(StatusCodes.Status418ImATeapot);
            exceptionContext.ExceptionHandled.Should().BeTrue();
        }
    }
}