using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.LinkedAccounts;
using NHSOnline.Backend.PfsApi.Filters;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.Filters
{
    [TestClass]
    public class ProxyAuditingMiddlewareTests
    {
        private IFixture _fixture;
        private UserSession _mockUserSession;
        private Guid _patientId;
        private RequestDelegate _next;
        private DefaultHttpContext _context;
        private Mock<ILogger<ProxyAuditingMiddleware>> _mockLogger;
        private Mock<IGpSystem> _mockGpSystem;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private Mock<ILinkedAccountsService> _mockLinkedAccountService;

       [TestInitialize]
        public void TestInitialize()
        {
            _next = _ => Task.CompletedTask;
            _patientId = Guid.NewGuid();
            _context = new DefaultHttpContext();

            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _mockLogger = _fixture.Freeze<Mock<ILogger<ProxyAuditingMiddleware>>>();
            _mockUserSession = _fixture.Create<UserSession>();
            _mockGpSystemFactory = _fixture.Freeze<Mock<IGpSystemFactory>>();
            _mockGpSystem = _fixture.Freeze<Mock<IGpSystem>>();
            _mockLinkedAccountService = _fixture.Freeze<Mock<ILinkedAccountsService>>();

            _mockGpSystemFactory.Setup(
                    x => x.CreateGpSystem(_mockUserSession.GpUserSession.Supplier))
                .Returns(_mockGpSystem.Object);

            _mockGpSystem.Setup(x => x.GetLinkedAccountsService())
                .Returns(_mockLinkedAccountService.Object);
            
            _mockGpSystem.Setup(x => x.SupportsLinkedAccounts).Returns(true);
            
            _context.SetUserSession(_mockUserSession);
        }


        [TestMethod]
        public async Task LinkedAccountAuditInfo_IsStoredInContext()
        {
            // Arrange
            _context.Request.Headers.Add(Constants.HttpHeaders.PatientId, _patientId.ToString());
            var result = _fixture.Create<LinkedAccountAuditInfo>();

            _mockLinkedAccountService.Setup(
                    x => x.GetProxyAuditData(_mockUserSession.GpUserSession, _patientId))
                .Returns(result).Verifiable();
                 
            var subject = new ProxyAuditingMiddleware(_next, _mockGpSystemFactory.Object, _mockLogger.Object);

            // Act
            await subject.Invoke(_context);

            // Assert
            _mockLinkedAccountService.Verify();
            _context.GetLinkedAccountAuditInfo().Should().Be(result);
        }
        
        [TestMethod]
        public async Task LinkedAccountAuditInfo_NotStoredInContext_WhenUserSessionNotAvailable()
        {
            // Arrange
            _context.Items.Remove(Constants.HttpContextItems.UserSession);
            
            var subject = new ProxyAuditingMiddleware(_next, _mockGpSystemFactory.Object, _mockLogger.Object);

            // Act
            await subject.Invoke(_context);

            // Assert
            _context.GetLinkedAccountAuditInfo().Should().BeNull();
        }
        
        [TestMethod]
        public async Task LinkedAccountAuditInfo_NotStoredInContext_WhenGpSystemDoesNotSupportLinkedAccounts()
        {
            // Arrange
            _mockGpSystem.Setup(x => x.SupportsLinkedAccounts).Returns(false);
                 
            var subject = new ProxyAuditingMiddleware(_next, _mockGpSystemFactory.Object, _mockLogger.Object);

            // Act
            await subject.Invoke(_context);

            // Assert
            _context.GetLinkedAccountAuditInfo().Should().BeNull();
        }

        [TestMethod]
        public async Task LinkedAccountAuditInfo_NotStoredInContext_WhenPatientIdHeaderNotFound()
        {
            // Arrange                 
            var subject = new ProxyAuditingMiddleware(_next, _mockGpSystemFactory.Object, _mockLogger.Object);

            // Act
            await subject.Invoke(_context);

            // Assert
            _context.GetLinkedAccountAuditInfo().Should().BeNull();
        }
        
        [DataTestMethod]
        [DataRow(null, "/v1/patient/configuration")]
        [DataRow(null, "/v1/patient/journey-configuration")]
        public async Task DoesNotLogPatientHeaderNotFoundWarning_WhenRequestPathIsInvalid(string patientId, string requestPath)
        {
            // Arrange
            _context.Request.Headers.Add(Constants.HttpHeaders.PatientId, patientId);
            _context.Request.Path = requestPath;
            var expectedLogMessage = "NHSO-Patient-Id Header not found";
                 
            var subject = new ProxyAuditingMiddleware(_next, _mockGpSystemFactory.Object, _mockLogger.Object);

            // Act
            await subject.Invoke(_context);

            // Assert
            _context.GetLinkedAccountAuditInfo().Should().BeNull();
            _mockLogger.VerifyLogger(LogLevel.Warning, expectedLogMessage, Times.Never());
        }
        
                
        [DataTestMethod]
        [DataRow(null, "/v1/patient/configuration1234")]
        public async Task LogsPatientHeaderNotFoundWarning_WhenRequestPathIsValid(string patientId, string requestPath)
        {
            // Arrange
            _context.Request.Headers.Add(Constants.HttpHeaders.PatientId, patientId);
            _context.Request.Path = requestPath;
            var expectedLogMessage = "NHSO-Patient-Id Header not found";
                 
            var subject = new ProxyAuditingMiddleware(_next, _mockGpSystemFactory.Object, _mockLogger.Object);

            // Act
            await subject.Invoke(_context);

            // Assert
            _mockLogger.VerifyLogger(LogLevel.Warning, expectedLogMessage, Times.Once());
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("1234")]
        [DataRow("")]
        public async Task LinkedAccountAuditInfo_NotStoredInContext_WhenPatientIdNotParsable(string patientId)
        {
            // Arrange
            _context.Request.Headers.Add(Constants.HttpHeaders.PatientId, patientId);
                 
            var subject = new ProxyAuditingMiddleware(_next, _mockGpSystemFactory.Object, _mockLogger.Object);

            // Act
            await subject.Invoke(_context);

            // Assert
            _context.GetLinkedAccountAuditInfo().Should().BeNull();
        }
    }
}