using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.LinkedAccounts;
using NHSOnline.Backend.GpSystems.LinkedAccounts.Models;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.PfsApi.Areas.ServiceJourneyRules;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.ServiceJourneyRules
{
    [TestClass]
    public class LinkedAccountPatientConfigVisitorTests
    {
        private IFixture _fixture;
        private Mock<ILogger<ServiceJourneyRulesController>> _mockLogger;
        private Mock<IAuditor> _mockAuditor;
        private SessionConfigurationSettings _config;
        private LinkedAccountPatientConfigVisitor _systemUnderTest;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private Mock<ISessionCacheService> _mockSessionCacheService;
        private Mock<IGpSystem> _mockGpSystem;
        private Mock<ILinkedAccountsService> _mockLinkedAccountsService;
        private Mock<GpUserSession> _gpUserSession;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            _mockLogger = new Mock<ILogger<ServiceJourneyRulesController>>();
            _mockAuditor = new Mock<IAuditor>();
            _config = new SessionConfigurationSettings(true);
            _mockGpSystemFactory = new Mock<IGpSystemFactory>();
            _mockGpSystem = new Mock<IGpSystem>();
            _mockSessionCacheService = _fixture.Freeze<Mock<ISessionCacheService>>();
            _mockLinkedAccountsService = new Mock<ILinkedAccountsService>();
            _gpUserSession = new Mock<GpUserSession>();

            _systemUnderTest = new LinkedAccountPatientConfigVisitor(
                _mockLogger.Object,
                _mockAuditor.Object,
                _config,
                _mockGpSystemFactory.Object,
                _mockSessionCacheService.Object);
        }

        [TestMethod]
        public async Task VisitP9WithNoGpSessionSuccessfulDoesNotReturnAnyLinkedAccounts()
        {
            // Arrange
            var userSession = new P9UserSession("csrfToken",
                It.IsAny<CitizenIdUserSession>(),
                "nhsNumber",
                "im1ConnectionToken",
                "serviceDeskReference");

            // Act
            var result = await _systemUnderTest.Visit(userSession);

            // Assert
            var response = result.Should().BeAssignableTo<LinkedAccountsConfigResult.Success>().Subject;
            response.LinkedAccounts.Should().BeEmpty();
        }

        [TestMethod]
        public async Task VisitP9WithGpSessionSuccessfulReturnsLinkedAccounts()
        {
            // Arrange
            _gpUserSession.Setup(x => x.HasLinkedAccounts).Returns(true);

            var userSession = new P9UserSession("csrfToken",
                "nhsNumber",
                It.IsAny<CitizenIdUserSession>(),
                _gpUserSession.Object,
                "im1ConnectionToken");

            LinkedAccountsResult linkedAccountResult = new LinkedAccountsResult.Success(
                new List<LinkedAccount>
                {
                   new LinkedAccount
                   {
                       FullName = "Test Linked Account",
                       AgeYears = 35,
                       GivenName = "Test Account",
                       Id = new Guid(),
                       DisplayPersonalizedContent = true
                   }
                },
                true);

            _mockLinkedAccountsService = new Mock<ILinkedAccountsService>();
            _mockLinkedAccountsService.Setup(x => x.GetLinkedAccounts(userSession.GpUserSession))
                .ReturnsAsync(linkedAccountResult);

            _mockGpSystem = new Mock<IGpSystem>();
            _mockGpSystem.Setup(x => x.GetLinkedAccountsService())
                .Returns(_mockLinkedAccountsService.Object);
            _mockGpSystem.Setup(x => x.SupportsLinkedAccounts)
                .Returns(true);

            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(It.IsAny<Supplier>()))
                .Returns(_mockGpSystem.Object);

            // Act
            var result = await _systemUnderTest.Visit(userSession);

            // Assert
            var response = result.Should().BeAssignableTo<LinkedAccountsConfigResult.Success>().Subject;
            response.LinkedAccounts.Should().HaveCount(1);
            response.LinkedAccounts.Should().NotBeEmpty();
        }

        [TestMethod]
        public async Task VisitP5UserSessionSuccessfulDoesNotReturnAnyLinkedAccounts()
        {
            // Arrange
            var userSession = new P5UserSession("csrfToken",
                It.IsAny<CitizenIdUserSession>());

            // Act
            var result = await _systemUnderTest.Visit(userSession);

            // Assert
            var response = result.Should().BeAssignableTo<LinkedAccountsConfigResult.Success>().Subject;
            response.LinkedAccounts.Should().BeEmpty();
        }
    }
}