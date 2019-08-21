using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.Areas.OrganDonation;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.PfsApi.OrganDonation;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet.Filters;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.OrganDonation
{
    [TestClass]
    public class OrganDonationControllerGetTests
    {
        private OrganDonationController _systemUnderTest;
        private IFixture _fixture;
        private Mock<IOrganDonationService> _mockOrganDonationService;
        private UserSession _userSession;
        private Mock<IAuditor> _mockAuditor;
        private Mock<IGpSystem> _mockGpSystem;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private Mock<IDemographicsService> _mockDemographicsService;

        private const string RequestAuditType = "OrganDonation_Get_Request";
        private const string ResponseAuditType = "OrganDonation_Get_Response";

        private const string RequestAuditMessage = "Attempting to get organ donation record";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _userSession = _fixture.Create<UserSession>();
            _mockOrganDonationService = _fixture.Freeze<Mock<IOrganDonationService>>();
            _mockAuditor = _fixture.Freeze<Mock<IAuditor>>();

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.Items[Constants.HttpContextItems.UserSession]).Returns(_userSession);

            var demographicsResult = new DemographicsResult.Success(new DemographicsResponse());

            _mockDemographicsService = _fixture.Freeze<Mock<IDemographicsService>>();
            _mockDemographicsService
                .Setup(x => x.GetDemographics(_userSession.GpUserSession))
                .Returns(Task.FromResult((DemographicsResult) demographicsResult));

            _mockGpSystem = _fixture.Freeze<Mock<IGpSystem>>();
            _mockGpSystem
                .Setup(x => x.GetDemographicsService())
                .Returns(_mockDemographicsService.Object);

            _mockGpSystemFactory = _fixture.Freeze<Mock<IGpSystemFactory>>();
            _mockGpSystemFactory
                .Setup(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier))
                .Returns(_mockGpSystem.Object);

            _systemUnderTest = _fixture.Create<OrganDonationController>();

            _systemUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResult_WhenServiceReturnsNewRegistrationResult()
        {
            // Arrange
            var organDonationRegistration = _fixture.Create<OrganDonationRegistration>();
            var newResult = new OrganDonationResult.NewRegistration(organDonationRegistration);

            _mockOrganDonationService
                .Setup(x => x.GetOrganDonation(It.IsAny<DemographicsResult>(), _userSession))
                .Returns(Task.FromResult((OrganDonationResult) newResult));

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            var value = result.Should().BeAssignableTo<OkObjectResult>().Subject.Value;
            value.Should().BeEquivalentTo(organDonationRegistration);
            _mockOrganDonationService.Verify(x => x.GetOrganDonation(It.IsAny<DemographicsResult>(), _userSession));
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x =>
                x.Audit(ResponseAuditType, "A default organ donation registration has been generated"));
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResult_WhenServiceReturnsExistingRegistrationResult()
        {
            // Arrange
            var organDonationRegistration = _fixture.Create<OrganDonationRegistration>();
            var newResult = new OrganDonationResult.ExistingRegistration(organDonationRegistration);

            _mockOrganDonationService
                .Setup(x => x.GetOrganDonation(It.IsAny<DemographicsResult>(), _userSession))
                .Returns(Task.FromResult((OrganDonationResult) newResult));

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            var value = result.Should().BeAssignableTo<OkObjectResult>().Subject.Value;
            value.Should().BeEquivalentTo(organDonationRegistration);
            _mockOrganDonationService.Verify(x => x.GetOrganDonation(It.IsAny<DemographicsResult>(), _userSession));
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType, "An existing organ donation registration been found"));
        }

        [TestMethod]
        public async Task Get_ReturnsInternalServerError_WhenServiceReturnSearchErrorResult()
        {
            // Arrange
            var newResult = new OrganDonationResult.SearchError();

            _mockOrganDonationService
                .Setup(x => x.GetOrganDonation(It.IsAny<DemographicsResult>(), _userSession))
                .Returns(Task.FromResult((OrganDonationResult) newResult));

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

            _mockOrganDonationService.Verify(x => x.GetOrganDonation(It.IsAny<DemographicsResult>(), _userSession));
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x =>
                x.Audit(ResponseAuditType, "There was an issue searching for an organ donation record"));
        }

        [TestMethod]
        public async Task Get_ReturnsBadGateway_WhenServiceReturnSearchUpstreamErrorResult()
        {
            // Arrange
            var response = _fixture.Create<PfsErrorResponse>();
            var newResult = new OrganDonationResult.SearchUpstreamError(response);

            _mockOrganDonationService
                .Setup(x => x.GetOrganDonation(It.IsAny<DemographicsResult>(), _userSession))
                .Returns(Task.FromResult((OrganDonationResult) newResult));

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
            statusCodeResult.Value.Should().Be(response);

            _mockOrganDonationService.Verify(x => x.GetOrganDonation(It.IsAny<DemographicsResult>(), _userSession));
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType, "There was an upstream error when searching for an organ donation record"));
        }

        [TestMethod]
        public async Task Get_ReturnsGatewayTimeout_WhenServiceReturnSearchTimeoutResult()
        {
            // Arrange
            var newResult = new OrganDonationResult.SearchTimeout();

            _mockOrganDonationService
                .Setup(x => x.GetOrganDonation(It.IsAny<DemographicsResult>(), _userSession))
                .Returns(Task.FromResult((OrganDonationResult) newResult));

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status504GatewayTimeout);

            _mockOrganDonationService.Verify(x => x.GetOrganDonation(It.IsAny<DemographicsResult>(), _userSession));
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType, "The organ donation system took too long to respond"));
        }

        [TestMethod]
        public async Task Get_ReturnsInternalServerError_WhenServiceReturnDemographicsInternalError()
        {
            // Arrange
            var newResult = new OrganDonationResult.DemographicsInternalServerError();

            _mockOrganDonationService
                .Setup(x => x.GetOrganDonation(It.IsAny<DemographicsResult>(), _userSession))
                .Returns(Task.FromResult((OrganDonationResult) newResult));

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

            _mockOrganDonationService.Verify(x => x.GetOrganDonation(It.IsAny<DemographicsResult>(), _userSession));
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(
                x => x.Audit(ResponseAuditType, "Error received from demographics"));
        }


        [TestMethod]
        public async Task Get_ReturnsInternalServerError_WhenServiceReturnDemographicsRetrievalFailedResult()
        {
            // Arrange
            var newResult = new OrganDonationResult.DemographicsRetrievalFailed();

            _mockOrganDonationService
                .Setup(x => x.GetOrganDonation(It.IsAny<DemographicsResult>(), _userSession))
                .Returns(Task.FromResult((OrganDonationResult) newResult));

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

            _mockOrganDonationService.Verify(x => x.GetOrganDonation(It.IsAny<DemographicsResult>(), _userSession));
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(
                x => x.Audit(ResponseAuditType, "There was an issue retrieving the demographics record"));
        }

        [TestMethod]
        public async Task Get_ReturnsForbidden_WhenServiceReturnDemographicsForbiddenResult()
        {
            // Arrange
            var newResult = new OrganDonationResult.DemographicsForbidden();

            _mockOrganDonationService
                .Setup(x => x.GetOrganDonation(It.IsAny<DemographicsResult>(), _userSession))
                .Returns(Task.FromResult((OrganDonationResult) newResult));

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status403Forbidden);

            _mockOrganDonationService.Verify(x => x.GetOrganDonation(It.IsAny<DemographicsResult>(), _userSession));
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(
                x => x.Audit(ResponseAuditType, "Access to demographics was forbidden"));
        }

        [TestMethod]
        public async Task Get_ReturnsBadGateway_WhenServiceReturnDemographicsBadGatewayResult()
        {
            // Arrange
            var newResult = new OrganDonationResult.DemographicsBadGateway();

            _mockOrganDonationService
                .Setup(x => x.GetOrganDonation(It.IsAny<DemographicsResult>(), _userSession))
                .Returns(Task.FromResult((OrganDonationResult) newResult));

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status502BadGateway);

            _mockOrganDonationService.Verify(x => x.GetOrganDonation(It.IsAny<DemographicsResult>(), _userSession));
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(
                x => x.Audit(ResponseAuditType, "There was an issue retrieving the demographics record"));
        }
    }
}