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
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.PfsApi.Areas.OrganDonation;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using NHSOnline.Backend.PfsApi.OrganDonation;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.OrganDonation
{
    [TestClass]
    public sealed class OrganDonationControllerPostTests: IDisposable
    {
        private OrganDonationController _systemUnderTest;
        private Mock<IOrganDonationService> _mockOrganDonationService;
        private P9UserSession _userSession;
        private Mock<IAuditor> _mockAuditor;
        private Mock<IOrganDonationValidationService> _mockValidator;

        private const string RequestAuditType = "OrganDonation_Registration_Request";
        private const string ResponseAuditType = "OrganDonation_Registration_Response";

        private const string RequestAuditMessage = "Attempting to register organ donation decision";

        [TestInitialize]
        public void TestInitialize()
        {
            _userSession = new P9UserSession("csrfToken", "nhsNumber", new CitizenIdUserSession(), new EmisUserSession(), "im1token");
            _mockOrganDonationService = new Mock<IOrganDonationService>();
            _mockAuditor = new Mock<IAuditor>();

            _mockValidator = new Mock<IOrganDonationValidationService>();
            _mockValidator
                .Setup(x => x.IsPostValid(It.IsAny<OrganDonationRegistrationRequest>()))
                .Returns(true);

            _systemUnderTest = new OrganDonationController(
                new Mock<ILogger<OrganDonationController>>().Object,
                new Mock<IGpSystemFactory>().Object,
                _mockOrganDonationService.Object,
                _mockAuditor.Object,
                _mockValidator.Object);
        }

        [TestMethod]
        public async Task Post_ReturnsSuccessfulResult_WhenServiceReturnsSuccessResult()
        {
            // Arrange
            var organDonationRegistrationResponse = new OrganDonationRegistrationResponse();
            var newResult = new OrganDonationRegistrationResult.SuccessfullyRegistered(organDonationRegistrationResponse);

            _mockOrganDonationService
                .Setup(x => x.Register(It.IsAny<OrganDonationRegistrationRequest>(), _userSession))
                .Returns(Task.FromResult((OrganDonationRegistrationResult) newResult));

            // Act
            var result = await _systemUnderTest.Post(new OrganDonationRegistrationRequest(), _userSession);

            // Assert
            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            using (new AssertionScope())
            {
                objectResult.StatusCode.Should().Be(StatusCodes.Status201Created);
                objectResult.Value.Should().BeEquivalentTo(organDonationRegistrationResponse);
            }

            _mockOrganDonationService.Verify(x => x.Register(It.IsAny<OrganDonationRegistrationRequest>(), _userSession));
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x =>
                x.Audit(ResponseAuditType, "The organ donation decision has been successfully registered"));
        }

        [TestMethod]
        public async Task Post_ReturnsBadRequest_WhenRequestFailsValidation()
        {
            // Arrange
            _mockValidator
                .Setup(x => x.IsPostValid(It.IsAny<OrganDonationRegistrationRequest>()))
                .Returns(false);

            // Act
            var result = await _systemUnderTest.Post(new OrganDonationRegistrationRequest(), _userSession);

            // Assert
            result.Should().BeOfType<BadRequestResult>();

            _mockOrganDonationService.Verify(x => x.Register(It.IsAny<OrganDonationRegistrationRequest>(), _userSession), Times.Never);
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x =>
                x.Audit(ResponseAuditType, "The organ donation registration request failed validation"));
        }

        [TestMethod]
        public async Task Post_ReturnsGatewayTimeout_WhenServiceReturnTimeoutResult()
        {
            // Arrange
            var timeoutResult = new OrganDonationRegistrationResult.Timeout();

            _mockOrganDonationService
                .Setup(x => x.Register(It.IsAny<OrganDonationRegistrationRequest>(), _userSession))
                .Returns(Task.FromResult((OrganDonationRegistrationResult) timeoutResult));

            // Act
            var result = await _systemUnderTest.Post(new OrganDonationRegistrationRequest(), _userSession);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status504GatewayTimeout);

            _mockOrganDonationService.Verify(x => x.Register(It.IsAny<OrganDonationRegistrationRequest>(), _userSession));
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType, "The organ donation registration system took too long to respond"));
        }

        [TestMethod]
        public async Task Post_ReturnsBadGateway_WhenServiceReturnUpstreamErrorResult()
        {
            // Arrange
            var response = new PfsErrorResponse();
            var upstreamErrorResult = new OrganDonationRegistrationResult.UpstreamError(response);

            _mockOrganDonationService
                .Setup(x => x.Register(It.IsAny<OrganDonationRegistrationRequest>(), _userSession))
                .Returns(Task.FromResult((OrganDonationRegistrationResult) upstreamErrorResult));

            // Act
            var result = await _systemUnderTest.Post(new OrganDonationRegistrationRequest(), _userSession);

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            using (new AssertionScope())
            {
                statusCodeResult.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
                statusCodeResult.Value.Should().Be(response);
            }

            _mockOrganDonationService.Verify(x => x.Register(It.IsAny<OrganDonationRegistrationRequest>(), _userSession));
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x =>
                x.Audit(ResponseAuditType, "There was an upstream error when registering the organ donation decision"));
        }

        [TestMethod]
        public async Task Post_ReturnsInternalServerError_WhenServiceReturnSystemErrorResult()
        {
            // Arrange
            var systemErrorResult = new OrganDonationRegistrationResult.SystemError();

            _mockOrganDonationService
                .Setup(x => x.Register(It.IsAny<OrganDonationRegistrationRequest>(), _userSession))
                .Returns(Task.FromResult((OrganDonationRegistrationResult) systemErrorResult));

            // Act
            var result = await _systemUnderTest.Post(new OrganDonationRegistrationRequest(), _userSession);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

            _mockOrganDonationService.Verify(x => x.Register(It.IsAny<OrganDonationRegistrationRequest>(), _userSession));
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(
                x => x.Audit(ResponseAuditType, "There was an issue registering the organ donation decision"));
        }

        public void Dispose() => _systemUnderTest?.Dispose();
    }
}