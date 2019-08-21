using System;
using System.Collections.Generic;
using System.Net.Http;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Support.AspNet.Filters;
using UnitTestHelper;

namespace NHSOnline.Backend.Support.UnitTests.AspNet.Filters
{
    [TestClass]
    public class TimeoutExceptionFilterAttributeTests
    {
        private readonly Mock<ILogger<TimeoutExceptionFilterAttribute>> _mockLogger =
            new Mock<ILogger<TimeoutExceptionFilterAttribute>>();

        private TimeoutExceptionFilterAttribute _systemUnderTest;
        private Mock<List<IFilterMetadata>> _mockFilterMetadata;
        private ExceptionContext _httpActionExecutedContext;
        private Mock<HttpContext> _mockHttpContext;
        private ActionContext _actionContext;
        private IFixture _fixture;
        private Mock<IErrorReferenceGenerator> _mockServiceDeskErrorReferenceGenerator;
        private NhsTimeoutException _nhsTimeoutException;
        private string _serviceDeskReference;

        [TestInitialize]
        public void TestInitialise()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());
            _mockServiceDeskErrorReferenceGenerator = new Mock<IErrorReferenceGenerator>();
            _systemUnderTest =
                new TimeoutExceptionFilterAttribute(_mockLogger.Object, _mockServiceDeskErrorReferenceGenerator.Object);
            _mockFilterMetadata = new Mock<List<IFilterMetadata>>();
            _mockHttpContext = new Mock<HttpContext>();
            _actionContext = new ActionContext()
            {
                HttpContext = new Mock<HttpContext>().Object,
                RouteData = new Mock<RouteData>().Object,
                ActionDescriptor = new Mock<ActionDescriptor>().Object
            };

            _httpActionExecutedContext = new ExceptionContext(_actionContext, _mockFilterMetadata.Object)
            {
                Exception = new TimeoutException(),
                HttpContext = _mockHttpContext.Object
            };

            _nhsTimeoutException = new NhsTimeoutException()
            {
                SourceApi = _fixture.Create<SourceApi>()
            };

            _serviceDeskReference = _fixture.Create<string>();
        }

        [TestMethod]
        public void OnException_TimeoutExceptionCaught_Returns_504GatewayTimeout()
        {
            //Arrange
            _httpActionExecutedContext = new ExceptionContext(_actionContext, _mockFilterMetadata.Object)
            {
                Exception = new NhsTimeoutException(),
                HttpContext = _mockHttpContext.Object
            };
            
            _mockServiceDeskErrorReferenceGenerator.Setup(x =>
                x.GenerateAndLogErrorReference(ErrorCategory.Timeout,
                    StatusCodes.Status504GatewayTimeout, _nhsTimeoutException.SourceApi)).Returns(_serviceDeskReference);

            var expected = new PfsErrorResponse
            {
                ServiceDeskReference = _serviceDeskReference,
            };

            //Act
            _systemUnderTest.OnException(_httpActionExecutedContext);

            //Assert
            var objectResult = _httpActionExecutedContext.Result.Should().BeAssignableTo<ObjectResult>().Subject;
            objectResult.Value.Should().BeEquivalentTo(expected);
            _mockLogger.VerifyLogger(LogLevel.Error,
                $"Operation timed out - exception: {_httpActionExecutedContext.Exception}", Times.Once());
            _mockLogger.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void OnException_Non_TimeoutException_Returns_Null()
        {
            //Arrange
            _httpActionExecutedContext = new ExceptionContext(_actionContext, _mockFilterMetadata.Object)
            {
                Exception = new HttpRequestException(),
                HttpContext = _mockHttpContext.Object
            };

            //Act
            _systemUnderTest.OnException(_httpActionExecutedContext);

            //Assert
            _httpActionExecutedContext.Result.Should().Be(null);
            _mockLogger.VerifyLogger(LogLevel.Error,
                $"Operation timed out - exception: {_httpActionExecutedContext.Exception}", Times.Never());
        }
    }
}