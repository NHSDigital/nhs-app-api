using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.ApiSupport.Filters;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.ApiSupport.UnitTests.Filters
{
    [TestClass]
    public class UnauthorisedGpSystemHttpRequestExceptionFilterAttributeTests
    {
        private IFixture _fixture;
        private UnauthorisedGpSystemHttpRequestExceptionFilterAttributeAttribute _sut;
        private Mock<ILogger<UnauthorisedGpSystemHttpRequestExceptionFilterAttributeAttribute>> _logger;

        [TestInitialize]
        public void Setup()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _logger = _fixture.Freeze<Mock<ILogger<UnauthorisedGpSystemHttpRequestExceptionFilterAttributeAttribute>>>();
            _sut = new UnauthorisedGpSystemHttpRequestExceptionFilterAttributeAttribute(_logger.Object);
        }

        [TestMethod]
        public void OnException_CatchesUnauthorisedHttpResponseExceptionFilter_AndSetsResultTo401()
        {
            // Arrange
            var actionContext = new ActionContext
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new Microsoft.AspNetCore.Routing.RouteData(),
                ActionDescriptor = new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor(),
            };

            var exceptionContext = new ExceptionContext(actionContext, new List<IFilterMetadata>())
            {
                Exception = new UnauthorisedGpSystemHttpRequestException(),
            };

            // Act
            _sut.OnException(exceptionContext);

            // Assert
            exceptionContext.Result.Should().NotBeNull();
            var result = exceptionContext.Result.Should().BeOfType<StatusCodeResult>().Subject;
            result.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        }

        [TestMethod]
        public void OnException_DoesNotCatchOtherExceptionType_ResultIsNotSet()
        {
            // Arrange
            var actionContext = new ActionContext
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new Microsoft.AspNetCore.Routing.RouteData(),
                ActionDescriptor = new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor(),
            };

            var exceptionContext = new ExceptionContext(actionContext, new List<IFilterMetadata>())
            {
                Exception = new InvalidOperationException(),
            };

            // Act
            _sut.OnException(exceptionContext);

            // Assert
            exceptionContext.Result.Should().BeNull();
        }
    }
}