using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
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
using NHSOnline.Backend.PfsApi.ServiceJourneyRules;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.UnitTests.Filters
{
    [TestClass]
    public class JourneyFeatureFilterAttributeTests
    {
        private JourneyFeatureFilterAttribute _systemUnderTest;
        private AuthorizationFilterContext _context;
        private UserSession _userSession;
        private ServiceJourneyRulesResponse _sjrResponse;

        private Mock<HttpContext> _mockHttpContext;
        private Mock<IServiceJourneyRulesClient> _mockSjrClient;

        [TestInitialize]
        public void TestInitialize()
        {
            _systemUnderTest = new JourneyFeatureFilterAttribute(JourneyFeature.NominatedPharmacy);

            _userSession = new UserSession()
            {
                GpUserSession = new EmisUserSession()
                {
                    OdsCode = "X10000"
                }
            };            
            
            _sjrResponse = new ServiceJourneyRulesResponse()
            {
                Journeys = new Journeys()
                {
                    NominatedPharmacy = false,
                }
            };

            // Setup the Mocks
            var mockLogger = new Mock<ILogger<JourneyFeatureFilterAttribute>>();
            var mockServiceProvider = new Mock<IServiceProvider>();

            _mockSjrClient = new Mock<IServiceJourneyRulesClient>();
            _mockHttpContext = new Mock<HttpContext>();

            _mockHttpContext
                .Setup(x => x.RequestServices)
                .Returns(mockServiceProvider.Object);

            _mockHttpContext
                .Setup(x => x.Items)
                .Returns(new Dictionary<object, object>
                    { { Constants.HttpContextItems.UserSession, _userSession } });

            mockServiceProvider
                .Setup(x => x.GetService(typeof(IServiceJourneyRulesClient)))
                .Returns(_mockSjrClient.Object);

            mockServiceProvider
                .Setup(x => x.GetService(typeof(ILogger<JourneyFeatureFilterAttribute>)))
                .Returns(mockLogger.Object);
          
            _mockSjrClient
                .Setup(x => x.GetServiceJourneyRules(_userSession.GpUserSession.OdsCode))
                .ReturnsAsync(
                    new ServiceJourneyRulesApiObjectResponse<ServiceJourneyRulesResponse>(HttpStatusCode.Created)
                    {
                        Body = _sjrResponse
                    });
            
            var actionContext = new ActionContext()
            {
                HttpContext = _mockHttpContext.Object,
                RouteData = new RouteData(),
                ActionDescriptor = new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor(),
            };
            
            _context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
        }
        
        [DataRow(false)]
        [DataRow(null)]
        [DataTestMethod]
        public async Task OnAuthorizationAsync_FindsNominatedPharmacyIsNotEnabled_AndReturns403(bool? nomPharmValue)
        {
            // Arrange
            _sjrResponse.Journeys.NominatedPharmacy = nomPharmValue;

            // Act
            await _systemUnderTest.OnAuthorizationAsync(_context);

            // Assert
            _context.Result.Should().NotBeNull();
            var result = _context.Result.Should().BeOfType<StatusCodeResult>().Subject;
            result.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
        }        
        
        [TestMethod]
        public async Task OnAuthorizationAsync_FindsNominatedPharmacyIsNotEnabled_AndReturnsCustomErrorCode()
        {
            // Arrange
            _systemUnderTest = new JourneyFeatureFilterAttribute(
                JourneyFeature.NominatedPharmacy, HttpStatusCode.InternalServerError);
            
            _sjrResponse.Journeys.NominatedPharmacy = false;

            // Act
            await _systemUnderTest.OnAuthorizationAsync(_context);

            // Assert
            _context.Result.Should().NotBeNull();
            var result = _context.Result.Should().BeOfType<StatusCodeResult>().Subject;
            result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        } 
    
        [TestMethod]
        public async Task OnAuthorizationAsync_FindsNominatedPharmacyIstEnabled()
        {
            // Arrange
            _sjrResponse.Journeys.NominatedPharmacy = true;

            // Act
            await _systemUnderTest.OnAuthorizationAsync(_context);

            // Assert
            _context.Result.Should().BeNull();
        }
        
        [TestMethod]
        public async Task OnAuthorizationAsync_GetServiceJourneyRules_ReturnsUnsuccessfulResponse()
        {
            // Arrange
            _sjrResponse.Journeys.NominatedPharmacy = true;

            _mockSjrClient
                .Setup(x => x.GetServiceJourneyRules(_userSession.GpUserSession.OdsCode))
                .ReturnsAsync(
                    new ServiceJourneyRulesApiObjectResponse<ServiceJourneyRulesResponse>(HttpStatusCode.InternalServerError)
                    {
                        Body = _sjrResponse
                    });
            
            // Act
            await _systemUnderTest.OnAuthorizationAsync(_context);

            // Assert
            _context.Result.Should().NotBeNull();
            var result = _context.Result.Should().BeOfType<StatusCodeResult>().Subject;
            result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }
    }
}
