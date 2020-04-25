using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.Areas.Session;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.PfsApi.CitizenId;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules;
using NHSOnline.Backend.PfsApi.Session;
using NHSOnline.Backend.PfsApi.UserInfo;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Session
{
    [TestClass]
    public sealed class SessionControllerDeleteTests : IDisposable
    {
        private Mock<IAuditor> _mockAuditor;
        private SessionController _systemUnderTest;
        private P9UserSession _userSession;
        private Mock<HttpContext> _httpContextMock;
        private Mock<IGpSessionManager> _mockGpSessionManager;

        private const string DeleteRequestAuditType = "Session_Delete_Request";
        private const string DeleteResponseAuditType = "Session_Delete_Response";

        [TestInitialize]
        public void TestInitialize()
        {
            _userSession = new P9UserSession("csrfToken", new CitizenIdUserSession(), new EmisUserSession(), "im1token");
            _mockAuditor = new Mock<IAuditor>();

             var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IAuthenticationService)))
                .Returns(new Mock<IAuthenticationService>().Object);

            _httpContextMock = new Mock<HttpContext>();
            _httpContextMock
                .SetupGet(h => h.RequestServices)
                .Returns(serviceProviderMock.Object);

            _mockGpSessionManager = new Mock<IGpSessionManager>();

            _systemUnderTest = new SessionController(
                new Mock<ICitizenIdSessionService>().Object,
                new UserSessionService(),
                new ConfigurationSettings(),
                new Mock<ILogger<SessionController>>().Object,
                _mockAuditor.Object,
                new Mock<IOdsCodeMassager>().Object,
                new Mock<IServiceJourneyRulesService>().Object,
                new Mock<IErrorReferenceGenerator>().Object,
                new Mock<IUserInfoService>().Object,
                _mockGpSessionManager.Object,
                new Mock<IAntiforgery>().Object,
                new Mock<IUserSessionManager>().Object)
            {
                ControllerContext = new ControllerContext { HttpContext = _httpContextMock.Object }
            };
        }

        [TestMethod]
        public async Task Delete_DeletingSessionThrowsException_Returns500InternalServiceError()
        {
            // Arrange
            _mockGpSessionManager
                .Setup(x => x.CloseAndDeleteSession(_userSession))
                .ReturnsAsync(new CloseSessionResult.Failure());

            _systemUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = _httpContextMock.Object
            };

            // Act
            var result = await _systemUnderTest.Delete(_userSession);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Delete_DeletingSessionSucceeds_Returns204NoContent()
        {
            // Arrange
            _mockGpSessionManager
                .Setup(x => x.CloseAndDeleteSession(_userSession))
                .ReturnsAsync(new CloseSessionResult.Success());

            // Act
            var result = await _systemUnderTest.Delete(_userSession);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status204NoContent);

            _mockAuditor.Verify(x => x.Audit(DeleteRequestAuditType, It.IsAny<string>(), It.IsAny<object[]>()));
            _mockAuditor.Verify(x => x.AuditSessionEvent(
                _userSession.CitizenIdUserSession.AccessToken,
                _userSession.GpUserSession.NhsNumber,
                _userSession.GpUserSession.Supplier,
                DeleteResponseAuditType,
                It.IsAny<string>()));
        }

        public void Dispose()
        {
            _systemUnderTest.Dispose();
        }
    }
}