using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.CidApi.Filters;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.CidApi.UnitTests.Filters
{
    [TestClass]
    public class UnauthorisedGpSystemHttpRequestExceptionFilterAttributeTests
    {
        private UnauthorisedGpSystemHttpRequestExceptionFilterAttribute _sut;
        private Mock<HttpContext> _mockHttpContext;
        private ActionContext _actionContext;

        [TestInitialize]
        public void Setup()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var logger = fixture.Freeze<Mock<ILogger<UnauthorisedGpSystemHttpRequestExceptionFilterAttribute>>>();
            
            _sut = new UnauthorisedGpSystemHttpRequestExceptionFilterAttribute(logger.Object);
            
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
        public async Task OnExceptionAsync_CatchesUnauthorisedHttpResponseException_AndSetsResultTo401()
        {
            // Arrange
            var exceptionContext = new ExceptionContext(_actionContext, new List<IFilterMetadata>())
            {
                Exception = new UnauthorisedGpSystemHttpRequestException(),
            };

            // Act
            await _sut.OnExceptionAsync(exceptionContext);

            // Assert
            exceptionContext.Result.Should().NotBeNull();
            var result = exceptionContext.Result.Should().BeOfType<StatusCodeResult>().Subject;
            result.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        }

        [TestMethod]
        public async Task OnExceptionAsync_DoesNotCatchOtherExceptionType_ResultIsNotSet()
        {
            // Arrange
            var exceptionContext = new ExceptionContext(_actionContext, new List<IFilterMetadata>())
            {
                Exception = new InvalidOperationException(),
            };

            // Act
            await _sut.OnExceptionAsync(exceptionContext);

            // Assert
            exceptionContext.Result.Should().BeNull();
        }
    }
}
