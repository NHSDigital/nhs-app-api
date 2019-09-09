using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.ServiceJourneyRulesApi.Controllers;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.Service;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.UnitTests.Controllers
{
    [TestClass]
    public class ServiceJourneyRulesControllerTests
    {
        private IFixture _fixture;
        
        private ServiceJourneyRulesController _systemUnderTest;
        private Mock<ILoggerFactory> _mockLogger;
        
        private Mock<IServiceJourneyRulesService> _mockServiceJourneyRulesService;

        private const string TestOdsCode = "A29928";
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _mockLogger = _fixture.Freeze<Mock<ILoggerFactory>>();
            
            _mockServiceJourneyRulesService = new Mock<IServiceJourneyRulesService>();
            _systemUnderTest = new ServiceJourneyRulesController(_mockLogger.Object, _mockServiceJourneyRulesService.Object);
        }
        
        [TestMethod]
        public void Get_Success()
        {
            // Arrange
            var expectedResponseBody = _fixture.Create<ServiceJourneyRulesResponse>();

            _mockServiceJourneyRulesService.Setup(x => x.GetServiceJourneyRulesForOdsCode(TestOdsCode))
                .Returns(expectedResponseBody);

            // Act
            var getResponse = _systemUnderTest.Get(TestOdsCode);
            
            // Assert
            var statusCodeResult = getResponse.Should().BeAssignableTo<OkObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            var journeysResponse = statusCodeResult.Value.Should().BeAssignableTo<ServiceJourneyRulesResponse>().Subject;
            journeysResponse.Should().BeEquivalentTo(expectedResponseBody);
        }

        [TestMethod]
        public void Get_withNullOdsCode_ReturnsBadRequest()
        {
            // Act
            var getResponse = _systemUnderTest.Get(null);
            
            // Assert
            getResponse.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [TestMethod]
        public void Get_withEmptyOdsCode_ReturnsBadRequest()
        {
            // Act
            var getResponse = _systemUnderTest.Get(" ");
            
            // Assert
            getResponse.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [TestMethod]
        public void Get_withMissingOdsCode_ReturnsNotFound()
        {
            // Arrange
            _mockServiceJourneyRulesService.Setup(x => x.GetServiceJourneyRulesForOdsCode(TestOdsCode))
                .Returns(new ServiceJourneyRulesResponse()
                {
                    Journeys = null
                });
            
            // Act
            var getResponse = _systemUnderTest.Get(TestOdsCode);

            // Assert
            getResponse.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }
    }
}