using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.PfsApi.Filters;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.UnitTests.Filters
{
    [TestClass]
    public class ProxyingNotAllowedAttributeTests
    {
        private ProxyingNotAllowedAttribute _systemUnderTest;
        private AuthorizationFilterContext _context;
        private HttpContext _httpContext;
        private Mock<IUserSessionService> _mockUserSessionService;

        [TestInitialize]
        public void TestInitialize()
        {
            _systemUnderTest = new ProxyingNotAllowedAttribute();

            // Setup the Mocks
            var mockLogger = new Mock<ILogger<ProxyingNotAllowedAttribute>>();
            _mockUserSessionService = new Mock<IUserSessionService>();
            var mockServiceProvider = new Mock<IServiceProvider>();

            _httpContext = new DefaultHttpContext { RequestServices = mockServiceProvider.Object };

            mockServiceProvider
                .Setup(x => x.GetService(typeof(ILogger<ProxyingNotAllowedAttribute>)))
                .Returns(mockLogger.Object);

            mockServiceProvider
                .Setup(x => x.GetService(typeof(IUserSessionService)))
                .Returns(_mockUserSessionService.Object);

            var actionContext = new ActionContext
            {
                HttpContext = _httpContext,
                RouteData = new RouteData(),
                ActionDescriptor = new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor(),
            };

            _context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
        }

        [TestMethod]
        public void OnAuthorizationAsync_WhenHeaderMatchesIdInSession_ContinuesWithoutError()
        {
            // Arrange
            var userSession = ArrangeP9UserSession();
            _httpContext.Request.Headers.Add(Constants.HttpHeaders.PatientId, userSession.GpUserSession.Id.ToString());

            // Act
            _systemUnderTest.OnAuthorization(_context);

            // Assert
            _context.Result.Should().BeNull();
        }

        [TestMethod]
        public void OnAuthorizationAsync_WhenHeaderDoesNotMatchIdInSession_ReturnsError()
        {
            // Arrange
            ArrangeP9UserSession();
            _httpContext.Request.Headers.Add(Constants.HttpHeaders.PatientId, Guid.NewGuid().ToString());

            // Act
            _systemUnderTest.OnAuthorization(_context);

            // Assert
            _context.Result.Should().NotBeNull();
            var result = _context.Result.Should().BeOfType<StatusCodeResult>().Subject;
            result.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
        }

        [TestMethod]
        public void OnAuthorizationAsync_WhenP5UserSession_ContinuesWithoutError()
        {
            // Arrange
            ArrangeP5UserSession();
            _httpContext.Request.Headers.Add(Constants.HttpHeaders.PatientId, Guid.NewGuid().ToString());

            // Act
            _systemUnderTest.OnAuthorization(_context);

            // Assert
            _context.Result.Should().BeNull();
        }

        private P9UserSession ArrangeP9UserSession()
        {
            var userSession = new P9UserSession(
                string.Empty,
                new CitizenIdUserSession(),
                new EmisUserSession
                {
                    OdsCode = "X10000",
                    Id = Guid.NewGuid(),
                },
                string.Empty);

            _mockUserSessionService
                .Setup(x => x.GetUserSession<P9UserSession>())
                .Returns(Option.Some(userSession));

            return userSession;
        }

        private void ArrangeP5UserSession()
        {
            _mockUserSessionService
                .Setup(x => x.GetUserSession<P9UserSession>())
                .Returns(Option.None<P9UserSession>());
        }
    }
}
