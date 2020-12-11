using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
using NHSOnline.Backend.PfsApi.ServiceJourneyRules;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.ServiceJourneyRules
{
    [TestClass]
    public sealed class ServiceJourneyRulesControllerTests: IDisposable
    {
        private ServiceJourneyRulesController _systemUnderTest;
        private Mock<ILogger<ServiceJourneyRulesController>> _mockLogger;
        private Mock<IServiceJourneyRulesService> _mockServiceJourneyRulesService;
        private SessionConfigurationSettings _sessionConfigSettings;
        private Mock<GpUserSession> _gpUserSession;
        private P9UserSession _userSession;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private Guid _patientId;
        private Mock<ILinkedAccountsService> _mockLinkedAccountService;
        private Mock<ISessionCacheService> _mockSessionCacheService;
        private Mock<IGpSystem> _mockGpSystem;
        private string _defaultOdsCode;

        [TestInitialize]
        public void TestInitialize()
        {
            _patientId = Guid.NewGuid();
            _defaultOdsCode = "A123456";

            _mockSessionCacheService = new Mock<ISessionCacheService>();
            _mockServiceJourneyRulesService = new Mock<IServiceJourneyRulesService>();
            _sessionConfigSettings = new SessionConfigurationSettings(true);
            _gpUserSession = new Mock<GpUserSession>();

            _userSession = new P9UserSession("csrfToken", "nhsNumber", new CitizenIdUserSession(), _gpUserSession.Object, "im1token")
            {
                GpUserSession = { NhsNumber = "Nhs number", OdsCode = "ODS Code" }
            };

            _mockLogger = new Mock<ILogger<ServiceJourneyRulesController>>();
            _mockLogger.SetupLogger(LogLevel.Information, $"Fetching Service Journey Rules for {_userSession.GpUserSession.OdsCode}", null);

            _mockGpSystemFactory = new Mock<IGpSystemFactory>();
            _mockLinkedAccountService = new Mock<ILinkedAccountsService>();
            _mockGpSystem = new Mock<IGpSystem>();

            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier))
                .Returns(_mockGpSystem.Object);

            _mockGpSystem.Setup(x => x.GetLinkedAccountsService())
                .Returns(_mockLinkedAccountService.Object);

            _systemUnderTest = new ServiceJourneyRulesController(
                _mockLogger.Object,
                new Mock<IAuditor>().Object,
                _mockServiceJourneyRulesService.Object,
                _sessionConfigSettings,
                _mockGpSystemFactory.Object,
                _mockSessionCacheService.Object);
        }

        [TestMethod]
        public async Task Get_WhenServiceReturnsSuccessfully_ReturnsSuccessfulResult()
        {
            // Arrange
            var expectedResponse = new ServiceJourneyRulesResponse { Journeys = new Journeys() };
            _mockServiceJourneyRulesService.Setup(x => x.GetServiceJourneyRulesForOdsCode(_userSession.OdsCode))
                .ReturnsAsync(new ServiceJourneyRulesConfigResult.Success(expectedResponse));

            // Act
            var result = await _systemUnderTest.Get(_userSession);

            // Assert
            _mockServiceJourneyRulesService.Verify();
            _mockLogger.Verify();

            var value = result.Should().BeAssignableTo<OkObjectResult>().Subject.Value;
            var actualResponse = value.Should().BeAssignableTo<ServiceJourneyRulesResponse>().Subject;
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }


        [TestMethod]
        public async Task Get_WhenServiceDoesNotFindConfiguration_ReturnsNotFound()
        {
            // Arrange
            _mockServiceJourneyRulesService.Setup(x => x.GetServiceJourneyRulesForOdsCode(_userSession.OdsCode))
                .ReturnsAsync(new ServiceJourneyRulesConfigResult.NotFound());

            // Act
            var result = await _systemUnderTest.Get(_userSession);

            // Assert
            _mockServiceJourneyRulesService.Verify();
            _mockLogger.Verify();

            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [TestMethod]
        public async Task Get_WhenServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockServiceJourneyRulesService.Setup(x => x.GetServiceJourneyRulesForOdsCode(_userSession.OdsCode))
                .ReturnsAsync(new ServiceJourneyRulesConfigResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.Get(_userSession);

            // Assert
            _mockServiceJourneyRulesService.Verify();
            _mockLogger.Verify();

            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task GetLinkedAccountPatientConfig_WhenGpSystemDoesNotSupportLinkedAccounts_DoesNotCallService()
        {
            // Arrange
            _sessionConfigSettings.ProxyEnabled = true;
             _gpUserSession.Setup(x => x.HasLinkedAccounts).Returns(true);
            _mockGpSystem.SetupGet(x => x.SupportsLinkedAccounts).Returns(false);

            var expectedResponse = new LinkedAccountsConfigResponse
            {
                Id = _userSession.GpUserSession.Id,
                HasLinkedAccounts = false,
                LinkedAccounts = Enumerable.Empty<LinkedAccount>(),
            };

            // Act
            var result = await _systemUnderTest.GetLinkedAccountPatientConfig(_gpUserSession.Object, _userSession);

            // Assert
            var value = result.Should().BeAssignableTo<OkObjectResult>().Subject.Value;
            var actualResponse = value.Should().BeAssignableTo<LinkedAccountsConfigResponse>().Subject;
            actualResponse.Should().BeEquivalentTo(expectedResponse);
            _mockLinkedAccountService.Verify(x => x.GetLinkedAccounts(It.IsAny<GpUserSession>()), Times.Never());
        }

        [TestMethod]
        public async Task GetLinkedAccountPatientConfig_WhenProxyNotEnabled_CallsServiceToGetLinkedAccounts_ButDoesNotReturnResult()
        {
            // Arrange
            _sessionConfigSettings.ProxyEnabled = false;
            _gpUserSession.Setup(x => x.HasLinkedAccounts).Returns(true);
            _mockGpSystem.SetupGet(x => x.SupportsLinkedAccounts).Returns(true);

            _mockLinkedAccountService.Setup(x => x.GetLinkedAccounts(_userSession.GpUserSession))
                .ReturnsAsync(new LinkedAccountsResult.Success(new List<LinkedAccount>(), false))
                .Verifiable();

            var expectedResponse = new LinkedAccountsConfigResponse
            {
                Id = _userSession.GpUserSession.Id,
                HasLinkedAccounts = false,
                LinkedAccounts = Enumerable.Empty<LinkedAccount>(),
            };

            // Act
            var result = await _systemUnderTest.GetLinkedAccountPatientConfig(_gpUserSession.Object, _userSession);

            // Assert
            var value = result.Should().BeAssignableTo<OkObjectResult>().Subject.Value;
            var actualResponse = value.Should().BeAssignableTo<LinkedAccountsConfigResponse>().Subject;
            actualResponse.Should().BeEquivalentTo(expectedResponse);
            _mockLinkedAccountService.VerifyAll();
        }

        [TestMethod]
        public async Task GetLinkedAccountPatientConfig_WhenEverythingIsEnabled_CallsServiceToGetLinkedAccountsAndReturns()
        {
            // Arrange
            _sessionConfigSettings.ProxyEnabled = true;
            _gpUserSession.Setup(x => x.HasLinkedAccounts).Returns(true);
            _mockGpSystem.SetupGet(x => x.SupportsLinkedAccounts).Returns(true);

            var linkedAccounts = new List<LinkedAccount> { new LinkedAccount() };

            _mockLinkedAccountService.Setup(x => x.GetLinkedAccounts(_userSession.GpUserSession))
                .ReturnsAsync(new LinkedAccountsResult.Success(linkedAccounts, false))
                .Verifiable();

            var expectedResponse = new LinkedAccountsConfigResponse
            {
                Id = _userSession.GpUserSession.Id,
                HasLinkedAccounts = true,
                LinkedAccounts = linkedAccounts,
            };

            // Act
            var result = await _systemUnderTest.GetLinkedAccountPatientConfig(_gpUserSession.Object, _userSession);

            // Assert
            var value = result.Should().BeAssignableTo<OkObjectResult>().Subject.Value;
            var actualResponse = value.Should().BeAssignableTo<LinkedAccountsConfigResponse>().Subject;
            actualResponse.Should().BeEquivalentTo(expectedResponse);
            _mockLinkedAccountService.VerifyAll();
        }

        [TestMethod]
        public async Task GetLinkedAccountPatientConfig_WhenEverythingIsEnabledButNoLinkedAccountsReturned_ReturnsFalseForHasLinkedAccounts()
        {
            // Arrange
            _sessionConfigSettings.ProxyEnabled = true;
            _gpUserSession.Setup(x => x.HasLinkedAccounts).Returns(true);
            _mockGpSystem.SetupGet(x => x.SupportsLinkedAccounts).Returns(true);

            _mockLinkedAccountService.Setup(x => x.GetLinkedAccounts(_userSession.GpUserSession))
                .ReturnsAsync(new LinkedAccountsResult.Success(new List<LinkedAccount>(), false))
                .Verifiable();

            var expectedResponse = new LinkedAccountsConfigResponse
            {
                Id = _userSession.GpUserSession.Id,
                HasLinkedAccounts = false,
                LinkedAccounts = Enumerable.Empty<LinkedAccount>(),
            };

            // Act
            var result = await _systemUnderTest.GetLinkedAccountPatientConfig(_gpUserSession.Object, _userSession);

            // Assert
            var value = result.Should().BeAssignableTo<OkObjectResult>().Subject.Value;
            var actualResponse = value.Should().BeAssignableTo<LinkedAccountsConfigResponse>().Subject;
            actualResponse.Should().BeEquivalentTo(expectedResponse);
            _mockLinkedAccountService.VerifyAll();
        }

        [TestMethod]
        public async Task GetLinkedAccountPatientConfig_CallsSessionCacheServiceUpdate_WhenResultFromServiceSaysUpdatedIsRequired()
        {
            // Arrange
            _sessionConfigSettings.ProxyEnabled = true;
            _gpUserSession.Setup(x => x.HasLinkedAccounts).Returns(true);
            _mockGpSystem.SetupGet(x => x.SupportsLinkedAccounts).Returns(true);

            var linkedAccounts = new[] { new LinkedAccount() };

            _mockLinkedAccountService.Setup(x => x.GetLinkedAccounts(_userSession.GpUserSession))
                .ReturnsAsync(new LinkedAccountsResult.Success(linkedAccounts, true))
                .Verifiable();

            var expectedResponse = new LinkedAccountsConfigResponse
            {
                Id = _userSession.GpUserSession.Id,
                HasLinkedAccounts = true,
                LinkedAccounts = linkedAccounts,
            };

            // Act
            var result = await _systemUnderTest.GetLinkedAccountPatientConfig(_gpUserSession.Object, _userSession);

            // Assert
            var value = result.Should().BeAssignableTo<OkObjectResult>().Subject.Value;
            var actualResponse = value.Should().BeAssignableTo<LinkedAccountsConfigResponse>().Subject;
            actualResponse.Should().BeEquivalentTo(expectedResponse);
            _mockLinkedAccountService.VerifyAll();
            _mockSessionCacheService.Verify(x => x.UpdateUserSession(_userSession), Times.Once());
        }

        [TestMethod]
        public async Task GetLinkedAccountPatientConfig_DoesNotCallSessionCacheServiceUpdate_WhenResultFromServiceSaysUpdatedIsNotRequired()
        {
            // Arrange
            _sessionConfigSettings.ProxyEnabled = true;
            _gpUserSession.Setup(x => x.HasLinkedAccounts).Returns(true);
            _mockGpSystem.SetupGet(x => x.SupportsLinkedAccounts).Returns(true);

            var linkedAccounts = new[] { new LinkedAccount() };

            _mockLinkedAccountService.Setup(x => x.GetLinkedAccounts(_userSession.GpUserSession))
                .ReturnsAsync(new LinkedAccountsResult.Success(linkedAccounts, false))
                .Verifiable();

            var expectedResponse = new LinkedAccountsConfigResponse
            {
                Id = _userSession.GpUserSession.Id,
                HasLinkedAccounts = true,
                LinkedAccounts = linkedAccounts,
            };

            // Act
            var result = await _systemUnderTest.GetLinkedAccountPatientConfig(_gpUserSession.Object, _userSession);

            // Assert
            var value = result.Should().BeAssignableTo<OkObjectResult>().Subject.Value;
            var actualResponse = value.Should().BeAssignableTo<LinkedAccountsConfigResponse>().Subject;
            actualResponse.Should().BeEquivalentTo(expectedResponse);
            _mockLinkedAccountService.VerifyAll();
            _mockSessionCacheService.Verify(x => x.UpdateUserSession(_userSession), Times.Never());
        }

        [TestMethod]
        public async Task GetLinkedAccountConfiguration_WhenServiceReturnsSuccessfully_ReturnsSuccessfulResult()
        {
            // Arrange
            _mockLinkedAccountService.Setup(x => x.GetOdsCodeForLinkedAccount(_userSession.GpUserSession, _patientId))
                .Returns(_defaultOdsCode);

            var expectedResponse = new ServiceJourneyRulesResponse
            {
                Journeys = new Journeys()
            };

            _mockServiceJourneyRulesService.Setup(x => x.GetServiceJourneyRulesForLinkedAccount(_defaultOdsCode))
                .ReturnsAsync(new ServiceJourneyRulesConfigResult.Success(expectedResponse));

            // Act
            var result = await _systemUnderTest.GetLinkedAccountConfiguration(_patientId, _gpUserSession.Object);

            // Assert
            var value = result.Should().BeAssignableTo<OkObjectResult>().Subject.Value;
            var actualResponse = value.Should().BeAssignableTo<ServiceJourneyRulesResponse>().Subject;
            actualResponse.Should().BeEquivalentTo(expectedResponse);
            _mockServiceJourneyRulesService.Verify();
            _mockLogger.Verify();
            _mockGpSystemFactory.Verify();
        }

        [TestMethod]
        public async Task GetLinkedAccountConfiguration_WhenServiceDoesNotFindConfiguration_ReturnsNotFound()
        {
            // Arrange
            _mockLinkedAccountService.Setup(x => x.GetOdsCodeForLinkedAccount(_userSession.GpUserSession, _patientId))
                .Returns(_defaultOdsCode);

            _mockServiceJourneyRulesService.Setup(x => x.GetServiceJourneyRulesForLinkedAccount(_defaultOdsCode))
                .ReturnsAsync(new ServiceJourneyRulesConfigResult.NotFound());

            // Act
            var result = await _systemUnderTest.GetLinkedAccountConfiguration(_patientId, _gpUserSession.Object);

            // Assert
            _mockGpSystemFactory.Verify();
            _mockServiceJourneyRulesService.Verify();
            _mockLogger.Verify();

            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        public void Dispose() => _systemUnderTest?.Dispose();
    }
}