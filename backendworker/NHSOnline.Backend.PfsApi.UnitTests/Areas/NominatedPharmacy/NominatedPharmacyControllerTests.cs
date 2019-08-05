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
using NHSOnline.Backend.NominatedPharmacy;
using NHSOnline.Backend.NominatedPharmacy.Models;
using NHSOnline.Backend.PfsApi.GpSearch.Models.Pharmacy;
using System.Net;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy;
using NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy.Models;
using NHSOnline.Backend.PfsApi.GpSearch.Models;
using NHSOnline.Backend.PfsApi.GpSearch.Pharmacy;
using NHSOnline.Backend.PfsApi.GpSearch;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.NominatedPharmacy
{
    [TestClass]
    public class NominatedPharmacyControllerTests
    {
        private const string OdsCode = "AB123";
        private const string UpdatedOdsCode = "BB999";
        private const string NominatedPharmacyType = "P1";
        private readonly string _pertinentSerialChangeNumber = Guid.NewGuid().ToString();
        
        private NominatedPharmacyController _systemUnderTest;
        private IFixture _fixture;
        private UserSession _userSession;

        private Mock<INominatedPharmacyService> _mockNominatedPharmacyService;
        private Mock<IPharmacySearchService> _mockPharmacySearchService;
        private Mock<IPharmacyService> _mockPharmacyService;
        private Mock<INominatedPharmacyGatewayUpdateService> _mockNominatedPharmacyGatewayUpdateService;
        private Mock<IPharmacyDetailsToPharmacyDetailsResponseMapper> _mockMapper;
        private Mock<IAuditor> _auditor;
        private Mock<INominatedPharmacyConfigurationSettings> _configMock;
        private Mock<IGpSearchService> _mockGpSearchService;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _fixture.Customize<UserSession>(c => c
                .With(u => u.GpUserSession, _fixture.Create<EmisUserSession>()));

            _userSession = _fixture.Create<UserSession>();

            _mockNominatedPharmacyService = _fixture.Freeze<Mock<INominatedPharmacyService>>();
            _mockPharmacyService = _fixture.Freeze<Mock<IPharmacyService>>();
            _mockPharmacySearchService = _fixture.Freeze<Mock<IPharmacySearchService>>();
            _mockNominatedPharmacyGatewayUpdateService = _fixture.Freeze<Mock<INominatedPharmacyGatewayUpdateService>>();
            _mockMapper = _fixture.Freeze<Mock<IPharmacyDetailsToPharmacyDetailsResponseMapper>>();
            _auditor = _fixture.Freeze<Mock<IAuditor>>();
            _configMock = _fixture.Freeze<Mock<INominatedPharmacyConfigurationSettings>>();
            _configMock.SetupGet(x => x.IsNominatedPharmacyEnabled).Returns(true);
            _mockGpSearchService = _fixture.Freeze<Mock<IGpSearchService>>();

            var httpContextItems = new Dictionary<object, object>
            {
                { Support.Constants.HttpContextItems.UserSession, _userSession }
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
        public async Task Get_ReturnsSuccessfulResult_WhenServicesReturnsSuccessfully()
        {
            // Arrange
            string nhsNumber = _userSession.GpUserSession.NhsNumber;

            var nominatedPharmacyResult = new GetNominatedPharmacyResult(HttpStatusCode.OK, OdsCode, _pertinentSerialChangeNumber, true, NominatedPharmacyType);
            
            var pharmacyOrgansation = _fixture.Create<Organisation>();
            var pharmacyDetailResponse = new PharmacyDetailResponse(HttpStatusCode.OK, pharmacyOrgansation);
            var isGpPracticeEpsEnabledResponse = new IsGpPracticeEpsEnabledResponse(HttpStatusCode.OK, true);

            _mockGpSearchService
                .Setup(x => x.IsGpPracticeEPSEnabled(_userSession.GpUserSession.OdsCode))
                .Returns(Task.FromResult(isGpPracticeEpsEnabledResponse))
                .Verifiable();

            _mockNominatedPharmacyService
                .Setup(x => x.GetNominatedPharmacy(nhsNumber, _userSession.CitizenIdUserSession))
                .Returns(Task.FromResult(nominatedPharmacyResult))
                .Verifiable();

            _mockPharmacyService
                .Setup(x => x.GetPharmacyDetail(OdsCode))
                .Returns(Task.FromResult(pharmacyDetailResponse))
                .Verifiable();
            
            _mockPharmacyService
                .Setup(x => x.IsValidPharmacySubType(pharmacyDetailResponse))
                .Returns(true)
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
            _mockGpSearchService.Verify();

            var value = result.Should().BeAssignableTo<OkObjectResult>().Subject.Value;
            value.Should().BeEquivalentTo(new PharmacyDetailsResponse{ NominatedPharmacyEnabled = true, PharmacyDetails = mappedResult });
            mappedResult.PharmacyType.Should().Be(NominatedPharmacyType);
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResultWithEnabledFalse_WhenNominatedPharmacyNotEnabled()
        {
            // Arrange
            string nhsNumber = _userSession.GpUserSession.NhsNumber;

            PharmacyDetailsResponse response = new PharmacyDetailsResponse
            {
                NominatedPharmacyEnabled = false,
                PharmacyDetails = null
            };

            _configMock.SetupGet(x => x.IsNominatedPharmacyEnabled).Returns(false);

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _mockGpSearchService.Verify(x => x.IsGpPracticeEPSEnabled(It.IsAny<string>()), Times.Never);
            _mockNominatedPharmacyService.Verify(x => x.GetNominatedPharmacy(It.IsAny<string>(), It.IsAny<CitizenIdUserSession>()), Times.Never);
            _mockPharmacyService.Verify(x => x.GetPharmacyDetail(It.IsAny<string>()), Times.Never);
            _mockMapper.Verify(x => x.Map(It.IsAny<Organisation>()), Times.Never);

            var value = result.Should().BeAssignableTo<OkObjectResult>().Subject;
            value.StatusCode.Should().Be(StatusCodes.Status200OK);
            value.Value.Should().BeEquivalentTo(response);
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResultWithEnabledFalse_WhenGpPracticeIsNotEnabled()
        {
            // Arrange
            string nhsNumber = _userSession.GpUserSession.NhsNumber;

            PharmacyDetailsResponse response = new PharmacyDetailsResponse
            {
                NominatedPharmacyEnabled = false,
                PharmacyDetails = null
            };

            var isGpPracticeEpsEnabledResponse = new IsGpPracticeEpsEnabledResponse(HttpStatusCode.OK, false);

            _mockGpSearchService
                .Setup(x => x.IsGpPracticeEPSEnabled(_userSession.GpUserSession.OdsCode))
                .Returns(Task.FromResult(isGpPracticeEpsEnabledResponse))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _mockGpSearchService.Verify();
            _mockNominatedPharmacyService.Verify(x => x.GetNominatedPharmacy(It.IsAny<string>(), It.IsAny<CitizenIdUserSession>()), Times.Never);
            _mockPharmacyService.Verify(x => x.GetPharmacyDetail(It.IsAny<string>()), Times.Never);
            _mockMapper.Verify(x => x.Map(It.IsAny<Organisation>()), Times.Never);

            var value = result.Should().BeAssignableTo<OkObjectResult>().Subject;
            value.StatusCode.Should().Be(StatusCodes.Status200OK);
            value.Value.Should().BeEquivalentTo(response);
        }

        [TestMethod]
        public async Task Get_ReturnsErrorResultWithEnabledFalse_WhenGpSearchReturnsInternalServerError()
        {
            // Arrange
            string nhsNumber = _userSession.GpUserSession.NhsNumber;

            var isGpPracticeEpsEnabledResponse = new IsGpPracticeEpsEnabledResponse(HttpStatusCode.InternalServerError);

            _mockGpSearchService
                .Setup(x => x.IsGpPracticeEPSEnabled(_userSession.GpUserSession.OdsCode))
                .Returns(Task.FromResult(isGpPracticeEpsEnabledResponse))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _mockGpSearchService.Verify();
            _mockNominatedPharmacyService.Verify(x => x.GetNominatedPharmacy(It.IsAny<string>(), It.IsAny<CitizenIdUserSession>()), Times.Never);
            _mockPharmacyService.Verify(x => x.GetPharmacyDetail(It.IsAny<string>()), Times.Never);
            _mockMapper.Verify(x => x.Map(It.IsAny<Organisation>()), Times.Never);

            var value = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            value.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [DataTestMethod]
        [DataRow(HttpStatusCode.BadRequest)]
        [DataRow(HttpStatusCode.NotFound)]
        [DataRow(HttpStatusCode.BadGateway)]
        public async Task Get_ReturnsErrorResultWithEnabledFalse_WhenGpSearchReturnsErrorOtherThanInternalServerError(HttpStatusCode httpStatusCode)
        {
            // Arrange
            string nhsNumber = _userSession.GpUserSession.NhsNumber;

            var isGpPracticeEpsEnabledResponse = new IsGpPracticeEpsEnabledResponse(httpStatusCode);

            _mockGpSearchService
                .Setup(x => x.IsGpPracticeEPSEnabled(_userSession.GpUserSession.OdsCode))
                .Returns(Task.FromResult(isGpPracticeEpsEnabledResponse))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _mockGpSearchService.Verify();
            _mockNominatedPharmacyService.Verify(x => x.GetNominatedPharmacy(It.IsAny<string>(), It.IsAny<CitizenIdUserSession>()), Times.Never);
            _mockPharmacyService.Verify(x => x.GetPharmacyDetail(It.IsAny<string>()), Times.Never);
            _mockMapper.Verify(x => x.Map(It.IsAny<Organisation>()), Times.Never);

            var value = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            value.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }
        
        [TestMethod]
        public async Task Get_ReturnsSuccessWithEnabledFalse_WhenIsValidPharmacySubTypeReturnsFalse()
        {
            // Arrange
            string nhsNumber = _userSession.GpUserSession.NhsNumber;

            var nominatedPharmacyResult = new GetNominatedPharmacyResult(HttpStatusCode.OK, OdsCode, _pertinentSerialChangeNumber, true, NominatedPharmacyType);
            
            var pharmacyOrgansation = _fixture.Create<Organisation>();
            var pharmacyDetailResponse = new PharmacyDetailResponse(HttpStatusCode.OK, pharmacyOrgansation);
            var isGpPracticeEpsEnabledResponse = new IsGpPracticeEpsEnabledResponse(HttpStatusCode.OK, true);

            _mockGpSearchService
                .Setup(x => x.IsGpPracticeEPSEnabled(_userSession.GpUserSession.OdsCode))
                .Returns(Task.FromResult(isGpPracticeEpsEnabledResponse))
                .Verifiable();

            _mockNominatedPharmacyService
                .Setup(x => x.GetNominatedPharmacy(nhsNumber, _userSession.CitizenIdUserSession))
                .Returns(Task.FromResult(nominatedPharmacyResult))
                .Verifiable();

            _mockPharmacyService
                .Setup(x => x.GetPharmacyDetail(OdsCode))
                .Returns(Task.FromResult(pharmacyDetailResponse))
                .Verifiable(); 
            
            _mockPharmacyService
                .Setup(x => x.IsValidPharmacySubType(pharmacyDetailResponse))
                .Returns(false)
                .Verifiable(); 

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _mockNominatedPharmacyService.Verify();
            _mockGpSearchService.Verify();
            _mockPharmacyService.Verify();

            var value = result.Should().BeAssignableTo<OkObjectResult>().Subject.Value;
            value.Should().BeEquivalentTo(new PharmacyDetailsResponse{ NominatedPharmacyEnabled = false, PharmacyDetails = null });
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

            var nominatedPharmacyResult = new GetNominatedPharmacyResult(HttpStatusCode.OK, odsCode, _pertinentSerialChangeNumber, true, NominatedPharmacyType);
            var isGpPracticeEpsEnabledResponse = new IsGpPracticeEpsEnabledResponse(HttpStatusCode.OK, true);

            _mockGpSearchService
                .Setup(x => x.IsGpPracticeEPSEnabled(_userSession.GpUserSession.OdsCode))
                .Returns(Task.FromResult(isGpPracticeEpsEnabledResponse))
                .Verifiable();

            _mockNominatedPharmacyService
                .Setup(x => x.GetNominatedPharmacy(nhsNumber, _userSession.CitizenIdUserSession))
                .Returns(Task.FromResult(nominatedPharmacyResult))
                .Verifiable();
            
            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _mockGpSearchService.Verify();
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
            var isGpPracticeEpsEnabledResponse = new IsGpPracticeEpsEnabledResponse(HttpStatusCode.OK, true);

            _mockGpSearchService
                .Setup(x => x.IsGpPracticeEPSEnabled(_userSession.GpUserSession.OdsCode))
                .Returns(Task.FromResult(isGpPracticeEpsEnabledResponse))
                .Verifiable();

            _mockNominatedPharmacyService
                .Setup(x => x.GetNominatedPharmacy(nhsNumber, _userSession.CitizenIdUserSession))
                .Returns(Task.FromResult(nominatedPharmacyResult))
                .Verifiable();
            
            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _mockGpSearchService.Verify();
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

            var nominatedPharmacyResult = new GetNominatedPharmacyResult(HttpStatusCode.OK, OdsCode, _pertinentSerialChangeNumber, true, NominatedPharmacyType );
            var isGpPracticeEpsEnabledResponse = new IsGpPracticeEpsEnabledResponse(HttpStatusCode.OK, true);

            _mockGpSearchService
                .Setup(x => x.IsGpPracticeEPSEnabled(_userSession.GpUserSession.OdsCode))
                .Returns(Task.FromResult(isGpPracticeEpsEnabledResponse))
                .Verifiable();

            _mockNominatedPharmacyService
                .Setup(x => x.GetNominatedPharmacy(nhsNumber, _userSession.CitizenIdUserSession))
                .Returns(Task.FromResult(nominatedPharmacyResult))
                .Verifiable();

            var pharmacyDetailResponse = new PharmacyDetailResponse(HttpStatusCode.BadRequest);

            _mockPharmacyService
                .Setup(x => x.GetPharmacyDetail(OdsCode))
                .Returns(Task.FromResult(pharmacyDetailResponse))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _mockGpSearchService.Verify();
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
                .Setup(x => x.UpdateNominatedPharmacy(nhsNumber, UpdatedOdsCode, _userSession.CitizenIdUserSession))
                .Returns(Task.FromResult(new StatusCodeResult((int)HttpStatusCode.OK)))
                .Verifiable();
            
            var updateNominatedPharmacyRequest = new UpdateNominatedPharmacyRequest
            {
                OdsCode = UpdatedOdsCode
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
                .Setup(x => x.UpdateNominatedPharmacy(nhsNumber, UpdatedOdsCode, _userSession.CitizenIdUserSession))
                .Throws<Exception>()
                .Verifiable();

            var updateNominatedPharmacyRequest = new UpdateNominatedPharmacyRequest
            {
                OdsCode = UpdatedOdsCode
            };

            // Act
            var result = await _systemUnderTest.Update(updateNominatedPharmacyRequest);

            // Assert
            _mockNominatedPharmacyGatewayUpdateService.Verify();
            var value = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            value.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        public async Task Update_Returns403Forbidden_WhenNominatedPharmacyNotEnabled()
        {
            // Arrange
            _configMock.SetupGet(x => x.IsNominatedPharmacyEnabled).Returns(false);
            
            var updateNominatedPharmacyRequest = new UpdateNominatedPharmacyRequest
            {
                OdsCode = UpdatedOdsCode
            };

            // Act
            var result = await _systemUnderTest.Update(updateNominatedPharmacyRequest);

            // Assert
            _mockNominatedPharmacyGatewayUpdateService.Verify(x => x.UpdateNominatedPharmacy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CitizenIdUserSession>()), Times.Never);
            var value = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            value.StatusCode.Should().Be((int)HttpStatusCode.Forbidden);
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
            _auditor.Verify(x => x.Audit(AuditingOperations.SearchNominatedPharmacyAuditTypeResponse, It.IsAny<string>()));

            var value = result.Should().BeAssignableTo<OkObjectResult>().Subject.Value;
            value.Should().BeEquivalentTo(mappedResult);
        }

        [DataTestMethod]
        [DataRow(HttpStatusCode.BadRequest, HttpStatusCode.BadGateway)]
        [DataRow(HttpStatusCode.Forbidden, HttpStatusCode.BadGateway)]
        [DataRow(HttpStatusCode.InternalServerError, HttpStatusCode.InternalServerError)]
        public async Task Search_ReturnsCorrectStatusCode_WhenServiceReturnsWithNonSuccessStatusCode(HttpStatusCode httpStatusCodeFromService, HttpStatusCode expectedResultStatusCode)
        {
            // Arrange
            string postcode = "ABC";
            
            var pharmacySearchResponse = new PharmacySearchResponse(httpStatusCodeFromService);

            _mockPharmacySearchService
                .Setup(x => x.Search(postcode))
                .Returns(Task.FromResult(pharmacySearchResponse))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Search(postcode);

            // Assert
            _mockPharmacySearchService.Verify();

            var value = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            value.StatusCode.Should().Be((int)expectedResultStatusCode);
        }

        [TestMethod]
        public async Task Search_Returns500InternalServerError_WhenExceptionOccursInMapper()
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
                .Throws<Exception>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Search(postcode);

            // Assert
            _mockPharmacySearchService.Verify();
            _mockMapper.Verify();

            var value = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            value.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}
