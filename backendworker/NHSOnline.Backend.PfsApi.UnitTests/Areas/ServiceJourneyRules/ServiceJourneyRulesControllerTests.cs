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
        private UserSession _userSession;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _mockServiceJourneyRulesService = _fixture.Freeze<Mock<IServiceJourneyRulesService>>();
            
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

            _systemUnderTest = _fixture.Create<ServiceJourneyRulesController>();
            
            _systemUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };
        }

        [TestMethod]
        public async Task Get_WhenServiceReturnsSuccessfully_ReturnsSuccessfulResult_LinkedAccountsIsFalse()
        {
            // Arrange
            var expectedResponse = new ServiceJourneyRulesResponse();
            expectedResponse.Journeys = new Journeys();
            
            _mockServiceJourneyRulesService.Setup(x => x.GetServiceJourneyRulesForOdsCode(_userSession.GpUserSession.OdsCode, false))
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
        public async Task Get_WhenServiceReturnsSuccessfully_ReturnsSuccessfulResult_LinkedAccountsToTrue()
        {
            // Arrange
            var expectedResponse = new ServiceJourneyRulesResponse();
            expectedResponse.Journeys = new Journeys();
            expectedResponse.Journeys.HasLinkedAccounts = true;
            
            _mockServiceJourneyRulesService.Setup(x => x.GetServiceJourneyRulesForOdsCode(_userSession.GpUserSession.OdsCode, false))
                .ReturnsAsync(new ServiceJourneyRulesConfigResult.Success(expectedResponse));

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _mockServiceJourneyRulesService.Verify();
            _mockLogger.Verify();
            
            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeAssignableTo<ServiceJourneyRulesResponse>()
                .Subject.Should().BeEquivalentTo(expectedResponse);
        }
        
        [TestMethod]
        public async Task Get_WhenServiceDoesNotFindConfiguration_ReturnsNotFound()
        {
            // Arrange
            _mockServiceJourneyRulesService.Setup(x => x.GetServiceJourneyRulesForOdsCode(_userSession.GpUserSession.OdsCode, false))
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
            _mockServiceJourneyRulesService.Setup(x => x.GetServiceJourneyRulesForOdsCode(_userSession.GpUserSession.OdsCode, false))
                .ReturnsAsync(new ServiceJourneyRulesConfigResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _mockServiceJourneyRulesService.Verify();
            _mockLogger.Verify();
            
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }
    }
}