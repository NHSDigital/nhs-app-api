using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.LinkedAccounts;
using NHSOnline.Backend.GpSystems.LinkedAccounts.Models;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.PfsApi.Areas.ServiceJourneyRules;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.Support;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.ServiceJourneyRules
{
    [TestClass]
    public class ServiceJourneyRulesControllerTests
    {
        private ServiceJourneyRulesController _systemUnderTest;
        private IFixture _fixture;
        private Mock<ILogger<ServiceJourneyRulesController>> _mockLogger;
        private Mock<IServiceJourneyRulesService> _mockServiceJourneyRulesService;
        private SessionConfigurationSettings _sessionConfigSettings;
        private UserSession _userSession;
       private Mock<IGpSystemFactory> _mockGpSystemFactory;
       private Guid _patientId;
       private Mock<ILinkedAccountsService> _mockLinkedAccountService;
       private Mock<IGpSystem> _mockGpSystem;
       private string _defaultOdsCode;
       
       
        
        [TestInitialize]
        public void TestInitialize()
        {
            _patientId = Guid.NewGuid();
            _defaultOdsCode = "A123456";
           
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _mockServiceJourneyRulesService = _fixture.Freeze<Mock<IServiceJourneyRulesService>>();
            _sessionConfigSettings = _fixture.Freeze<SessionConfigurationSettings>();
            _userSession = _fixture.Create<UserSession>();
            _userSession.GpUserSession.NhsNumber = _fixture.Create<string>();
            _userSession.GpUserSession.OdsCode = _fixture.Create<string>();

            var httpContextItems = new Dictionary<object, object>
            {
                { Constants.HttpContextItems.UserSession, _userSession }
            };
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.SetupGet(x => x.Items).Returns(httpContextItems);
            
            _mockLogger = _fixture.Freeze<Mock<ILogger<ServiceJourneyRulesController>>>();
            _mockLogger.SetupLogger(LogLevel.Information, $"Fetching Service Journey Rules for {_userSession.GpUserSession.OdsCode}", null);

            _mockGpSystemFactory = _fixture.Freeze<Mock<IGpSystemFactory>>();
            _mockLinkedAccountService = new Mock<ILinkedAccountsService>();
            _mockGpSystem = new Mock<IGpSystem>();
            
            _systemUnderTest = _fixture.Create<ServiceJourneyRulesController>();
            
            _systemUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };
        }

        [TestMethod]
        public async Task Get_WhenServiceReturnsSuccessfully_ReturnsSuccessfulResult()
        {
            // Arrange
            var expectedResponse = new ServiceJourneyRulesResponse();
            expectedResponse.Journeys = new Journeys();
            _mockServiceJourneyRulesService.Setup(x => x.GetServiceJourneyRulesForOdsCode(_userSession.GpUserSession.OdsCode))
                .ReturnsAsync(new ServiceJourneyRulesConfigResult.Success(expectedResponse));

            // Act
            var result = await _systemUnderTest.Get();

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
            _mockServiceJourneyRulesService.Setup(x => x.GetServiceJourneyRulesForOdsCode(_userSession.GpUserSession.OdsCode))
                .ReturnsAsync(new ServiceJourneyRulesConfigResult.NotFound());

            // Act
            var result = await _systemUnderTest.Get();

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
            _mockServiceJourneyRulesService.Setup(x => x.GetServiceJourneyRulesForOdsCode(_userSession.GpUserSession.OdsCode))
                .ReturnsAsync(new ServiceJourneyRulesConfigResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _mockServiceJourneyRulesService.Verify();
            _mockLogger.Verify();
            
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [DataRow(true, true, true)]
        [DataRow(true, false, false)]
        [DataRow(false, true, false)]
        [DataRow(false, false, false)]
        [DataTestMethod]
        public async Task GetLinkedAccountPatientConfig_ReturnsSucessfully(bool proxyEnabledFeatureToggle, bool hasLinkedAccounts, bool expectedValue)
        {
            // Arrange
            _sessionConfigSettings.ProxyEnabled = proxyEnabledFeatureToggle;
            _userSession.GpUserSession.HasLinkedAccounts = hasLinkedAccounts;
            
            var expectedResponse = new LinkedAccountsConfigResponse
            { 
                Id = _userSession.GpUserSession.Id,
                HasLinkedAccounts = expectedValue
            };

            //Act
            var result = await _systemUnderTest.GetLinkedAccountPatientConfig();
           
            //Assert
            result.Should().BeAssignableTo<OkObjectResult>();

            var value = result.Should().BeAssignableTo<OkObjectResult>().Subject.Value;
            var actualResponse = value.Should().BeAssignableTo<LinkedAccountsConfigResult.Success>().Subject.Response;
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }
        
        
        
        [TestMethod]
        public async Task GetLinkedAccountConfiguration_WhenServiceReturnsSuccessfully_ReturnsSuccessfulResult()
        {
            // Arrange
            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier))
                .Returns(_mockGpSystem.Object);

            _mockGpSystem.Setup(x => x.GetLinkedAccountsService())
                .Returns(_mockLinkedAccountService.Object);

            _mockLinkedAccountService.Setup(x => x.GetOdsCodeForLinkedAccount(_userSession.GpUserSession, _patientId))
                .Returns(_defaultOdsCode);
            
            var expectedResponse = new ServiceJourneyRulesResponse
            {
                Journeys = new Journeys()
            };
            
            _mockServiceJourneyRulesService.Setup(x => x.GetServiceJourneyRulesForLinkedAccount(_defaultOdsCode))
                .ReturnsAsync(new ServiceJourneyRulesConfigResult.Success(expectedResponse));

            // Act
            var result = await _systemUnderTest.GetLinkedAccountConfiguration(_patientId);

            // Assert
            _mockGpSystemFactory.Verify();
            _mockServiceJourneyRulesService.Verify();
            _mockLogger.Verify();
            
            var value = result.Should().BeAssignableTo<OkObjectResult>().Subject.Value;
            var actualResponse = value.Should().BeAssignableTo<ServiceJourneyRulesResponse>().Subject;
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }
        
        [TestMethod]
        public async Task GetLinkedAccountConfiguration_WhenServiceDoesNotFindConfiguration_ReturnsNotFound()
        {
            // Arrange
            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier))
                .Returns(_mockGpSystem.Object);

            _mockGpSystem.Setup(x => x.GetLinkedAccountsService())
                .Returns(_mockLinkedAccountService.Object);

            _mockLinkedAccountService.Setup(x => x.GetOdsCodeForLinkedAccount(_userSession.GpUserSession, _patientId))
                .Returns(_defaultOdsCode);
            
            _mockServiceJourneyRulesService.Setup(x => x.GetServiceJourneyRulesForLinkedAccount(_defaultOdsCode))
                .ReturnsAsync(new ServiceJourneyRulesConfigResult.NotFound());

            // Act
            var result = await _systemUnderTest.GetLinkedAccountConfiguration(_patientId);

            // Assert
            _mockGpSystemFactory.Verify();
            _mockServiceJourneyRulesService.Verify();
            _mockLogger.Verify();
            
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }
    }
}