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
        private NominatedPharmacyController _systemUnderTest;
        private IFixture _fixture;
        private UserSession _userSession;

        private Mock<ILogger<NominatedPharmacyController>> _mockLogger;
        private Mock<INominatedPharmacyService> _mockNominatedPharmacyService;
        private Mock<IPharmacySearchService> _mockPharmacySearchService;
        private Mock<IPharmacyService> _mockPharmacyService;
        private Mock<IPharmacyDetailsToPharmacyDetailsResponseMapper> _mockMapper;
        private string pertinentSerialChangeNumber = new Guid().ToString();

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
            const string odsCode = "AB123";

            var nominatedPharmacyResult = new GetNominatedPharmacyResult(HttpStatusCode.OK, odsCode, pertinentSerialChangeNumber);
            
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

            var mappedResult = _fixture.Create<PharmacyDetailsResponse>();

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
            value.Should().BeEquivalentTo(mappedResult);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public async Task Get_ReturnsSuccessfulResult_WhenNoNominatedPharmacySet(string odsCode)
        {
            // Arrange
            string nhsNumber = _userSession.GpUserSession.NhsNumber;

            var nominatedPharmacyResult = new GetNominatedPharmacyResult(HttpStatusCode.OK, odsCode, pertinentSerialChangeNumber);
            
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
            value.StatusCode.Should().Be(StatusCodes.Status200OK);
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
            value.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Get_Returns500_IfNominatedPharmacySearchIsSuccessfulButPharmacyDetailsSearchIsUnsuccessful()
        {
            // Arrange
            string odsCode = "AB123";
            string nhsNumber = _userSession.GpUserSession.NhsNumber;
            string pertinentSerialChangeNumber = new Guid().ToString();

            var nominatedPharmacyResult = new GetNominatedPharmacyResult(HttpStatusCode.OK, odsCode, pertinentSerialChangeNumber);
            
            _mockNominatedPharmacyService
                .Setup(x => x.GetNominatedPharmacy(nhsNumber))
                .Returns(Task.FromResult(nominatedPharmacyResult))
                .Verifiable();

            var pharmacyOrgansation = _fixture.Create<Organisation>();
            var pharmacyDetailResponse = new PharmacyDetailResponse(HttpStatusCode.InternalServerError);

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
            value.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Update_ReturnsSuccessful200Result_WhenServiceReturnsSuccessfully()
        {
            // Arrange
            #region GetPharmacyResultSetup
            string nhsNumber = _userSession.GpUserSession.NhsNumber;
            const string odsCode = "AB123";
            string pertinentSerialChangeNumber = new Guid().ToString();

            var nominatedPharmacyResult = new GetNominatedPharmacyResult(HttpStatusCode.OK, odsCode, pertinentSerialChangeNumber);

            _mockNominatedPharmacyService
                .Setup(x => x.GetNominatedPharmacy(nhsNumber))
                .Returns(Task.FromResult(nominatedPharmacyResult))
                .Verifiable();

            #endregion

            #region UpdatePharmacyResultSetup
            const string updatedOdsCode = "BB999";

            var updateNominatedPharmacyResult = new UpdateNominatedPharmacyResult(HttpStatusCode.OK);

            _mockNominatedPharmacyService
                .Setup(x => x.UpdateNominatedPharmacy(nhsNumber, updatedOdsCode, pertinentSerialChangeNumber))
                .Returns(Task.FromResult(updateNominatedPharmacyResult))
                .Verifiable();

            var updateNominatedPharmacyRequest = new UpdateNominatedPharmacyRequest
            {
                OdsCode = updatedOdsCode
            };

            #endregion

            // Act
            var result = await _systemUnderTest.Update(updateNominatedPharmacyRequest);

            // Assert
            _mockNominatedPharmacyService.Verify();

            _mockNominatedPharmacyService.Verify(x => x.GetNominatedPharmacy(It.IsAny<string>()), Times.Exactly(1));
            _mockNominatedPharmacyService.Verify(x => x.UpdateNominatedPharmacy(nhsNumber, updatedOdsCode, pertinentSerialChangeNumber), Times.Once);
            var value = result.Should().BeAssignableTo<OkResult>().Subject;
        }

        [TestMethod]
        public async Task Update_ReturnsSuccessful500Result_WhenGetPharmacyFails()
        {
            // Arrange
            #region GetPharmacyResultSetup
            string odsCode = "AB123";
            string nhsNumber = _userSession.GpUserSession.NhsNumber;
            string pertinentSerialChangeNumber = new Guid().ToString();
            const string updatedOdsCode = "BB999";

            var nominatedPharmacyResult = new GetNominatedPharmacyResult(HttpStatusCode.BadGateway, odsCode, pertinentSerialChangeNumber);

            _mockNominatedPharmacyService
                .Setup(x => x.GetNominatedPharmacy(nhsNumber))
                .Returns(Task.FromResult(nominatedPharmacyResult))
                .Verifiable();

            #endregion

            var updateNominatedPharmacyRequest = new UpdateNominatedPharmacyRequest
            {
                OdsCode = updatedOdsCode
            };

            // Act
            var result = await _systemUnderTest.Update(updateNominatedPharmacyRequest);

            // Assert
            _mockNominatedPharmacyService.Verify(x => x.GetNominatedPharmacy(It.IsAny<string>()), Times.Once);
            _mockNominatedPharmacyService.Verify(x => x.UpdateNominatedPharmacy(nhsNumber, updatedOdsCode, pertinentSerialChangeNumber), Times.Never);

            var value = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            value.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Update_ReturnsSuccessful400Result_WhenUpdatePharmacyFails()
        {
            // Arrange
            #region GetPharmacyResultSetup
            string odsCode = "AB123";
            string nhsNumber = _userSession.GpUserSession.NhsNumber;
            string pertinentSerialChangeNumber = new Guid().ToString();

            var nominatedPharmacyResult = new GetNominatedPharmacyResult(HttpStatusCode.OK, odsCode, pertinentSerialChangeNumber);

            _mockNominatedPharmacyService
                .Setup(x => x.GetNominatedPharmacy(nhsNumber))
                .Returns(Task.FromResult(nominatedPharmacyResult))
                .Verifiable();

            #endregion

            #region UpdatePharmacyResultSetup
            const string updatedOdsCode = "BB999";

            var updateNominatedPharmacyResult = new UpdateNominatedPharmacyResult(HttpStatusCode.BadRequest);

            _mockNominatedPharmacyService
                .Setup(x => x.UpdateNominatedPharmacy(nhsNumber, updatedOdsCode, pertinentSerialChangeNumber))
                .Returns(Task.FromResult(updateNominatedPharmacyResult))
                .Verifiable();

            var updateNominatedPharmacyRequest = new UpdateNominatedPharmacyRequest
            {
                OdsCode = updatedOdsCode
            };

            #endregion

            // Act
            var result = await _systemUnderTest.Update(updateNominatedPharmacyRequest);

            // Assert
            _mockNominatedPharmacyService.Verify(x => x.GetNominatedPharmacy(It.IsAny<string>()), Times.Once);
            _mockNominatedPharmacyService.Verify(x => x.UpdateNominatedPharmacy(nhsNumber,updatedOdsCode, pertinentSerialChangeNumber), Times.Once);
            var value = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            value.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }
        
        [TestMethod]
        public async Task Update_ReturnsBadGateway_WhenGetPharmacyHadMissingPertinentSerialChangeNumber()
        {
            // Arrange
            #region GetPharmacyResultSetup
            string odsCode = "AB123";
            string nhsNumber = _userSession.GpUserSession.NhsNumber;
            string pertinentSerialChangeNumber = null;
            const string updatedOdsCode = "BB999";

            var nominatedPharmacyResult = new GetNominatedPharmacyResult(HttpStatusCode.OK, odsCode, pertinentSerialChangeNumber);

            _mockNominatedPharmacyService
                .Setup(x => x.GetNominatedPharmacy(nhsNumber))
                .Returns(Task.FromResult(nominatedPharmacyResult))
                .Verifiable();

            #endregion

            var updateNominatedPharmacyRequest = new UpdateNominatedPharmacyRequest
            {
                OdsCode = updatedOdsCode
            };

            // Act
            var result = await _systemUnderTest.Update(updateNominatedPharmacyRequest);

            // Assert
            _mockNominatedPharmacyService.Verify(x => x.GetNominatedPharmacy(It.IsAny<string>()), Times.Once);

            var value = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            value.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
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

            var pharmacy1 = _fixture.Create<PharmacyDetailsResponse>();
            var mappedResult = new List<PharmacyDetailsResponse> { pharmacy1 };

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
