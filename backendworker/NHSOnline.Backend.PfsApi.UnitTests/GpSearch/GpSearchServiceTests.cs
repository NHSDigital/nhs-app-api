using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.GpSearch;
using NHSOnline.Backend.PfsApi.GpSearch.Models;

namespace NHSOnline.Backend.PfsApi.UnitTests.GpSearch
{
    [TestClass]
    public class GpSearchServiceTests
    {
        private GpSearchService _gpSearchService;
        private ILogger<GpSearchService> _logger;
        private IFixture _fixture;
        private Mock<IGpLookupClient> _gpLookupClient;
        private IGpLookupConfig _gpLookupConfig;
        private INhsSearchResultChecker _nhsSearchResultChecker;
        private IPostcodeParser _postcodeParser;
        private ILogger<NhsSearchResultChecker> _nhsResultCheckerLogger;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _logger = _fixture.Freeze<ILogger<GpSearchService>>();
            _gpLookupClient = new Mock<IGpLookupClient>();
            _gpLookupConfig = _fixture.Freeze<IGpLookupConfig>();
            _nhsResultCheckerLogger = _fixture.Freeze<ILogger<NhsSearchResultChecker>>();
            _nhsSearchResultChecker = new NhsSearchResultChecker(_nhsResultCheckerLogger);
            _postcodeParser = new PostcodeParser();
            
            _gpSearchService = new GpSearchService(_logger, _gpLookupClient.Object, _gpLookupConfig, _nhsSearchResultChecker,
                _postcodeParser);     
        }

        public async Task IsGpPracticeEpsEnabled_WhenOdsCodeIsBlank_ReturnsBadRequestEnabledFalse()
        {
            // Act
            var result = await _gpSearchService.IsGpPracticeEPSEnabled(string.Empty);

            // Assert
            _gpLookupClient.Verify(x => x.GpSearch(It.IsAny<OrganisationSearchData>()), Times.Never);
            result.HttpStatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.IsGpEpsEnabled.Should().Be(false);
        }

        public async Task IsGpPracticeEpsEnabled_WhenNoGpPracticeReturned_ReturnsNotFoundAndEnabledFalse()
        {
            // Arrange
            const string odsCode = "AB234";
            
            var organisationReturnResult =
                new GpLookupClient.NhsSearchApiObjectResponse<NhsOrganisationSearchResponse>(HttpStatusCode.OK)
                {
                    Body = new NhsOrganisationSearchResponse
                    {
                        Organisations = new List<Organisation>(),
                        OrganisationCount = 0,
                    }
                };

            _gpLookupClient.Setup(x => x.GpSearch(It.IsAny<OrganisationSearchData>()))
                .Returns(Task.FromResult(organisationReturnResult))
                .Verifiable();

            // Act
            var result = await _gpSearchService.IsGpPracticeEPSEnabled(odsCode);

            // Assert
            _gpLookupClient.Verify();
            result.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
            result.IsGpEpsEnabled.Should().Be(false);
        }

        public async Task IsGpPracticeEpsEnabled_WhenGpPracticeDoesntHaveAnyMetrics_ReturnsFalse()
        {
            // Arrange
            const string odsCode = "AB234";

            var organisationReturnResults = new List<Organisation>
            {
                new Organisation { OrganisationName = "Test GP Practice" }
            };

            var organisationReturnResult =
                new GpLookupClient.NhsSearchApiObjectResponse<NhsOrganisationSearchResponse>(HttpStatusCode.OK)
                {
                    Body = new NhsOrganisationSearchResponse
                    {
                        Organisations = organisationReturnResults,
                        OrganisationCount = 1,
                    }
                };

            _gpLookupClient.Setup(x => x.GpSearch(It.IsAny<OrganisationSearchData>()))
                .Returns(Task.FromResult(organisationReturnResult))
                .Verifiable();

            // Act
            var result = await _gpSearchService.IsGpPracticeEPSEnabled(odsCode);

            // Assert
            _gpLookupClient.Verify();
            result.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            result.IsGpEpsEnabled.Should().Be(false);
        }

        [TestMethod]
        public async Task IsGpPracticeEpsEnabled_WhenGpPracticeIsNotEpsEnabled_ReturnsFalse()
        {
            // Arrange
            const string odsCode = "AB234";
            const int metricIdOtherThanToEpsEnabled = 999;

            var organisationReturnResults = new List<Organisation>
            {
                new Organisation { OrganisationName = "Test GP Practice", Metrics = $"[{{\"MetricID\": {metricIdOtherThanToEpsEnabled} }}]" }
            };

            var organisationReturnResult =
                new GpLookupClient.NhsSearchApiObjectResponse<NhsOrganisationSearchResponse>(HttpStatusCode.OK)
                {
                    Body = new NhsOrganisationSearchResponse
                    {
                        Organisations = organisationReturnResults,
                        OrganisationCount = 1,
                    }
                };

            _gpLookupClient.Setup(x => x.GpSearch(It.IsAny<OrganisationSearchData>()))
                .Returns(Task.FromResult(organisationReturnResult))
                .Verifiable();

            // Act
            var result = await _gpSearchService.IsGpPracticeEPSEnabled(odsCode);

            // Assert
            _gpLookupClient.Verify();
            result.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            result.IsGpEpsEnabled.Should().Be(false);
        }

        [TestMethod]
        public async Task IsGpPracticeEpsEnabled_WhenSuccessfulResponseWithEpsEnabled_ReturnsTrue()
        {
            // Arrange
            const string odsCode = "AB234";

            var organisationReturnResults = new List<Organisation>
            {
                new Organisation { OrganisationName = "Test GP Practice", Metrics = $"[{{\"MetricID\": {Constants.MetricIdForEPSEnabled} }}]" }
            };

            var organisationReturnResult =
                new GpLookupClient.NhsSearchApiObjectResponse<NhsOrganisationSearchResponse>(HttpStatusCode.OK)
                {
                    Body = new NhsOrganisationSearchResponse
                    {
                        Organisations = organisationReturnResults,
                        OrganisationCount = 1,
                    }
                };

            _gpLookupClient.Setup(x => x.GpSearch(It.Is<OrganisationSearchData>(
                o => o.Top == 1 &&
                string.Equals(o.Select, "OrganisationID,OrganisationName,NACSCode,Metrics", System.StringComparison.OrdinalIgnoreCase) &&
                string.Equals(o.Filter, $"OrganisationTypeID eq 'GPB' and NACSCode eq '{odsCode}'", System.StringComparison.OrdinalIgnoreCase))
                ))
                .Returns(Task.FromResult(organisationReturnResult))
                .Verifiable();

            // Act
            var result = await _gpSearchService.IsGpPracticeEPSEnabled(odsCode);

            // Assert
            _gpLookupClient.Verify();
            result.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            result.IsGpEpsEnabled.Should().Be(true);
        }

        [TestMethod]
        [DataRow("L15BW")]
        [DataRow("L1 5BW")]
        [DataRow("L1")]
        public async Task
            Search_WhenCalledWithValidPostcode_CallsForLatLongAndSearchesForOrganisationOnLatitudeAndLongitude(string searchTerm)
        {
            // Arrange
            const string testLatitude = "1223.4345";
            const string testLongitude = "2323.232";
            
            var organisationReturnResults = new List<Organisation>
            {
                new Organisation { OrganisationName = "Test GP Practice" },
                new Organisation { OrganisationName = "Test GP Practice 2" }
            };
            
            var organisationReturnResult =
                new GpLookupClient.NhsSearchApiObjectResponse<NhsOrganisationSearchResponse>(HttpStatusCode.OK)
                {
                    Body = new NhsOrganisationSearchResponse
                    {
                        Organisations = organisationReturnResults,
                        OrganisationCount = 2,
                    }
                };
            
            _gpLookupClient.Setup(x => x.GpPostcodeSearch(It.IsAny<OrganisationPostcodeSearchData>()))
                .Returns(Task.FromResult(organisationReturnResult));

            var postcodeReturnResult =
                new GpLookupClient.NhsSearchApiObjectResponse<NhsPostcodeSearchResponse>(HttpStatusCode.OK)
                {
                    Body = new NhsPostcodeSearchResponse
                    {
                        PostcodeData = new List<PostcodeData>
                        {
                            new PostcodeData { Latitude = testLatitude, Longitude = testLongitude }
                        }
                    }
                };
                
            _gpLookupClient.Setup(x => x.PostcodeSearch(It.IsAny<PostcodeSearchData>()))
                .Returns(Task.FromResult(postcodeReturnResult));
            
            // Act
            var result = await _gpSearchService.Search(searchTerm);

            // Assert
            _gpLookupClient.Verify(x=>x.PostcodeSearch(It.IsAny<PostcodeSearchData>()));
            _gpLookupClient.Verify(x=>x.GpPostcodeSearch(It.IsAny<OrganisationPostcodeSearchData>()));
            var response = result.Should().BeAssignableTo<GpSearchResult.Success>().Subject.Response;
            response.OrganisationQueryCount.Should().Be(2);
            response.Organisations.Should().BeEquivalentTo(organisationReturnResults);
        }
        
        [TestMethod]
        public async Task Search_WhenCalledWithACity_ReturnsTheListOfMatchingOrganisationsInASuccessfulResponse()
        {
            // Arrange
            const string validPostcodeSearchTerm = "Liverpool";

            var organisationReturnResults = new List<Organisation>
            {
                new Organisation { OrganisationName = "Test GP Practice" },
                new Organisation { OrganisationName = "Test GP Practice 2" }
            };
            
            var organisationReturnResult =
                new GpLookupClient.NhsSearchApiObjectResponse<NhsOrganisationSearchResponse>(HttpStatusCode.OK)
                {
                    Body = new NhsOrganisationSearchResponse
                    {
                        Organisations = organisationReturnResults,
                        OrganisationCount = 2,
                    }
                };
            
            _gpLookupClient.Setup(x => x.GpSearch(It.IsAny<OrganisationSearchData>()))
                .Returns(Task.FromResult(organisationReturnResult));
            
            // Act
            var result = await _gpSearchService.Search(validPostcodeSearchTerm);
            
            // Assert
            _gpLookupClient.Verify(x=>x.GpSearch(It.IsAny<OrganisationSearchData>()));
            var response = result.Should().BeAssignableTo<GpSearchResult.Success>().Subject.Response;
            response.OrganisationQueryCount.Should().Be(2);
            response.Organisations.Should().BeEquivalentTo(organisationReturnResults);
        }
        
        [TestMethod]
        public async Task Search_WhenCalledWithACityAndResponseBodyIsNull_CallsOrganisationSearchAndReturnsInternalServerErrorResponse()
        {
            // Arrange
            const string validPostcodeSearchTerm = "Manchester";
            
            var organisationReturnResult =
                new GpLookupClient.NhsSearchApiObjectResponse<NhsOrganisationSearchResponse>(HttpStatusCode.OK)
                {
                    Body = null,
                };
            
            _gpLookupClient.Setup(x => x.GpSearch(It.IsAny<OrganisationSearchData>()))
                .Returns(Task.FromResult(organisationReturnResult));
            
            // Act
            var result = await _gpSearchService.Search(validPostcodeSearchTerm);

            //Assert
            _gpLookupClient.Verify(x=>x.GpSearch(It.IsAny<OrganisationSearchData>()));
            result.Should().BeAssignableTo<GpSearchResult.InternalServerError>();
        }
        
        [TestMethod]
        [DataRow("")]
        [DataRow(null)]
        public async Task Search_WhenCalledWWithInvalidSearchString_ReturnsBadRequest(string searchTerm)
        {
            //Act
            var result = await _gpSearchService.Search(searchTerm);
            
            //Assert
            result.Should().BeAssignableTo<GpSearchResult.BadRequest>();
        }
        
        [TestMethod]
        public async Task Search_WhenCalledAndNhsPostcodeSearchReturnsUnsuccessfulStatusCode_ReturnsInternalServerError()
        {
            
            // Arrange
            const string searchTerm = "L12DA";

            var postcodeReturnResult =
                new GpLookupClient.NhsSearchApiObjectResponse<NhsPostcodeSearchResponse>(HttpStatusCode.Unauthorized);
                
            _gpLookupClient.Setup(x => x.PostcodeSearch(It.IsAny<PostcodeSearchData>()))
                .Returns(Task.FromResult(postcodeReturnResult));
            
            // Act
            var result = await _gpSearchService.Search(searchTerm);

            // Assert
            result.Should().BeAssignableTo<GpSearchResult.InternalServerError>();
        }
        
        [TestMethod]
        public async Task Search_WhenCalledAndNhsPostcodeSearchReturnsSuccessfulButNoPostcodeData_ReturnsSuccessful()
        {
            
            // Arrange
            const string searchTerm = "L12DA";

            var postcodeReturnResult =
                new GpLookupClient.NhsSearchApiObjectResponse<NhsPostcodeSearchResponse>(HttpStatusCode.OK)
                {
                    Body = new NhsPostcodeSearchResponse 
                    { 
                        PostcodeData = new List<PostcodeData>()
                    },
                };
                
            _gpLookupClient.Setup(x => x.PostcodeSearch(It.IsAny<PostcodeSearchData>()))
                .Returns(Task.FromResult(postcodeReturnResult));
            
            // Act
            var result = await _gpSearchService.Search(searchTerm);

            // Assert
            var response = result.Should().BeAssignableTo<GpSearchResult.Success>().Subject.Response;
            response.OrganisationQueryCount.Should().Be(0);
        }
    }
}