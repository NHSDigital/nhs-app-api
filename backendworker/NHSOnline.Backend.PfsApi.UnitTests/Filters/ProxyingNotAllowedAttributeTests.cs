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

namespace NHSOnline.Backend.PfsApi.UnitTests.Filters
{
    [TestClass]
    public class ProxyingNotAllowedAttributeTests
    {
        private ProxyingNotAllowedAttribute _systemUnderTest;
        private AuthorizationFilterContext _context;
        private P9UserSession _userSession;
        private HttpContext _httpContext;

        [TestInitialize]
        public void TestInitialize()
        {
            _systemUnderTest = new ProxyingNotAllowedAttribute();

            _userSession = new P9UserSession()
            {
                GpUserSession = new EmisUserSession()
                {
                    OdsCode = "X10000",
                    Id = Guid.NewGuid(),
                }
            };

            // Setup the Mocks
            var mockLogger = new Mock<ILogger<ProxyingNotAllowedAttribute>>();
            var mockServiceProvider = new Mock<IServiceProvider>();

            _httpContext = new DefaultHttpContext();
            _httpContext.RequestServices = mockServiceProvider.Object;
            _httpContext.Items.Add(Constants.HttpContextItems.UserSession, _userSession);

            mockServiceProvider
                .Setup(x => x.GetService(typeof(ILogger<ProxyingNotAllowedAttribute>)))
                .Returns(mockLogger.Object);

            var actionContext = new ActionContext()
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
            _httpContext.Request.Headers.Add(Constants.HttpHeaders.PatientId, _userSession.GpUserSession.Id.ToString());

            // Act
            _systemUnderTest.OnAuthorization(_context);

            // Assert
            _context.Result.Should().BeNull();
        }

        [TestMethod]
        public void OnAuthorizationAsync_WhenHeaderDoesNotMatchIdInSession_ReturnsError()
        {
            // Arrange
            _httpContext.Request.Headers.Add(Constants.HttpHeaders.PatientId, Guid.NewGuid().ToString());

            // Act
            _systemUnderTest.OnAuthorization(_context);

            // Assert
            _context.Result.Should().NotBeNull();
            var result = _context.Result.Should().BeOfType<StatusCodeResult>().Subject;
            result.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
        }
    }
}
