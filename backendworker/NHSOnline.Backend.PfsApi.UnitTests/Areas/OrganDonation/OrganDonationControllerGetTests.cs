using System;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.Areas.OrganDonation;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.PfsApi.Filters;
using NHSOnline.Backend.PfsApi.OrganDonation;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.OrganDonation
{
    [TestClass]
    public sealed class OrganDonationControllerGetTests: IDisposable
    {
        private OrganDonationController _systemUnderTest;
        private Mock<IOrganDonationService> _mockOrganDonationService;
        private P9UserSession _userSession;
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
            _userSession = new P9UserSession("csrfToken", "nhsNumber", new CitizenIdUserSession(),
                new EmisUserSession(), "im1token");
            _mockOrganDonationService = new Mock<IOrganDonationService>();
            _mockAuditor = new Mock<IAuditor>();

            var demographicsResult = new DemographicsResult.Success(new DemographicsResponse());

            _mockDemographicsService = new Mock<IDemographicsService>();
            _mockDemographicsService
                .Setup(x => x.GetDemographics(
                    It.Is<GpLinkedAccountModel>(
                        d => d.GpUserSession == _userSession.GpUserSession && d.PatientId == Guid.Empty)))
                .Returns(Task.FromResult((DemographicsResult) demographicsResult));

            _mockGpSystem = new Mock<IGpSystem>();
            _mockGpSystem
                .Setup(x => x.GetDemographicsService())
                .Returns(_mockDemographicsService.Object);

            _mockGpSystemFactory = new Mock<IGpSystemFactory>();
            _mockGpSystemFactory
                .Setup(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier))
                .Returns(_mockGpSystem.Object);

            _systemUnderTest = new OrganDonationController(
                new Mock<ILogger<OrganDonationController>>().Object,
                _mockGpSystemFactory.Object,
                _mockOrganDonationService.Object,
                _mockAuditor.Object,
                new Mock<IOrganDonationValidationService>().Object,
                new Mock<IMetricLogger>().Object);
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResult_WhenServiceReturnsNewRegistrationResult()
        {
            // Arrange
            var organDonationRegistration = new OrganDonationRegistration();
            var newResult = new OrganDonationResult.NewRegistration(organDonationRegistration);

            _mockOrganDonationService
                .Setup(x => x.GetOrganDonation(It.IsAny<DemographicsResult>(), _userSession))
                .Returns(Task.FromResult((OrganDonationResult) newResult));

            // Act
            var result = await _systemUnderTest.Get(_userSession);

            // Assert
            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeEquivalentTo(organDonationRegistration);
            _mockOrganDonationService.Verify(x => x.GetOrganDonation(It.IsAny<DemographicsResult>(), _userSession));
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x =>
                x.Audit(ResponseAuditType, "A default organ donation registration has been generated"));
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResult_WhenServiceReturnsExistingRegistrationResult()
        {
            // Arrange
            var organDonationRegistration = new OrganDonationRegistration();
            var newResult = new OrganDonationResult.ExistingRegistration(organDonationRegistration);

            _mockOrganDonationService
                .Setup(x => x.GetOrganDonation(It.IsAny<DemographicsResult>(), _userSession))
                .Returns(Task.FromResult((OrganDonationResult) newResult));

            // Act
            var result = await _systemUnderTest.Get(_userSession);

            // Assert
            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeEquivalentTo(organDonationRegistration);
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
            var result = await _systemUnderTest.Get(_userSession);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

            _mockOrganDonationService.Verify(x => x.GetOrganDonation(It.IsAny<DemographicsResult>(), _userSession));
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x =>
                x.Audit(ResponseAuditType, "There was an issue searching for an organ donation record"));
        }

        [TestMethod]
        public async Task Get_ReturnsBadGateway_WhenServiceReturnSearchUpstreamErrorResult()
        {
            // Arrange
            var response = new PfsErrorResponse();
            var newResult = new OrganDonationResult.SearchUpstreamError(response);

            _mockOrganDonationService
                .Setup(x => x.GetOrganDonation(It.IsAny<DemographicsResult>(), _userSession))
                .Returns(Task.FromResult((OrganDonationResult) newResult));

            // Act
            var result = await _systemUnderTest.Get(_userSession);

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            using (new AssertionScope())
            {
                statusCodeResult.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
                statusCodeResult.Value.Should().Be(response);
            }

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
            var result = await _systemUnderTest.Get(_userSession);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status504GatewayTimeout);

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
            var result = await _systemUnderTest.Get(_userSession);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

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
            var result = await _systemUnderTest.Get(_userSession);

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
            var result = await _systemUnderTest.Get(_userSession);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status403Forbidden);

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
            var result = await _systemUnderTest.Get(_userSession);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);

            _mockOrganDonationService.Verify(x => x.GetOrganDonation(It.IsAny<DemographicsResult>(), _userSession));
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(
                x => x.Audit(ResponseAuditType, "There was an issue retrieving the demographics record"));
        }

        [TestMethod]
        public void EnsureControllerHasProxyingNotAllowedAttribute_ToPreventProxyAccess()
        {
            typeof(OrganDonationController).Should().BeDecoratedWith<ProxyingNotAllowedAttribute>();
        }

        public void Dispose() => _systemUnderTest?.Dispose();
    }
}