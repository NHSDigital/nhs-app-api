using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Jose;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.PfsApi.Areas.Session;
using NHSOnline.Backend.PfsApi.Filters;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.UnitTests.Filters
{
    [TestClass]
    public class UnauthorisedGpSystemHttpRequestExceptionFilterAttributeTests
    {
        private P9UserSession _p9Session;
        private GpUserSession _gpSession;

        private IUserSessionService _sessionService;
        private ISessionCacheService _sessionCacheService;
        private IErrorReferenceGenerator _errorReferenceGenerator;

        private HttpContext _mockHttpContext;
        private ActionContext _actionContext;
        private ExceptionContext _exceptionContext;
        private IActionResult _gpSessionUnavailableResult;

        private UnauthorisedGpSystemHttpRequestExceptionFilterAttribute _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            var logger = Mock.Of<ILogger<UnauthorisedGpSystemHttpRequestExceptionFilterAttribute>>();

            _gpSession = Mock.Of<GpUserSession>();
            _p9Session = new P9UserSession(
                "csrf",
                "1234567890",
                new CitizenIdUserSession
                {
                    OdsCode = "A12345"
                },
                _gpSession,
                "con_token");

            Mock.Get(_gpSession)
                .Setup(gp => gp.Supplier)
                .Returns(Supplier.Emis);

            _sessionService = Mock.Of<IUserSessionService>();
            _sessionCacheService = Mock.Of<ISessionCacheService>();
            _errorReferenceGenerator = Mock.Of<IErrorReferenceGenerator>();

            _gpSessionUnavailableResult = Mock.Of<IActionResult>();

            var sessionErrorResultBuilder = Mock.Of<ISessionErrorResultBuilder>();

            Mock.Get(sessionErrorResultBuilder)
                .Setup(b => b.BuildResult(It.IsAny<ErrorTypes.GPSessionUnavailable>()))
                .Returns(_gpSessionUnavailableResult);

            Mock.Get(_sessionService)
                .Setup(s => s.GetUserSession<UserSession>())
                .Returns(Option.Some<UserSession>(_p9Session));

            _mockHttpContext = Mock.Of<HttpContext>();

            _actionContext = new ActionContext
            {
                HttpContext = _mockHttpContext,
                RouteData = new Microsoft.AspNetCore.Routing.RouteData(),
                ActionDescriptor = new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor(),
            };

            _exceptionContext = new ExceptionContext(_actionContext, new List<IFilterMetadata>())
            {
                Exception = new UnauthorisedGpSystemHttpRequestException(),
                ExceptionHandled = false
            };

            _systemUnderTest = new UnauthorisedGpSystemHttpRequestExceptionFilterAttribute(
                logger,
                _sessionService,
                _sessionCacheService,
                _errorReferenceGenerator,
                sessionErrorResultBuilder);
        }

        [TestMethod]
        public async Task OnExceptionAsync_UnauthorisedHttpResponseExceptionP9UserSession_SetsResultToGpSessionUnavailable()
        {
            await _systemUnderTest.OnExceptionAsync(_exceptionContext);

            Assert.AreEqual(_gpSessionUnavailableResult, _exceptionContext.Result);
        }

        [TestMethod]
        public async Task OnExceptionAsync_UnauthorisedHttpResponseExceptionP5UserSession_SetsResultTo401()
        {
            Mock.Get(_sessionService)
                .Setup(s => s.GetUserSession<UserSession>())
                .Returns(
                    Option.Some<UserSession>(new P5UserSession(string.Empty, new CitizenIdUserSession())));

            await _systemUnderTest.OnExceptionAsync(_exceptionContext);

            _exceptionContext.Result.Should().NotBeNull()
                .And.BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        }

        [TestMethod]
        public async Task OnExceptionAsync_UnauthorisedHttpResponseExceptionNoUserSession_SetsResultTo401()
        {
            Mock.Get(_sessionService)
                .Setup(s => s.GetUserSession<UserSession>())
                .Returns(Option.None<UserSession>());

            await _systemUnderTest.OnExceptionAsync(_exceptionContext);

            _exceptionContext.Result
                .Should()
                .NotBeNull()
                .And
                .BeOfType<StatusCodeResult>()
                .Subject
                .StatusCode
                .Should()
                .Be(StatusCodes.Status401Unauthorized);
        }

        [TestMethod]
        public async Task OnExceptionAsync_DoesNotCatchOtherExceptionType_ResultIsNotSet()
        {
            _exceptionContext.Exception = new InvalidAlgorithmException("oops");

            await _systemUnderTest.OnExceptionAsync(_exceptionContext);

            _exceptionContext.Result
                .Should()
                .BeNull();
        }

        [TestMethod]
        public async Task OnException_ExceptionAlreadyHandled_ResultUnchanged()
        {
            _exceptionContext.Exception = new InvalidCastException("bad");
            _exceptionContext.ExceptionHandled = true;
            _exceptionContext.Result = new StatusCodeResult(StatusCodes.Status418ImATeapot);

            await _systemUnderTest.OnExceptionAsync(_exceptionContext);

            _exceptionContext.Result
                .Should()
                .BeOfType<StatusCodeResult>()
                .Subject
                .StatusCode
                .Should()
                .Be(StatusCodes.Status418ImATeapot);

            _exceptionContext.ExceptionHandled
                .Should()
                .BeTrue();
        }

        [TestMethod]
        public async Task OnExceptionAsync_UnauthorisedHttpResponseExceptionP9UserSession_ClearsDownGpUserSession()
        {
            await _systemUnderTest.OnExceptionAsync(_exceptionContext);

            Assert.AreEqual(typeof(NullGpSession), _p9Session.GpUserSession.GetType());
            Mock.Get(_sessionCacheService)
                .Verify(s => s.UpdateUserSession(_p9Session));
        }
    }
}
