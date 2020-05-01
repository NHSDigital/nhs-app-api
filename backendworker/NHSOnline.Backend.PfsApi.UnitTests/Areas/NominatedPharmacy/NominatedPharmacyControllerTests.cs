using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.NominatedPharmacy;
using NHSOnline.Backend.NominatedPharmacy.Models;
using NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy;
using NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy.Models;
using NHSOnline.Backend.PfsApi.GpSearch;
using NHSOnline.Backend.PfsApi.GpSearch.Models;
using NHSOnline.Backend.PfsApi.GpSearch.Models.Pharmacy;
using NHSOnline.Backend.PfsApi.GpSearch.Pharmacy;
using NHSOnline.Backend.Support;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.NominatedPharmacy
{
    [TestClass]
    public sealed class NominatedPharmacyControllerTests: IDisposable
    {
        private const string OdsCode = "AB123";
        private const string UpdatedOdsCode = "BB999";
        private const string NominatedPharmacyType = "P1";
        private const string ObjectId = "ABXXXY";
        private readonly string _pertinentSerialChangeNumber = Guid.NewGuid().ToString();

        private NominatedPharmacyController _systemUnderTest;
        private P9UserSession _userSession;

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
            _userSession = new P9UserSession("csrfToken", new CitizenIdUserSession(), new EmisUserSession(), "im1token");

            _mockNominatedPharmacyService = new Mock<INominatedPharmacyService>();
            _mockPharmacyService = new Mock<IPharmacyService>();
            _mockPharmacySearchService = new Mock<IPharmacySearchService>();
            _mockNominatedPharmacyGatewayUpdateService = new Mock<INominatedPharmacyGatewayUpdateService>();
            _mockMapper = new Mock<IPharmacyDetailsToPharmacyDetailsResponseMapper>();
            _auditor = new Mock<IAuditor>();
            _configMock = new Mock<INominatedPharmacyConfigurationSettings>();
            _configMock.SetupGet(x => x.IsNominatedPharmacyEnabled).Returns(true);
            _mockGpSearchService = new Mock<IGpSearchService>();

            _systemUnderTest = new NominatedPharmacyController(
                new Mock<ILogger<NominatedPharmacyController>>().Object,
                _mockNominatedPharmacyService.Object,
                _mockPharmacyService.Object,
                _mockMapper.Object,
                _mockPharmacySearchService.Object,
                _mockNominatedPharmacyGatewayUpdateService.Object,
                _auditor.Object,
                _configMock.Object,
                _mockGpSearchService.Object);
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResult_WhenServicesReturnsSuccessfully()
        {
            // Arrange
            var nhsNumber = _userSession.GpUserSession.NhsNumber;

            var nominatedPharmacyResult = new GetNominatedPharmacyResult.Success(
                new GetNominatedPharmacyResponse(HttpStatusCode.OK, OdsCode, _pertinentSerialChangeNumber, true, NominatedPharmacyType, ObjectId));

            var pharmacyOrganisation = new Organisation();
            var pharmacyDetailResponse = new PharmacyDetailResponse(HttpStatusCode.OK, pharmacyOrganisation);
            var isGpPracticeEpsEnabledResponse = new IsGpPracticeEpsEnabledResponse(HttpStatusCode.OK, true);

            _mockGpSearchService
                .Setup(x => x.IsGpPracticeEPSEnabled(_userSession.GpUserSession.OdsCode))
                .Returns(Task.FromResult(isGpPracticeEpsEnabledResponse))
                .Verifiable();

            _mockNominatedPharmacyService
                .Setup(x => x.GetNominatedPharmacy(nhsNumber, _userSession.CitizenIdUserSession))
                .Returns(Task.FromResult<GetNominatedPharmacyResult>(nominatedPharmacyResult))
                .Verifiable();

            _mockPharmacyService
                .Setup(x => x.GetPharmacyDetail(OdsCode))
                .Returns(Task.FromResult(pharmacyDetailResponse))
                .Verifiable();

            _mockPharmacyService
                .Setup(x => x.IsValidPharmacySubType(pharmacyDetailResponse))
                .Returns(true)
                .Verifiable();

            var mappedResult = new PharmacyDetails();

            _mockMapper
                .Setup(x => x.Map(pharmacyDetailResponse.Pharmacy))
                .Returns(mappedResult)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Get(_userSession);

            // Assert
            _mockNominatedPharmacyService.Verify();
            _mockPharmacyService.Verify();
            _mockMapper.Verify();
            _mockGpSearchService.Verify();

            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeEquivalentTo(new PharmacyDetailsResponse{ NominatedPharmacyEnabled = true, PharmacyDetails = mappedResult });
            mappedResult.PharmacyType.Should().Be(NominatedPharmacyType);
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResultWithEnabledFalse_WhenNominatedPharmacyNotEnabled()
        {
            // Arrange
            var response = new PharmacyDetailsResponse
            {
                NominatedPharmacyEnabled = false,
                PharmacyDetails = null
            };

            _configMock.SetupGet(x => x.IsNominatedPharmacyEnabled).Returns(false);

            // Act
            var result = await _systemUnderTest.Get(_userSession);

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
            var response = new PharmacyDetailsResponse
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
            var result = await _systemUnderTest.Get(_userSession);

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
            var isGpPracticeEpsEnabledResponse = new IsGpPracticeEpsEnabledResponse(HttpStatusCode.InternalServerError);

            _mockGpSearchService
                .Setup(x => x.IsGpPracticeEPSEnabled(_userSession.GpUserSession.OdsCode))
                .Returns(Task.FromResult(isGpPracticeEpsEnabledResponse))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Get(_userSession);

            // Assert
            _mockGpSearchService.Verify();
            _mockNominatedPharmacyService.Verify(x => x.GetNominatedPharmacy(It.IsAny<string>(), It.IsAny<CitizenIdUserSession>()), Times.Never);
            _mockPharmacyService.Verify(x => x.GetPharmacyDetail(It.IsAny<string>()), Times.Never);
            _mockMapper.Verify(x => x.Map(It.IsAny<Organisation>()), Times.Never);

            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [DataTestMethod]
        [DataRow(HttpStatusCode.BadRequest)]
        [DataRow(HttpStatusCode.NotFound)]
        [DataRow(HttpStatusCode.BadGateway)]
        public async Task Get_ReturnsErrorResultWithEnabledFalse_WhenGpSearchReturnsErrorOtherThanInternalServerError(HttpStatusCode httpStatusCode)
        {
            // Arrange
            var isGpPracticeEpsEnabledResponse = new IsGpPracticeEpsEnabledResponse(httpStatusCode);

            _mockGpSearchService
                .Setup(x => x.IsGpPracticeEPSEnabled(_userSession.GpUserSession.OdsCode))
                .Returns(Task.FromResult(isGpPracticeEpsEnabledResponse))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Get(_userSession);

            // Assert
            _mockGpSearchService.Verify();
            _mockNominatedPharmacyService.Verify(x => x.GetNominatedPharmacy(It.IsAny<string>(), It.IsAny<CitizenIdUserSession>()), Times.Never);
            _mockPharmacyService.Verify(x => x.GetPharmacyDetail(It.IsAny<string>()), Times.Never);
            _mockMapper.Verify(x => x.Map(It.IsAny<Organisation>()), Times.Never);

            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessWithEnabledFalse_WhenIsValidPharmacySubTypeReturnsFalse()
        {
            // Arrange
            var nhsNumber = _userSession.GpUserSession.NhsNumber;

            var nominatedPharmacyResult = new GetNominatedPharmacyResult.Success(new GetNominatedPharmacyResponse(
                HttpStatusCode.OK, OdsCode, _pertinentSerialChangeNumber, true, NominatedPharmacyType, ObjectId));

            var pharmacyOrganisation = new Organisation();
            var pharmacyDetailResponse = new PharmacyDetailResponse(HttpStatusCode.OK, pharmacyOrganisation);
            var isGpPracticeEpsEnabledResponse = new IsGpPracticeEpsEnabledResponse(HttpStatusCode.OK, true);

            _mockGpSearchService
                .Setup(x => x.IsGpPracticeEPSEnabled(_userSession.GpUserSession.OdsCode))
                .Returns(Task.FromResult(isGpPracticeEpsEnabledResponse))
                .Verifiable();

            _mockNominatedPharmacyService
                .Setup(x => x.GetNominatedPharmacy(nhsNumber, _userSession.CitizenIdUserSession))
                .Returns(Task.FromResult<GetNominatedPharmacyResult>(nominatedPharmacyResult))
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
            var result = await _systemUnderTest.Get(_userSession);

            // Assert
            _mockNominatedPharmacyService.Verify();
            _mockGpSearchService.Verify();
            _mockPharmacyService.Verify();

            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeEquivalentTo(new PharmacyDetailsResponse{ NominatedPharmacyEnabled = false, PharmacyDetails = null });
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public async Task Get_ReturnsSuccessfulResult_WhenNoNominatedPharmacySet(string odsCode)
        {
            // Arrange
            var nhsNumber = _userSession.GpUserSession.NhsNumber;

            var response = new PharmacyDetailsResponse
            {
                NominatedPharmacyEnabled = true,
                PharmacyDetails = null
            };

            var nominatedPharmacyResult = new GetNominatedPharmacyResult.Success(new GetNominatedPharmacyResponse(
                HttpStatusCode.OK, odsCode, _pertinentSerialChangeNumber, true, NominatedPharmacyType, ObjectId));
            var isGpPracticeEpsEnabledResponse = new IsGpPracticeEpsEnabledResponse(HttpStatusCode.OK, true);

            _mockGpSearchService
                .Setup(x => x.IsGpPracticeEPSEnabled(_userSession.GpUserSession.OdsCode))
                .Returns(Task.FromResult(isGpPracticeEpsEnabledResponse))
                .Verifiable();

            _mockNominatedPharmacyService
                .Setup(x => x.GetNominatedPharmacy(nhsNumber, _userSession.CitizenIdUserSession))
                .Returns(Task.FromResult<GetNominatedPharmacyResult>(nominatedPharmacyResult))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Get(_userSession);

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
            var nhsNumber = _userSession.GpUserSession.NhsNumber;
            var nominatedPharmacyResult = new GetNominatedPharmacyResult.InternalServerError();

            var isGpPracticeEpsEnabledResponse = new IsGpPracticeEpsEnabledResponse(HttpStatusCode.OK, true);

            _mockGpSearchService
                .Setup(x => x.IsGpPracticeEPSEnabled(_userSession.GpUserSession.OdsCode))
                .Returns(Task.FromResult(isGpPracticeEpsEnabledResponse))
                .Verifiable();

            _mockNominatedPharmacyService
                .Setup(x => x.GetNominatedPharmacy(nhsNumber, _userSession.CitizenIdUserSession))
                .Returns(Task.FromResult<GetNominatedPharmacyResult>(nominatedPharmacyResult))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Get(_userSession);

            // Assert
            _mockGpSearchService.Verify();
            _mockNominatedPharmacyService.Verify();
            _mockPharmacyService.Verify(x => x.GetPharmacyDetail(It.IsAny<string>()), Times.Never);
            _mockMapper.Verify(x => x.Map(It.IsAny<Organisation>()), Times.Never);

            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        public async Task Get_Returns502_IfGetNominatedPharmacySpineIsSuccessfulButGetPharmacyDetailIsUnsuccessful()
        {
            // Arrange
            var nhsNumber = _userSession.GpUserSession.NhsNumber;

            var nominatedPharmacyResult = new GetNominatedPharmacyResult.Success(new GetNominatedPharmacyResponse(
                HttpStatusCode.OK, OdsCode, _pertinentSerialChangeNumber, true, NominatedPharmacyType, ObjectId));

            var isGpPracticeEpsEnabledResponse = new IsGpPracticeEpsEnabledResponse(HttpStatusCode.OK, true);

            _mockGpSearchService
                .Setup(x => x.IsGpPracticeEPSEnabled(_userSession.GpUserSession.OdsCode))
                .Returns(Task.FromResult(isGpPracticeEpsEnabledResponse))
                .Verifiable();

            _mockNominatedPharmacyService
                .Setup(x => x.GetNominatedPharmacy(nhsNumber, _userSession.CitizenIdUserSession))
                .Returns(Task.FromResult<GetNominatedPharmacyResult>(nominatedPharmacyResult))
                .Verifiable();

            var pharmacyDetailResponse = new PharmacyDetailResponse(HttpStatusCode.BadRequest);

            _mockPharmacyService
                .Setup(x => x.GetPharmacyDetail(OdsCode))
                .Returns(Task.FromResult(pharmacyDetailResponse))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Get(_userSession);

            // Assert
            _mockGpSearchService.Verify();
            _mockNominatedPharmacyService.Verify();
            _mockPharmacyService.Verify();
            _mockMapper.Verify(x => x.Map(It.IsAny<Organisation>()), Times.Never);

            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int)HttpStatusCode.BadGateway);
        }

        [TestMethod]
        public async Task Update_ReturnsResultFromService_WhenServiceReturnsSuccessfully()
        {
            // Arrange
            var nhsNumber = _userSession.GpUserSession.NhsNumber;
            var updatePharmacyResult = new UpdateNominatedPharmacyResponse.Success(OdsCode, UpdatedOdsCode);

            _mockNominatedPharmacyGatewayUpdateService
                .Setup(x => x.UpdateNominatedPharmacy(nhsNumber, UpdatedOdsCode, _userSession.CitizenIdUserSession))
                .Returns(Task.FromResult((UpdateNominatedPharmacyResponse)updatePharmacyResult))
                .Verifiable();

            var updateNominatedPharmacyRequest = new UpdateNominatedPharmacyRequest
            {
                OdsCode = UpdatedOdsCode
            };

            // Act
            var result = await _systemUnderTest.Update(updateNominatedPharmacyRequest, _userSession);

            // Assert
            _mockNominatedPharmacyGatewayUpdateService.Verify();
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int)HttpStatusCode.OK);

            _auditor.Verify(x => x.Audit(AuditingOperations.UpdatedNominatedPharmacyRequest, It.IsAny<string>()));
            _auditor.Verify(x => x.Audit(AuditingOperations.UpdatedNominatedPharmacyResponse, It.IsAny<string>()));
        }


        [TestMethod]
        public async Task Update_Returns502FromService_WhenServiceReturnsUpdatedButStillOldCode()
        {
            // Arrange
            var nhsNumber = _userSession.GpUserSession.NhsNumber;
            var updatePharmacyResult = new UpdateNominatedPharmacyResponse.UpdatedButStillOldCode(OdsCode, UpdatedOdsCode);

            _mockNominatedPharmacyGatewayUpdateService
                .Setup(x => x.UpdateNominatedPharmacy(nhsNumber, UpdatedOdsCode, _userSession.CitizenIdUserSession))
                .Returns(Task.FromResult((UpdateNominatedPharmacyResponse)updatePharmacyResult))
                .Verifiable();

            var updateNominatedPharmacyRequest = new UpdateNominatedPharmacyRequest
            {
                OdsCode = UpdatedOdsCode
            };

            // Act
            var result = await _systemUnderTest.Update(updateNominatedPharmacyRequest, _userSession);

            // Assert
            _mockNominatedPharmacyGatewayUpdateService.Verify();
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int)HttpStatusCode.BadGateway);

            _auditor.Verify(x => x.Audit(AuditingOperations.UpdatedNominatedPharmacyRequest, It.IsAny<string>()));
            _auditor.Verify(x => x.Audit(AuditingOperations.UpdatedNominatedPharmacyResponse, It.IsAny<string>()));
        }

        [TestMethod]
        public async Task Update_Returns502FromService_WhenServiceReturnsBadGateway()
        {
            // Arrange
            var nhsNumber = _userSession.GpUserSession.NhsNumber;
            var updatePharmacyResult = new UpdateNominatedPharmacyResponse.BadGateway(OdsCode, UpdatedOdsCode);

            _mockNominatedPharmacyGatewayUpdateService
                .Setup(x => x.UpdateNominatedPharmacy(nhsNumber, UpdatedOdsCode, _userSession.CitizenIdUserSession))
                .Returns(Task.FromResult((UpdateNominatedPharmacyResponse)updatePharmacyResult))
                .Verifiable();

            var updateNominatedPharmacyRequest = new UpdateNominatedPharmacyRequest
            {
                OdsCode = UpdatedOdsCode
            };

            // Act
            var result = await _systemUnderTest.Update(updateNominatedPharmacyRequest, _userSession);

            // Assert
            _mockNominatedPharmacyGatewayUpdateService.Verify();
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int)HttpStatusCode.BadGateway);

            _auditor.Verify(x => x.Audit(AuditingOperations.UpdatedNominatedPharmacyRequest, It.IsAny<string>()));
            _auditor.Verify(x => x.Audit(AuditingOperations.UpdatedNominatedPharmacyResponse, It.IsAny<string>()));
        }

        [TestMethod]
        public async Task Update_Returns500FromService_WhenServiceReturnsInternalServerError()
        {
            // Arrange
            var nhsNumber = _userSession.GpUserSession.NhsNumber;
            var updatePharmacyResult = new UpdateNominatedPharmacyResponse.InternalServerError(HttpStatusCode.InternalServerError);

            _mockNominatedPharmacyGatewayUpdateService
                .Setup(x => x.UpdateNominatedPharmacy(nhsNumber, UpdatedOdsCode, _userSession.CitizenIdUserSession))
                .Returns(Task.FromResult((UpdateNominatedPharmacyResponse)updatePharmacyResult))
                .Verifiable();

            var updateNominatedPharmacyRequest = new UpdateNominatedPharmacyRequest
            {
                OdsCode = UpdatedOdsCode
            };

            // Act
            var result = await _systemUnderTest.Update(updateNominatedPharmacyRequest, _userSession);

            // Assert
            _mockNominatedPharmacyGatewayUpdateService.Verify();
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);

            _auditor.Verify(x => x.Audit(AuditingOperations.UpdatedNominatedPharmacyRequest, It.IsAny<string>()));
            _auditor.Verify(x => x.Audit(AuditingOperations.UpdatedNominatedPharmacyResponse, It.IsAny<string>()));
        }


        [DataTestMethod]
        [DataRow(HttpStatusCode.InternalServerError)]
        [DataRow(HttpStatusCode.BadGateway)]
        public async Task Update_ReturnsCorrectStatusCodeFromService_WhenServiceReturnsGetNominatedPharmacyFailure(HttpStatusCode statusCode)
        {
            // Arrange
            var nhsNumber = _userSession.GpUserSession.NhsNumber;
            var updatePharmacyResult = new UpdateNominatedPharmacyResponse.GetNominatedPharmacyFailure(new StatusCodeResult((int)statusCode));

            _mockNominatedPharmacyGatewayUpdateService
                .Setup(x => x.UpdateNominatedPharmacy(nhsNumber, UpdatedOdsCode, _userSession.CitizenIdUserSession))
                .Returns(Task.FromResult((UpdateNominatedPharmacyResponse)updatePharmacyResult))
                .Verifiable();

            var updateNominatedPharmacyRequest = new UpdateNominatedPharmacyRequest
            {
                OdsCode = UpdatedOdsCode
            };

            // Act
            var result = await _systemUnderTest.Update(updateNominatedPharmacyRequest, _userSession);

            // Assert
            _mockNominatedPharmacyGatewayUpdateService.Verify();
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int)statusCode);

            _auditor.Verify(x => x.Audit(AuditingOperations.UpdatedNominatedPharmacyRequest, It.IsAny<string>()));
            _auditor.Verify(x => x.Audit(AuditingOperations.UpdatedNominatedPharmacyResponse, It.IsAny<string>()));
        }

        [TestMethod]
        public async Task Update_ReturnsInternalServerError_WhenServiceThrowsException()
        {
            // Arrange
            var nhsNumber = _userSession.GpUserSession.NhsNumber;

            _mockNominatedPharmacyGatewayUpdateService
                .Setup(x => x.UpdateNominatedPharmacy(nhsNumber, UpdatedOdsCode, _userSession.CitizenIdUserSession))
                .Throws<Exception>()
                .Verifiable();

            var updateNominatedPharmacyRequest = new UpdateNominatedPharmacyRequest
            {
                OdsCode = UpdatedOdsCode
            };

            // Act
            var result = await _systemUnderTest.Update(updateNominatedPharmacyRequest, _userSession);

            // Assert
            _mockNominatedPharmacyGatewayUpdateService.Verify();
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
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
            var result = await _systemUnderTest.Update(updateNominatedPharmacyRequest, _userSession);

            // Assert
            _mockNominatedPharmacyGatewayUpdateService.Verify(x => x.UpdateNominatedPharmacy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CitizenIdUserSession>()), Times.Never);
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int)HttpStatusCode.Forbidden);
        }

        [TestMethod]
        public async Task Search_ReturnsSuccessfulResult_WhenServiceReturnsSuccessfully()
        {
            // Arrange
            const string postcode = "ABC";

            var pharmacy = new PharmacyDetails();
            var pharmacyResultList = new List<PharmacyDetails> { pharmacy };
            var pharmacySearchResult = new PharmacySearchResult.Success(pharmacyResultList);
            var expectedResult = new PharmacySearchResultResponse
            {
                Pharmacies = pharmacyResultList,
            };
            
            _mockPharmacySearchService
                .Setup(x => x.Search(postcode))
                .Returns(Task.FromResult((PharmacySearchResult)pharmacySearchResult))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Search(postcode);

            // Assert
            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeEquivalentTo(expectedResult);

            _mockPharmacySearchService.Verify();
            _auditor.Verify(x => x.Audit(AuditingOperations.SearchNominatedPharmacyAuditTypeRequest, It.IsAny<string>()));
            _auditor.Verify(x => x.Audit(AuditingOperations.SearchNominatedPharmacyAuditTypeResponse, It.IsAny<string>()));
        }


        [TestMethod]
        public async Task Search_ReturnsSuccessfulResult_WhenServiceReturnsWithInvalidPostcode()
        {
            // Arrange
            const string postcode = "ABC";

            var pharmacySearchResult = new PharmacySearchResult.InvalidPostcode();

            _mockPharmacySearchService
                .Setup(x => x.Search(postcode))
                .Returns(Task.FromResult((PharmacySearchResult)pharmacySearchResult))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Search(postcode);

            // Assert
            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeEquivalentTo(new PharmacySearchResultResponse());

            _mockPharmacySearchService.Verify();
            _auditor.Verify(x => x.Audit(AuditingOperations.SearchNominatedPharmacyAuditTypeRequest, It.IsAny<string>()));
            _auditor.Verify(x => x.Audit(AuditingOperations.SearchNominatedPharmacyAuditTypeResponse, It.IsAny<string>()));
        }

        [TestMethod]
        public async Task OnlineOnlyPharmacySearch_ReturnsSuccessfulResult_WhenServiceReturnsSuccessfully()
        {
            // Arrange
            var pharmacy = new PharmacyDetails();
            var pharmacyResultList = new List<PharmacyDetails> { pharmacy };
            var pharmacySearchResult = new PharmacySearchResult.Success(pharmacyResultList);
            var expectedResult = new PharmacySearchResultResponse
            {
                Pharmacies = pharmacyResultList,
            };

            _mockPharmacySearchService
                .Setup(x => x.SearchOnlineOnlyPharmacies())
                .Returns(Task.FromResult((PharmacySearchResult)pharmacySearchResult))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.OnlineOnlyPharmacySearch(null);

            // Assert
            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeEquivalentTo(expectedResult);

            _mockPharmacySearchService.Verify();
            
            _auditor.Verify(x => x.Audit(
                AuditingOperations.SearchNominatedPharmacyAuditTypeRequest, 
                "Attempting to fetch a random list of Online Pharmacies"));
            
            _auditor.Verify(x => x.Audit(AuditingOperations.SearchNominatedPharmacyAuditTypeResponse, It.IsAny<string>()));
        }
        
        
        [TestMethod]
        public async Task OnlineOnlyPharmacySearchByName_ReturnsSuccessfulResult_WhenServiceReturnsSuccessfully()
        {
            // Arrange
            var searchTerm = "test";
            var pharmacy = new PharmacyDetails();
            var pharmacyResultList = new List<PharmacyDetails> { pharmacy };
            const int pharmacyResultCount = 1;
            var pharmacySearchResult = new PharmacySearchResult.Success(pharmacyResultList, pharmacyResultCount);
            
            var expectedResult = new PharmacySearchResultResponse
            {
                Pharmacies = pharmacyResultList,
                PharmacyCount = pharmacyResultCount,
            };

            _mockPharmacySearchService
                .Setup(x => x.SearchOnlineOnlyPharmacies(searchTerm))
                .Returns(Task.FromResult((PharmacySearchResult)pharmacySearchResult))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.OnlineOnlyPharmacySearch(searchTerm);

            // Assert
            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeEquivalentTo(expectedResult);

            _mockPharmacySearchService.Verify();
            
            _auditor.Verify(x => x.Audit(
                AuditingOperations.SearchNominatedPharmacyAuditTypeRequest, 
                $"Attempting to search for Online Pharmacies by name using search term: {searchTerm}"));
            
            _auditor.Verify(x => x.Audit(AuditingOperations.SearchNominatedPharmacyAuditTypeResponse, It.IsAny<string>()));
        }
        
        [TestMethod]
        public async Task OnlineOnlyPharmacySearchByName_ReturnsUnsafeSearchTermResult_WhenSearchTermIsDeemedUnsafe()
        {
            // Arrange
            var searchTerm = "<nastyScriptTag>";
            var expectedResult = new PharmacySearchResultResponse();

            // Act
            var result = await _systemUnderTest.OnlineOnlyPharmacySearch(searchTerm);

            // Assert
            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeEquivalentTo(expectedResult);

            _mockPharmacySearchService.Verify(x => x.SearchOnlineOnlyPharmacies(It.IsAny<string>()), Times.Never);
            
            _auditor.Verify(x => x.Audit(AuditingOperations.SearchNominatedPharmacyAuditTypeResponse, It.IsAny<string>()));
        }

        public void Dispose() => _systemUnderTest?.Dispose();
    }
}
