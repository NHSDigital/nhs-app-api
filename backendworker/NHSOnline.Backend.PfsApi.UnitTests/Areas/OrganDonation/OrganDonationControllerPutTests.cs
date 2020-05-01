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
using NHSOnline.Backend.PfsApi.OrganDonation;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.OrganDonation
{
    [TestClass]
    public sealed class OrganDonationControllerPutTests : IDisposable
    {
        private OrganDonationController _systemUnderTest;
        private Mock<IOrganDonationService> _mockOrganDonationService;
        private P9UserSession _userSession;
        private Mock<IAuditor> _mockAuditor;
        private Mock<IOrganDonationValidationService> _mockValidator;

        private const string RequestAuditType = "OrganDonation_Update_Request";
        private const string ResponseAuditType = "OrganDonation_Update_Response";

        private const string RequestAuditMessage = "Attempting to update organ donation decision";

        [TestInitialize]
        public void TestInitialize()
        {
            _userSession = new P9UserSession("csrfToken", new CitizenIdUserSession(), new EmisUserSession(), "im1token");
            _mockOrganDonationService = new Mock<IOrganDonationService>();
            _mockAuditor = new Mock<IAuditor>();

            _mockValidator = new Mock<IOrganDonationValidationService>();
            _mockValidator
                .Setup(x => x.IsPutValid(It.IsAny<OrganDonationRegistrationRequest>()))
                .Returns(true);

            _systemUnderTest = new OrganDonationController(
                new Mock<ILogger<OrganDonationController>>().Object,
                new Mock<IGpSystemFactory>().Object,
                _mockOrganDonationService.Object,
                _mockAuditor.Object,
                _mockValidator.Object);
        }

        [TestMethod]
        public async Task Put_ReturnsSuccessfulResult_WhenServiceReturnsSuccessResult()
        {
            // Arrange
            var organDonationRegistrationResponse = new OrganDonationRegistrationResponse();
            var newResult = new OrganDonationRegistrationResult.SuccessfullyRegistered(organDonationRegistrationResponse);
            
            _mockOrganDonationService
                .Setup(x => x.Update(It.IsAny<OrganDonationRegistrationRequest>(), _userSession))
                .Returns(Task.FromResult((OrganDonationRegistrationResult) newResult));

            // Act
            var result = await _systemUnderTest.Put(new OrganDonationRegistrationRequest(), _userSession);

            // Assert
            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeEquivalentTo(organDonationRegistrationResponse);
            _mockOrganDonationService.Verify(x => x.Update(It.IsAny<OrganDonationRegistrationRequest>(), _userSession));
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x =>
                x.Audit(ResponseAuditType, "The organ donation decision has been successfully updated"));
        }

        [TestMethod]
        public async Task Put_ReturnsBadRequest_WhenRequestFailsValidation()
        {
            // Arrange
            _mockValidator
                .Setup(x => x.IsPutValid(It.IsAny<OrganDonationRegistrationRequest>()))
                .Returns(false);
            
            // Act
            var result = await _systemUnderTest.Put(new OrganDonationRegistrationRequest(), _userSession);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
            
            
            _mockOrganDonationService.Verify(x => x.Update(It.IsAny<OrganDonationRegistrationRequest>(), _userSession), Times.Never);
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType, "The organ donation update registration request failed validation"));
        }
        
        [TestMethod]
        public async Task Put_ReturnsGatewayTimeout_WhenServiceReturnTimeoutResult()
        {
            // Arrange
            var timeoutResult = new OrganDonationRegistrationResult.Timeout();

            _mockOrganDonationService.Setup(x => x.Update(It.IsAny<OrganDonationRegistrationRequest>(), _userSession))
                .Returns(Task.FromResult((OrganDonationRegistrationResult) timeoutResult));

            // Act
            var result = await _systemUnderTest.Put(new OrganDonationRegistrationRequest(), _userSession);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status504GatewayTimeout);

            _mockOrganDonationService.Verify(x => x.Update(It.IsAny<OrganDonationRegistrationRequest>(), _userSession));
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType, "The organ donation registration update system took too long to respond"));
        }

        [TestMethod]
        public async Task Put_ReturnsBadGateway_WhenServiceReturnUpstreamErrorResult()
        {
            // Arrange
            var response = new PfsErrorResponse();
            var upstreamErrorResult = new OrganDonationRegistrationResult.UpstreamError(response);

            _mockOrganDonationService
                .Setup(x => x.Update(It.IsAny<OrganDonationRegistrationRequest>(), _userSession))
                .Returns(Task.FromResult((OrganDonationRegistrationResult) upstreamErrorResult));

            // Act
            var result = await _systemUnderTest.Put(new OrganDonationRegistrationRequest(), _userSession);

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            using (new AssertionScope())
            {
                statusCodeResult.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
                statusCodeResult.Value.Should().Be(response);
            }

            _mockOrganDonationService.Verify(x => x.Update(It.IsAny<OrganDonationRegistrationRequest>(), _userSession));
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => 
                x.Audit(ResponseAuditType, "There was an upstream error when registering the organ donation decision update"));
        }
        
        [TestMethod]
        public async Task Put_ReturnsInternalServerError_WhenServiceReturnSystemErrorResult()
        {
            // Arrange
            var systemErrorResult = new OrganDonationRegistrationResult.SystemError();

            _mockOrganDonationService
                .Setup(x => x.Update(It.IsAny<OrganDonationRegistrationRequest>(), _userSession))
                .Returns(Task.FromResult((OrganDonationRegistrationResult) systemErrorResult));

            // Act
            var result = await _systemUnderTest.Put(new OrganDonationRegistrationRequest(), _userSession);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

            _mockOrganDonationService.Verify(x => x.Update(It.IsAny<OrganDonationRegistrationRequest>(), _userSession));
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(
                x => x.Audit(ResponseAuditType, "There was an issue registering the organ donation decision update"));
        }

        public void Dispose() => _systemUnderTest?.Dispose();
    }
}