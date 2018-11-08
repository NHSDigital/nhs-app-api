using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.Session;
using NHSOnline.Backend.Worker.CitizenId;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.GpSystems.Session;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.Worker.Settings;
using NHSOnline.Backend.Worker.Support.Auditing;
using NHSOnline.Backend.Worker.Support.Session;

namespace NHSOnline.Backend.Worker.UnitTests.Areas.Session
{
    [TestClass]
    public sealed class SessionControllerDeleteTests : IDisposable
    {
        private IFixture _fixture;
        private Mock<ICitizenIdService> _mockCitizenIdService;
        private Mock<IOdsCodeLookup> _mockOdsCodeLookup;
        private Mock<IOptions<ConfigurationSettings>> _configurationSettings;
        private Mock<ILogger<SessionController>> _mockLogger;
        private Mock<IAntiforgery> _mockAntiforgery;
        private Mock<IAuditor> _mockAuditor;
        private Mock<ISessionCacheService> _mockSessionCacheService;
        private Mock<ISessionService> _mockSessionService;
        private Mock<IGpSystem> _mockGpSystem;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private Mock<IMinimumAgeValidator> _mockMinimumAgeValidator;
        
        private SessionController _systemUnderTest;
        private UserSession _tppUserSession;
        private Mock<HttpContext> _httpContextMock;
        
        private const string DeleteRequestAuditType = "Session_Delete_Request";
        private const string DeleteResponseAuditType = "Session_Delete_Response";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());
            
            _tppUserSession = _fixture.Create<TppUserSession>();
            _mockSessionService = new Mock<ISessionService>();
            _mockSessionCacheService = _fixture.Freeze<Mock<ISessionCacheService>>();
            _mockGpSystemFactory = _fixture.Freeze<Mock<IGpSystemFactory>>();
            _mockGpSystem = new Mock<IGpSystem>();
            _mockCitizenIdService = _fixture.Freeze<Mock<ICitizenIdService>>();
            _mockOdsCodeLookup = new Mock<IOdsCodeLookup>();
            _configurationSettings = _fixture.Freeze<Mock<IOptions<ConfigurationSettings>>>();
            _mockLogger = new Mock<ILogger<SessionController>>();
            _mockAuditor = new Mock<IAuditor>();
            _mockAntiforgery = _fixture.Freeze<Mock<IAntiforgery>>();
            _mockMinimumAgeValidator = new Mock<IMinimumAgeValidator>();

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
            _httpContextMock.Setup(x => x.Items[Constants.HttpContextItems.UserSession]).Returns(_tppUserSession);
            _httpContextMock.SetupGet(h => h.RequestServices).Returns(serviceProviderMock.Object);
            
            _mockGpSystemFactory
                .Setup(x => x.CreateGpSystem(_tppUserSession.Supplier))
                .Returns(_mockGpSystem.Object);

            _mockGpSystem
                .Setup(x => x.GetSessionService())
                .Returns(_mockSessionService.Object);
            
            _mockGpSystem
                .Setup(x => x.GetSessionService())
                .Returns(_mockSessionService.Object);

            _systemUnderTest = new SessionController(
                _mockCitizenIdService.Object,
                _mockGpSystemFactory.Object,
                _mockSessionCacheService.Object,
                _mockOdsCodeLookup.Object,
                _configurationSettings.Object,
                _mockLogger.Object,
                _mockAuditor.Object,
                _mockAntiforgery.Object,
                _mockMinimumAgeValidator.Object
            )
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContextMock.Object
                }
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
            var sessionLogoffResult = new SessionLogoffResult.SuccessfullyDeleted(_tppUserSession);

            _mockSessionService
                .Setup(x => x.Logoff(_tppUserSession))
                .ReturnsAsync(sessionLogoffResult)
                .Verifiable();
            
            //Act
            var result = await _systemUnderTest.Delete();

            // Assert
            _mockSessionService.Verify();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
            _mockSessionService.Verify(x => x.Logoff(_tppUserSession));
            _mockSessionCacheService.Verify(x => x.DeleteUserSession(_tppUserSession.Key));
            _mockAuditor.Verify(x => x.Audit(DeleteRequestAuditType, It.IsAny<string>(), It.IsAny<object[]>()));
            _mockAuditor.Verify(x => x.AuditWithExplicitNhsNumber(It.IsAny<string>(), It.IsAny<Supplier>(), DeleteResponseAuditType, It.IsAny<string>()));
        }

        [TestMethod]
        public async Task Delete_GpSupplierSessionLogoffFails_SessionDeletionContinuesAndReturns204NoContent() 
        {   
            // Arrange            
            var sessionLogoffResult = new SessionLogoffResult.SupplierSystemUnavailable();

            _mockSessionService
                .Setup(x => x.Logoff(_tppUserSession))
                .ReturnsAsync(sessionLogoffResult)
                .Verifiable();
            
            // Act
            var result = await _systemUnderTest.Delete();

            // Assert
            _mockSessionService.Verify();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
            _mockSessionService.Verify(x => x.Logoff(_tppUserSession));
            _mockAuditor.Verify(x => x.Audit(DeleteRequestAuditType, It.IsAny<string>(), It.IsAny<object[]>()));
            _mockAuditor.Verify(x => x.AuditWithExplicitNhsNumber(It.IsAny<string>(), It.IsAny<Supplier>(), DeleteResponseAuditType, It.IsAny<string>()));
        }
        
        public void Dispose()
        {
            _systemUnderTest.Dispose();
        }
    }
}