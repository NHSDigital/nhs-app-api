using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.PfsApi.GpSearch;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
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

        private IFixture _fixture;
   
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            
            _logger = _fixture.Freeze<ILogger<PharmacySearchService>>();
            _gpLookupClient = new Mock<IGpLookupClient>();
            _gpLookupConfig = _fixture.Freeze<IGpLookupConfig>();
            _nhsSearchResultChecker = new Mock<INhsSearchResultChecker>();
            _postcodeParser = new PostcodeParser();

            _pharmacySearchService = new PharmacySearchService(_logger, _gpLookupClient.Object, _gpLookupConfig, _nhsSearchResultChecker.Object,
                _postcodeParser); 
        }

        [TestMethod]
        [DataRow("SE13", "Type eq 'PostcodeOutCode'")]
        [DataRow("SE13 6JZ", "LocalType eq 'Postcode'")]
        public async Task
            Search_WhenCalledWithValidPostcode_ReturnsListOfPharmacies(string postcode, string filterName)
        {
            // Arrange
           var postcodeSearchResult =
                new GpLookupClient.NhsSearchApiObjectResponse<NhsPostcodeSearchResponse>(HttpStatusCode.OK)
                {
                    Body = new NhsPostcodeSearchResponse 
                    { 
                        PostcodeData = new List<PostcodeData>
                        {
                            new PostcodeData
                            {
                                Latitude = "1",
                                Longitude = "2"
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
                .Setup(x => x.GpPostcodeSearch(It.IsAny<OrganisationPostcodeSearchData>()))
                .Returns(Task.FromResult(pharmacyResponse));

           _nhsSearchResultChecker
               .Setup(x => x.CheckPharmacies(pharmacyResponse, It.IsAny<string>()))
               .Returns(pharmacySearchResponse);
            
            // Act
            var result = await _pharmacySearchService.Search(postcode);

            // Assert
            var response = result.Should().BeAssignableTo<PharmacySearchResponse>().Subject;
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.PostcodeCoordinate.Latitude.Should().Be(1);
            response.PostcodeCoordinate.Longitude.Should().Be(2);
            response.Pharmacies.Count.Should().Be(1);
            response.Pharmacies[0].OrganisationName.Should().Be("organisation_name_from_checker");
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
            var response = result.Should().BeAssignableTo<PharmacySearchResponse>().Subject;
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
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
            var response = result.Should().BeAssignableTo<PharmacySearchResponse>().Subject;
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Pharmacies.Count.Should().Be(0);
        }
    }
}
