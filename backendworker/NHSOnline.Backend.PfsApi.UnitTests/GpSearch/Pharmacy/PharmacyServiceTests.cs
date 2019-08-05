using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.GpSearch;
using NHSOnline.Backend.PfsApi.GpSearch.Models;
using NHSOnline.Backend.PfsApi.GpSearch.Pharmacy;
using NHSOnline.Backend.PfsApi.GpSearch.Models.Pharmacy;

namespace NHSOnline.Backend.PfsApi.UnitTests.GpSearch.Pharmacy
{
    [TestClass]
    public class PharmacyServiceTests
    {
        private PharmacyService _pharmacyService;
        private IFixture _fixture;
        private Mock<IGpLookupClient> _gpLookupClient;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _gpLookupClient = _fixture.Freeze<Mock<IGpLookupClient>>();

            _pharmacyService = _fixture.Create<PharmacyService>();
        }

        [TestMethod]
        public async Task
            GetPharmacyDetail_WhenCalledWithValidODSCode_ReturnsPharmacyDetail()
        {
            // Arrange
            var validOdsCode = _fixture.Create<string>();

            var organisationReturnResults = new List<Organisation>
            {
                _fixture.Create<Organisation>(),
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

            _gpLookupClient
                .Setup(x => x.PharmacySearch(It.IsAny<OrganisationSearchData>()))
                .Returns(Task.FromResult(organisationReturnResult));


            // Act
            var result = await _pharmacyService.GetPharmacyDetail(validOdsCode);

            // Assert
            var response = result.Should().BeAssignableTo<PharmacyDetailResponse>().Subject;
            response.StatusCode.Should().Be(200);
            response.Pharmacy.Should().BeEquivalentTo(organisationReturnResults[0]);
        }

        [TestMethod]
        public async Task
            GetPharmacyDetail_WhenCalledWithInvalidODSCode_ReturnsArgumentException()
        {
            // Arrange
            var validOdsCode = " ";

            // Act
            var result = await _pharmacyService.GetPharmacyDetail(validOdsCode);

            // Assert
            var response = result.Should().BeAssignableTo<PharmacyDetailResponse>().Subject;
            response.StatusCode.Should().Be(500);
            response.Pharmacy.Should().BeNull();
        }

        [TestMethod]
        public async Task
            GetPharmacyDetail_WhenCalledWithValidODSCode_ReturnsBadRequest()
        {
            // Arrange
            var validOdsCode = _fixture.Create<string>();

            var organisationReturnResult =
                new GpLookupClient.NhsSearchApiObjectResponse<NhsOrganisationSearchResponse>(HttpStatusCode.BadRequest);

            _gpLookupClient
                .Setup(x => x.PharmacySearch(It.IsAny<OrganisationSearchData>()))
                .Returns(Task.FromResult(organisationReturnResult));


            // Act
            var result = await _pharmacyService.GetPharmacyDetail(validOdsCode);

            // Assert
            var response = result.Should().BeAssignableTo<PharmacyDetailResponse>().Subject;
            response.StatusCode.Should().Be(400);
            response.Pharmacy.Should().BeNull();
        }

        [TestMethod]
        public async Task
            GetPharmacyDetail_WhenCalledWithValidODSCode_ReturnsOKButNoPharmacy_Should()
        {
            // Arrange
            var validOdsCode = _fixture.Create<string>();

            var organisationReturnResult =
                new GpLookupClient.NhsSearchApiObjectResponse<NhsOrganisationSearchResponse>(HttpStatusCode.OK)
                {
                    Body = null
                };

            _gpLookupClient
                .Setup(x => x.PharmacySearch(It.IsAny<OrganisationSearchData>()))
                .Returns(Task.FromResult(organisationReturnResult));


            // Act
            var result = await _pharmacyService.GetPharmacyDetail(validOdsCode);

            // Assert
            var response = result.Should().BeAssignableTo<PharmacyDetailResponse>().Subject;
            response.StatusCode.Should().Be(404);
            response.Pharmacy.Should().BeNull();
        }

        [TestMethod]
        public async Task
            GetPharmacyDetail_WhenCalledWithValidODSCode_ThrowsAnException()
        {
            // Arrange
            var validOdsCode = _fixture.Create<string>();

            _gpLookupClient
                .Setup(x => x.PharmacySearch(It.IsAny<OrganisationSearchData>()))
                .ThrowsAsync(new HttpRequestException())
                .Verifiable();

            // Act
            var result = await _pharmacyService.GetPharmacyDetail(validOdsCode);

            // Assert
            _gpLookupClient.Verify();
            result.Should().BeAssignableTo<PharmacyDetailResponse>();
            result.StatusCode.Should().Be(503);
            result.Pharmacy.Should().BeNull();
        }
        
        [TestMethod]
        [DataRow(Constants.OrganisationSubTypeForInternetPharmacy, false)]
        [DataRow(Constants.OrganisationSubTypeForCommunityPharmacy, true)]
        [DataRow(null, false)]
        public void IsValidPharmacySubType_ReturnsCorrectResult_ForDifferentOrganisationSubTypes(string organisationSubType, bool expectedResult)
        {
            // Arrange
            var pharmacyDetailResponse = _fixture.Create<PharmacyDetailResponse>();
            pharmacyDetailResponse.Pharmacy.OrganisationSubType = organisationSubType;

            // Act
            var result = _pharmacyService.IsValidPharmacySubType(pharmacyDetailResponse);

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}
