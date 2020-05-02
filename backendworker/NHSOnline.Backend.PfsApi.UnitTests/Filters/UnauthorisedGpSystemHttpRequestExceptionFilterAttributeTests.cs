using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.PfsApi.Filters;
using NHSOnline.Backend.PfsApi.Session;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.UnitTests.Filters
{
    [TestClass]
    public class UnauthorisedGpSystemHttpRequestExceptionFilterAttributeTests
    {
        private UnauthorisedGpSystemHttpRequestExceptionFilterAttribute _systemUnderTest;
        private Mock<HttpContext> _mockHttpContext;
        private Mock<IUserSessionManager> _userSessionManager;
        private Mock<IUserSessionService> _userSessionService;

        [TestInitialize]
        public void TestInitialize()
        {
            var logger = new Mock<ILogger<UnauthorisedGpSystemHttpRequestExceptionFilterAttribute>>();
            
            _userSessionManager = new Mock<IUserSessionManager>();
            _userSessionService = new Mock<IUserSessionService>();
            _systemUnderTest = new UnauthorisedGpSystemHttpRequestExceptionFilterAttribute(_userSessionService.Object, _userSessionManager.Object, logger.Object);
            
            _mockHttpContext = new Mock<HttpContext>();
        }

        [TestMethod]
        public async Task OnExceptionAsync_UnauthorisedHttpResponseExceptionP9UserSession_CallUserSessionManagerDelete()
        {
            // Arrange
            var userSession = new P9UserSession(string.Empty, new CitizenIdUserSession(), new EmisUserSession(), string.Empty);
            ArrangeUserSession(userSession);
            var exceptionContext = CreateExceptionContext<UnauthorisedGpSystemHttpRequestException>(exceptionHandled: false);

            // Act
            await _systemUnderTest.OnExceptionAsync(exceptionContext);

            // Assert
            _userSessionManager.Verify(x => x.Delete(_mockHttpContext.Object, userSession));
        }

        [TestMethod]
        public async Task OnExceptionAsync_UnauthorisedHttpResponseExceptionP9UserSession_SetsResultTo401()
        {
            // Arrange
            var userSession = new P9UserSession(string.Empty, new CitizenIdUserSession(), new EmisUserSession(), string.Empty);
            ArrangeUserSession(userSession);
            var exceptionContext = CreateExceptionContext<UnauthorisedGpSystemHttpRequestException>(exceptionHandled: false);

            // Act
            await _systemUnderTest.OnExceptionAsync(exceptionContext);

            // Assert
            exceptionContext.Result.Should().NotBeNull()
                .And.BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        }

        [TestMethod]
        public async Task OnExceptionAsync_UnauthorisedHttpResponseExceptionP5UserSession_CallsUserSessionManagerDelete()
        {
            // Arrange
            var userSession = new P5UserSession(string.Empty, new CitizenIdUserSession());
            ArrangeUserSession(userSession);
            var exceptionContext = CreateExceptionContext<UnauthorisedGpSystemHttpRequestException>(exceptionHandled: false);

            // Act
            await _systemUnderTest.OnExceptionAsync(exceptionContext);

            // Assert
            _userSessionManager.Verify(x => x.Delete(_mockHttpContext.Object, userSession));
        }

        [TestMethod]
        public async Task OnExceptionAsync_UnauthorisedHttpResponseExceptionP5UserSession_SetsResultTo401()
        {
            // Arrange
            var userSession = new P5UserSession(string.Empty, new CitizenIdUserSession());
            ArrangeUserSession(userSession);
            var exceptionContext = CreateExceptionContext<UnauthorisedGpSystemHttpRequestException>(exceptionHandled: false);

            // Act
            await _systemUnderTest.OnExceptionAsync(exceptionContext);

            // Assert
            exceptionContext.Result.Should().NotBeNull()
                .And.BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        }

        [TestMethod]
        public async Task OnExceptionAsync_UnauthorisedHttpResponseExceptionNoUserSession_DoesNotCallUserSessionManagerDelete()
        {
            // Arrange
            ArrangeUserSession(Option.None<UserSession>());
            var exceptionContext = CreateExceptionContext<UnauthorisedGpSystemHttpRequestException>(exceptionHandled: false);

            // Act
            await _systemUnderTest.OnExceptionAsync(exceptionContext);

            // Assert
            _userSessionManager.Verify(
                x => x.Delete(_mockHttpContext.Object, It.IsAny<UserSession>()),
                Times.Never);
        }

        [TestMethod]
        public async Task OnExceptionAsync_UnauthorisedHttpResponseExceptionNoUserSession_SetsResultTo401()
        {
            // Arrange
            ArrangeUserSession(Option.None<UserSession>());
            var exceptionContext = CreateExceptionContext<UnauthorisedGpSystemHttpRequestException>(exceptionHandled: false);

            // Act
            await _systemUnderTest.OnExceptionAsync(exceptionContext);

            // Assert
            exceptionContext.Result.Should().NotBeNull()
                .And.BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        }

        [TestMethod]
        public async Task OnExceptionAsync_DoesNotCatchOtherExceptionType_ResultIsNotSet()
        {
            // Arrange
            var exceptionContext = CreateExceptionContext<InvalidOperationException>(exceptionHandled: false);

            // Act
            await _systemUnderTest.OnExceptionAsync(exceptionContext);

            // Assert
            exceptionContext.Result.Should().BeNull();
        }
        
        [TestMethod]
        public async Task OnException_ExceptionAlreadyHandled_ResultUnchanged()
        {
            // Arrange
            var exceptionContext = CreateExceptionContext<UnauthorisedGpSystemHttpRequestException>(
                exceptionHandled: true,
                result: new StatusCodeResult(StatusCodes.Status418ImATeapot));
            
            // Act
            await _systemUnderTest.OnExceptionAsync(exceptionContext);
            
            // Assert
            exceptionContext.Result.Should().BeOfType<StatusCodeResult>().Subject
                .StatusCode.Should().Be(StatusCodes.Status418ImATeapot);
            exceptionContext.ExceptionHandled.Should().BeTrue();
        }

        private ExceptionContext CreateExceptionContext<TException>(
            IActionResult result = null,
            bool? exceptionHandled = null)
            where TException : Exception, new()
        {
            var actionContext = new ActionContext
            {
                HttpContext = _mockHttpContext.Object,
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor()
            };
            return new ExceptionContext(actionContext, new List<IFilterMetadata>())
            {
                Exception = new TException(),
                ExceptionHandled = exceptionHandled ?? false,
                Result = result
            };
        }

        private void ArrangeUserSession(UserSession userSession) => ArrangeUserSession(Option.Some(userSession));

        private void ArrangeUserSession(Option<UserSession> userSession)
        {
            _userSessionService.Setup(x => x.GetUserSession<UserSession>()).Returns(userSession);
        }
    }
}
