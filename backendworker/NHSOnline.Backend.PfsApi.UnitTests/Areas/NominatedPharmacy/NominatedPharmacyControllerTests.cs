using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Support;
using UnitTestHelper;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.NominatedPharmacy;
using NHSOnline.Backend.NominatedPharmacy.Models;
using NHSOnline.Backend.Worker.GpSearch.Models.Pharmacy;
using System.Net;
using NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy;
using NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy.Models;
using NHSOnline.Backend.PfsApi.GpSearch.Models;
using NHSOnline.Backend.PfsApi.GpSearch.Pharmacy;
using NHSOnline.Backend.PfsApi.GpSearch.Models.Pharmacy;
using NHSOnline.Backend.Support.Auditing;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.NominatedPharmacy
{
    [TestClass]
    public class NominatedPharmacyControllerTests
    {
        const string odsCode = "AB123";
        const string updatedOdsCode = "BB999";
        const string NominatedPharmacyType = "P1";
        string pertinentSerialChangeNumber = Guid.NewGuid().ToString();
        
        private NominatedPharmacyController _systemUnderTest;
        private IFixture _fixture;
        private UserSession _userSession;

        private Mock<ILogger<NominatedPharmacyController>> _mockLogger;
        private Mock<INominatedPharmacyService> _mockNominatedPharmacyService;
        private Mock<IPharmacySearchService> _mockPharmacySearchService;
        private Mock<IPharmacyService> _mockPharmacyService;
        private Mock<INominatedPharmacyGatewayUpdateService> _mockNominatedPharmacyGatewayUpdateService;
        private Mock<IPharmacyDetailsToPharmacyDetailsResponseMapper> _mockMapper;
        private Mock<IAuditor> _auditor;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _fixture.Customize<UserSession>(c => c
                .With(u => u.GpUserSession, _fixture.Create<EmisUserSession>()));

            _userSession = _fixture.Create<UserSession>();

            _mockLogger = _fixture.Freeze<Mock<ILogger<NominatedPharmacyController>>>();
            _mockNominatedPharmacyService = _fixture.Freeze<Mock<INominatedPharmacyService>>();
            _mockPharmacyService = _fixture.Freeze<Mock<IPharmacyService>>();
            _mockPharmacySearchService = _fixture.Freeze<Mock<IPharmacySearchService>>();
            _mockNominatedPharmacyGatewayUpdateService = _fixture.Freeze<Mock<INominatedPharmacyGatewayUpdateService>>();
            _mockMapper = _fixture.Freeze<Mock<IPharmacyDetailsToPharmacyDetailsResponseMapper>>();
            _auditor = _fixture.Freeze<Mock<IAuditor>>();

            var httpContextItems = new Dictionary<object, object>
            {
                { Constants.HttpContextItems.UserSession, _userSession }
            };

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.SetupGet(x => x.Items).Returns(httpContextItems);
            
            _systemUnderTest = _fixture.Create<NominatedPharmacyController>();

            _systemUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResult_WhenServiceReturnsSuccessfully()
        {
            // Arrange
            string nhsNumber = _userSession.GpUserSession.NhsNumber;

            var nominatedPharmacyResult = new GetNominatedPharmacyResult(HttpStatusCode.OK, odsCode, pertinentSerialChangeNumber, true, NominatedPharmacyType);
            
            var pharmacyOrgansation = _fixture.Create<Organisation>();
            var pharmacyDetailResponse = new PharmacyDetailResponse(HttpStatusCode.OK, pharmacyOrgansation);
            
            _mockNominatedPharmacyService
                .Setup(x => x.GetNominatedPharmacy(nhsNumber))
                .Returns(Task.FromResult(nominatedPharmacyResult))
                .Verifiable();

            _mockPharmacyService
                .Setup(x => x.GetPharmacyDetail(odsCode))
                .Returns(Task.FromResult(pharmacyDetailResponse))
                .Verifiable();

            var mappedResult = _fixture.Create<PharmacyDetails>();

            _mockMapper
                .Setup(x => x.Map(pharmacyDetailResponse.Pharmacy))
                .Returns(mappedResult)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _mockNominatedPharmacyService.Verify();
            _mockPharmacyService.Verify();
            _mockMapper.Verify();

            var value = result.Should().BeAssignableTo<OkObjectResult>().Subject.Value;
            value.Should().BeEquivalentTo(new PharmacyDetailsResponse{ NominatedPharmacyEnabled = true, PharmacyDetails = mappedResult });
            mappedResult.PharmacyType.Should().Be(NominatedPharmacyType);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public async Task Get_ReturnsSuccessfulResult_WhenNoNominatedPharmacySet(string odsCode)
        {
            // Arrange
            string nhsNumber = _userSession.GpUserSession.NhsNumber;

            PharmacyDetailsResponse response = new PharmacyDetailsResponse
            {
                NominatedPharmacyEnabled = true,
                PharmacyDetails = null
            };

            var nominatedPharmacyResult = new GetNominatedPharmacyResult(HttpStatusCode.OK, odsCode, pertinentSerialChangeNumber, true, NominatedPharmacyType);
            
            _mockNominatedPharmacyService
                .Setup(x => x.GetNominatedPharmacy(nhsNumber))
                .Returns(Task.FromResult(nominatedPharmacyResult))
                .Verifiable();
            
            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _mockNominatedPharmacyService.Verify();
            _mockPharmacyService.Verify(x => x.GetPharmacyDetail(It.IsAny<string>()), Times.Never);
            _mockMapper.Verify(x => x.Map(It.IsAny<Organisation>()), Times.Never);

            var value = result.Should().BeAssignableTo<OkObjectResult>().Subject;
            value.StatusCode.Should().Be(StatusCodes.Status200OK);
            value.Value.Should().BeEquivalentTo(response);
        }

        [TestMethod]
        public async Task Get_Returns500AndDoesNotCallPharmacySearch_IfSpineSearchForNominatedPharmacyIsUnsuccessful()
        {
            // Arrange
            string nhsNumber = _userSession.GpUserSession.NhsNumber;
            var nominatedPharmacyResult = new GetNominatedPharmacyResult(HttpStatusCode.InternalServerError);
            
            _mockNominatedPharmacyService
                .Setup(x => x.GetNominatedPharmacy(nhsNumber))
                .Returns(Task.FromResult(nominatedPharmacyResult))
                .Verifiable();
            
            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _mockNominatedPharmacyService.Verify();
            _mockPharmacyService.Verify(x => x.GetPharmacyDetail(It.IsAny<string>()), Times.Never);
            _mockMapper.Verify(x => x.Map(It.IsAny<Organisation>()), Times.Never);

            var value = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            value.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        public async Task Get_Returns502_IfGetNominatedPharmacySpineIsSuccessfulButGetPharmacyDetailIsUnsuccessful()
        {
            // Arrange
            string nhsNumber = _userSession.GpUserSession.NhsNumber;

            var nominatedPharmacyResult = new GetNominatedPharmacyResult(HttpStatusCode.OK, odsCode, pertinentSerialChangeNumber, true, NominatedPharmacyType );
            
            _mockNominatedPharmacyService
                .Setup(x => x.GetNominatedPharmacy(nhsNumber))
                .Returns(Task.FromResult(nominatedPharmacyResult))
                .Verifiable();

            var pharmacyDetailResponse = new PharmacyDetailResponse(HttpStatusCode.BadRequest);

            _mockPharmacyService
                .Setup(x => x.GetPharmacyDetail(odsCode))
                .Returns(Task.FromResult(pharmacyDetailResponse))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _mockNominatedPharmacyService.Verify();
            _mockPharmacyService.Verify();
            _mockMapper.Verify(x => x.Map(It.IsAny<Organisation>()), Times.Never);

            var value = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            value.StatusCode.Should().Be((int)HttpStatusCode.BadGateway);
        }

        [TestMethod]
        public async Task Update_ReturnsResultFromService_WhenServiceReturnsSuccessfully()
        {
            // Arrange
            string nhsNumber = _userSession.GpUserSession.NhsNumber;

            _mockNominatedPharmacyGatewayUpdateService
                .Setup(x => x.UpdateNominatedPharmacy(nhsNumber, updatedOdsCode))
                .Returns(Task.FromResult(new StatusCodeResult((int)HttpStatusCode.OK)))
                .Verifiable();
            
            var updateNominatedPharmacyRequest = new UpdateNominatedPharmacyRequest
            {
                OdsCode = updatedOdsCode
            };

            // Act
            var result = await _systemUnderTest.Update(updateNominatedPharmacyRequest);

            // Assert
            _mockNominatedPharmacyGatewayUpdateService.Verify();            
            var value = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            value.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task Update_ReturnsInternalServerError_WhenServiceThrowsException()
        {
            // Arrange
            string nhsNumber = _userSession.GpUserSession.NhsNumber;

            _mockNominatedPharmacyGatewayUpdateService
                .Setup(x => x.UpdateNominatedPharmacy(nhsNumber, updatedOdsCode))
                .Throws<Exception>()
                .Verifiable();

            var updateNominatedPharmacyRequest = new UpdateNominatedPharmacyRequest
            {
                OdsCode = updatedOdsCode
            };

            // Act
            var result = await _systemUnderTest.Update(updateNominatedPharmacyRequest);

            // Assert
            _mockNominatedPharmacyGatewayUpdateService.Verify();
            var value = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            value.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        public async Task Search_ReturnsSuccessfulResult_WhenServiceReturnsSuccessfully()
        {
            // Arrange
            string postcode = "ABC";

            var org1 = _fixture.Create<Organisation>();
            var organisations = new List<Organisation> { org1 };

            var postcodeCoordinate = new GeoCoordinatePortable.GeoCoordinate
            {
                Latitude = 1,
                Longitude = 2,
            };

            var pharmacySearchResponse = new PharmacySearchResponse(HttpStatusCode.OK, organisations)
            {
                PostcodeCoordinate = postcodeCoordinate,
            };

            _mockPharmacySearchService
                .Setup(x => x.Search(postcode))
                .Returns(Task.FromResult(pharmacySearchResponse))
                .Verifiable();

            var pharmacy1 = _fixture.Create<PharmacyDetails>();
            var mappedResult = new List<PharmacyDetails> { pharmacy1 };

            _mockMapper
                .Setup(x => x.Map(organisations, postcodeCoordinate))
                .Returns(mappedResult)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Search(postcode);

            // Assert
            _mockPharmacySearchService.Verify();
            _mockMapper.Verify();
            _auditor.Verify(x => x.Audit(Constants.AuditingTitles.SearchNominatedPharmacyAuditTypeResponse, It.IsAny<string>()));

            var value = result.Should().BeAssignableTo<OkObjectResult>().Subject.Value;
            value.Should().BeEquivalentTo(mappedResult);
        }
    }
}
