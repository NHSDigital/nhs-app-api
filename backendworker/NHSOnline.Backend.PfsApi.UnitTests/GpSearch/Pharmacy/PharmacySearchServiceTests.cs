using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.PfsApi.GpSearch;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using GeoCoordinatePortable;
using Microsoft.Extensions.Logging;
using Moq;
using NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy;
using NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy.Models;
using NHSOnline.Backend.PfsApi.GpSearch.Models;
using NHSOnline.Backend.PfsApi.GpSearch.Models.Pharmacy;
using NHSOnline.Backend.PfsApi.GpSearch.Pharmacy;

namespace NHSOnline.Backend.PfsApi.UnitTests.GpSearch.Pharmacy
{
    [TestClass]
    public class PharmacySearchServiceTests
    {
        private PharmacySearchService _pharmacySearchService;
        private ILogger<PharmacySearchService> _logger;
        private Mock<IGpLookupClient> _gpLookupClient;
        private IGpLookupConfig _gpLookupConfig;
        private Mock<INhsSearchResultChecker> _nhsSearchResultChecker;
        private IPostcodeParser _postcodeParser;
        private Mock<IPharmacyDetailsToPharmacyDetailsResponseMapper> _mockMapper;

        private IFixture _fixture;

        private const int EPSEnabledMetricID = 10051;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            
            _logger = _fixture.Freeze<ILogger<PharmacySearchService>>();
            _gpLookupClient = new Mock<IGpLookupClient>();
            _gpLookupConfig = _fixture.Freeze<IGpLookupConfig>();
            _nhsSearchResultChecker = new Mock<INhsSearchResultChecker>();
            _postcodeParser = new PostcodeParser();
            _mockMapper = _fixture.Freeze<Mock<IPharmacyDetailsToPharmacyDetailsResponseMapper>>();

            _pharmacySearchService = new PharmacySearchService(_logger, _gpLookupClient.Object, _gpLookupConfig, _nhsSearchResultChecker.Object,
                _postcodeParser, _mockMapper.Object); 
        }

        [TestMethod]
        [DataRow("SE13", "Type eq 'PostcodeOutCode'")]
        [DataRow("SE13 6JZ", "LocalType eq 'Postcode'")]
        public async Task
            Search_WhenCalledWithValidPostcode_ReturnsListOfPharmacies(string postcode, string filterName)
        {
            // Arrange
            const string latitude = "1";
            const string longitude = "2";

            var postcodeSearchResult =
                new GpLookupClient.NhsSearchApiObjectResponse<NhsPostcodeSearchResponse>(HttpStatusCode.OK)
                {
                    Body = new NhsPostcodeSearchResponse 
                    { 
                        PostcodeData = new List<PostcodeData>
                        {
                            new PostcodeData
                            {
                                Latitude = latitude,
                                Longitude = longitude,
                            }
                        }
                    },
                };

           var pharmacyResponse = new
               GpLookupClient.NhsSearchApiObjectResponse<NhsOrganisationSearchResponse>(HttpStatusCode.OK)
               {
                   Body = new NhsOrganisationSearchResponse
                   {
                       OrganisationCount = 1,
                       Organisations = new List<Organisation>
                       {
                           new Organisation
                           {
                               OrganisationName = "abc"
                           }
                       }
                   }
               };
           
           var pharmacySearchResponse = new PharmacySearchResponse(HttpStatusCode.OK, new List<Organisation>
           {
               new Organisation
               {
                   OrganisationName = "organisation_name_from_checker"
               }
           });
            
           _gpLookupClient
                .Setup(x => x.PostcodeSearch(
                    It.Is<PostcodeSearchData>(p =>
                        string.Equals(p.Search, $"\"{postcode}\"", StringComparison.Ordinal) &&
                        p.Count &&
                        p.Top == 1 &&
                        string.Equals(p.Filter, filterName, StringComparison.Ordinal))))
                .Returns(Task.FromResult(postcodeSearchResult)); 

           _gpLookupClient
                .Setup(x => x.GpPostcodeSearch(It.Is<OrganisationPostcodeSearchData>(p =>
                    p.Search.Equals("Metrics:("+EPSEnabledMetricID+")", StringComparison.Ordinal) &&
                    p.QueryType.Equals("full", StringComparison.Ordinal) &&
                    p.SearchMode.Equals("all", StringComparison.Ordinal) &&
                    p.Filter.Contains("OrganisationSubType eq 'Community Pharmacy'", StringComparison.Ordinal) &&
                    p.OrderBy.Equals($"geo.distance(Geocode, geography'POINT({longitude} {latitude})')", StringComparison.Ordinal) &&
                    p.Top == _gpLookupConfig.PharmacySearchApiLimit)))
                .Returns(Task.FromResult(pharmacyResponse));

           _nhsSearchResultChecker
               .Setup(x => x.CheckPharmacies(pharmacyResponse, It.IsAny<string>()))
               .Returns(pharmacySearchResponse);

           var pharmacyDetailsList = new List<PharmacyDetails>()
           {
               new PharmacyDetails()
           };

           _mockMapper.Setup(
                   x => x.Map(It.IsAny<List<Organisation>>(), It.IsAny<GeoCoordinate>()))
               .Returns(pharmacyDetailsList);
           
            // Act
            var result = await _pharmacySearchService.Search(postcode);

            // Assert
            var response = result.Should().BeAssignableTo<PharmacySearchResult.Success>().Subject;
            response.Pharmacies.Count().Should().Be(1);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public async Task
            Search_WhenCalledWithInvalidPostcode_ReturnsError(string postcode)
        {
            // Act
            var result = await _pharmacySearchService.Search(postcode);

            // Assert
            result.Should().BeAssignableTo<PharmacySearchResult.BadRequest>();
        }  
        
        [TestMethod]
        [DataRow("High Street")]
        public async Task 
            Search_WhenCalledWithFreeText_ReturnsEmptyResponse(string postcode)
        {
            // Arrange
            var postcodeSearchResult =
                new GpLookupClient.NhsSearchApiObjectResponse<NhsPostcodeSearchResponse>(HttpStatusCode.OK)
                {
                    Body = null
                };
            
            _gpLookupClient
                .Setup(x => x.PostcodeSearch(It.IsAny<PostcodeSearchData>()))
                .Returns(Task.FromResult(postcodeSearchResult));

            // Act
            var result = await _pharmacySearchService.Search(postcode);

            // Assert
            result.Should().BeAssignableTo<PharmacySearchResult.InvalidPostcode>();
        } 
        
        [TestMethod]
        [DataRow("BT484AB")]
        public async Task
            Search_WhenCalledWithNotFoundPostcode_ReturnsEmptyListSuccess(string postcode)
        {
            // Arrange
            var postcodeSearchResult =
                new GpLookupClient.NhsSearchApiObjectResponse<NhsPostcodeSearchResponse>(HttpStatusCode.OK)
                {
                    Body = new NhsPostcodeSearchResponse 
                    { 
                        PostcodeData = new List<PostcodeData>()
                    },
                };
            
            _gpLookupClient
                .Setup(x => x.PostcodeSearch(It.IsAny<PostcodeSearchData>()))
                .Returns(Task.FromResult(postcodeSearchResult));

            // Act
            var result = await _pharmacySearchService.Search(postcode);

            // Assert
            result.Should().BeAssignableTo<PharmacySearchResult.PostcodeResultFailure>();
        }
    }
}
