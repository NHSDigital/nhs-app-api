using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.Areas.Session;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.Support.Auditing;
using NHSOnline.Backend.Support;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Session
{
    [TestClass]
    public sealed class SessionControllerDeleteTests : IDisposable
    {
        private IFixture _fixture;
        private Mock<IAuditor> _mockAuditor;
        private Mock<ISessionCacheService> _mockSessionCacheService;
        private Mock<ISessionService> _mockSessionService;
        private Mock<IGpSystem> _mockGpSystem;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private SessionController _systemUnderTest;
        private UserSession _userSession;
        private Mock<HttpContext> _httpContextMock;

        private const string DeleteRequestAuditType = "Session_Delete_Request";
        private const string DeleteResponseAuditType = "Session_Delete_Response";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _userSession = _fixture.Create<UserSession>();
            _mockSessionService = _fixture.Freeze<Mock<ISessionService>>();
            _mockSessionCacheService = _fixture.Freeze<Mock<ISessionCacheService>>();
            _mockGpSystemFactory = _fixture.Freeze<Mock<IGpSystemFactory>>();
            _mockGpSystem = _fixture.Freeze<Mock<IGpSystem>>();
            _mockAuditor = _fixture.Freeze<Mock<IAuditor>>();

            var authenticationServiceMock = new Mock<IAuthenticationService>();
            authenticationServiceMock
                .Setup(
                    x => x.SignOutAsync(
                        It.IsAny<HttpContext>(),
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        It.IsAny<AuthenticationProperties>()
                    )
                )
                .Returns(Task.FromResult(true));

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IAuthenticationService)))
                .Returns(authenticationServiceMock.Object);

            _httpContextMock = new Mock<HttpContext>();
            _httpContextMock.Setup(x => x.Items[Constants.HttpContextItems.UserSession]).Returns(_userSession);
            _httpContextMock.SetupGet(h => h.RequestServices).Returns(serviceProviderMock.Object);

            _mockGpSystemFactory
                .Setup(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier))
                .Returns(_mockGpSystem.Object);

            _mockGpSystem
                .Setup(x => x.GetSessionService())
                .Returns(_mockSessionService.Object);

            _mockGpSystem
                .Setup(x => x.GetSessionService())
                .Returns(_mockSessionService.Object);

            _systemUnderTest = _fixture.Create<SessionController>();

            _systemUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = _httpContextMock.Object
            };
        }

        [TestMethod]
        public async Task Delete_DeletingSessionThrowsException_Returns500InternalServiceError()
        {
            // Arrange
            _systemUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = _httpContextMock.Object
            };

            _mockSessionCacheService
                .Setup(x => x.DeleteUserSession(It.IsAny<string>()))
                .Throws<Exception>();

            // Act
            var result = await _systemUnderTest.Delete();

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Delete_DeletingSessionSucceeds_Returns204NoContent()
        {
            // Arrange            
            var sessionLogoffResult = new SessionLogoffResult.Success(_userSession.GpUserSession);

            _mockSessionService
                .Setup(x => x.Logoff(_userSession.GpUserSession))
                .ReturnsAsync(sessionLogoffResult)
                .Verifiable();

            //Act
            var result = await _systemUnderTest.Delete();

            // Assert
            _mockSessionService.Verify();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
            _mockSessionService.Verify(x => x.Logoff(_userSession.GpUserSession));
            _mockSessionCacheService.Verify(x => x.DeleteUserSession(_userSession.Key));
            _mockAuditor.Verify(x => x.Audit(DeleteRequestAuditType, It.IsAny<string>(), It.IsAny<object[]>()));
            _mockAuditor.Verify(x => x.AuditWithExplicitNhsNumber(It.IsAny<string>(), It.IsAny<Supplier>(), DeleteResponseAuditType, It.IsAny<string>()));
        }

        [TestMethod]
        public async Task Delete_GpSupplierSessionLogoffFails_SessionDeletionContinuesAndReturns204NoContent()
        {
            // Arrange            
            var sessionLogoffResult = new SessionLogoffResult.BadGateway();

            _mockSessionService
                .Setup(x => x.Logoff(_userSession.GpUserSession))
                .ReturnsAsync(sessionLogoffResult)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Delete();

            // Assert
            _mockSessionService.Verify();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
            _mockSessionService.Verify(x => x.Logoff(_userSession.GpUserSession));
            _mockAuditor.Verify(x => x.Audit(DeleteRequestAuditType, It.IsAny<string>(), It.IsAny<object[]>()));
            _mockAuditor.Verify(x => x.AuditWithExplicitNhsNumber(It.IsAny<string>(), It.IsAny<Supplier>(), DeleteResponseAuditType, It.IsAny<string>()));
        }

        public void Dispose()
        {
            _systemUnderTest.Dispose();
        }
    }
}